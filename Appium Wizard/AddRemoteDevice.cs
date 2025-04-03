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
    public partial class AddRemoteDevice : Form
    {
        public AddRemoteDevice()
        {
            InitializeComponent();
        }

        private async void findDevicesButton_Click(object sender, EventArgs e)
        {
            string fileUrl = IPTextBox.Text;
            var json = await Common.ReadJsonFromRemote(fileUrl);
            if (!string.IsNullOrEmpty(json)) { 
            

            } 
        }
    }
}
