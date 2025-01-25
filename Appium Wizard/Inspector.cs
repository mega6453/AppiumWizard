using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Appium_Wizard
{
    public partial class Inspector : Form
    {
        string sessionId;
        int port;
        public Inspector(string sessiondId = "", int port = 0)
        {
            this.sessionId = sessiondId;
            this.port = port;
            InitializeComponent();
        }

        private void Inspector_Load(object sender, EventArgs e)
        {
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await inspectorWebView.EnsureCoreWebView2Async(null);
            inspectorWebView.Source = new Uri("https://inspector.appiumpro.com/inspector.html");
            if (!string.IsNullOrEmpty(sessionId))
            {
                inspectorWebView.NavigationCompleted += async (s, e) =>
                {
                    try
                    {
                        string newScript = ScriptGenerator.GenerateScriptToAttachSession(sessionId, port);
                        string result = await inspectorWebView.CoreWebView2.ExecuteScriptAsync(newScript);
                        //MessageBox.Show(result);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed to attach to session. Please attach manually.");
                    }
                };
            }
        }

        private void Inspector_Shown(object sender, EventArgs e)
        {
            GoogleAnalytics.SendEvent("Inspector_Shown");
        }
    }


    public class ScriptGenerator
    {
        public static string GenerateScriptToAttachSession(string sessionid, int port)
        {
            string script = $@"
                    const elements = document.querySelectorAll('*'); // Select all elements
                    const textToClick = 'Attach to Session…'; // Replace with the text you're searching for

                    let found = false;
                    for (let element of elements) {{
                      if (element.textContent.trim() === textToClick) {{
                        console.log('Element found:', element); // Log the element
                        element.click(); // Simulate the click
                        found = true;
                        break; // Stop searching once the element is clicked
                      }}
                    }}

                    var element = document.getElementById(""customServerPort"");

                    // Set the value property of the element
                    element.value = '{port}';
                    if (found) {{
                      setTimeout(() => {{ // Introduce a delay
                        const inputElements = document.querySelectorAll('input[type=""search""], textarea');
                        for (let inputElement of inputElements) {{
                          inputElement.value ='{sessionid}'; // Set the value of the textbox
                          inputElement.dispatchEvent(new Event('input', {{ bubbles: true }})); // Trigger input event
                        }}

                        // Find the button by its text content and click it
                        const buttons = document.querySelectorAll('button');
                        for (let button of buttons) {{
                          if (button.textContent.trim() === 'Attach to Session' && !button.disabled) {{
                            console.log('Button found:', button); // Log the button
                            button.click(); // Simulate the click
                            break; // Stop once the button is clicked
                          }}
                        }}
                      }}, 500); // Adjust the delay as needed
                    }}
                    ";
            return script;
        }

        public static string GenerateScriptToAttachSession2(string sessionid)
        {
            string script = $@"
    const elements = document.querySelectorAll('*'); // Select all elements
    const textToClick = 'Attach to Session…'; // Replace with the text you're searching for

    let found = false;
    for (let element of elements) {{
        if (element.textContent.trim() === textToClick) {{
            console.log('Element found:', element); // Log the element
            element.click(); // Simulate the click
            found = true;
            break; // Stop searching once the element is clicked
        }}
    }}

    if (found) {{
        setTimeout(() => {{ // Introduce a delay
            const inputElements = document.querySelectorAll('input[type=""search""], textarea');
            for (let inputElement of inputElements) {{
                inputElement.value = '{sessionid}'; // Set the value of the textbox
                inputElement.dispatchEvent(new Event('input', {{ bubbles: true }})); // Trigger input event

                // Simulate Enter key press with a timeout
                setTimeout(() => {{
                    const enterKeyEvent = new KeyboardEvent('keydown', {{
                        key: 'Enter',
                        keyCode: 13, // Key code for Enter
                        code: 'Enter',
                        which: 13,
                        bubbles: true,
                        cancelable: true
                    }});
                    inputElement.dispatchEvent(enterKeyEvent);
                }}, 1000); // Timeout duration in milliseconds (e.g., 1000 ms = 1 second)

                // Find the button by its text content and click it
                const buttons = document.querySelectorAll('button');
                for (let button of buttons) {{
                    if (button.textContent.trim() === 'Attach to Session' && !button.disabled) {{
                        console.log('Button found:', button); // Log the button
                        button.click(); // Simulate the click
                        break; // Stop once the button is clicked
                    }}
                }}
            }}
        }}, 500); // Adjust the delay as needed
    }}
    ";
            return script;
        }
    }
}
