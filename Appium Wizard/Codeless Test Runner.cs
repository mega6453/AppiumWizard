using System.Collections.Generic;

namespace Appium_Wizard
{
    public partial class Codeless_Test_Runner : Form
    {
        private Dictionary<string, List<string>> dataGridView2Values = new Dictionary<string, List<string>>();
        public Codeless_Test_Runner()
        {
            InitializeComponent();
            InitializeCommandProperties();
        }


        private void DataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Check if the current cell is a ComboBox cell
            //if (dataGridView1.CurrentCell is DataGridViewComboBoxCell)
            //{
            //    ComboBox comboBox = e.Control as ComboBox;
            //    if (comboBox != null)
            //    {
            //        // Unsubscribe from previous event handlers to avoid duplication
            //        comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;

            //        // Subscribe to the SelectedIndexChanged event
            //        comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            //    }
            //}
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                string selectedValue = comboBox.SelectedItem.ToString();
                PopulateProperties(selectedValue);
            }
        }

        private void SaveDataGridView2Values()
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
            {
                string currentCommand = dataGridView1.CurrentCell.Value.ToString();
                if (!string.IsNullOrEmpty(currentCommand))
                {
                    List<string> values = new List<string>();

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.Cells[1].Value != null)
                        {
                            values.Add(row.Cells[1].Value.ToString());
                        }
                        else
                        {
                            values.Add(""); // Add empty string for null values
                        }
                    }

                    // Save the values to the dictionary
                    if (dataGridView2Values.ContainsKey(currentCommand))
                    {
                        dataGridView2Values[currentCommand] = values;
                    }
                    else
                    {
                        dataGridView2Values.Add(currentCommand, values);
                    }
                }
            }
        }

        private void RestoreDataGridView2Values(string command)
        {

            if (command != null && dataGridView2Values.ContainsKey(command))
            {
                List<string> values = dataGridView2Values[command];
                for (int i = 0; i < values.Count && i < dataGridView2.Rows.Count; i++)
                {
                    dataGridView2.Rows[i].Cells[1].Value = values[i];
                }
            }
        }

        private void PopulateProperties(string command)
        {
            dataGridView2.Rows.Clear();
            if (command != null && commandProperties.ContainsKey(command))
            {
                foreach (var property in commandProperties[command])
                {
                    dataGridView2.Rows.Add(property, "");
                }
            }
        }

        private Dictionary<string, List<string>> commandProperties;
        private void InitializeCommandProperties()
        {
            // Initialize command-properties mapping
            commandProperties = new Dictionary<string, List<string>>
            {
                { "Set Device", new List<string> { "Device"} },
                { "Click Element", new List<string> { "Element XPath"} },
                { "Send Text", new List<string> { "Element XPath", "Text"} },
                { "Wait for element visible", new List<string> { "Element XPath", "Timeout"} },
                { "Wait for element to vanish", new List<string> { "Element XPath", "Timeout" } },
                { "Wait in seconds", new List<string> { "Time in seconds"} },
                { "Install App", new List<string> { "Path" } },
                { "Launch App", new List<string> { "Package Name/Bundle Id" } },
                { "Uninstall App", new List<string> { "Package Name/Bundle Id" } },
                { "Execute Script", new List<string> { "Command","Json" } },
                { "Take Screenshot", new List<string> { } },
                { "Device Action", new List<string> { "Action" } }
            };
        }


        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            //if (dataGridView1.SelectedRows.Count > 0)
            //{
            //    DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            //    string cellValue = selectedRow.Cells[1].Value?.ToString();
            //    PopulateProperties(cellValue);
            //    // Restore previously entered values for the selected command
            //    RestoreDataGridView2Values(cellValue);
            //    //PopulateProperties(cellValue);
            //    //if (cellValue == "Set Device")
            //    //{

            //    //}
            //    //else if (cellValue == "Click Element")
            //    //{
            //    //}
            //    //else if (cellValue == "Send Text")
            //    //{
            //    //}
            //    //else if (cellValue == "Wait for element visible")
            //    //{
            //    //}
            //    //else if (cellValue == "Wait for element to vanish")
            //    //{
            //    //}
            //    //else if (cellValue == "Wait in seconds")
            //    //{
            //    //}
            //    //else if (cellValue == "Install App")
            //    //{
            //    //}
            //    //else if (cellValue == "Launch App")
            //    //{
            //    //}
            //    //else if (cellValue == "Uninstall App")
            //    //{
            //    //}
            //    //else if (cellValue == "Execute Script")
            //    //{
            //    //}
            //    //else if (cellValue == "Take Screenshot")
            //    //{
            //    //}
            //    //else if (cellValue == "Device Action")
            //    //{
            //    //}

            //    //MessageBox.Show($"You selected row with value: {cellValue}");
            //}
        }

        private void dataGridView2_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // Save current values from dataGridView2 before switching
            SaveDataGridView2Values();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString();
            dataGridView1.Rows.Add(selectedItem);
        }
    }
}