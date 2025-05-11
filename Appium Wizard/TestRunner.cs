using Newtonsoft.Json.Linq;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class TestRunner : Form
    {
        private List<Tuple<string, Dictionary<string, string>>> actionData = new List<Tuple<string, Dictionary<string, string>>>();
        private Dictionary<string, string> deviceNameToUdidMap = new Dictionary<string, string>(); // Mapping of device names to UDIDs
        private Dictionary<string, string> deviceNameToOsTypeMap = new Dictionary<string, string>();
        private List<string> deviceNames = new List<string>();
        //private List<string> deviceNames = new List<string> { "Device1", "Device2", "Device3" };
        bool isAndroid;
        string selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP;
        string URL;
        int port;
        // isandroid and selectedeviceudid will have issues when set device is set 2nd time with different device.. need to set these varibales while startin performactions.. 
        public TestRunner()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var devicesList = Database.QueryDataFromDevicesTable();
            foreach (var device in devicesList)
            {
                string deviceName = device["Name"];
                string deviceUdid = device["UDID"];
                string osType = device["OS"]; // Assuming the database contains an "OS" field

                deviceNames.Add(deviceName);
                deviceNameToUdidMap[deviceName] = deviceUdid; // Populate the mapping
                deviceNameToOsTypeMap[deviceName] = osType;   // Populate the OS type mapping
            }
            comboBoxActions.SelectedItem = "Set Device";
            commandGridView.Columns[0].Width = commandGridView.Width - 5;
            propertyGridView.Columns[0].Width = (propertyGridView.Width / 2) - 5;
            propertyGridView.Columns[1].Width = (propertyGridView.Width / 2);
        }

        private void ComboBoxActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedAction = comboBoxActions.SelectedItem.ToString();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            // Add properties based on the selected action
            switch (selectedAction)
            {
                case "Click Element":
                    properties.Add("XPath", "");
                    break;
                case "Send Text":
                    properties.Add("XPath", "");
                    properties.Add("Text to Enter", "");
                    break;
                case "Set Device":
                    properties.Add("Device Name", "");
                    break;
                case "Wait for element visible":
                    properties.Add("XPath", "");
                    properties.Add("Timeout (ms)", "");
                    break;
                case "Wait for element to vanish":
                    properties.Add("XPath", "");
                    properties.Add("Timeout (ms)", "");
                    break;
                case "Sleep":
                    properties.Add("Duration (ms)", "");
                    break;
                case "Install App":
                    properties.Add("App Path", "");
                    break;
                case "Launch App":
                    properties.Add("App Package", "");
                    properties.Add("App Activity", "");
                    break;
                case "Kill App":
                    properties.Add("App Package", "");
                    break;
                case "Uninstall App":
                    properties.Add("App Package", "");
                    break;
                case "Execute Script":
                    properties.Add("Script", "");
                    break;
                case "Take Screenshot":
                    break;
                case "Device Action":
                    properties.Add("Action", "");
                    break;
            }

            // Add the action and its properties to the data list
            actionData.Add(new Tuple<string, Dictionary<string, string>>(selectedAction, properties));

            // Add the action to DataGridView1
            int rowIndex = commandGridView.Rows.Add(selectedAction);
            commandGridView.ClearSelection();
            DataGridViewRow newRow = commandGridView.Rows[rowIndex];
            // Highlight the newly created row
            newRow.Selected = true;
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (commandGridView.SelectedRows.Count > 0)
                {
                    int selectedIndex = commandGridView.SelectedRows[0].Index;

                    // Clear DataGridView2 rows
                    propertyGridView.Rows.Clear();

                    // Load properties for the selected action
                    var selectedActionData = actionData[selectedIndex];
                    string actionName = selectedActionData.Item1; // Assuming Item1 holds the action name

                    foreach (var property in selectedActionData.Item2)
                    {
                        var row = new DataGridViewRow();
                        row.CreateCells(propertyGridView);

                        row.Cells[0].Value = property.Key;


                        if (actionName == "Set Device" && property.Key == "Device Name")
                        {
                            // Create a ComboBox cell for the value field
                            var comboBoxCell = new DataGridViewComboBoxCell
                            {
                                DataSource = deviceNames // Populate with device names
                            };
                            comboBoxCell.Value = property.Value;
                            row.Cells[1] = comboBoxCell;
                        }
                        else if (actionName == "Device Action")
                        {
                            var comboBoxCell = new DataGridViewComboBoxCell();

                            List<string> items = new List<string> { "Home", "Back" };
                            comboBoxCell.DataSource = items;
                            comboBoxCell.Value = property.Value;
                            row.Cells[1] = comboBoxCell;
                        }
                        else
                        {
                            // Create a standard TextBox cell for the value field
                            row.Cells[1].Value = property.Value;
                        }

                        propertyGridView.Rows.Add(row);
                    }
                    ValidateFields(selectedIndex);
                }
            }
            catch (Exception ex)
            {
                // Log or handle exception
                Console.WriteLine(ex.Message);
            }
        }


        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure a row is selected in DataGridView1 and a cell is edited in DataGridView2
            if (commandGridView.SelectedRows.Count > 0 && e.RowIndex >= 0)
            {
                int selectedIndex = commandGridView.SelectedRows[0].Index;

                // Update the property value in the data list
                var property = propertyGridView.Rows[e.RowIndex].Cells[0].Value.ToString();
                var value = propertyGridView.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
                actionData[selectedIndex].Item2[property] = value;
                if (property == "Device Name")
                {
                    commandGridView.Rows[0].Tag = deviceNameToUdidMap[value];
                }


                // Check if the property being updated is "Device Name"
                //if (property == "Device Name" && deviceNameToUdidMap.ContainsKey(value))
                //{
                //    selectedUDID = deviceNameToUdidMap[value]; // Update the selected UDID
                //    Console.WriteLine($"Selected Device UDID: {selectedUDID}"); // For debugging

                //    // Update the OS type based on the selected device name
                //    if (deviceNameToOsTypeMap.ContainsKey(value))
                //    {
                //        string osType = deviceNameToOsTypeMap[value];
                //        isAndroid = osType.Equals("Android", StringComparison.OrdinalIgnoreCase);
                //        Console.WriteLine($"Selected OS Type: {osType}, isAndroid: {isAndroid}"); // For debugging
                //    }
                //}

                // Update the corresponding cell in DataGridView1
                //commandGridView.Rows[selectedIndex].Cells[0].Value = $"{property}: {value}";
                commandGridView.Rows[selectedIndex].Cells[0].Value = FormatActionText(selectedIndex);
                ValidateFields(selectedIndex);
            }
        }


        private void ValidateFields(int selectedIndex)
        {
            var selectedActionData = actionData[selectedIndex];
            bool hasEmptyFields = selectedActionData.Item2.Values.Any(value => string.IsNullOrWhiteSpace(value));

            if (hasEmptyFields)
            {
                commandGridView.Rows[selectedIndex].ErrorText = "Some fields are empty.";
                commandGridView.Rows[selectedIndex].DefaultCellStyle.BackColor = Color.LightPink; // Highlight row
            }
            else
            {
                commandGridView.Rows[selectedIndex].ErrorText = string.Empty;
                commandGridView.Rows[selectedIndex].DefaultCellStyle.BackColor = Color.White; // Reset row color
            }
        }

        private void DataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                // Get the index of the row being deleted
                int rowIndex = e.Row.Index;
                // Prevent deletion of the first row
                if (rowIndex == 0)
                {
                    MessageBox.Show("You cannot delete the default row.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                else
                {
                    // Remove the corresponding entry from actionData
                    actionData.RemoveAt(rowIndex);

                    // Clear DataGridView2 if the deleted row is selected
                    if (commandGridView.SelectedRows.Count > 0 && commandGridView.SelectedRows[0].Index == rowIndex)
                    {
                        propertyGridView.Rows.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while deleting row: {ex.Message}");
            }
        }
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var comboBoxCell = propertyGridView[e.ColumnIndex, e.RowIndex] as DataGridViewComboBoxCell;

            if (comboBoxCell != null)
            {
                // Set a default value if it's null
                comboBoxCell.Value = comboBoxCell.Items[0]; // Or set it to a meaningful default value
            }
        }

        private async void runOnceButton_Click(object sender, EventArgs e)
        {
            var isAnyError = checkIfAnyErrors();
            if (isAnyError)
            {
                MessageBox.Show("Please check the rows with errors and fix it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //OpenDevice openDevice = new OpenDevice(selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP);
                //var isStarted = await openDevice.StartBackgroundTasks();

                selectedUDID = commandGridView.Rows[0].Tag as string;
                if (ScreenControl.devicePorts.ContainsKey(selectedUDID))
                {
                    port = ScreenControl.devicePorts[selectedUDID].Item2;
                    URL = "http://127.0.0.1:" + port;
                    performActions();
                }
                else
                {
                    MessageBox.Show("Please open device and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private async void repeatButton_Click(object sender, EventArgs e)
        {
            var isAnyError = checkIfAnyErrors();
            if (isAnyError)
            {
                MessageBox.Show("Please check the rows with errors and fix it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the number of repetitions:", "Repeat Action", "1");
                if (string.IsNullOrEmpty(input)) // Handle Cancel or empty input
                {
                    // User clicked "Cancel" or left the input empty, simply exit the method
                    return;
                }
                if (int.TryParse(input, out int repetitions))
                {
                    if (repetitions <= 0)
                    {
                        MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //OpenDevice openDevice = new OpenDevice(selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP);
                        //var isStarted = await openDevice.StartBackgroundTasks();

                        selectedUDID = commandGridView.Rows[0].Tag as string;
                        if (ScreenControl.devicePorts.ContainsKey(selectedUDID))
                        {
                            port = ScreenControl.devicePorts[selectedUDID].Item2;
                            URL = "http://127.0.0.1:" + port;
                            for (int i = 0; i < repetitions; i++)
                            {
                                performActions();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please open device and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }                        
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private bool checkIfAnyErrors()
        {
            foreach (DataGridViewRow row in commandGridView.Rows)
            {
                // Check if the row already has an error
                if (!string.IsNullOrWhiteSpace(row.ErrorText))
                {
                    return true; // An error exists
                }
            }

            return false; // No errors found
        }

        private async void performActions()
        {
            string sessionId = string.Empty;
            if (isAndroid)
            {
                sessionId = AndroidAPIMethods.GetSessionID(port);
            }
            else
            {
                sessionId = iOSAPIMethods.GetWDASessionID(URL);
            }
            foreach (var action in actionData)
            {
                string actionType = action.Item1;
                Dictionary<string, string> properties = action.Item2;

                StringBuilder message = new StringBuilder();
                message.AppendLine($"Action Type: {actionType}");
                message.AppendLine("Properties:");

                foreach (var property in properties)
                {
                    message.AppendLine($"  {property.Key}: {property.Value}");
                }

                switch (actionType)
                {
                    case "Click Element":
                        if (isAndroid)
                        {
                            AndroidAPIMethods.ClickElement(properties["XPath"]);
                        }
                        else
                        {
                            iOSAPIMethods.ClickElement(URL, sessionId, properties["XPath"]);
                        }
                        break;
                    case "Send Text":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            iOSAPIMethods.SendText(URL, sessionId, properties["XPath"], properties["Text to Enter"]);
                        }
                        break;
                    case "Set Device":
                        if (deviceNameToUdidMap.ContainsKey(properties["Device Name"]))
                        {
                            selectedUDID = deviceNameToUdidMap[properties["Device Name"]]; // Update the selected UDID
                            Console.WriteLine($"Selected Device UDID: {selectedUDID}"); // For debugging

                            // Update the OS type based on the selected device name
                            if (deviceNameToOsTypeMap.ContainsKey(properties["Device Name"]))
                            {
                                string osType = deviceNameToOsTypeMap[properties["Device Name"]];
                                isAndroid = osType.Equals("Android", StringComparison.OrdinalIgnoreCase);
                                Console.WriteLine($"Selected OS Type: {osType}, isAndroid: {isAndroid}"); // For debugging
                            }
                        }
                        break;
                    case "Wait for element visible":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            int timeout = int.TryParse(properties["Timeout (ms)"], out var parsedTimeout) ? parsedTimeout : 0;
                            await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], timeout);
                        }
                        break;
                    case "Wait for element to vanish":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            int timeout = int.TryParse(properties["Timeout (ms)"], out var parsedTimeout) ? parsedTimeout : 0;
                            await WaitUntilElementVanished(URL, sessionId, properties["XPath"], timeout);
                        }
                        break;
                    case "Sleep":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            int duration = int.TryParse(properties["Duration (ms)"], out var parsedTimeout) ? parsedTimeout : 0;
                            await Task.Delay(duration);
                        }
                        break;
                    case "Install App":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            iOSAsyncMethods.GetInstance().InstallApp(selectedUDID, properties["App Path"]);
                        }
                        break;
                    case "Launch App":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            iOSAPIMethods.LaunchApp(URL, sessionId, properties["App Package"]);
                        }
                        break;
                    case "Kill App":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            iOSAPIMethods.KillApp(URL, sessionId, properties["App Package"]);
                        }
                        break;
                    case "Uninstall App":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            iOSMethods.GetInstance().UninstallApp(selectedUDID, properties["App Package"]);
                        }
                        break;
                    case "Execute Script":
                        if (isAndroid)
                        {

                        }
                        else
                        {

                        }
                        break;
                    case "Take Screenshot":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                            string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mmtt");
                            string filePath = Path.Combine(downloadPath, $"Screenshot_{selectedDeviceName}_{timestamp}.png");
                            filePath = "\"" + filePath + "\"";
                            iOSAPIMethods.TakeScreenshot(URL, filePath.Replace("\"", ""));
                        }
                        break;
                    case "Device Action":
                        if (isAndroid)
                        {

                        }
                        else
                        {
                            if (properties["Action"].Equals("Home"))
                            {
                                iOSAPIMethods.GoToHome(port);
                            }
                        }
                        break;
                    default:
                        MessageBox.Show($"Unknown Command: {actionType}", "Unknown Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
        }

        private string FormatActionText(int actionIndex)
        {
            var action = actionData[actionIndex];
            string actionType = action.Item1;
            Dictionary<string, string> properties = action.Item2;

            switch (actionType)
            {
                case "Click Element":
                    return $"Click element at {properties["XPath"]}";
                case "Send Text":
                    return $"Send text \"{properties["Text to Enter"]}\" to {properties["XPath"]}";
                case "Wait for Element":
                    return $"Wait for element at {properties["XPath"]} for {properties["Timeout (ms)"]} ms";
                case "Set Device":
                    return $"Set device to {properties["Device Name"]}";
                case "Wait for element visible":
                    return $"Wait for element visible at {properties["XPath"]} for {properties["Timeout (ms)"]} ms";
                case "Wait for element to vanish":
                    return $"Wait for element to vanish at {properties["XPath"]} for {properties["Timeout (ms)"]} ms";
                case "Sleep":
                    return $"Sleep for {properties["Duration (ms)"]} milli seconds";
                case "Install App":
                    return $"Install app from {properties["App Path"]}";
                case "Launch App":
                    return $"Launch app with package {properties["App Package"]} and activity {properties["App Activity"]}";
                case "Kill App":
                    return $"Kill app with package {properties["App Package"]}";
                case "Uninstall App":
                    return $"Uninstall app with package {properties["App Package"]}";
                case "Execute Script":
                    return $"Execute script: {properties["Script"]}";
                case "Take Screenshot":
                    return $"Take screenshot and save to {properties["Save Path"]}";
                case "Device Action":
                    return $"Perform device action : \"{properties["Action"]}\"";
                default:
                    return $"{actionType}: \"{properties["Action"]}\"";
            }
        }

        public async Task<bool> WaitUntilElementDisplayed(string url, string sessionId, string xPath, int timeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                // Call the API asynchronously
                if (await Task.Run(() => iOSAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                {
                    // Stop the loop if the API returns true
                    return true;
                }

                // Add a small delay to avoid tight looping (optional)
                await Task.Delay(100); // 100 milliseconds
            }

            // Timeout reached, return false
            return false;
        }

        public async Task<bool> WaitUntilElementVanished(string url, string sessionId, string xPath, int timeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                // Call the API asynchronously
                if (!await Task.Run(() => iOSAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                {
                    // Stop the loop if the API returns false, meaning the element has vanished
                    return true;
                }

                // Add a small delay to avoid tight looping (optional)
                await Task.Delay(100); // 100 milliseconds
            }

            // Timeout reached, return false
            return false;
        }

    }
}
