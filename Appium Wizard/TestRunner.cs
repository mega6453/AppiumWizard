using System.Data.Common;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class TestRunner : Form
    {
        bool isAndroid;
        List<Tuple<string, string>> rowTuples = new List<Tuple<string, string>>();
        int actionColumnIndex = 0;
        int InputColumnIndex = 1;

        public TestRunner()
        {
            InitializeComponent();
        }


        private void TestRunner_Load(object sender, EventArgs e)
        {

        }


        private void runOnceButton_Click(object sender, EventArgs e)
        {
            validateInput();
            addActionsToTuples();
            performActions(rowTuples);
        }

        private void repeatButton_Click(object sender, EventArgs e)
        {
            bool isValid = validateInput();
            if (!isValid)
            {
                return;
            }
            addActionsToTuples();
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the number of repetitions:", "Repeat Action", "1");
            if (int.TryParse(input, out int repetitions))
            {
                for (int i = 0; i < repetitions; i++)
                {
                    performActions(rowTuples);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void performActions(List<Tuple<string, string>> rowTuples)
        {
            foreach (var tuple in rowTuples)
            {
                MessageBox.Show($"ComboBox Value: {tuple.Item1}, TextBox Value: {tuple.Item2}");
                if (tuple.Item1.Equals("Click Element"))
                {
                    if (isAndroid)
                    {

                    }
                    else
                    {

                    }
                }
                else if (tuple.Item1.Equals("Send Text"))
                {
                    if (isAndroid)
                    {

                    }
                    else
                    {

                    }
                }
                else if (tuple.Item1.Equals("Wait for element visible"))
                {
                    if (isAndroid)
                    {

                    }
                    else
                    {

                    }
                }
                else if (tuple.Item1.Equals("Wait for element to vanish"))
                {
                    if (isAndroid)
                    {

                    }
                    else
                    {

                    }
                }
                else if (tuple.Item1.Equals("Wait in seconds"))
                {
                    //task delay
                }
            }
        }

        private bool validateInput()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var comboBoxValue = row.Cells[actionColumnIndex].Value;
                var textBoxValue = row.Cells[InputColumnIndex].Value;

                if (comboBoxValue == null || string.IsNullOrWhiteSpace(comboBoxValue.ToString()) ||
                    textBoxValue == null || string.IsNullOrWhiteSpace(textBoxValue.ToString()))
                {
                    MessageBox.Show($"Row {row.Index + 1} has empty values. Please fill all fields before proceeding.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void addActionsToTuples()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var comboBoxValue = row.Cells[actionColumnIndex].Value;
                string comboBoxText = comboBoxValue?.ToString() ?? "None";

                var textBoxValue = row.Cells[InputColumnIndex].Value;
                string textBoxText = textBoxValue?.ToString() ?? "None";

                Tuple<string, string> rowTuple = Tuple.Create(comboBoxText, textBoxText);
                rowTuples.Add(rowTuple);
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To delete a row - Click on first column cell of a row and click delete button on your keyboard.\n\nTo Select Multiple rows - Press control and then select rows.", "Help");
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (dataGridView1.CurrentCell != null)
            //{
            //    var selectedValue = dataGridView1.CurrentCell.Value;
            //    if (selectedValue != null)
            //    {
            //        if (selectedValue.Equals("Send Text"))
            //        {
            //            string textInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the text that needs to be sent:", "Send Text", "");
            //            string elementInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the Xpath of the text field:", "Send Text", "");
            //            if (string.IsNullOrEmpty(elementInput) | string.IsNullOrWhiteSpace(elementInput))
            //            {
            //                MessageBox.Show("Please enter a valid Xpath.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            }
            //            else
            //            {
            //                string combinedText = textInput + " to " + elementInput;
            //                int currentRowIndex = dataGridView1.CurrentCell.RowIndex;
            //                int currentColumnIndex = dataGridView1.CurrentCell.ColumnIndex;
            //                dataGridView1.Rows[currentRowIndex].Cells[currentColumnIndex + 1].Value = combinedText;
            //            }
            //        }
            //    }
            //}
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 0 && e.Control is ComboBox comboBox)
            {
                // Remove existing event handler to avoid duplicate subscriptions
                comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            }
        }
        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected value from the ComboBox
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.SelectedItem != null)
            {
                string selectedValue = comboBox.SelectedItem.ToString();
                if (selectedValue.Equals("Send Text"))
                {
                    // Show input boxes for user input
                    string textInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the text that needs to be sent:", "Send Text", "");
                    string elementInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the Xpath of the text field:", "Send Text", "");
                    if (string.IsNullOrEmpty(elementInput) || string.IsNullOrWhiteSpace(elementInput))
                    {
                        MessageBox.Show("Please enter a valid Xpath.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        // Combine the inputs and set the value in the next column
                        string combinedText = textInput + " to " + elementInput;
                        int currentRowIndex = dataGridView1.CurrentCell.RowIndex;
                        dataGridView1.Rows[currentRowIndex].Cells[dataGridView1.CurrentCell.ColumnIndex + 1].Value = combinedText;
                    }
                }
            }
        }
    }
}