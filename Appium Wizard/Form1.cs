using Appium_Wizard.Appium_Wizard;

namespace Appium_Wizard
{
    public partial class Form1 : Form
    {
        private ScrcpyEmbedder scrcpyEmbedder;

        public Form1()
        {
            InitializeComponent();

            scrcpyEmbedder = new ScrcpyEmbedder(@"C:\Users\mc\Desktop\scrcpy\scrcpy.exe");
            this.Controls.Add(scrcpyEmbedder.HostPanel);
            scrcpyEmbedder.HostPanel.Dock = DockStyle.Fill;

            // Hide the form immediately after InitializeComponent
            this.Visible = false;

            this.Shown += Form1_Shown;
            this.FormClosing += Form1_FormClosing;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            bool started = await scrcpyEmbedder.StartAsync();
            if (!started)
            {
                MessageBox.Show("Failed to start scrcpy embedding.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Show form after embedding is ready
            this.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            scrcpyEmbedder?.Dispose();
        }
    }
}

