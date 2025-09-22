using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Appium_Wizard
{
    public partial class Form1 : Form
    {
        // Win32 API constants
        const int GWL_STYLE = -16;
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;
        const int WS_MINIMIZE = 0x20000000;
        const int WS_MAXIMIZEBOX = 0x00010000;
        const int WS_MINIMIZEBOX = 0x00020000;
        const int WS_SYSMENU = 0x00080000;
        const int WS_CHILD = 0x40000000;

        const int SWP_NOZORDER = 0x0004;
        const int SWP_NOACTIVATE = 0x0010;
        const int SWP_FRAMECHANGED = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private Process scrcpyProcess;
        private Panel hostPanel;

        public Form1()
        {
            InitializeComponent();

            // Create a panel to host scrcpy window
            hostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            this.Controls.Add(hostPanel);

            // Start embedding scrcpy automatically after form is shown
            this.Load += Form1_Load;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Give the form a moment to fully load
            await Task.Delay(500);
            await EmbedScrcpyAsync();
        }

        private async Task EmbedScrcpyAsync()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\mc\\Desktop\\scrcpy-win64-v3.3.1\\scrcpy.exe",
                    Arguments = "--max-size 1080 --window-title=\"scrcpy_embed\" --stay-awake --disable-screensaver",
                    //Arguments = "--max-size 800 --max-fps 60 --window-title=\"scrcpy_embed\" --stay-awake --disable-screensaver",
                    UseShellExecute = false,
                    CreateNoWindow = true, // Hide the cmd window
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                scrcpyProcess = new Process { StartInfo = psi };

                bool started = scrcpyProcess.Start();
                if (!started)
                {
                    ShowError("Failed to start scrcpy process.");
                    return;
                }

                // Wait for scrcpy to initialize and create its window
                IntPtr scrcpyHandle = IntPtr.Zero;
                for (int attempts = 0; attempts < 50; attempts++) // Wait up to 10 seconds
                {
                    await Task.Delay(200);

                    if (scrcpyProcess.HasExited)
                    {
                        string error = "";
                        try { error = await scrcpyProcess.StandardError.ReadToEndAsync(); }
                        catch { }

                        ShowError($"Scrcpy exited with code: {scrcpyProcess.ExitCode}\nError: {error}");
                        return;
                    }

                    scrcpyHandle = FindScrcpyWindow();
                    if (scrcpyHandle != IntPtr.Zero)
                        break;
                }

                if (scrcpyHandle == IntPtr.Zero)
                {
                    ShowError("Could not find scrcpy window. Make sure your device is connected and USB debugging is enabled.");
                    return;
                }

                // Resize form to match scrcpy window size first
                await ResizeFormToScrcpyWindow(scrcpyHandle);

                // Then embed the window
                EmbedWindow(scrcpyHandle);
            }
            catch (Exception ex)
            {
                ShowError($"Error starting scrcpy: {ex.Message}");
            }
        }

        private async Task ResizeFormToScrcpyWindow(IntPtr scrcpyHandle)
        {
            // Wait a bit for the scrcpy window to stabilize
            await Task.Delay(1000);

            // Get the scrcpy window size
            RECT scrcpyRect;
            if (GetWindowRect(scrcpyHandle, out scrcpyRect))
            {
                int scrcpyWidth = scrcpyRect.Right - scrcpyRect.Left;
                int scrcpyHeight = scrcpyRect.Bottom - scrcpyRect.Top;

                // Get the current form's client area and window size to calculate borders
                Rectangle clientArea = this.ClientRectangle;
                Rectangle windowArea = this.Bounds;

                int borderWidth = windowArea.Width - clientArea.Width;
                int borderHeight = windowArea.Height - clientArea.Height;

                // Set the form size to match scrcpy window plus borders
                this.Invoke((Action)(() =>
                {
                    this.Size = new Size(scrcpyWidth + borderWidth, scrcpyHeight + borderHeight);
                    this.CenterToScreen();
                }));
            }
        }

        private IntPtr FindScrcpyWindow()
        {
            if (scrcpyProcess == null || scrcpyProcess.HasExited)
                return IntPtr.Zero;

            IntPtr foundWindow = IntPtr.Zero;
            uint processId = (uint)scrcpyProcess.Id;

            EnumWindows((hWnd, lParam) =>
            {
                if (!IsWindowVisible(hWnd))
                    return true;

                uint windowProcessId;
                GetWindowThreadProcessId(hWnd, out windowProcessId);

                if (windowProcessId == processId)
                {
                    StringBuilder windowTitle = new StringBuilder(256);
                    GetWindowText(hWnd, windowTitle, windowTitle.Capacity);

                    string title = windowTitle.ToString();
                    if (title.Contains("scrcpy") || string.IsNullOrEmpty(title))
                    {
                        foundWindow = hWnd;
                        return false; // Stop enumeration
                    }
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return foundWindow;
        }

        private void EmbedWindow(IntPtr windowHandle)
        {
            try
            {
                // Remove window decorations and make it a child
                int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);
                int newStyle = currentStyle;

                // Remove all window decorations
                newStyle &= ~WS_CAPTION;
                newStyle &= ~WS_THICKFRAME;
                newStyle &= ~WS_MINIMIZEBOX;
                newStyle &= ~WS_MAXIMIZEBOX;
                newStyle &= ~WS_SYSMENU;

                // Make it a child window
                newStyle |= WS_CHILD;

                SetWindowLong(windowHandle, GWL_STYLE, newStyle);

                // Set the panel as the parent
                SetParent(windowHandle, hostPanel.Handle);

                // Resize and position the window to fill the panel exactly
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0,
                    hostPanel.Width, hostPanel.Height,
                    SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);

                // Handle panel resize to keep the embedded window sized correctly
                hostPanel.Resize += (s, e) =>
                {
                    SetWindowPos(windowHandle, IntPtr.Zero, 0, 0,
                        hostPanel.Width, hostPanel.Height,
                        SWP_NOZORDER | SWP_NOACTIVATE);
                };

                // Change panel color to indicate success
                hostPanel.BackColor = Color.DarkBlue;
            }
            catch (Exception ex)
            {
                ShowError($"Error embedding window: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowError(message)));
                return;
            }

            hostPanel.BackColor = Color.DarkRed;
            MessageBox.Show(message, "Scrcpy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                if (scrcpyProcess != null && !scrcpyProcess.HasExited)
                {
                    scrcpyProcess.Kill();
                    scrcpyProcess.Dispose();
                }
            }
            catch { }

            base.OnFormClosed(e);
        }
    }
}