namespace Appium_Wizard
{
    public partial class DeviceLookUp : Form
    {
        string labelText;
        public DeviceLookUp(string text)
        {
            InitializeComponent();
            this.labelText = text;
        }

        private void DeviceLookUp_Load(object sender, EventArgs e)
        {
            this.Show();
            if (labelText.Contains("iOS"))
            {
                this.Text = "Detect iOS Device";
            }
            if (labelText.Contains("Android"))
            {
                this.Text = "Detect Android Device";
            }
            label1.Text = this.labelText;
            Refresh();
        }
    }
}
