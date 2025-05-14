using Newtonsoft.Json;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.Text;

namespace Appium_Wizard
{
    public partial class TestRunner : Form
    {
        private List<Tuple<string, Dictionary<string, string>>> actionData = new List<Tuple<string, Dictionary<string, string>>>();
        private static Dictionary<string, string> deviceNameToUdidMap = new Dictionary<string, string>();
        private Dictionary<string, string> deviceNameToOsTypeMap = new Dictionary<string, string>();
        private List<string> deviceNames = new List<string>();
        bool isAndroid;
        string? selectedUDID, selectedOS, selectedDeviceVersion, selectedDeviceName, selectedDeviceConnection, selectedDeviceIP, URL;
        int port;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isRunning = false;
        private string scriptFilePath = string.Empty;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        // isandroid and selectedeviceudid will have issues when set device is set 2nd time with different device.. need to set these varibales while startin performactions.. 
        public TestRunner()
        {
            InitializeComponent();
            commandGridView.Columns[1].Width = commandGridView.Width - 5;
            propertyGridView.Columns[0].Width = (propertyGridView.Width / 2) - 5;
            propertyGridView.Columns[1].Width = (propertyGridView.Width / 2);
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
                case "Take Screenshot":
                    break;
                case "Device Action":
                    properties.Add("Action", "");
                    break;
            }

            // Add the action and its properties to the data list
            actionData.Add(new Tuple<string, Dictionary<string, string>>(selectedAction, properties));
            actionActiveStates.Add(false);

            // Add the action to DataGridView1
            int rowIndex = commandGridView.Rows.Add(); // Add a new row
            commandGridView.Rows[rowIndex].Cells[1].Value = selectedAction; // Set the value in column 1
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
                commandGridView.Rows[selectedIndex].Cells[1].Value = FormatActionText(selectedIndex);
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
                if (!string.IsNullOrWhiteSpace(row.ErrorText))
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
                selectedUDID = commandGridView.Rows[0].Tag as string;
                selectedDeviceName = GetDeviceNameByUdid(selectedUDID);
                if (ScreenControl.devicePorts.ContainsKey(selectedUDID))
                {
                    port = ScreenControl.devicePorts[selectedUDID].Item2;
                    URL = "http://127.0.0.1:" + port;
                    await performActions(cancellationTokenSource.Token);
                }
                else
                {
                    MessageBox.Show("Please open the device " + selectedDeviceName + " from Main screen and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    runOnceButton.Text = "Run Once";
                    repeatButton.Enabled = true;
                    isRunning = false;
                    UpdateScreenControl("");
                    return;
                }
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
                        selectedUDID = commandGridView.Rows[0].Tag as string;
                        selectedDeviceName = GetDeviceNameByUdid(selectedUDID);
                        if (ScreenControl.devicePorts.ContainsKey(selectedUDID))
                        {
                            port = ScreenControl.devicePorts[selectedUDID].Item2;
                            URL = "http://127.0.0.1:" + port;
                            for (int i = 0; i < repetitions; i++)
                            {
                                await performActions(cancellationTokenSource.Token);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please open the device "+ selectedDeviceName + " from Main screen and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            repeatButton.Text = "Repeat";
                            runOnceButton.Enabled = true;
                            isRunning = false;
                            UpdateScreenControl("");
                            return;
                        }
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
            }
        }

        private async Task performActions(CancellationToken cancellationToken)
        {
            await Task.Run(async () =>
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
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrWhiteSpace(sessionId) || sessionId.Equals("nosession"))
                {
                    MessageBox.Show("Please open device and then try running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                for (int i = 0; i < actionData.Count; i++)
                {
                    if (!actionActiveStates[i]) continue; // Skip inactive actions

                    var action = actionData[i];
                    string actionType = action.Item1;
                    Dictionary<string, string> properties = action.Item2;

                    cancellationToken.ThrowIfCancellationRequested();
                    UpdateScreenControl("");
                    switch (actionType)
                    {
                        case "Click Element":
                            UpdateScreenControl("Click " + properties["XPath"]);
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
                            UpdateScreenControl("Send Text " + properties["Text to Enter"] + " to " + properties["XPath"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                iOSAPIMethods.SendText(URL, sessionId, properties["XPath"], properties["Text to Enter"]);
                            }
                            break;
                        case "Set Device":
                            UpdateScreenControl("Set Device - " + properties["Device Name"]);
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
                            int visibleTimeout = int.TryParse(properties["Timeout (ms)"], out var visibleParsedTimeout) ? visibleParsedTimeout : 0;
                            UpdateScreenControl("Wait for element visible - " + properties["XPath"] + " - "+ visibleTimeout);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                await WaitUntilElementDisplayed(URL, sessionId, properties["XPath"], visibleTimeout, cancellationToken);
                            }
                            break;
                        case "Wait for element to vanish":
                            int vanishTimeout = int.TryParse(properties["Timeout (ms)"], out var vanishParsedTimeout) ? vanishParsedTimeout : 0;
                            UpdateScreenControl("Wait for element to vanish - " + properties["XPath"] + " - " + vanishTimeout);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                await WaitUntilElementVanished(URL, sessionId, properties["XPath"], vanishTimeout, cancellationToken);
                            }
                            break;
                        case "Sleep":
                            UpdateScreenControl("Sleep " + properties["Duration (ms)"] + " ms");
                            int duration = int.TryParse(properties["Duration (ms)"], out var parsedTimeout) ? parsedTimeout : 0;
                            await Task.Delay(duration, cancellationToken);
                            break;
                        case "Install App":
                            UpdateScreenControl("Install App " + properties["App Path"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                iOSAsyncMethods.GetInstance().InstallApp(selectedUDID, properties["App Path"]);
                            }
                            break;
                        case "Launch App":
                            UpdateScreenControl("Launch App " + properties["App Package"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                iOSAPIMethods.LaunchApp(URL, sessionId, properties["App Package"]);
                            }
                            break;
                        case "Kill App":
                            UpdateScreenControl("Kill App " + properties["App Package"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                iOSAPIMethods.KillApp(URL, sessionId, properties["App Package"]);
                            }
                            break;
                        case "Uninstall App":
                            UpdateScreenControl("Uninstall App " + properties["App Package"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
                            }
                            else
                            {
                                iOSMethods.GetInstance().UninstallApp(selectedUDID, properties["App Package"]);
                            }
                            break;
                        case "Take Screenshot":
                            UpdateScreenControl("Take Screenshot and Save in Downloads");
                            if (isAndroid)
                            {
                                // Android-specific implementation
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
                            UpdateScreenControl("Device Action : " + properties["Action"]);
                            if (isAndroid)
                            {
                                // Android-specific implementation
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
            if (ScreenControl.udidScreenControl.ContainsKey(selectedUDID))
            {
                var screenControl = ScreenControl.udidScreenControl[selectedUDID];
                screenControl.UpdateStatusLabel(screenControl, statusText);
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
                case "Take Screenshot":
                    return $"Take screenshot and save to Downloads folder";
                case "Device Action":
                    return $"Perform device action : \"{properties["Action"]}\"";
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

        public async Task<bool> WaitUntilElementVanished(string url, string sessionId, string xPath, int timeout, CancellationToken cancellationToken)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                cancellationToken.ThrowIfCancellationRequested();

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
                var result = MessageBox.Show("A test is currently running. Do you want to stop the test and close the form?",
                                             "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
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

        private void saveAsButton_Click(object sender, EventArgs e)
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
    }
}
