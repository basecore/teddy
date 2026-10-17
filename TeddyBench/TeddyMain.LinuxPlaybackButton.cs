using System.Drawing;
using System.Windows.Forms;

namespace TeddyBench
{
    public partial class TeddyMain
    {
        internal void ConfigureLinuxPlaybackButton()
        {
            lstTonies.DoubleClick -= LinuxPlayClick;
            lstTonies.DoubleClick += lstTonies_DoubleClick;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.UseVisualStyleBackColor = false;
            btnPlay.Paint += delegate(object sender, PaintEventArgs e)
            {
                Rectangle bounds = btnPlay.ClientRectangle;
                if (bounds.Width < 3 || bounds.Height < 3) return;
                using (var background = new SolidBrush(Color.White))
                    e.Graphics.FillRectangle(background, bounds);
                bounds.Width--;
                bounds.Height--;
                using (var border = new Pen(Color.FromArgb(100, 116, 139)))
                    e.Graphics.DrawRectangle(border, bounds);
                using (var format = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap
                })
                using (var foreground = new SolidBrush(btnPlay.Enabled ? Color.Black : Color.Gray))
                {
                    string caption = linuxPlaybackCancellation == null ? "Play" : "Stop";
                    e.Graphics.DrawString(caption, btnPlay.Font, foreground,
                        new RectangleF(1, 1, bounds.Width - 1, bounds.Height - 1), format);
                }
                if (btnPlay.Focused && btnPlay.ShowFocusCuesForLinux())
                    ControlPaint.DrawFocusRectangle(e.Graphics,
                        Rectangle.Inflate(btnPlay.ClientRectangle, -3, -3));
            };
            btnPlay.TextChanged += delegate { btnPlay.Invalidate(); };
            btnPlay.EnabledChanged += delegate { btnPlay.Invalidate(); };
            btnPlay.GotFocus += delegate { btnPlay.Invalidate(); };
            btnPlay.LostFocus += delegate { btnPlay.Invalidate(); };
            btnPlay.Invalidate();
        }
    }

    internal static class LinuxPlaybackFocus
    {
        internal static bool ShowFocusCuesForLinux(this Button button)
        {
            return button.Focused;
        }
    }
}
