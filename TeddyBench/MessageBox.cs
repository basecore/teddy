using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeddyBench
{
    // Resolve existing unqualified MessageBox.Show calls in this namespace.
    internal static class MessageBox
    {
        public static DialogResult Show(string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1,
            MessageBoxOptions options = (MessageBoxOptions)0)
        {
            return Show(null, text, caption, buttons, icon, defaultButton, options);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1,
            MessageBoxOptions options = (MessageBoxOptions)0)
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
                return System.Windows.Forms.MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);

            Control control = owner as Control;
            if (control != null && control.InvokeRequired)
                return (DialogResult)control.Invoke(new Func<DialogResult>(() =>
                    Show(owner, text, caption, buttons, icon, defaultButton, options)));

            DialogResult[] results;
            switch (buttons)
            {
                case MessageBoxButtons.OK: results = new[] { DialogResult.OK }; break;
                case MessageBoxButtons.OKCancel: results = new[] { DialogResult.OK, DialogResult.Cancel }; break;
                case MessageBoxButtons.YesNo: results = new[] { DialogResult.Yes, DialogResult.No }; break;
                case MessageBoxButtons.YesNoCancel: results = new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel }; break;
                case MessageBoxButtons.RetryCancel: results = new[] { DialogResult.Retry, DialogResult.Cancel }; break;
                case MessageBoxButtons.AbortRetryIgnore: results = new[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore }; break;
                default: throw new NotSupportedException("Unsupported message box buttons: " + buttons);
            }

            using (Form dialog = new Form())
            {
                dialog.Text = caption ?? "";
                dialog.ClientSize = new Size(640, 360);
                dialog.MinimumSize = new Size(480, 260);
                dialog.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.FormBorderStyle = FormBorderStyle.Sizable;

                TextBox message = new TextBox();
                message.Multiline = true;
                message.ReadOnly = true;
                message.ScrollBars = ScrollBars.Vertical;
                message.BorderStyle = BorderStyle.None;
                message.BackColor = SystemColors.Control;
                message.Text = text ?? "";
                message.SetBounds(18, 18, 604, 270);
                message.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dialog.Controls.Add(message);

                FlowLayoutPanel panel = new FlowLayoutPanel();
                panel.Dock = DockStyle.Bottom;
                panel.Height = 65;
                panel.Padding = new Padding(10);
                panel.FlowDirection = FlowDirection.LeftToRight;
                dialog.Controls.Add(panel);

                Button cancel = null;
                Button preferred = null;
                int preferredIndex = Math.Min((int)defaultButton / 256, results.Length - 1);
                for (int i = 0; i < results.Length; i++)
                {
                    Button button = new Button();
                    button.Text = results[i].ToString();
                    button.Size = new Size(130, 38);
                    button.DialogResult = results[i];
                    panel.Controls.Add(button);
                    if (results[i] == DialogResult.Cancel) cancel = button;
                    if (i == preferredIndex) preferred = button;
                    if (buttons == MessageBoxButtons.OK) cancel = button;
                }

                // No window-close shortcut for choices without a cancel action.
                dialog.ControlBox = cancel != null;
                dialog.CancelButton = cancel;
                dialog.AcceptButton = preferred;
                dialog.Shown += delegate { if (preferred != null) preferred.Focus(); };
                dialog.FormClosing += delegate(object sender, FormClosingEventArgs e)
                {
                    if (dialog.DialogResult == DialogResult.None)
                    {
                        if (cancel == null) e.Cancel = true;
                        else dialog.DialogResult = cancel.DialogResult;
                    }
                };
                return owner == null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
            }
        }
    }
}
