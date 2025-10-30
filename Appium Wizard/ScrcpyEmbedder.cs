namespace Appium_Wizard
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace Appium_Wizard
    {
        public class ScrcpyEmbedder : IDisposable
        {
            // Add these Win32 API calls at the top of ScrcpyEmbedder class
            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            static extern bool UpdateWindow(IntPtr hWnd);

            // Constants for ShowWindow
            const int SW_HIDE = 0;
            const int SW_SHOW = 5;

            const int SW_SHOWNOACTIVATE = 4;
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

            delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            private Process scrcpyProcess;
            private Panel hostPanel;

            /// <summary>
            /// Gets the Panel control that hosts the embedded scrcpy window.
            /// Add this panel to your form or container.
            /// </summary>
            public Panel HostPanel => hostPanel;

            /// <summary>
            /// Path to scrcpy executable.
            /// </summary>
            public string ScrcpyPath { get; set; }

            /// <summary>
            /// Additional arguments for scrcpy.
            /// </summary>
            //public string ScrcpyArguments { get; set; } = "--max-size 1080 --window-title=\"scrcpy_embed\" --stay-awake --disable-screensaver";
            public string ScrcpyArguments { get; set; } = "--max-size 1080 --window-title=\"scrcpy_embed\" --stay-awake --disable-screensaver --window-x=-2000 --window-y=-2000";

            public ScrcpyEmbedder(string scrcpyExePath)
            {
                if (string.IsNullOrWhiteSpace(scrcpyExePath))
                    throw new ArgumentNullException(nameof(scrcpyExePath));

                ScrcpyPath = scrcpyExePath;

                hostPanel = new Panel
                {
                    //Dock = DockStyle.Fill,
                    BackColor = Color.Black
                };
            }

            public async Task<bool> StartAsync(string udid)
            {
                try
                {
                    if (scrcpyProcess != null && !scrcpyProcess.HasExited)
                        throw new InvalidOperationException("Scrcpy is already running.");

                    ScrcpyArguments = ScrcpyArguments + " -s "+udid;

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = ScrcpyPath,
                        Arguments = ScrcpyArguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    scrcpyProcess = new Process { StartInfo = psi };

                    bool started = scrcpyProcess.Start();
                    if (!started)
                        throw new InvalidOperationException("Failed to start scrcpy process.");

                    // Wait for scrcpy to initialize and create its window
                    IntPtr scrcpyHandle = IntPtr.Zero;
                    for (int attempts = 0; attempts < 50; attempts++)
                    {
                        await Task.Delay(200);

                        if (scrcpyProcess.HasExited)
                        {
                            string error = "";
                            try { error = await scrcpyProcess.StandardError.ReadToEndAsync(); }
                            catch { }

                            throw new InvalidOperationException($"Scrcpy exited with code: {scrcpyProcess.ExitCode} Error: { error }");
                        }

                        scrcpyHandle = FindScrcpyWindow();
                        if (scrcpyHandle != IntPtr.Zero)
                            break;
                    }

                    if (scrcpyHandle == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("Could not find scrcpy window. Make sure your device is connected and USB debugging is enabled.");
                    }

                    // Comment out or remove the form resizing to prevent flashing
                    // await ResizeParentFormToScrcpyWindow(scrcpyHandle);

                    // Embed the scrcpy window inside the panel
                    EmbedWindow(scrcpyHandle);

                    return true;
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    return false;
                }
            }


            /// <summary>
            /// Stops the scrcpy process and cleans up.
            /// </summary>
            public void Stop()
            {
                try
                {
                    if (scrcpyProcess != null && !scrcpyProcess.HasExited)
                    {
                        scrcpyProcess.Kill();
                        scrcpyProcess.Dispose();
                        scrcpyProcess = null;
                    }
                }
                catch { }
            }

            private async Task ResizeParentFormToScrcpyWindow(IntPtr scrcpyHandle)
            {
                await Task.Delay(1000); // Wait a bit for the scrcpy window to stabilize

                if (GetWindowRect(scrcpyHandle, out RECT scrcpyRect))
                {
                    int scrcpyWidth = scrcpyRect.Right - scrcpyRect.Left;
                    int scrcpyHeight = scrcpyRect.Bottom - scrcpyRect.Top;

                    if (hostPanel.Parent is Form parentForm)
                    {
                        // Calculate border sizes
                        Rectangle clientArea = parentForm.ClientRectangle;
                        Rectangle windowArea = parentForm.Bounds;

                        int borderWidth = windowArea.Width - clientArea.Width;
                        int borderHeight = windowArea.Height - clientArea.Height;

                        // Calculate total height of docked controls
                        int dockedControlsHeight = 0;
                        foreach (Control control in parentForm.Controls)
                        {
                            if (control.Dock == DockStyle.Top || control.Dock == DockStyle.Bottom)
                            {
                                dockedControlsHeight += control.Height;
                            }
                        }

                        parentForm.Invoke((Action)(() =>
                        {
                            parentForm.Size = new Size(
                                scrcpyWidth + borderWidth,
                                scrcpyHeight + borderHeight + dockedControlsHeight
                            );
                            CenterFormOnScreen(parentForm);
                        }));
                    }
                }
            }

            /// <summary>
            /// Centers a form on the screen manually
            /// </summary>
            private void CenterFormOnScreen(Form form)
            {
                Screen screen = Screen.FromControl(form);
                Rectangle workingArea = screen.WorkingArea;

                int x = (workingArea.Width - form.Width) / 2 + workingArea.X;
                int y = (workingArea.Height - form.Height) / 2 + workingArea.Y;

                form.Location = new Point(x, y);
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
                // Hide the window first to prevent flashing
                ShowWindow(windowHandle, SW_HIDE);

                // Remove window decorations and make it a child
                int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);
                int newStyle = currentStyle;

                newStyle &= ~WS_CAPTION;
                newStyle &= ~WS_THICKFRAME;
                newStyle &= ~WS_MINIMIZEBOX;
                newStyle &= ~WS_MAXIMIZEBOX;
                newStyle &= ~WS_SYSMENU;

                newStyle |= WS_CHILD;

                SetWindowLong(windowHandle, GWL_STYLE, newStyle);

                // Set the panel as the parent
                SetParent(windowHandle, hostPanel.Handle);

                // Resize and position the window to fill the panel exactly
                SetWindowPos(windowHandle, IntPtr.Zero, 0, 0,
                    hostPanel.Width, hostPanel.Height,
                    SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);

                // Now show the window in its embedded state
                ShowWindow(windowHandle, SW_SHOWNOACTIVATE);
                UpdateWindow(windowHandle);

                // Handle panel resize to keep the embedded window sized correctly
                hostPanel.Resize += (s, e) =>
                {
                    SetWindowPos(windowHandle, IntPtr.Zero, 0, 0,
                        hostPanel.Width, hostPanel.Height,
                        SWP_NOZORDER | SWP_NOACTIVATE);
                };

                // Change panel color to indicate success (optional)
                hostPanel.BackColor = Color.Black; // Keep it black instead of DarkBlue
            }

            private void EmbedWindowOld(IntPtr windowHandle)
            {
                // Remove window decorations and make it a child
                int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);
                int newStyle = currentStyle;

                newStyle &= ~WS_CAPTION;
                newStyle &= ~WS_THICKFRAME;
                newStyle &= ~WS_MINIMIZEBOX;
                newStyle &= ~WS_MAXIMIZEBOX;
                newStyle &= ~WS_SYSMENU;

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

            private void ShowError(string message)
            {
                if (hostPanel.InvokeRequired)
                {
                    hostPanel.Invoke(new Action(() => ShowError(message)));
                    return;
                }

                hostPanel.BackColor = Color.DarkRed;
                MessageBox.Show(hostPanel, message, "Scrcpy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            public void Dispose()
            {
                Stop();
                if (hostPanel != null)
                {
                    hostPanel.Dispose();
                    hostPanel = null;
                }
            }
        }
    }
}