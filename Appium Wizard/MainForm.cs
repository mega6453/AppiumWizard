using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class MainForm : Form
    {
        private List<Tuple<string, Dictionary<string, string>>> actionData = new List<Tuple<string, Dictionary<string, string>>>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
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
                    foreach (var property in selectedActionData.Item2)
                    {
                        propertyGridView.Rows.Add(property.Key, property.Value);
                    }
                }
            }
            catch (Exception)
            {

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

                // Update the corresponding cell in DataGridView1
                commandGridView.Rows[selectedIndex].Cells[0].Value = $"{property}: {value}";
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
            performActions();
        }

        private void repeatButton_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the number of repetitions:", "Repeat Action", "1");
            if (int.TryParse(input, out int repetitions))
            {
                for (int i = 0; i < repetitions; i++)
                {
                    performActions();
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                        MessageBox.Show(message.ToString(), "Set Device", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
