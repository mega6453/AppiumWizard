namespace Appium_Wizard
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            label1.Text = "Version : " + VersionInfo.VersionNumber;
        }
    }
}
