using System.Runtime.InteropServices;

namespace StayAwake;

public class StayAwakeContext : ApplicationContext
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

    [Flags]
    private enum EXECUTION_STATE : uint
    {
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002
    }

    private readonly NotifyIcon _trayIcon;
    private bool _isActive;

    public StayAwakeContext()
    {
        _trayIcon = new NotifyIcon
        {
            Icon = new Icon("Resources/gnome_eyes.ico"),
            ContextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    new ToolStripMenuItem("Activate", null, KeepAwake),
                    new ToolStripMenuItem("About", null, ShowAbout),
                    new ToolStripMenuItem("Exit", null, Exit)
                }
            },
            Text = "StayAwake - Inactive",
            Visible = true
        };
    }

    private void Exit(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        // Reset to default ThreadExecutionState before closing
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        Application.Exit();
    }

    private void KeepAwake(object? sender, EventArgs e)
    {
        if (sender != null)
        {
            var buttonText = (ToolStripItem)sender;

            _trayIcon.ContextMenuStrip!.Items[0].Text = buttonText.Text.Equals("Activate") ? "Deactivate" : "Activate";
            _trayIcon.Text = _trayIcon.Text.Equals("StayAwake - Inactive")
                ? "StayAwake - Active"
                : "StayAwake - Inactive";
        }

        _isActive = !_isActive;

        SetThreadExecutionState(_isActive ? EXECUTION_STATE.ES_DISPLAY_REQUIRED : EXECUTION_STATE.ES_CONTINUOUS);
    }

    private static void ShowAbout(object? sender, EventArgs e)
    {
        MessageBox.Show(
            "Prevent your display from automatically sleeping." +
            "\nCreated by Kyler" +
            "\nGitHub: https://github.com/ky-ler/StayAwake" +
            "\nApp Icon: https://github.com/daemonblade/gartoon-redux",
            "StayAwake",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    }
}