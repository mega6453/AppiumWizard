using NLog;

namespace Appium_Wizard
{
    public partial class CommonProgress : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CommonProgress()
        {
            InitializeComponent();
            //Icon = Properties.Resources.appiumlogo;

            int labelStartX = commonProgressLabel.Location.X;
            int formWidth = this.ClientSize.Width;
            int availableWidth = formWidth - labelStartX;
            commonProgressLabel.Width = availableWidth;
            commonProgressLabel.MaximumSize = new Size(availableWidth, 0);
        }

        public void UpdateStepLabel(string title, string stepText, int progressPercent = 50)
        {
            Logger.Debug(stepText);
            if (!this.Visible)
            {
                return;
            }
            if (progressPercent >= 100)
            {
                progressPercent = 95;
            }
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = title;
                    commonProgressLabel.ForeColor = Color.Black;
                    commonProgressLabel.Text = stepText;
                    commonProgressLabel.Refresh();
                    progressBar1.Value = progressPercent;
                });
            }
            else
            {
                this.Text = title;
                commonProgressLabel.ForeColor = Color.Black;
                commonProgressLabel.Text = stepText;
                commonProgressLabel.Refresh();
                progressBar1.Value = progressPercent;
            }
        }

        public void UpdateStepLabel(string stepText)
        {
            Logger.Debug(stepText);
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = "Run WebDriverAgent";
                    commonProgressLabel.ForeColor = Color.Red;
                    commonProgressLabel.Text = stepText;
                    commonProgressLabel.Refresh();
                });
            }
            else
            {
                this.Text = "Run WebDriverAgent";
                commonProgressLabel.ForeColor = Color.Red;
                commonProgressLabel.Text = stepText;
                commonProgressLabel.Refresh();
            }
        }

        public new void Close()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        base.Close();
                    });
                }
                else
                {
                    base.Close();
                }
            }
            catch (Exception)
            {
            }
        }


        public new void Hide()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    base.Hide();
                });
            }
            else
            {
                base.Hide();
            }
        }


        //MessageBox.Show("Sorry, Cannot cancel the execution in the current version.\nMay be in future version 😊", "Cancel Execution", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
