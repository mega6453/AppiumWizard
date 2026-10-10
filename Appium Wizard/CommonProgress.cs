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

        public bool IsCancelled { get; private set; }

        private readonly TaskCompletionSource cancelRequested = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        // Completes when the user clicks Cancel.
        public Task CancelRequested => cancelRequested.Task;

        // Awaits work, but returns false if the user cancels. After Cancel, the running work gets
        // graceMs to finish; if it doesn't, it is abandoned (it keeps running in the background).
        public async Task<bool> RunCancellable(Task work, int graceMs = 10000)
        {
            if (await Task.WhenAny(work, CancelRequested) == work)
            {
                await work;
                return true;
            }
            await Task.WhenAny(work, Task.Delay(graceMs));
            return false;
        }

        public void EnableCancel()
        {
            cancelButton.Visible = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            IsCancelled = true;
            cancelButton.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            commonProgressLabel.ForeColor = Color.Black;
            commonProgressLabel.Text = "Cancelling... Please wait for the current step to finish.";
            commonProgressLabel.Refresh();
            cancelRequested.TrySetResult();
        }

        public void UpdateStepLabel(string title, string stepText, int progressPercent = 50)
        {
            Logger.Debug(stepText);
            if (!this.Visible || IsCancelled)
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
                    if (title.Contains("Closing Appium Wizard"))
                    {
                        progressBar1.Visible = false;
                    }
                    progressBar1.Value = progressPercent;
                });
            }
            else
            {
                this.Text = title;
                commonProgressLabel.ForeColor = Color.Black;
                commonProgressLabel.Text = stepText;
                commonProgressLabel.Refresh();
                if (title.Contains("Closing Appium Wizard"))
                {
                    progressBar1.Visible = false;
                }
                progressBar1.Value = progressPercent;
            }
        }

        public void UpdateStepLabel(string stepText)
        {
            Logger.Debug(stepText);
            if (IsCancelled)
            {
                return;
            }
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
