using System.Reflection;

namespace Appium_Wizard
{
    public partial class ServerConfig : Form
    {
        AppiumServerSetup serverSetup;
        public ServerConfig()
        {
            InitializeComponent();
            serverSetup = new AppiumServerSetup();
        }

        private void ServerConfig_Load(object sender, EventArgs e)
        {
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Get Status", "Please wait while Getting Appium server status...");

            Dictionary<string, string> readPortData = Database.QueryDataFromPortNumberTable();
            PortTextBox1.Text = readPortData["PortNumber1"];
            PortTextBox2.Text = readPortData["PortNumber2"];
            PortTextBox3.Text = readPortData["PortNumber3"];
            PortTextBox4.Text = readPortData["PortNumber4"];
            PortTextBox5.Text = readPortData["PortNumber5"];

            if (PortTextBox1.Text != string.Empty)
            {
                bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox1.Text));
                if (isRunning)
                {
                    StatusLabel1.Text = "Running";
                }
                else
                {
                    StatusLabel1.Text = "Not Running";
                }
            }

            if (PortTextBox2.Text != string.Empty)
            {
                bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox2.Text));
                if (isRunning)
                {
                    StatusLabel2.Text = "Running";
                }
                else
                {
                    StatusLabel2.Text = "Not Running";
                }
            }

            if (PortTextBox3.Text != string.Empty)
            {
                bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox3.Text));
                if (isRunning)
                {
                    StatusLabel3.Text = "Running";
                }
                else
                {
                    StatusLabel3.Text = "Not Running";
                }
            }

            if (PortTextBox4.Text != string.Empty)
            {
                bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox4.Text));
                if (isRunning)
                {
                    StatusLabel4.Text = "Running";
                }
                else
                {
                    StatusLabel4.Text = "Not Running";
                }
            }

            if (PortTextBox5.Text != string.Empty)
            {
                bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox5.Text));
                if (isRunning)
                {
                    StatusLabel5.Text = "Running";
                }
                else
                {
                    StatusLabel5.Text = "Not Running";
                }
            }

            commonProgress.Close();
            Task.Run(() =>
            {
                while (true)
                {
                    bool isRunning = serverSetup.IsAppiumServerRunning(int.Parse(PortTextBox1.Text));
                    if (isRunning)
                    {
                        Invoke(new Action(() => StatusLabel1.Text = "Running"));
                        GoogleAnalytics.SendEvent("ServerRunningInFirstPort");
                        break;
                    }
                    Thread.Sleep(2000);
                }
            });
        }

        private void StartServer(TextBox portTextbox, Label statusLabel, int serverNumber)
        {
            int portNumber = int.Parse(portTextbox.Text);
            serverSetup = new AppiumServerSetup();
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Start Server", "Please wait while Starting Appium server on port " + portNumber + ".This may take 30 seconds...");
            bool result = Common.IsPortBeingUsed(portNumber);
            if (result)
            {
                statusLabel.Text = "Not Running";
                MessageBox.Show("Port " + portNumber + " is being used by " + Common.RunNetstatAndFindProcessByPort(portNumber).Item2 + ".Please try to configure in different port.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("Failed to start Server on port " + portNumber + ".\nPlease try to configure in different port.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                GoogleAnalytics.SendEvent("StartServer_Port_Busy");
            }
            else
            {
                int wdaLocalPort = Common.GetFreePort();
                int screenport = Common.GetFreePort();
                serverSetup.StartAppiumServer(portNumber, wdaLocalPort, serverNumber, screenport);
                int count = 1;
                while (!ExecutionStatus.serverStarted)
                {
                    if (!string.IsNullOrEmpty(ExecutionStatus.statusText))
                    {
                        if (ExecutionStatus.statusText.Equals("address already in use"))
                        {
                            statusLabel.Text = "Not Running";
                            MessageBox.Show("Port " + portNumber + " is being used by " + Common.RunNetstatAndFindProcessByPort(4723).Item2 + ".Please try to configure in different port.", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                        else if (count == 10)
                        {
                            MessageBox.Show("Timed out after 30 seconds", "Error on Starting Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            GoogleAnalytics.SendEvent("StartServer_30Sec_Timedout");
                            break;
                        }
                    }
                    Thread.Sleep(3000);
                    count++;
                }
                if (ExecutionStatus.serverStarted)
                {
                    statusLabel.Text = "Running";
                    Database.UpdateDataIntoPortNumberTable("PortNumber" + serverNumber, portNumber);
                    MainScreen.runningProcessesPortNumbers.Add(portNumber);
                    GoogleAnalytics.SendEvent("ServerStarted");
                }
            }
            commonProgress.Close();
        }

        private void StopServer(TextBox portTextbox, Label statusLabel)
        {
            int portNumber = int.Parse(portTextbox.Text);
            CommonProgress commonProgress = new CommonProgress();
            commonProgress.Show();
            commonProgress.UpdateStepLabel("Stop Server", "Please wait while Stopping Appium server on port " + portNumber + "...");
            serverSetup.StopAppiumServer(portNumber);
            CheckPortAndUpdateLabel(portNumber, statusLabel);
            commonProgress.Close();
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }



        private void StartButton1_Click(object sender, EventArgs e)
        {
            StartServer(PortTextBox1, StatusLabel1, 1);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void StartButton2_Click(object sender, EventArgs e)
        {
            StartServer(PortTextBox2, StatusLabel2, 2);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void StartButton3_Click(object sender, EventArgs e)
        {
            StartServer(PortTextBox3, StatusLabel3, 3);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void StartButton4_Click(object sender, EventArgs e)
        {
            StartServer(PortTextBox4, StatusLabel4, 4);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        private void StartButton5_Click(object sender, EventArgs e)
        {
            StartServer(PortTextBox5, StatusLabel5, 5);
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }

        //---------------------------------------------------------

        private void StopButton1_Click(object sender, EventArgs e)
        {
            StopServer(PortTextBox1, StatusLabel1);
        }

        private void StopButton2_Click(object sender, EventArgs e)
        {
            StopServer(PortTextBox2, StatusLabel2);
        }

        private void StopButton3_Click(object sender, EventArgs e)
        {
            StopServer(PortTextBox3, StatusLabel3);
        }

        private void StopButton4_Click(object sender, EventArgs e)
        {
            StopServer(PortTextBox4, StatusLabel4);
        }

        private void StopButton5_Click(object sender, EventArgs e)
        {
            StopServer(PortTextBox5, StatusLabel5);
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

        private void CheckPortAndUpdateLabel(int port, Label statusLabel)
        {
            bool isRunning = serverSetup.IsAppiumServerRunning(port);
            if (isRunning)
            {
                statusLabel.Text = "Running";
            }
            else
            {
                statusLabel.Text = "Not Running";
            }
        }

        private void ServerConfig_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent(MethodBase.GetCurrentMethod().Name);
        }
    }
}
