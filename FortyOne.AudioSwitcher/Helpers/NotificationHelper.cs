using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FortyOne.AudioSwitcher.Helpers
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using global::FortyOne.AudioSwitcher.Helpers.FortyOne.AudioSwitcher.Helpers;

    namespace FortyOne.AudioSwitcher.Helpers
    {
        public class NotificationPopup : Form
        {
            private const int WS_EX_NOACTIVATE = 0x08000000; // Prevents activation
            private const int WS_EX_TOOLWINDOW = 0x00000080; // Hides from Alt+Tab
            private const int SWP_NOACTIVATE = 0x0010;       // Do not activate the window
            private const int SWP_NOZORDER = 0x0004;         // Keep Z order unchanged
            private const int HWND_TOPMOST = -1;            // Always on top
            private const int WM_NCACTIVATE = 0x0086;       // Non-client area activation
            private const int WM_MOUSEACTIVATE = 0x0021;    // Mouse activation event
            private const int MA_NOACTIVATEANDEAT = 0x0004; // Ignore activation

            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            [DllImport("user32.dll")]
            private static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            private IntPtr lastFocusedWindow;

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                    return cp;
                }
            }

            public NotificationPopup()
            {
                FormBorderStyle = FormBorderStyle.None;
                ShowIcon = false;
                ShowInTaskbar = false;
                TopMost = true;
            }

            protected override bool ShowWithoutActivation => true;

            public new void Show()
            {
                lastFocusedWindow = GetForegroundWindow(); // Save current focus
                base.Show();
                SetWindowPos(Handle, (IntPtr)HWND_TOPMOST, Left, Top, Width, Height, SWP_NOACTIVATE | SWP_NOZORDER);
            }

            // Prevents activation through Windows messages
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_NCACTIVATE)
                {
                    m.Result = (IntPtr)1; // Block activation
                    return;
                }
                if (m.Msg == WM_MOUSEACTIVATE)
                {
                    m.Result = (IntPtr)MA_NOACTIVATEANDEAT; // Prevent activation on click
                    return;
                }
                base.WndProc(ref m);
            }

            // Restore focus to the previous window after showing the popup
            protected override void OnShown(EventArgs e)
            {
                base.OnShown(e);
                if (lastFocusedWindow != IntPtr.Zero)
                    SetForegroundWindow(lastFocusedWindow);
            }
        }
    }

    public static class NotificationHelper
    {
        [DllImport("dwmapi.dll")]
        private static extern int DwmGetColorizationColor(out uint pcrColor, out bool pfOpaqueBlend);

        public static void SendToastNotification(string primaryDeviceName, string primaryDeviceType, 
            string secondaryDeviceName = null, string secondaryDeviceType = null)
        {
            // Create the message with the header and normal text
            string primaryHeader = $"Default {primaryDeviceType.ToLower()} device changed";
            string primaryBody = primaryDeviceName;
            string secondaryHeader = null;
            string secondaryBody = null;

            if (secondaryDeviceName != null && secondaryDeviceType != null)
            {
                secondaryHeader = $"Default {secondaryDeviceType.ToLower()} device changed";
                secondaryBody = secondaryDeviceName;
            }

            // Show the popup
            ShowPopup(primaryHeader, primaryBody, secondaryHeader, secondaryBody);
        }

        private static void ShowPopup(string primaryHeader, string primaryBody, string secondaryHeader, string secondaryBody)
        {
            bool hasSecondary = secondaryBody != null && secondaryHeader != null;

            // Get system theme (light or dark)
            string theme = GetSystemTheme();

            // Get accent color
            Color accentColor = GetAccentColor();

            // Create and configure the form to act as a custom popup
            NotificationPopup popupForm = new NotificationPopup
            {
                FormBorderStyle = FormBorderStyle.None, // No borders
                BackColor = theme == "Dark" ? Color.FromArgb(30, 30, 30) : Color.White, // Dark mode or Light mode
                Opacity = 1,                         // Ensure opacity is 100%
                StartPosition = FormStartPosition.Manual,
                Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - 310, 10), // Adjust position to ensure it's within visible bounds
                Size = !hasSecondary ? new Size(300, 80) : new Size(300, 170), // Set size of the popup
                ShowIcon = false,
                ShowInTaskbar = false, // Hide from Taskbar and Alt+Tab
                Icon = null,
                TopMost = false, // Set to false to prevent stealing focus
                Visible = false,  // Ensure the form is visible
            };

            // Header label (larger, bold)
            Label primaryHeaderLabel = new Label
            {
                Text = primaryHeader,
                ForeColor = theme == "Dark" ? Color.White : Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Padding = new Padding(5, 0, 0, 0), // Add left padding
                Width = popupForm.Width - 20, // Set the width of the label
                Location = new Point(10, 10), // Location of the header inside the popup
            };

            // Body label (normal text)
            Label primaryBodyLabel = new Label
            {
                Text = primaryBody,
                ForeColor = theme == "Dark" ? Color.White : Color.Black,
                Font = new Font("Arial", 10),
                Padding = new Padding(5, 0, 0, 0), // Add left padding
                Width = popupForm.Width - 20, // Set the width of the label
                Location = new Point(10, 50), // Location of the body text inside the popup (adjusted for spacing)
            };

            // Header label (larger, bold)
            Label secondaryHeaderLabel = new Label
            {
                Text = secondaryHeader,
                ForeColor = theme == "Dark" ? Color.White : Color.Black,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Padding = new Padding(5, 0, 0, 0), // Add left padding
                Width = popupForm.Width - 20, // Set the width of the label
                Location = new Point(10, 90), // Location of the header inside the popup
            };

            // Body label (normal text)
            Label secondaryBodyLabel = new Label
            {
                Text = secondaryBody,
                ForeColor = theme == "Dark" ? Color.White : Color.Black,
                Font = new Font("Arial", 10),
                Padding = new Padding(5, 0, 0, 0), // Add left padding
                Width = popupForm.Width - 20, // Set the width of the label
                Location = new Point(10, 140), // Location of the body text inside the popup (adjusted for spacing)
            };

            // Set accent color as background color for body text
            primaryBodyLabel.ForeColor = accentColor;
            secondaryBodyLabel.ForeColor = accentColor;

            // Add the labels to the popup form
            popupForm.Controls.Add(primaryHeaderLabel);
            popupForm.Controls.Add(primaryBodyLabel);
            if (hasSecondary)
            {
                popupForm.Controls.Add(secondaryHeaderLabel);
                popupForm.Controls.Add(secondaryBodyLabel);
            }

            // Timer to close the popup after 5 seconds
            Timer timer = new Timer
            {
                Interval = 5000 // 5 seconds
            };
            timer.Tick += (sender, e) =>
            {
                popupForm.Close(); // Close the popup
            };
            timer.Start();

            // Show the popup
            popupForm.Show();
        }

        // Get system theme (light or dark)
        private static string GetSystemTheme()
        {
            string theme = "Light"; // Default to light

            try
            {
                // Check registry for system theme (Windows 10 and newer)
                var themeKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
                if (themeKey != null)
                {
                    var value = themeKey.GetValue("AppsUseLightTheme");
                    if (value != null && (int)value == 0)
                    {
                        theme = "Dark"; // Dark mode
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (for cases where the registry doesn't work)
                Console.WriteLine(ex.Message);
            }

            return theme;
        }

        // Get the system accent color
        private static Color GetAccentColor()
        {
            uint color;
            bool isOpaque;
            DwmGetColorizationColor(out color, out isOpaque);

            // Convert the uint to a Color (ARGB)
            Color accentColor = Color.FromArgb(
                (int)(color & 0xFF),               // Blue
                (int)((color >> 8) & 0xFF),        // Green
                (int)((color >> 16) & 0xFF),       // Red
                (int)((color >> 24) & 0xFF)        // Alpha
            );

            return accentColor;
        }
    }
}
