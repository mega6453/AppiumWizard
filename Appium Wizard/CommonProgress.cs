namespace Appium_Wizard
{
    public partial class CommonProgress : Form
    {
        public CommonProgress()
        {
            InitializeComponent();
            //Icon = Properties.Resources.appiumlogo;
        }

        public void UpdateStepLabel(string title, string stepText)
        {
            this.Text = title;
            commonProgressLabel.ForeColor = Color.Black;
            commonProgressLabel.Text = stepText;
            commonProgressLabel.Refresh();
        }

        public void UpdateStepLabel(string stepText)
        {
            commonProgressLabel.ForeColor = Color.Red;
            commonProgressLabel.Text = stepText;
            commonProgressLabel.Refresh();
        }
    }
}
