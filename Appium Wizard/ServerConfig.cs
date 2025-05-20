using System.Reflection;

namespace Appium_Wizard
{
    public partial class ServerConfig : Form
    {
        AppiumServerSetup serverSetup;
        Dictionary<string, string> readPortData;
        int port1, port2, port3, port4, port5;
        public ServerConfig()
        {
            InitializeComponent();
            serverSetup = new AppiumServerSetup();
            readPortData = Database.QueryDataFromPortNumberTable();
            port1 = int.Parse(readPortData["PortNumber1"]);
            port2 = int.Parse(readPortData["PortNumber2"]);
            port3 = int.Parse(readPortData["PortNumber3"]);
            port4 = int.Parse(readPortData["PortNumber4"]);
            port5 = int.Parse(readPortData["PortNumber5"]);
        }

        private void ServerConfig_Load(object sender, EventArgs e)
        {
            PortTextBox1.Text = port1 == 0 ? string.Empty : port1.ToString();
            PortTextBox2.Text = port2 == 0 ? string.Empty : port2.ToString();
            PortTextBox3.Text = port3 == 0 ? string.Empty : port3.ToString();
            PortTextBox4.Text = port4 == 0 ? string.Empty : port4.ToString();
            PortTextBox5.Text = port5 == 0 ? string.Empty : port5.ToString();
            PortTextBox1_TextChanged(this, EventArgs.Empty);
            PortTextBox2_TextChanged(this, EventArgs.Empty);
            PortTextBox3_TextChanged(this, EventArgs.Empty);
            PortTextBox4_TextChanged(this, EventArgs.Empty);
            PortTextBox5_TextChanged(this, EventArgs.Empty);

            if (StatusLabel1.Text.Equals("Running"))
            {
                StartButton1.Enabled = false;
            }
            if (StatusLabel2.Text.Equals("Running"))
            {
                StartButton2.Enabled = false;
            }
            if (StatusLabel3.Text.Equals("Running"))
            {
                StartButton3.Enabled = false;
            }
            if (StatusLabel4.Text.Equals("Running"))
            {
                StartButton4.Enabled = false;
            }
            if (StatusLabel5.Text.Equals("Running"))
            {
                StartButton5.Enabled = false;
            }


            Task.Run(() =>
            {
                while (true)
                {
                    if (port1 != 0)
                    {
                        bool isRunning = serverSetup.IsAppiumServerRunning(port1);
                        if (isRunning)
                        {
                            try
                            {
                                Invoke(new Action(() => StatusLabel1.Text = "Running"));
                                GoogleAnalytics.SendEvent("ServerRunningInFirstPort");
                                break;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    Task.Delay(2000);
                }
            });
        }

        public async Task isServerRunning(MainScreen mainScreen = null)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = mainScreen;
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 10);
            bool isRunning1 = false;
            bool isRunning2 = false;
            bool isRunning3 = false;
            bool isRunning4 = false;
            bool isRunning5 = false;
            await Task.Run(() =>
            {
                if (port1 != 0)
                {
                    isRunning1 = serverSetup.IsAppiumServerRunning(port1);
                    commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 20);
                }
                if (port2 != 0)
                {
                    isRunning2 = serverSetup.IsAppiumServerRunning(port2);
                    commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 40);
                }
                if (port3 != 0)
                {
                    isRunning3 = serverSetup.IsAppiumServerRunning(port3);
                    commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 60);
                }
                if (port4 != 0)
                {
                    isRunning4 = serverSetup.IsAppiumServerRunning(port4);
                    commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 80);
                }
                if (port5 != 0)
                {
                    isRunning5 = serverSetup.IsAppiumServerRunning(port5);
                    commonProgress.UpdateStepLabel("Get Status", "Please wait while getting Appium server status...", 100);
                }
            });

            //-----
            if (isRunning1)
            {
                StatusLabel1.Text = "Running";
            }
            else
            {
                StatusLabel1.Text = "Not Running";
            }
            //-----
            if (isRunning2)
            {
                StatusLabel2.Text = "Running";
            }
            else
            {
                StatusLabel2.Text = "Not Running";
            }
            //-----
            if (isRunning3)
            {
                StatusLabel3.Text = "Running";
            }
            else
            {
                StatusLabel3.Text = "Not Running";
            }
            //-----
            if (isRunning4)
            {
                StatusLabel4.Text = "Running";
            }
            else
            {
                StatusLabel4.Text = "Not Running";
            }
            //-----
            if (isRunning5)
            {
                StatusLabel5.Text = "Running";
            }
            else
            {
                StatusLabel5.Text = "Not Running";
            }
            commonProgress.Close();
        }


        private async Task StartServer(TextBox portTextbox, Label statusLabel, int serverNumber)
        {
            int portNumber = int.Parse(portTextbox.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            serverSetup = new AppiumServerSetup();
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Owner = this;
            commonProgress.Show();
            bool result = false;
            await Task.Run(() =>
            {
                commonProgress.UpdateStepLabel("Start Server", "Please wait while checking the availability of the port " + portNumber + "...", 10);
                result = Common.IsPortBeingUsed(portNumber);
            });
            if (result)
            {
                statusLabel.Text = "Not Running";
                var confirmationResult = MessageBox.Show("Port " + portNumber + " is being used by " + Common.RunNetstatAndFindProcessByPort(LoadingScreen.appiumPort).Item2 + ".\n\nDo you want to kill that process and start appium server in that port?\n\nIf No, then please try to configure in different port.", "Error on Starting Server", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (confirmationResult == DialogResult.Yes)
                {
                    Common.KillProcessByPortNumber(portNumber);
                }
                else
                {
                    commonProgress.Close();
                    return;
                }
            }
            commonProgress.UpdateStepLabel("Start Server", "Please wait while starting Appium server on port " + portNumber + ".This may take 30+ seconds...", 10);
            await Task.Run(() =>
            {
                string command = Database.QueryDataFromServerFinalCommandTable()["Server" + serverNumber];
                serverSetup.StartAppiumServer(portNumber, serverNumber, command);
            });
            int count = 1;
            while (!serverSetup.serverStarted)
            {
                if (!string.IsNullOrEmpty(serverSetup.statusText))
                {
                    if (serverSetup.statusText.Equals("address already in use"))
                    {
                        statusLabel.Text = "Not Running";
                        MessageBox.Show("Port " + portNumber + " is being used by " + Common.RunNetstatAndFindProcessByPort(4723).Item2 + ".Please try to configure in different port.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                    else if (count == 20)
                    {
                        MessageBox.Show("Timed out after 60 seconds:\nPlease check the Final command in the Server Setup -> Settings and fix if command has any issue.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        GoogleAnalytics.SendEvent("StartServer_45Sec_Timedout");
                        break;
                    }
                }
                commonProgress.Invoke((System.Windows.Forms.MethodInvoker)(() =>
                {
                    commonProgress.UpdateStepLabel("Start Server", "Please wait while starting Appium server on port " + portNumber + ".This may take 30+ seconds...", 10 * count);
                }));
                count++;
                if (count == 20)
                {
                    MessageBox.Show("Timed out after 60 seconds:\nPlease check the Final command in the Server Setup -> Settings and fix if command has any issue.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GoogleAnalytics.SendEvent("StartServer_45Sec_Timedout");
                    break;
                }
                await Task.Delay(3000);
            }
            if (serverSetup.serverStarted)
            {
                statusLabel.Text = "Running";
                Database.UpdateDataIntoPortNumberTable("PortNumber" + serverNumber, portNumber);
                GoogleAnalytics.SendEvent("ServerStarted");
            }
            commonProgress.Close();
        }

        private async Task StopServer(TextBox portTextbox, Label statusLabel)
        {
            int portNumber = int.Parse(portTextbox.Text);
            bool isRunning = false;
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Stop Server", "Please wait while Stopping Appium server on port " + portNumber + "...");
            await Task.Run(() =>
            {
                serverSetup.StopAppiumServer(portNumber);
            });
            if (isRunning)
            {
                statusLabel.Text = "Running";
            }
            else
            {
                statusLabel.Text = "Not Running";
            }
            commonProgress.Close();
            GoogleAnalytics.SendEvent("StopServer");
        }



        private async void StartButton1_Click(object sender, EventArgs e)
        {
            await StartServer(PortTextBox1, StatusLabel1, 1);
            GoogleAnalytics.SendEvent("StartButton1_Click");
        }

        private async void StartButton2_Click(object sender, EventArgs e)
        {
            await StartServer(PortTextBox2, StatusLabel2, 2);
            GoogleAnalytics.SendEvent("StartButton2_Click");
        }

        private async void StartButton3_Click(object sender, EventArgs e)
        {
            await StartServer(PortTextBox3, StatusLabel3, 3);
            GoogleAnalytics.SendEvent("StartButton3_Click");
        }

        private async void StartButton4_Click(object sender, EventArgs e)
        {
            await StartServer(PortTextBox4, StatusLabel4, 4);
            GoogleAnalytics.SendEvent("StartButton4_Click");
        }

        private async void StartButton5_Click(object sender, EventArgs e)
        {
            await StartServer(PortTextBox5, StatusLabel5, 5);
            GoogleAnalytics.SendEvent("StartButton5_Click");
        }

        //---------------------------------------------------------

        private async void StopButton1_Click(object sender, EventArgs e)
        {
            await StopServer(PortTextBox1, StatusLabel1);
        }

        private async void StopButton2_Click(object sender, EventArgs e)
        {
            await StopServer(PortTextBox2, StatusLabel2);
        }

        private async void StopButton3_Click(object sender, EventArgs e)
        {
            await StopServer(PortTextBox3, StatusLabel3);
        }

        private async void StopButton4_Click(object sender, EventArgs e)
        {
            await StopServer(PortTextBox4, StatusLabel4);
        }

        private async void StopButton5_Click(object sender, EventArgs e)
        {
            await StopServer(PortTextBox5, StatusLabel5);
        }

        //---------------------------------------------------------

        private void StatusLabel1_TextChanged(object sender, EventArgs e)
        {
            StatusLabel_TextChanged(StatusLabel1, PortTextBox1, StartButton1, StopButton1);
        }

        private void StatusLabel2_TextChanged(object sender, EventArgs e)
        {
            StatusLabel_TextChanged(StatusLabel2, PortTextBox2, StartButton2, StopButton2);
        }

        private void StatusLabel3_TextChanged(object sender, EventArgs e)
        {
            StatusLabel_TextChanged(StatusLabel3, PortTextBox3, StartButton3, StopButton3);
        }

        private void StatusLabel4_TextChanged(object sender, EventArgs e)
        {
            StatusLabel_TextChanged(StatusLabel4, PortTextBox4, StartButton4, StopButton4);
        }

        private void StatusLabel5_TextChanged(object sender, EventArgs e)
        {
            StatusLabel_TextChanged(StatusLabel5, PortTextBox5, StartButton5, StopButton5);
        }

        private void StatusLabel_TextChanged(Label statusLabel, TextBox portTextbox, Button startButton, Button stopButton)
        {
            if (statusLabel.Text.Equals("Running"))
            {
                portTextbox.Enabled = false;
                startButton.Enabled = false;
                stopButton.Enabled = true;
            }
            else
            {
                portTextbox.Enabled = true;
                startButton.Enabled = true;
                stopButton.Enabled = false;
            }
        }

        //---------------------------------------------------------

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Textbox_KeyPress(sender, e);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Textbox_KeyPress(sender, e);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            Textbox_KeyPress(sender, e);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            Textbox_KeyPress(sender, e);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            Textbox_KeyPress(sender, e);
        }

        private void Textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //---------------------------------------------------------

        private void ServerConfig_Shown(object sender, EventArgs e)
        {
           GoogleAnalytics.SendEvent("ServerConfig_Shown");
        }

        private bool isValidPortNumber(int port)
        {
            if (port < 1 | port > 65535)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ConfigButton1_Click(object sender, EventArgs e)
        {
            int portNumber = int.Parse(PortTextBox1.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Server_Settings server_Settings = new Server_Settings("Server1", portNumber);
            server_Settings.ShowDialog();
        }

        private void ConfigButton2_Click(object sender, EventArgs e)
        {
            int portNumber = int.Parse(PortTextBox2.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Server_Settings server_Settings = new Server_Settings("Server2", portNumber);
            server_Settings.ShowDialog();
        }

        private void ConfigButton3_Click(object sender, EventArgs e)
        {
            int portNumber = int.Parse(PortTextBox3.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Server_Settings server_Settings = new Server_Settings("Server3", portNumber);
            server_Settings.ShowDialog();
        }

        private void ConfigButton4_Click(object sender, EventArgs e)
        {
            int portNumber = int.Parse(PortTextBox4.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Server_Settings server_Settings = new Server_Settings("Server4", portNumber);
            server_Settings.ShowDialog();
        }

        private void ConfigButton5_Click(object sender, EventArgs e)
        {
            int portNumber = int.Parse(PortTextBox5.Text);
            if (!isValidPortNumber(portNumber))
            {
                MessageBox.Show("Please enter a valid port number. For starting an Appium server, you can use any port in the range 1 to 65535.\n\nCommonly used ports for Appium server is from 4723 to 4730.", "Invalid Port Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Server_Settings server_Settings = new Server_Settings("Server5", portNumber);
            server_Settings.ShowDialog();
        }

        private void PortTextBox1_TextChanged(object sender, EventArgs e)
        {
            ConfigButton1.Enabled = !string.IsNullOrWhiteSpace(PortTextBox1.Text);
            StartButton1.Enabled = !string.IsNullOrWhiteSpace(PortTextBox1.Text);
        }

        private void PortTextBox2_TextChanged(object sender, EventArgs e)
        {
            ConfigButton2.Enabled = !string.IsNullOrWhiteSpace(PortTextBox2.Text);
            StartButton2.Enabled = !string.IsNullOrWhiteSpace(PortTextBox2.Text);
        }

        private void PortTextBox3_TextChanged(object sender, EventArgs e)
        {
            ConfigButton3.Enabled = !string.IsNullOrWhiteSpace(PortTextBox3.Text);
            StartButton3.Enabled = !string.IsNullOrWhiteSpace(PortTextBox3.Text);
        }

        private void PortTextBox4_TextChanged(object sender, EventArgs e)
        {
            ConfigButton4.Enabled = !string.IsNullOrWhiteSpace(PortTextBox4.Text);
            StartButton4.Enabled = !string.IsNullOrWhiteSpace(PortTextBox4.Text);
        }

        private void PortTextBox5_TextChanged(object sender, EventArgs e)
        {
            ConfigButton5.Enabled = !string.IsNullOrWhiteSpace(PortTextBox5.Text);
            StartButton5.Enabled = !string.IsNullOrWhiteSpace(PortTextBox5.Text);
        }
    }
}
