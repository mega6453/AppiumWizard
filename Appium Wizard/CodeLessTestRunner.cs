using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class CodeLessTestRunner : Form
    {
        public CodeLessTestRunner()
        {
            InitializeComponent();
        }

        private void CodeLessTestRunner_Load(object sender, EventArgs e)
        {
            InitializeCommandProperties();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString() + ":";

            // Create a new row
            int rowIndex = dataGridView1.Rows.Add(); // Add a new row and get its index
            DataGridViewRow newRow = dataGridView1.Rows[rowIndex];

            // Set the value for the first column (checkbox column) to checked
            newRow.Cells[0].Value = true; // Assuming first column is at index 0

            // Set the value for the second column (text column)
            newRow.Cells[1].Value = selectedItem; // Assuming second column is at index 1

            dataGridView1.ClearSelection();

            // Highlight the newly created row
            newRow.Selected = true;
            PopulateProperties(selectedItem);
        }


        private Dictionary<string, List<string>> commandProperties;
        private void InitializeCommandProperties()
        {
            // Initialize command-properties mapping
            commandProperties = new Dictionary<string, List<string>>
            {
                { "Set Device:", new List<string> { "Device"} },
                { "Click Element:", new List<string> { "Element XPath"} },
                { "Send Text:", new List<string> { "Element XPath", "Text"} },
                { "Wait for element visible:", new List<string> { "Element XPath", "Timeout"} },
                { "Wait for element to vanish:", new List<string> { "Element XPath", "Timeout" } },
                { "Wait in seconds:", new List<string> { "Time in seconds"} },
                { "Install App:", new List<string> { "Path" } },
                { "Launch App:", new List<string> { "Package Name/Bundle Id" } },
                { "Uninstall App:", new List<string> { "Package Name/Bundle Id" } },
                { "Execute Script:", new List<string> { "Command","Json" } },
                { "Take Screenshot:", new List<string> { } },
                { "Device Action:", new List<string> { "Action" } }
            };
        }


        private void PopulateProperties(string command)
        {
            string value = "";
            dataGridView2.Rows.Clear();
            if (command != null && commandProperties.ContainsKey(command))
            {
                foreach (var property in commandProperties[command])
                {
                    if (dataGridView2Values.ContainsKey(command))
                    {
                        if (dataGridView2Values[command].ContainsKey(property))
                        {
                            value = dataGridView2Values[command][property];
                        }
                    }

                    // Create a new row
                    int rowIndex = dataGridView2.Rows.Add();
                    DataGridViewRow row = dataGridView2.Rows[rowIndex];

                    // Set the property name
                    row.Cells[0].Value = property;

                    // Check if the command is "Set Device:"
                    if (command == "Set Device:")
                    {
                        // Create a combobox cell for the value
                        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();

                        // Add items to the combobox (you'll need to define these items)
                        comboBoxCell.Items.AddRange(new string[] { "Device1", "Device2", "Device3" });

                        // Set the selected value
                        comboBoxCell.Value = value;

                        // Assign the combobox cell to the second column
                        row.Cells[1] = comboBoxCell;
                    }
                    else
                    {
                        // For other commands, use a regular text cell
                        row.Cells[1].Value = value;
                    }
                }
            }
        }

        //private void PopulateProperties(string command)
        //{
        //    string value = "";
        //    dataGridView2.Rows.Clear();
        //    if (command != null && commandProperties.ContainsKey(command))
        //    {
        //        foreach (var property in commandProperties[command])
        //        {
        //            if (command == "Set Device:")
        //            {

        //            }
        //            if (dataGridView2Values.ContainsKey(command))
        //            {
        //                value = dataGridView2Values[command][property];
        //            }
        //            dataGridView2.Rows.Add(property, value);
        //        }
        //    }
        //}

        private void dataGridView1_SelectionChangedOld(object sender, EventArgs e)
        {
            // Check if any row is selected
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the index of the selected row
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

                // Get the value from the second column (index 1)
                var secondColumnValue = dataGridView1.Rows[selectedRowIndex].Cells[1].Value;

                // Perform your action based on the second column value
                if (secondColumnValue != null)
                {
                    string value = secondColumnValue.ToString();

                    // Example action: Display a message box with the value
                    //MessageBox.Show($"The value in the second column is: {value}");

                    // You can add more logic here based on the value


                    PopulateProperties(value);
                }
            }
        }

        private List<Tuple<string, Dictionary<string, string>>> selectedData = new List<Tuple<string, Dictionary<string, string>>>();






        private Dictionary<string, Dictionary<string, string>> dataGridView2Values = new Dictionary<string, Dictionary<string, string>>();

        private void SaveDataGridView2Values()
        {
            try
            {
                int selectedRowIndex = dataGridView1.SelectedRows[0].Index;
                var currentC = dataGridView1.Rows[selectedRowIndex].Cells[1].Value;
                if (currentC != null)
                {
                    string currentCommand = currentC.ToString();
                    currentCommand = currentCommand.ToString();
                    int colonIndex = currentCommand.IndexOf(':');
                    currentCommand = currentCommand.Substring(0, colonIndex + 1);

                    if (!string.IsNullOrEmpty(currentCommand))
                    {
                        Dictionary<string, string> values = new Dictionary<string, string>();
                        Dictionary<string, string> propertiesValues = new Dictionary<string, string>();
                        foreach (DataGridViewRow row in dataGridView2.Rows)
                        {
                            if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                            {
                                string key = row.Cells[0].Value.ToString();
                                string value = row.Cells[1].Value.ToString();
                                values[key] = value; // Add key-value pair to the inner dictionary
                                propertiesValues[key] = value;

                            }
                            else if (row.Cells[0].Value != null)
                            {
                                string key = row.Cells[0].Value.ToString();
                                values[key] = ""; // Add an empty string for null values in cells[1]
                            }
                        }

                        // Save the values to the outer dictionary
                        if (dataGridView2Values.ContainsKey(currentCommand))
                        {
                            dataGridView2Values[currentCommand] = values;
                        }
                        else
                        {
                            dataGridView2Values.Add(currentCommand, values);
                        }
                        Tuple<string, Dictionary<string, string>> dataTuple = new Tuple<string, Dictionary<string, string>>(currentCommand.ToString(), propertiesValues);
                        selectedData.Add(dataTuple);

                        // Optionally, display or process the collected data
                        ProcessSelectedData(selectedData);
                    }

                }
            }
            catch (Exception)
            {

            }
           
           
        }

        private void dataGridView2_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            //SaveDataGridView2Values();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //SaveDataGridView2Values();
        }



        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SaveDataGridView2Values();
            //if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.CurrentRow != null)
            //{

            //    int selectedRowIndex = dataGridView1.SelectedRows[0].Index;

            //    // Get the value from the second column (index 1)
            //    var gv1Text = dataGridView1.Rows[selectedRowIndex].Cells[1].Value;

            //    if (gv1Text != null)
            //    {
            //        Dictionary<string, string> propertiesValues = new Dictionary<string, string>();

            //        foreach (DataGridViewRow row in dataGridView2.Rows)
            //        {
            //            if (row.Cells[0].Value != null && row.Cells[1].Value != null)
            //            {
            //                string property = row.Cells[0].Value.ToString();
            //                string value = row.Cells[1].Value.ToString();
            //                propertiesValues[property] = value;
            //            }
            //        }

            //        // Create a tuple and add it to the list
            //        Tuple<string, Dictionary<string, string>> dataTuple = new Tuple<string, Dictionary<string, string>>(gv1Text.ToString(), propertiesValues);
            //        selectedData.Add(dataTuple);

            //        // Optionally, display or process the collected data
            //        ProcessSelectedData(selectedData);
            //    }

            //}
        }


        private void ProcessSelectedData(List<Tuple<string, Dictionary<string, string>>> data)
        {
            // Example: Display the collected data in the console or a message box
            foreach (var tuple in data)
            {
                string gv1Text = tuple.Item1;
                Dictionary<string, string> propertiesValues = tuple.Item2;

                Console.WriteLine($"GridView1 Text: {gv1Text}");
                foreach (var kvp in propertiesValues)
                {
                    Console.WriteLine($"Property: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }
    }
}
