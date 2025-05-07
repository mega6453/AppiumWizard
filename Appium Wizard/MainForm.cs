using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Text;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class MainForm : Form
    {
        private List<Tuple<string, Dictionary<string, string>>> actionData = new List<Tuple<string, Dictionary<string, string>>>();
        private Dictionary<string, string> deviceNameToUdidMap = new Dictionary<string, string>(); // Mapping of device names to UDIDs
        private Dictionary<string, string> deviceNameToOsTypeMap = new Dictionary<string, string>();
        private List<string> deviceNames = new List<string>();
        //private List<string> deviceNames = new List<string> { "Device1", "Device2", "Device3" };
        bool isAndroid;
        string selectedDeviceUDID;

        public MainForm()
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
                case "Wait for Element":
                    properties.Add("XPath", "");
                    properties.Add("Timeout (ms)", "");
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
                    properties.Add("Duration (seconds)", "");
                    break;
                case "Install App":
                    properties.Add("App Path", "");
                    break;
                case "Launch App":
                    properties.Add("App Package", "");
                    properties.Add("App Activity", "");
                    break;
                case "Uninstall App":
                    properties.Add("App Package", "");
                    break;
                case "Execute Script":
                    properties.Add("Script", "");
                    break;
                case "Take Screenshot":
                    properties.Add("Save Path", "");
                    break;
                case "Device Action":
                    properties.Add("Action Name", "");
                    properties.Add("Parameters", "");
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

                // Check if the property being updated is "Device Name"
                if (property == "Device Name" && deviceNameToUdidMap.ContainsKey(value))
                {
                    selectedDeviceUDID = deviceNameToUdidMap[value]; // Update the selected UDID
                    Console.WriteLine($"Selected Device UDID: {selectedDeviceUDID}"); // For debugging

                    // Update the OS type based on the selected device name
                    if (deviceNameToOsTypeMap.ContainsKey(value))
                    {
                        string osType = deviceNameToOsTypeMap[value];
                        isAndroid = osType.Equals("Android", StringComparison.OrdinalIgnoreCase);
                        Console.WriteLine($"Selected OS Type: {osType}, isAndroid: {isAndroid}"); // For debugging
                    }
                }

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


        private void runOnceButton_Click(object sender, EventArgs e)
        {
            var isAnyError = checkIfAnyErrors();
            if (isAnyError)
            {
                MessageBox.Show("Please check the rows with errors and fix it.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                performActions();
            }
        }

        private void repeatButton_Click(object sender, EventArgs e)
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
                    for (int i = 0; i < repetitions; i++)
                    {
                        performActions();
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

        private void performActions()
        {
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
                        MessageBox.Show(message.ToString(), "Click Element", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        break;
                    case "Send Text":
                        MessageBox.Show(message.ToString(), "Send Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Wait for Element":
                        MessageBox.Show(message.ToString(), "Wait for Element", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Set Device":
                        MessageBox.Show(isAndroid.ToString(), "Set Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Wait for element visible":
                        MessageBox.Show(message.ToString(), "Wait for Element Visible", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Wait for element to vanish":
                        MessageBox.Show(message.ToString(), "Wait for Element to Vanish", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Sleep":
                        MessageBox.Show(message.ToString(), "Sleep", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Install App":
                        MessageBox.Show(message.ToString(), "Install App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Launch App":
                        MessageBox.Show(message.ToString(), "Launch App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Uninstall App":
                        MessageBox.Show(message.ToString(), "Uninstall App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Execute Script":
                        MessageBox.Show(message.ToString(), "Execute Script", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Take Screenshot":
                        MessageBox.Show(message.ToString(), "Take Screenshot", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case "Device Action":
                        MessageBox.Show(message.ToString(), "Device Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    default:
                        MessageBox.Show($"Unknown action: {actionType}", "Unknown Action", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    return $"Sleep for {properties["Duration (seconds)"]} seconds";
                case "Install App":
                    return $"Install app from {properties["App Path"]}";
                case "Launch App":
                    return $"Launch app with package {properties["App Package"]} and activity {properties["App Activity"]}";
                case "Uninstall App":
                    return $"Uninstall app with package {properties["App Package"]}";
                case "Execute Script":
                    return $"Execute script: {properties["Script"]}";
                case "Take Screenshot":
                    return $"Take screenshot and save to {properties["Save Path"]}";
                case "Device Action":
                    return $"Perform device action \"{properties["Action Name"]}\" with parameters {properties["Parameters"]}";
                default:
                    return $"Unknown action: {actionType}";
            }
        }
    }
}
