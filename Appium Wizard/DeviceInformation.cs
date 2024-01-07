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
        }

        private void Add_Click(object sender, EventArgs e)
        {
            string DeviceName = "", OSVersion = "", OSType = "", udid = "", Model = "";
            int Width = 0, Height = 0;
            foreach (ListViewItem item in infoListView.Items)
            {
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
            }
            Database.InsertDataIntoDevicesTable(DeviceName.Replace("'", "''"), OSType, OSVersion, "Online", udid, Width, Height);
            mainScreen.addToList(DeviceName, OSVersion, udid, OSType, Model, "Online");
            Close();
        }

    }
}
