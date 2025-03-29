using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class Plugins : Form
    {
        string selectedPlugin, selectedVersion;
        public Plugins()
        {
            InitializeComponent();
        }

        private void pluginsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                ProcessStartInfo psInfo = new ProcessStartInfo
                {
                    FileName = "https://appium.io/docs/en/latest/ecosystem/plugins/",
                    UseShellExecute = true
                };
                Process.Start(psInfo);
                GoogleAnalytics.SendEvent("pluginsLinkLabel_LinkClicked");
            }
            catch (Exception exception)
            {
                GoogleAnalytics.SendExceptionEvent("pluginsLinkLabel_LinkClicked", exception.Message);
            }
        }

        private void Plugins_Load(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = listView1.Width / 2;
            listView1.Columns[1].Width = (listView1.Width / 2) - 10;
        }

        public async Task UpdatePluginList(CommonProgress commonProgress = null)
        {
            commonProgress.UpdateStepLabel("Fetch Plugins", "Please wait while fetching plugins list...", 75);
            listView1.Columns[0].Width = listView1.Width / 2;
            listView1.Columns[1].Width = (listView1.Width / 2) - 10;
            Dictionary<string, string> result = new Dictionary<string, string>();
            await Task.Run(() =>
            {
                result = Common.GetListOfInstalledPlugins();
            });
            listView1.Items.Clear();

            foreach (var kvp in result)
            {
                ListViewItem item = new ListViewItem(kvp.Key);
                item.SubItems.Add(kvp.Value);
                listView1.Items.Add(item);
            }
            commonProgress.Close();
            listView1_SelectedIndexChanged(listView1, EventArgs.Empty);
            GoogleAnalytics.SendEvent("UpdatePluginList");
        }

        private async void installButton_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("installButton_Click");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.Owner = this;
            if (showProgressCheckBox1.Checked)
            {
                commonProgress.UpdateStepLabel("Install Plugin", "Please wait while installing plugin " + selectedPlugin + "..." +
                                               "\n\nPlease close the cmd window once the execution completed to continue here...");
            }
            else
            {
                commonProgress.UpdateStepLabel("Install Plugin", "Please wait while installing plugin " + selectedPlugin + "...");
            }
            await Task.Run(() =>
            {
                Common.InstallPlugin(selectedPlugin, "install", showProgressCheckBox1.Checked);
            });
            await UpdatePluginList(commonProgress);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView1.SelectedItems[0];
                selectedPlugin = selectedItem.SubItems[0].Text;
                selectedVersion = selectedItem.SubItems[1].Text;
                showProgressCheckBox1.Enabled = true;
                if (selectedVersion.Equals("NotInstalled"))
                {
                    installButton.Enabled = true;
                    updateButton.Enabled = false;
                    uninstallButton.Enabled = false;
                }
                else
                {
                    installButton.Enabled = false;
                    updateButton.Enabled = true;
                    uninstallButton.Enabled = true;
                }
            }
            else
            {
                installButton.Enabled = false;
                uninstallButton.Enabled = false;
                showProgressCheckBox1.Enabled = false;
                updateButton.Enabled = false;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                otherPluginInstallButton.Enabled = true;
                showProgressCheckBox2.Enabled = true;
            }
            else
            {
                otherPluginInstallButton.Enabled = false;
                showProgressCheckBox2.Enabled = false;
            }
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("updateButton_Click");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.Owner = this;
            if (showProgressCheckBox1.Checked)
            {
                commonProgress.UpdateStepLabel("Update Plugin", "Please wait while updating plugin " + selectedPlugin + "..." +
                                               "\n\nPlease close the cmd window once the execution completed to continue here...");
            }
            else
            {
                commonProgress.UpdateStepLabel("Update Plugin", "Please wait while updating plugin " + selectedPlugin + "...");
            }
            await Task.Run(() =>
            {
                Common.InstallPlugin(selectedPlugin, "update", showProgressCheckBox1.Checked);
            });
            await UpdatePluginList(commonProgress);
        }

        private async void otherPluginInstallButton_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("otherPluginInstallButton_Click");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.Owner = this;
            if (showProgressCheckBox2.Checked)
            {
                commonProgress.UpdateStepLabel("Install Plugin", "Please wait while installing plugin " + textBox1.Text + "..." +
                                               "\n\nPlease close the cmd window once the execution completed to continue here...");
            }
            else
            {
                commonProgress.UpdateStepLabel("Install Plugin", "Please wait while installing plugin " + textBox1.Text + "...");
            }
            await Task.Run(() =>
            {
                Common.InstallPlugin(textBox1.Text, "install", showProgressCheckBox2.Checked);
            });
            await UpdatePluginList(commonProgress);
        }

        private void Plugins_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Plugins_Shown");
        }

        private async void uninstallButton_Click(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("uninstallButton_Click");
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.Owner = this;
            commonProgress.UpdateStepLabel("Uninstall Plugin", "Please wait while uninstalling plugin " + selectedPlugin + "...");
            await Task.Run(() =>
            {
                Common.UninstallPlugin(selectedPlugin, showProgressCheckBox1.Checked);
            });
            await UpdatePluginList(commonProgress);
        }

        private void showProgressCheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (showProgressCheckBox2.Checked)
            {
                MessageBox.Show("Checking this box will display the CMD window where the plugin installation execution occurs. You must manually close the CMD window once the execution is completed to continue accessing the Appium wizard.\"", "Show execution status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void showProgressCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (showProgressCheckBox1.Checked)
            {
                MessageBox.Show("Checking this box will display the CMD window where the plugin installation/updation execution occurs. You must manually close the CMD window once the execution is completed to continue accessing the Appium wizard.\"", "Show execution status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
