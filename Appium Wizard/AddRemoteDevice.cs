using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Appium_Wizard
{
    public partial class AddRemoteDevice : Form
    {
        public AddRemoteDevice()
        {
            InitializeComponent();
        }

        private async void findDevicesButton_Click(object sender, EventArgs e)
        {
            string pattern = @"\b(?:(?:2[0-5]{2}|1\d{2}|[1-9]?\d)\.){3}(?:2[0-5]{2}|1\d{2}|[1-9]?\d):([0-9]{1,5})\b";

            Match match = Regex.Match(IPTextBox.Text, pattern);

            if (match.Success)
            {
                CommonProgress commonProgress = new CommonProgress();
                commonProgress.Show();
                commonProgress.UpdateStepLabel("Find Remote Devices", "Please wait while looking for remote devices...");
                string fileUrl = IPTextBox.Text;
                var json = await Common.ReadJsonFromRemote("https://gist.githubusercontent.com/mega6453/143d120c372052d096bdddab080838dd/raw/9ce95f40dfade1391f523334ae1b1428daebc9ae/gistfile1.txt");
                //var json = File.ReadAllText("C:\\Users\\mc\\Desktop\\Appium Wizard\\Repo\\Appium Wizard\\bin\\Debug\\net8.0-windows10.0.17763.0\\win-x64\\Resources\\RemoteExecution\\sharedDevices.json");
                if (!string.IsNullOrEmpty(json))
                {
                    // Parse JSON data
                    JArray devices = JArray.Parse(json);

                    // Add devices to ListView
                    foreach (var device in devices)
                    {
                        ListViewItem item = new ListViewItem(device["DeviceName"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["DeviceOS"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["DeviceVersion"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["Model"]?.ToString() ?? "NA");
                        //item.SubItems.Add(device["DeviceStatus"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["DeviceUDID"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["SystemIPAddress"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["ProxyPort"]?.ToString() ?? "NA");
                        item.SubItems.Add(device["ScreenPort"]?.ToString() ?? "NA");
                        //item.SubItems.Add(device["DeviceConnection"]?.ToString() ?? "NA");
                        //item.SubItems.Add(device["DeviceIP"]?.ToString() ?? "");
                        remoteDevicesList.Items.Add(item);
                    }
                }
                commonProgress.Close();
            }
            else
            {
                MessageBox.Show("Please enter valid address. The accepted format is, IPAddress:PortNumber.","Find Devices",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void ReserveButton_Click(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Reserve Device","Please wait while reserving device...");
            if (remoteDevicesList.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = remoteDevicesList.SelectedItems[0];
                string name = selectedItem.SubItems[0].Text;
                string os = selectedItem.SubItems[1].Text;
                string version = selectedItem.SubItems[2].Text;
                string model = selectedItem.SubItems[3].Text;
                string udid = selectedItem.SubItems[2].Text;
                string status ="";
                string connection = "";
                string ipaddress = "";

                MainScreen.main.addToList(name,version,udid,os,model,status,connection,ipaddress);
            }
            commonProgress.Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
