namespace Appium_Wizard
{
    public partial class CommonProgress : Form
    {
        public CommonProgress()
        {
            InitializeComponent();
            //Icon = Properties.Resources.appiumlogo;
        }

        public void UpdateStepLabel(string title, string stepText, int progressPercent = 50)
        {
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
            commonProgressLabel.ForeColor = Color.Red;
            commonProgressLabel.Text = stepText;
            commonProgressLabel.Refresh();
        }
    }
}
