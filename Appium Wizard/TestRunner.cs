using Newtonsoft.Json;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Text;

namespace Appium_Wizard
{
    public partial class TestRunner : Form
    {
        private List<Tuple<string, Dictionary<string, string>>> actionData = new List<Tuple<string, Dictionary<string, string>>>();
        private static Dictionary<string, string> deviceNameToUdidMap = new Dictionary<string, string>();
        private Dictionary<string, string> deviceNameToOsTypeMap = new Dictionary<string, string>();
        private Dictionary<string, string> deviceUDIDToOsTypeMap = new Dictionary<string, string>();
        private List<string> deviceNames = new List<string>();
        bool isAndroid;
        string? selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP, URL;
        int port;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isRunning = false;
        private string scriptFilePath = string.Empty;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        string htmlReportPath;
        string reportsFolderPath;

        // isandroid and selectedeviceudid will have issues when set device is set 2nd time with different device.. need to set these varibales while startin performactions.. 
        public TestRunner()
        {
            InitializeComponent();
            commandGridView.Columns[0].Width = (int)(commandGridView.Width * 0.05);
            commandGridView.Columns[1].Width = (int)(commandGridView.Width * 0.95);
            propertyGridView.Columns[0].Width = (int)(propertyGridView.Width * 0.4);
            propertyGridView.Columns[1].Width = (int)(propertyGridView.Width * 0.6);
            reportsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Appium-Wizard-Reports");
            if (!Directory.Exists(reportsFolderPath))
            {
                Directory.CreateDirectory(reportsFolderPath);
            }
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
                deviceUDIDToOsTypeMap[deviceUdid] = osType;
            }
        }

        private List<bool> actionActiveStates = new List<bool>();
        private void ComboBoxActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedAction = comboBoxActions.SelectedItem.ToString();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            // Add properties based on the selected action
            switch (selectedAction)
            {
                case "Click Element":
                    properties.Add("XPath", "");
                    properties.Add("Wait For Element Visible (ms)", "5000");
                    break;
                case "Send Text":
                    properties.Add("XPath", "");
                    properties.Add("Text to Enter", "");
                    properties.Add("Wait For Element Visible (ms)", "5000");
                    break;
                case "Send Text With Random Values":
                    properties.Add("XPath", "");
                    properties.Add("Text Type", "Random Number");
                    properties.Add("Number of digits/characters", "");
                    properties.Add("Wait For Element Visible (ms)", "5000");
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
                    properties.Add("App BundleId(iOS)/Activity(Android)", "");
                    break;
                case "Kill App":
                    properties.Add("App Package", "");
                    break;
                case "Uninstall App":
                    properties.Add("App Package", "");
                    break;
                case "Take Screenshot":
                    // No properties
                    break;
                case "Device Action":
                    properties.Add("Action", "");
                    break;
            }

            // Add the action and its properties to the data list
            actionData.Add(new Tuple<string, Dictionary<string, string>>(selectedAction, properties));
            actionActiveStates.Add(true);

            // Add the action to DataGridView1
            int rowIndex = commandGridView.Rows.Add(); // Add a new row
            if (selectedAction == "Take Screenshot")
            {
                commandGridView.Rows[rowIndex].Cells[1].Value = "Take Screenshot and attach it to report";
            }
            else
            {
                commandGridView.Rows[rowIndex].Cells[1].Value = selectedAction; // Set the value in column 1
            }
            commandGridView.ClearSelection();
            DataGridViewRow newRow = commandGridView.Rows[rowIndex];
            // Highlight the newly created row
            newRow.Selected = true;
            newRow.Cells[0].Value = true;
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
                    StringBuilder propertyInfoBuilder = new StringBuilder();
                    foreach (var property in selectedActionData.Item2)
                    {
                        var row = new DataGridViewRow();
                        row.CreateCells(propertyGridView);

                        row.Cells[0].Value = property.Key;

                        if (actionName == "Send Text With Random Values" && property.Key == "Text Type")
                        {
                            // Create a ComboBox cell for the "Text Type" field
                            var comboBoxCell = new DataGridViewComboBoxCell
                            {
                                DataSource = new List<string> { "Random Number", "Random Alphabets", "Random Alphanumeric" }, // Dropdown options
                                Value = property.Value // Default value
                            };

                            // Validate the value to ensure it matches one of the dropdown options
                            if (comboBoxCell.DataSource is List<string> dataSource && !dataSource.Contains(comboBoxCell.Value))
                            {
                                comboBoxCell.Value = dataSource.FirstOrDefault(); // Set to the first valid option if invalid
                            }

                            row.Cells[1] = comboBoxCell;
                        }
                        else if (actionName == "Send Text With Random Values" && property.Key == "Number of digits/characters")
                        {
                            // Create a standard TextBox cell for "Number of digits/characters"
                            row.Cells[1].Value = property.Value;
                        }
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

                        // Append property description to the label text
                        if (propertyDescriptions.ContainsKey(property.Key))
                        {
                            propertyInfoBuilder.AppendLine($"{property.Key}: {propertyDescriptions[property.Key]}");
                            propertyInfoBuilder.AppendLine();
                        }
                    }
                    // Update the propertyInfo label with the constructed information
                    propertyInfo.Text = propertyInfoBuilder.ToString();
                    ValidateFields(selectedIndex);
                    if (actionName == "Take Screenshot")
                    {
                        propertyInfo.Text = "Take Screenshot and attach it to report";
                    }
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
                    commandGridView.Rows[selectedIndex].Tag = deviceNameToUdidMap[value];
                }

                if (property == "Text Type")
                {
                    var validOptions = new List<string> { "Random Number", "Random Alphabets", "Random Alphanumeric" };

                    // Validate the new value
                    if (!validOptions.Contains(value))
                    {
                        MessageBox.Show($"Invalid value for Text Type: {value}", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // No need to update the key; we keep the original key ("Number of digits/characters")
                    // The value of "Number of digits/characters" will be used for all Text Types
                }


                commandGridView.Rows[selectedIndex].Cells[1].Value = FormatActionText(selectedIndex);
                ValidateFields(selectedIndex);
            }
        }

        private void RefreshPropertyGrid(int selectedIndex)
        {
            propertyGridView.Rows.Clear();

            var selectedActionData = actionData[selectedIndex];
            string actionName = selectedActionData.Item1;

            foreach (var property in selectedActionData.Item2)
            {
                var row = new DataGridViewRow();
                row.CreateCells(propertyGridView);

                row.Cells[0].Value = property.Key;

                if (actionName == "Send Text With Random Values" && property.Key == "Text Type")
                {
                    var comboBoxCell = new DataGridViewComboBoxCell
                    {
                        DataSource = new List<string> { "Random Number", "Random Alphabets", "Random Alphanumeric" }, // Dropdown options
                        Value = property.Value // Ensure this value matches one of the dropdown options
                    };

                    // Validate the value to prevent the exception
                    if (!comboBoxCell.Items.Contains(comboBoxCell.Value))
                    {
                        comboBoxCell.Value = comboBoxCell.Items[0]; // Set to the first valid option if invalid
                    }

                    row.Cells[1] = comboBoxCell;
                }
                else if (actionName == "Send Text With Random Values")
                {
                    row.Cells[1].Value = property.Value;
                }
                else
                {
                    row.Cells[1].Value = property.Value;
                }

                propertyGridView.Rows.Add(row);
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
                    actionActiveStates.RemoveAt(rowIndex);
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


        private bool checkIfAnyErrors()
        {
            foreach (DataGridViewRow row in commandGridView.Rows)
            {
                // Check if the row already has an error
                if (!string.IsNullOrWhiteSpace(row.ErrorText) && row.Cells[0].Value is bool cellValue && cellValue)
                {
                    return true; // An error exists
                }
            }

            return false; // No errors found
        }

        private async void runOnceButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                // Stop execution
                cancellationTokenSource?.Cancel();
                runOnceButton.Text = "Run Once";
                repeatButton.Enabled = true;
                isRunning = false;
                UpdateScreenControl("");
                return;
            }

            // Start execution
            isRunning = true;
            runOnceButton.Text = "Stop";
            repeatButton.Enabled = false;
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var isAnyError = checkIfAnyErrors();
                if (isAnyError)
                {
                    MessageBox.Show("Please check the rows with errors and fix it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    runOnceButton.Text = "Run Once";
                    repeatButton.Enabled = true;
                    isRunning = false;
                    UpdateScreenControl("");
                    return;
                }
                GoogleAnalytics.SendEvent("RunOnce_TestRunner");
                string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mm.ss_tt");
                string filePath = Path.Combine(reportsFolderPath, $"TestRunner_{selectedDeviceName}_{timestamp}.html");
                htmlReportPath = filePath;
                CreateHtmlReport();
                await performActions(cancellationTokenSource.Token);
                FinalizeHtmlReport();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Execution stopped.", "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                runOnceButton.Text = "Run Once";
                repeatButton.Enabled = true;
                isRunning = false;
                UpdateScreenControl("");
            }
        }

        bool erroShown = false;
        private async void repeatButton_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                // Stop execution
                cancellationTokenSource?.Cancel();
                repeatButton.Text = "Repeat";
                runOnceButton.Enabled = true;
                isRunning = false;
                UpdateScreenControl("");
                return;
            }

            // Start execution
            isRunning = true;
            repeatButton.Text = "Stop";
            runOnceButton.Enabled = false;
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var isAnyError = checkIfAnyErrors();
                if (isAnyError)
                {
                    MessageBox.Show("Please check the rows with errors and fix it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    repeatButton.Text = "Repeat";
                    runOnceButton.Enabled = true;
                    isRunning = false;
                    UpdateScreenControl("");
                    return;
                }
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
                        GoogleAnalytics.SendEvent("Repeat_TestRunner", repetitions.ToString());
                        string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mm.ss_tt");
                        string filePath = Path.Combine(reportsFolderPath, $"TestRunner_{selectedDeviceName}_{timestamp}.html");
                        htmlReportPath = filePath;
                        CreateHtmlReport(repetitions);
                        repeatCountLabel.Text = $"0/{repetitions}";
                        for (int i = 1; i <= repetitions; i++)
                        {
                            if (!erroShown)
                            {
                                await performActions(cancellationTokenSource.Token);
                                repeatCountLabel.Text = $"{i}/{repetitions}";
                            }
                        }
                        erroShown = false;
                        FinalizeHtmlReport();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Execution stopped.", "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                repeatButton.Text = "Repeat";
                runOnceButton.Enabled = true;
                isRunning = false;
                UpdateScreenControl("");
            }
        }

        HashSet<int> usedRandomNumbers = new HashSet<int>(); // Track used random numbers
        private async Task performActions(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
            {
                string sessionId = string.Empty;
                Random random = new Random(); // Random number generator

                for (int i = 0; i < actionData.Count; i++)
                {
                    if (!actionActiveStates[i]) continue; // Skip inactive actions

                    var action = actionData[i];
                    string actionType = action.Item1;
                    Dictionary<string, string> properties = action.Item2;

                    if (actionType == "Set Device")
                    {
                        selectedUDID = commandGridView.Rows[i].Tag as string; // Get UDID from the current "Set Device" row
                        selectedDeviceName = GetDeviceNameByUdid(selectedUDID);
                        //Text = "Test Runner - " + selectedDeviceName;
                        if (deviceUDIDToOsTypeMap.ContainsKey(selectedUDID))
                        {
                            string osType = deviceUDIDToOsTypeMap[selectedUDID];
                            isAndroid = osType.Equals("Android", StringComparison.OrdinalIgnoreCase);
                        }
                        if (ScreenControl.devicePorts.ContainsKey(selectedUDID))
                        {
                            port = ScreenControl.devicePorts[selectedUDID].Item2;
                            URL = "http://127.0.0.1:" + port;
                            if (isAndroid)
                            {
                                sessionId = AndroidAPIMethods.GetSessionID(port);
                            }
                            else
                            {
                                sessionId = iOSAPIMethods.GetWDASessionID(URL);
                            }
                            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrWhiteSpace(sessionId) || sessionId.Equals("nosession"))
                            {
                                erroShown = true;
                                MessageBox.Show("Please open the device " + selectedDeviceName + " from Main screen and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            erroShown = true;
                            MessageBox.Show("Please open the device " + selectedDeviceName + " from Main screen and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    UpdateScreenControl("");
                    switch (actionType)
                    {
                        case "Click Element":
                            int clickTimeout = int.TryParse(properties["Wait For Element Visible (ms)"], out var clickParsedTimeout) ? clickParsedTimeout : 0;
                            string clickCommand = "Click " + properties["XPath"] + " | timeout:" + clickTimeout;
                            UpdateScreenControl(clickCommand);
                            await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], clickTimeout, cancellationToken);
                            string clickResponse;
                            if (isAndroid)
                            {
                                clickResponse = AndroidAPIMethods.ClickElement(URL, sessionId, properties["XPath"]);
                            }
                            else
                            {
                                clickResponse = iOSAPIMethods.ClickElement(URL, sessionId, properties["XPath"]);
                            }
                            AppendToHtmlReport(clickCommand, clickResponse);
                            break;

                        case "Send Text":
                            int sendTextTimeout = int.TryParse(properties["Wait For Element Visible (ms)"], out var sendTextParsedTimeout) ? sendTextParsedTimeout : 0;
                            string sendTextCommand = "Send Text " + properties["Text to Enter"] + " to " + properties["XPath"] + " | timeout:" + sendTextTimeout;
                            UpdateScreenControl(sendTextCommand);
                            string sendTextResponse;
                            await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], sendTextTimeout, cancellationToken);
                            if (isAndroid)
                            {
                                sendTextResponse = AndroidAPIMethods.SendText(URL, sessionId, properties["XPath"], properties["Text to Enter"]);
                            }
                            else
                            {
                                sendTextResponse = iOSAPIMethods.SendText(URL, sessionId, properties["XPath"], properties["Text to Enter"]);
                            }
                            AppendToHtmlReport(sendTextCommand, sendTextResponse);
                            break;

                        case "Send Text With Random Values":
                            string textType = properties["Text Type"];
                            string textToEnter;
                            int sendTextRandomValuesTimeout = int.TryParse(properties["Wait For Element Visible (ms)"], out var sendTextRandomParsedTimeout) ? sendTextRandomParsedTimeout : 0;
                            if (textType == "Random Number")
                            {
                                if (properties.TryGetValue("Number of digits/characters", out string numDigitsStr) && int.TryParse(numDigitsStr, out int numDigits))
                                {
                                    if (numDigits <= 0 || numDigits > 9)
                                    {
                                        MessageBox.Show("Number of digits must be between 1 and 9 in \"Send Text With Random Values\" row", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    int minValue = (int)Math.Pow(10, numDigits - 1);
                                    int maxValue = (int)Math.Pow(10, numDigits) - 1;

                                    textToEnter = random.Next(minValue, maxValue + 1).ToString();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid number of digits for random number generation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else if (textType == "Random Alphabets")
                            {
                                if (properties.TryGetValue("Number of digits/characters", out string numCharsStr) && int.TryParse(numCharsStr, out int numChars))
                                {
                                    textToEnter = new string(Enumerable.Range(0, numChars).Select(_ => (char)random.Next('A', 'Z' + 1)).ToArray());
                                }
                                else
                                {
                                    MessageBox.Show("Invalid number of characters for random alphabet generation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else if (textType == "Random Alphanumeric")
                            {
                                if (properties.TryGetValue("Number of digits/characters", out string numCharsStr) && int.TryParse(numCharsStr, out int numChars))
                                {
                                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                                    textToEnter = new string(Enumerable.Range(0, numChars).Select(_ => chars[random.Next(chars.Length)]).ToArray());
                                }
                                else
                                {
                                    MessageBox.Show("Invalid number of characters for random alphanumeric generation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                            else
                            {
                                textToEnter = properties["Text to Enter"];
                            }
                            string sendTextRandomValuesCommand = "Send Random Text to " + properties["XPath"] + " | timeout:" + sendTextRandomValuesTimeout;
                            string sendTextRandomValuesResponse;
                            UpdateScreenControl(sendTextRandomValuesCommand);
                            await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], sendTextRandomValuesTimeout, cancellationToken);
                            if (isAndroid)
                            {
                                sendTextRandomValuesResponse = AndroidAPIMethods.SendText(URL, sessionId, properties["XPath"], textToEnter);
                            }
                            else
                            {
                                sendTextRandomValuesResponse = iOSAPIMethods.SendText(URL, sessionId, properties["XPath"], textToEnter);
                            }
                            AppendToHtmlReport(sendTextRandomValuesCommand, sendTextRandomValuesResponse);
                            break;

                        case "Set Device":
                            string SetDeviceCommand = "Set Device - " + properties["Device Name"];
                            UpdateScreenControl(SetDeviceCommand);
                            if (deviceNameToUdidMap.ContainsKey(properties["Device Name"]))
                            {
                                selectedUDID = deviceNameToUdidMap[properties["Device Name"]]; // Update the selected UDID
                                // Update the OS type based on the selected device name
                                if (deviceNameToOsTypeMap.ContainsKey(properties["Device Name"]))
                                {
                                    string osType = deviceNameToOsTypeMap[properties["Device Name"]];
                                    isAndroid = osType.Equals("Android", StringComparison.OrdinalIgnoreCase);
                                }
                            }
                            AppendToHtmlReport(SetDeviceCommand, "OK");
                            break;

                        case "Wait for element visible":
                            int visibleTimeout = int.TryParse(properties["Timeout (ms)"], out var visibleParsedTimeout) ? visibleParsedTimeout : 0;
                            string waitForElementVisibleCommand = "Wait for element visible - " + properties["XPath"] + " - " + visibleTimeout;
                            UpdateScreenControl(waitForElementVisibleCommand);
                            bool waitForElementVisibleResult = await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], visibleTimeout, cancellationToken);
                            AppendToHtmlReport(waitForElementVisibleCommand, waitForElementVisibleResult.ToString());
                            break;

                        case "Wait for element to vanish":
                            int vanishTimeout = int.TryParse(properties["Timeout (ms)"], out var vanishParsedTimeout) ? vanishParsedTimeout : 0;
                            string waitForElementVanishCommand = "Wait for element to vanish - " + properties["XPath"] + " - " + vanishTimeout;
                            UpdateScreenControl(waitForElementVanishCommand);
                            bool waitForElementVanishResult = await WaitUntilElementVanished(URL, sessionId, properties["XPath"], vanishTimeout, cancellationToken);
                            AppendToHtmlReport(waitForElementVanishCommand, waitForElementVanishResult.ToString());
                            break;

                        case "Sleep":
                            string SleepCommand = "Sleep " + properties["Duration (ms)"] + " ms";
                            UpdateScreenControl(SleepCommand);
                            int duration = int.TryParse(properties["Duration (ms)"], out var parsedTimeout) ? parsedTimeout : 0;
                            await Task.Delay(duration, cancellationToken);
                            AppendToHtmlReport(SleepCommand, "-");
                            break;

                        case "Install App":
                            string InstallAppCommand = "Install App: " + properties["App Path"];
                            UpdateScreenControl(InstallAppCommand);
                            string appPath = properties["App Path"];
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().InstallApp(selectedUDID, appPath); // not working
                            }
                            else
                            {
                                iOSAsyncMethods.GetInstance().InstallApp(selectedUDID, appPath);
                            }
                            AppendToHtmlReport(InstallAppCommand, "-");
                            break;

                        case "Launch App":
                            string LaunchAppCommand = "Launch App: " + properties["App BundleId(iOS)/Activity(Android)"];
                            UpdateScreenControl(LaunchAppCommand);
                            string LaunchAppResult = "-";
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().LaunchApp(selectedUDID, properties["App BundleId(iOS)/Activity(Android)"]);
                            }
                            else
                            {
                                LaunchAppResult = iOSAPIMethods.LaunchApp(URL, sessionId, properties["App BundleId(iOS)/Activity(Android)"]);
                            }
                            AppendToHtmlReport(LaunchAppCommand, LaunchAppResult);
                            break;

                        case "Kill App":
                            string KillAppCommand = "Kill App: " + properties["App Package"];
                            UpdateScreenControl(KillAppCommand);
                            string KillAppResult = "-";
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().KillApp(selectedUDID, properties["App Package"]);
                            }
                            else
                            {
                                KillAppResult = iOSAPIMethods.KillApp(URL, sessionId, properties["App Package"]);
                            }
                            AppendToHtmlReport(KillAppCommand, KillAppResult);
                            break;

                        case "Uninstall App":
                            string UninstallAppCommand = "Uninstall App: " + properties["App Package"];
                            UpdateScreenControl(UninstallAppCommand);
                            if (isAndroid)
                            {
                                AndroidMethods.GetInstance().UnInstallApp(selectedUDID, properties["App Package"]);
                            }
                            else
                            {
                                iOSMethods.GetInstance().UninstallApp(selectedUDID, properties["App Package"]);
                            }
                            AppendToHtmlReport(UninstallAppCommand, "-");
                            break;

                        case "Take Screenshot":
                            string timestamp = DateTime.Now.ToString("dd-MMM-yyyy_hh.mm.ss_tt");
                            string screenshotFilePath = Path.Combine(reportsFolderPath, $"Screenshot_{selectedDeviceName}_{timestamp}.png");
                            string TakeScreenshotCommand = "Take Screenshot";
                            UpdateScreenControl(TakeScreenshotCommand);
                            string TakeScreenshotStatusDescription = "-";
                            if (isAndroid)
                            {
                                TakeScreenshotStatusDescription = AndroidAPIMethods.TakeScreenshot(port, screenshotFilePath);
                            }
                            else
                            {
                                TakeScreenshotStatusDescription = iOSAPIMethods.TakeScreenshot(URL, screenshotFilePath);
                            }
                            AppendToHtmlReport(TakeScreenshotCommand, TakeScreenshotStatusDescription, screenshotFilePath);
                            break;

                        case "Device Action":
                            string DeviceActionCommand = "Device Action: " + properties["Action"];
                            UpdateScreenControl(DeviceActionCommand);
                            string DeviceActionResult = "-";
                            if (isAndroid)
                            {
                                if (properties["Action"].Equals("Home"))
                                {
                                    AndroidMethods.GetInstance().GoToHome(selectedUDID);
                                }
                                else if (properties["Action"].Equals("Back"))
                                {
                                    AndroidMethods.GetInstance().Back(selectedUDID);
                                }
                            }
                            else
                            {
                                if (properties["Action"].Equals("Home"))
                                {
                                    DeviceActionResult = iOSAPIMethods.GoToHome(port);
                                }
                            }
                            AppendToHtmlReport(DeviceActionCommand, DeviceActionResult);
                            break;

                        default:
                            MessageBox.Show($"Unknown Command: {actionType}", "Unknown Command", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
                UpdateScreenControl("");
            });
        }

        static string GetDeviceNameByUdid(string udid)
        {
            foreach (var kvp in deviceNameToUdidMap)
            {
                if (kvp.Value == udid)
                {
                    return kvp.Key;
                }
            }
            return ""; // Return null if UDID not found
        }

        public void UpdateScreenControl(string statusText)
        {
            try
            {
                if (ScreenControl.udidScreenControl.ContainsKey(selectedUDID))
                {
                    var screenControl = ScreenControl.udidScreenControl[selectedUDID];
                    screenControl.UpdateStatusLabel(screenControl, statusText);
                }
            }
            catch (Exception)
            {
            }
        }

        private string FormatActionText(int actionIndex)
        {
            var action = actionData[actionIndex];
            string actionType = action.Item1;
            Dictionary<string, string> properties = action.Item2;

            switch (actionType)
            {
                case "Send Text":
                    return $"Send text \"{properties["Text to Enter"]}\" to {properties["XPath"]} , timeout: {properties["Wait For Element Visible (ms)"]} ms.";
                case "Send Text With Random Values":
                    return $"Send \"{properties["Text Type"]}\" with \"{properties["Number of digits/characters"]}\" digits/characters to \"{properties["XPath"]}\" , timeout: {properties["Wait For Element Visible (ms)"]} ms.";
                case "Click Element":
                    return $"Click element at {properties["XPath"]} , timeout: {properties["Wait For Element Visible (ms)"]} ms.";
                case "Set Device":
                    return $"Set device to {properties["Device Name"]}";
                case "Wait for element visible":
                    return $"Wait for element visible at {properties["XPath"]} for {properties["Timeout (ms)"]} ms";
                case "Wait for element to vanish":
                    return $"Wait for element to vanish at {properties["XPath"]} for {properties["Timeout (ms)"]} ms";
                case "Sleep":
                    return $"Sleep for {properties["Duration (ms)"]} milliseconds";
                case "Install App":
                    return $"Install app from {properties["App Path"]}";
                case "Launch App":
                    return $"Launch app with BundleId/Activity {properties["App BundleId(iOS)/Activity(Android)"]}";
                case "Kill App":
                    return $"Kill app with package {properties["App Package"]}";
                case "Uninstall App":
                    return $"Uninstall app with package {properties["App Package"]}";
                case "Take Screenshot":
                    return $"Take screenshot and attach it to report";
                case "Device Action":
                    return $"Perform device action: \"{properties["Action"]}\"";
                default:
                    return $"{actionType}: \"{properties["Action"]}\"";
            }
        }

        public async Task<bool> WaitUntilElementDisplayed(string url, string sessionId, string xPath, int timeout, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isAndroid)
                {
                    if (await Task.Run(() => AndroidAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                    {
                        // Stop the loop if the API returns true
                        return true;
                    }
                }
                else
                {
                    if (await Task.Run(() => iOSAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                    {
                        // Stop the loop if the API returns true
                        return true;
                    }
                }
                await Task.Delay(100); // 100 milliseconds
            }

            // Timeout reached, return false
            return false;
        }

        public async Task<bool> WaitUntilElementVanished(string url, string sessionId, string xPath, int timeout, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (isAndroid)
                {
                    if (!await Task.Run(() => AndroidAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!await Task.Run(() => iOSAPIMethods.isElementDisplayed(url, sessionId, xPath)))
                    {
                        return true;
                    }
                }
                await Task.Delay(100); // 100 milliseconds
            }

            // Timeout reached, return false
            return false;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select .json File";
            openFileDialog.Filter = "Json Files (*.json)|*.json";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                LoadScriptFromFile(filePath);
            }
            GoogleAnalytics.SendEvent("Load Script");
        }

        public void LoadScriptFromFile(string filePath)
        {
            try
            {
                // Read the JSON data from the specified file
                string jsonData = File.ReadAllText(filePath);

                // Deserialize the JSON data back into the actionData list
                actionData = JsonConvert.DeserializeObject<List<Tuple<string, Dictionary<string, string>>>>(jsonData);

                // Refresh the DataGridView with the loaded data
                commandGridView.Rows.Clear();
                actionActiveStates.Clear();

                foreach (var action in actionData)
                {
                    int rowIndex = commandGridView.Rows.Add(); // Add a new row
                    commandGridView.Rows[rowIndex].Cells[1].Value = action.Item1; // Set the value in column 1
                    commandGridView.Rows[rowIndex].Cells[0].Value = true;
                    actionActiveStates.Add(true);

                    if (action.Item1 == "Set Device" && action.Item2.ContainsKey("Device Name"))
                    {
                        string deviceName = action.Item2["Device Name"];
                        if (deviceNameToUdidMap.ContainsKey(deviceName))
                        {
                            commandGridView.Rows[rowIndex].Tag = deviceNameToUdidMap[deviceName]; // Restore the UDID
                        }
                    }

                    // Update the action text
                    commandGridView.Rows[rowIndex].Cells[1].Value = FormatActionText(rowIndex);
                }
                scriptFilePath = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(scriptFilePath))
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Json Files (*.json)|*.json";
                        saveFileDialog.Title = "Save Json File";

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string filePath = saveFileDialog.FileName;
                            SaveScriptToFile(filePath);
                        }
                    }
                }
                else
                {
                    SaveScriptToFile(scriptFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("Save Script");
        }

        public void SaveScriptToFile(string filePath)
        {
            try
            {
                // Serialize the actionData list to JSON
                string jsonData = JsonConvert.SerializeObject(actionData, Formatting.Indented);

                // Write the JSON data to the specified file
                File.WriteAllText(filePath, jsonData);
                scriptFilePath = filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving script: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestRunner_Shown(object sender, EventArgs e)
        {
            comboBoxActions.SelectedItem = "Set Device";
            GoogleAnalytics.SendEvent("TestRunner_Shown");
        }


        private void commandGridView_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the row and column under the mouse
            var hitTestInfo = commandGridView.HitTest(e.X, e.Y);
            rowIndexFromMouseDown = hitTestInfo.RowIndex;
            int columnIndexFromMouseDown = hitTestInfo.ColumnIndex;

            // Ensure a valid row is clicked and the column is not the checkbox column (index 0)
            if (rowIndexFromMouseDown > 0 && columnIndexFromMouseDown != 0)
            {
                // Begin drag-and-drop operation
                commandGridView.DoDragDrop(commandGridView.Rows[rowIndexFromMouseDown], DragDropEffects.Move);
            }
        }

        private void commandGridView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move; // Allow the drag-and-drop operation
        }

        private void commandGridView_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = commandGridView.PointToClient(new Point(e.X, e.Y));
            int targetIndex = commandGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            if (targetIndex >= 0 && rowIndexFromMouseDown >= 0 && rowIndexFromMouseDown != targetIndex)
            {
                // Move the row visually in the DataGridView
                DataGridViewRow rowToMove = commandGridView.Rows[rowIndexFromMouseDown];
                commandGridView.Rows.RemoveAt(rowIndexFromMouseDown);
                commandGridView.Rows.Insert(targetIndex, rowToMove);

                // Reorder the actionData and actionActiveStates lists
                var actionToMove = actionData[rowIndexFromMouseDown];
                actionData.RemoveAt(rowIndexFromMouseDown);
                actionData.Insert(targetIndex, actionToMove);

                var stateToMove = actionActiveStates[rowIndexFromMouseDown];
                actionActiveStates.RemoveAt(rowIndexFromMouseDown);
                actionActiveStates.Insert(targetIndex, stateToMove);

                // Re-select the moved row
                commandGridView.ClearSelection();
                commandGridView.Rows[targetIndex].Selected = true;
            }
            GoogleAnalytics.SendEvent("DragDropRows_TestRunner");
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("1. Adding Actions:");
            helpMessage.AppendLine("   - Select an action from the dropdown menu.");
            helpMessage.AppendLine("   - Fill in the required properties in the property grid.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("2. Setting a Device:");
            helpMessage.AppendLine("   - Choose 'Set Device' from the actions.");
            helpMessage.AppendLine("   - Select a device name from the dropdown.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("3. Saving and Loading Scripts:");
            helpMessage.AppendLine("   - Use the 'Save' button to save your actions to a JSON file.");
            helpMessage.AppendLine("   - Use the 'Load' button to load actions from a JSON file.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("4. Running Actions:");
            helpMessage.AppendLine("   - Use 'Run Once' to execute the actions once.");
            helpMessage.AppendLine("   - Use 'Repeat' to execute the actions multiple times.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("5. Reordering Actions:");
            helpMessage.AppendLine("   - Drag and drop actions to reorder them in the list.");
            helpMessage.AppendLine();
            helpMessage.AppendLine("6. Deleting Rows:");
            helpMessage.AppendLine("   - Select a row in the action list.");
            helpMessage.AppendLine("   - Press the 'Delete' key to delete the row.");
            helpMessage.AppendLine("   - Note: The default row (first row) cannot be deleted.");

            MessageBox.Show(helpMessage.ToString(), "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void TestRunner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning)
            {
                // Show confirmation dialog
                var result = MessageBox.Show("A test is currently running. Do you want to stop the test and close the runner?",
                                             "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    UpdateScreenControl("");
                    // Stop the test and allow form to close
                    cancellationTokenSource?.Cancel();
                    isRunning = false; // Reset the running flag
                }
                else
                {
                    // Cancel the form closing
                    e.Cancel = true;
                }
            }
        }


        private void saveButton_MouseHover(object sender, EventArgs e)
        {
            filePathToolTip.SetToolTip(saveButton, scriptFilePath);
        }

        private void commandGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the changed cell is in the checkbox column
            if (e.ColumnIndex == 0 && e.RowIndex >= 0 && e.RowIndex < actionActiveStates.Count)
            {
                DataGridViewRow row = commandGridView.Rows[e.RowIndex];
                bool isChecked = Convert.ToBoolean(row.Cells[0].Value);

                // Update the active state
                actionActiveStates[e.RowIndex] = isChecked;
            }
        }

        private void CreateHtmlReport(int repetitions=1)
        {
            string htmlContent = $@"
                             <!DOCTYPE html>
                             <html>
                             <head>
                                 <title>Appium Wizard Test Runner Execution Report</title>
                                 <style>
                                     body {{ font-family: Arial, sans-serif; margin: 20px; }}
                                     table {{ border-collapse: collapse; width: 100%; }}
                                     th, td {{ border: 1px solid #ddd; padding: 8px; }}
                                     th {{ background-color: #f4f4f4; }}
                                     .success {{ color: green; }}
                                     .error {{ color: red; }}
                                     h1 {{ text-align: center; }}
                                 </style>
                             </head>
                             <body>
                                 <h1>Appium Wizard Test Runner Execution Report - {repetitions} run(s)</h1>
                                 <table>
                                     <thead>
                                         <tr>
                                             <th>Timestamp</th>
                                             <th>Command</th>
                                             <th>Status Description</th>
                                         </tr>
                                     </thead>
                                     <tbody>
                             ";

            File.WriteAllText(htmlReportPath, htmlContent);
        }

        private void AppendToHtmlReport(string command, string output, string? screenshotPath = null)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string screenshotHtml = string.Empty;

            if (!string.IsNullOrEmpty(screenshotPath))
            {
                string relativePath = Path.GetFileName(screenshotPath);
                screenshotHtml = $@"<br><a href='{relativePath}' target='_blank'><img src='{relativePath}' alt='Screenshot' style='max-width: 300px; max-height: 300px;'/></a>";
            }

            string row = $@"
                            <tr>
                                <td>{timestamp}</td>
                                <td>{command}</td>
                                <td>{output}{screenshotHtml}</td>
                            </tr>
                        ";

            File.AppendAllText(htmlReportPath, row);
        }

        private void FinalizeHtmlReport()
        {
            string closingTags = @"
                                </tbody>
                            </table>
                        </body>
                        </html>
                        ";
            File.AppendAllText(htmlReportPath, closingTags);
        }

        private void openReportButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(htmlReportPath))
                {
                    MessageBox.Show("Please run a test and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = htmlReportPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("openReportButton_Click");
        }

        private void DropDownButton_Click(object sender, EventArgs e)
        {
            Point screenPoint = DropDownButton.PointToScreen(new Point(0, DropDownButton.Height));
            contextMenuStrip1.Show(screenPoint);
        }

        private void openReportFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(reportsFolderPath))
                {
                    Directory.CreateDirectory(reportsFolderPath);
                }
                Process.Start("explorer.exe", reportsFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening reports folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("openReportFolderToolStripMenuItem_Click");
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Json Files (*.json)|*.json";
                    saveFileDialog.Title = "Save Json File";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;
                        SaveScriptToFile(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error saving report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("saveAsToolStripMenuItem_Click");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Point screenPoint = saveDownButton.PointToScreen(new Point(0, saveDownButton.Height));
            contextMenuStrip2.Show(screenPoint);
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult newResult = MessageBox.Show("Do you want to open a new runner in this same window?\n\nYes - will close this runner and open a new one. Please make sure to save the script if you need in future.\n\nNo - Keeps this runner window open and opens a new runner in separate window.\n\n Cancel - Just close this confirmation popup.", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (newResult == DialogResult.Yes)
                {
                    TestRunner testRunner = new TestRunner();
                    testRunner.Show();
                    this.Close();
                }
                else if (newResult == DialogResult.No)
                {
                    TestRunner testRunner = new TestRunner();
                    testRunner.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error opening new runner", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            GoogleAnalytics.SendEvent("newButton_Click");
        }


        private Dictionary<string, string> propertyDescriptions = new Dictionary<string, string>
            {
                { "XPath", "Provide the XPath of the element you want to interact with." },
                { "Wait For Element Visible (ms)", "Timeout in milliseconds to wait for the element to become visible." },
                { "Text to Enter", "The text to input into the specified element." },
                { "Text Type", "Specify the type of random text: Random Number, Random Alphabets, or Random Alphanumeric." },
                { "Number of digits/characters", "Specify the number of digits or characters for random text generation." },
                { "Device Name", "Select the device name from the dropdown list." },
                { "Timeout (ms)", "Timeout in milliseconds to wait for the element to become visible or vanish." },
                { "Duration (ms)", "Duration in milliseconds to pause execution." },
                { "App Path", "Provide the file path of the app to be installed. APK for android and IPA for iOS." },
                { "App BundleId(iOS)/Activity(Android)", "Provide the bundle ID for iOS app, Activity name for the Android app to be launched." },
                { "App Package", "Provide the package name of the app to be killed or uninstalled." },
                { "Action", "Specify the device action to perform: Home or Back.\n\nNote: Not all operations supported in all OS." }
            };
    }
}
