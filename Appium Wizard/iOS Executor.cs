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
    public partial class iOS_Executor : Form
    {
        public static string selectediOSExecutor = "auto";
        ListView listView1;
        public iOS_Executor(ListView listView)
        {
            InitializeComponent();
            listView1 = listView;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (AutomaticRadioButton.Checked)
            {
                selectediOSExecutor = "auto";
            }
            else if (goRadioButton.Checked)
            {
                selectediOSExecutor = "go";
            }
            else
            {
                selectediOSExecutor = "py";
            }
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                selectedItem.Selected = false;
                selectedItem.Selected = true;
            }
            this.Close();
        }

        private void iOS_Executor_Load(object sender, EventArgs e)
        {
            AutomaticRadioButton.Checked = true;
        }
    }
}
