using System.Reflection;

namespace Appium_Wizard
{
    public partial class DeviceInformation : Form
    {
        MainScreen mainScreen;
        public DeviceInformation(MainScreen mainScreen)
        {
            this.mainScreen = mainScreen;
            InitializeComponent();
        }

        public ListView infoListView
        {
            get { return DeviceInfolistView; }
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            GoogleAnalytics.SendEvent("DeviceInformation_Cancel_Clicked");
        }

        private void Add_Click(object sender, EventArgs e)
        {
            string DeviceName = "", OSVersion = "", OSType = "", udid = "", Model = "", Connection = "", IPAddress = "";
            int Width = 0, Height = 0;
            foreach (ListViewItem item in infoListView.Items)
            {
                if (!infoListView.Items.ContainsKey("IP Address"))
                {
                    IPAddress = "";
                }
                string key = item.SubItems[0].Text;
                string value = item.SubItems[1].Text;
                if (key.Equals("Name"))
                {
                    DeviceName = value;
                }
                else if (key.Equals("Version"))
                {
                    OSVersion = value;
                }
                else if (key.Equals("OS"))
                {
                    OSType = value;
                }
                else if (key.Equals("Udid"))
                {
                    udid = value;
                }
                else if (key.Equals("Model"))
                {
                    Model = value;
                }
                else if (key.Equals("Width"))
                {
                    Width = int.Parse(value);
                }
                else if (key.Equals("Height"))
                {
                    Height = int.Parse(value);
                }
                else if (key.Equals("Connection Type"))
                {
                    Connection = value;
                }
                else if (key.Equals("IP Address"))
                {
                    IPAddress = value;
                }
            }
            Hide();
            Database.InsertDataIntoDevicesTable(DeviceName.Replace("'", "''"), OSType, OSVersion, "Online", udid, Width, Height, Connection, IPAddress);
            mainScreen.addToList(DeviceName, OSVersion, udid, OSType, Model, "Online", Connection, IPAddress);
            if (OSType.ToLower().Contains("ios"))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Model", Model);
                dic.Add("OSVersion", OSVersion);
                dic.Add("ConnectionType", Connection);
                GoogleAnalytics.SendEvent("Device_Added_iOS", dic);
            }
            else
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("Model", Model);
                dic.Add("OSVersion", OSVersion);
                dic.Add("ConnectionType", Connection);
                GoogleAnalytics.SendEvent("Device_Added_Android", dic);
            }
            Close();
        }

        private void DeviceInformation_Shown(object sender, EventArgs e)
        {
           GoogleAnalytics.SendEvent("DeviceInformation_Shown");
        }
    }
}
