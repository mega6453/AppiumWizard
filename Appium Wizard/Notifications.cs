namespace Appium_Wizard
{
    public partial class Notifications : Form
    {
        public Notifications()
        {
            InitializeComponent();
            var result = Database.QueryDataFromNotificationsTable();

            if (result.TryGetValue("DeviceConnected", out string deviceConnectedValue))
            {
                deviceConnectedEnableRadioButton.Checked = deviceConnectedValue == "Enable";
                deviceConnectedDisableRadioButton.Checked = deviceConnectedValue == "Disable";
            }

            if (result.TryGetValue("DeviceDisconnected", out string deviceDisconnectedValue))
            {
                DeviceDisconnectedEnableRadioButton.Checked = deviceDisconnectedValue == "Enable";
                DeviceDisconnectedDisableRadioButton.Checked = deviceDisconnectedValue == "Disable";
            }

            if (result.TryGetValue("Screenshot", out string screenshotValue))
            {
                ScreenshotEnableRadioButton.Checked = screenshotValue == "Enable";
                ScreenshotDisableRadioButton.Checked = screenshotValue == "Disable";
            }

            if (result.TryGetValue("ScreenRecording", out string screenRecordingValue))
            {
                ScreenRecordingEnableRadioButton.Checked = screenRecordingValue == "Enable";
                ScreenRecordingDisableRadioButton.Checked = screenRecordingValue == "Disable";
            }

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            string deviceConnected = deviceConnectedEnableRadioButton.Checked ? "Enable" : "Disable";
            string deviceDisconnected = DeviceDisconnectedEnableRadioButton.Checked ? "Enable" : "Disable";
            string screenshot = ScreenshotEnableRadioButton.Checked ? "Enable" : "Disable";
            string screenRecording = ScreenRecordingEnableRadioButton.Checked ? "Enable" : "Disable";
            Database.UpdateDataIntoNotificationsTable(deviceConnected, deviceDisconnected, screenshot, screenRecording);

            MainScreen.DeviceConnectedNotification = deviceConnected.Equals("Enable");
            MainScreen.DeviceDisconnectedNotification = deviceDisconnected.Equals("Enable");
            MainScreen.ScreenshotNotification = screenshot.Equals("Enable");
            MainScreen.ScreenRecordingNotification = screenRecording.Equals("Enable");
            Close();
        }

        private void Notifications_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Notifications_Shown");
        }
    }
}
