using Microsoft.Web.WebView2.WinForms;

namespace Appium_Wizard
{
    public class InspectorForm : Form
    {
        public InspectorForm(string url)
        {
            Text = "Appium Inspector";
            Width = 1200;
            Height = 800;

            var webView = new WebView2 { Dock = DockStyle.Fill };
            Controls.Add(webView);

            Load += async (s, e) =>
            {
                await webView.EnsureCoreWebView2Async();
                webView.CoreWebView2.Navigate(url);
            };
        }
    }
}
