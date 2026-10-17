using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TeddyBench
{
    public partial class TeddyMain
    {
        private bool linuxUiApplied;

        internal void ApplyLinuxUi()
        {
            if (linuxUiApplied) return;
            linuxUiApplied = true;

            ApplyLinuxColors(this);
            btnPlay.Text = "Play file";
            btnPlay.AccessibleName = "Play selected audio file";
            btnPlay.UseCompatibleTextRendering = false;

            helpToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem aboutFork = new ToolStripMenuItem("About this Linux fork...");
            aboutFork.Click += delegate {
                MessageBox.Show(this,
                    "TeddyBench - unofficial Linux adaptation\r\n\r\n" +
                    "Maintained by basecore with assistance from Perplexity AI.\r\n" +
                    "Based on the upstream TeddyBench 1.7.0 code line and subsequent master changes.\r\n\r\n" +
                    "Project: https://github.com/basecore/teddy\r\n" +
                    "Working branch: linux-1.7.0\r\n" +
                    "Upstream: toniebox-reverse-engineering/teddy\r\n" +
                    "Historical Linux work: AlbrechtL/teddy\r\n\r\n" +
                    "Runs using Mono and libgdiplus; this is not an official upstream release.\r\n" +
                    "Original authors and third-party notices remain applicable.\r\n\r\n" +
                    "Launch, CONTENT browsing and OGG export were tested on Linux Mint 22.3.\r\n" +
                    "Rendering and audio compatibility limitations remain.\r\n" +
                    "Playback and hardware-specific functions are not certified.",
                    "About the Linux fork", MessageBoxButtons.OK);
            };
            helpToolStripMenuItem.DropDownItems.Add(aboutFork);
            AddLinuxLink("Linux fork on GitHub", "https://github.com/basecore/teddy/tree/linux-1.7.0");
            AddLinuxLink("Linux releases", "https://github.com/basecore/teddy/releases");

            optionsToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem listMode = new ToolStripMenuItem("List view (rendering workaround)");
            listMode.CheckOnClick = true;
            View originalView = lstTonies.View;
            ColumnHeaderStyle originalHeaders = lstTonies.HeaderStyle;
            listMode.CheckedChanged += delegate {
                lstTonies.BeginUpdate();
                try
                {
                    lstTonies.View = listMode.Checked ? View.Details : originalView;
                    lstTonies.HeaderStyle = listMode.Checked ? ColumnHeaderStyle.Clickable : originalHeaders;
                    if (listMode.Checked && lstTonies.Columns.Count > 0)
                        lstTonies.Columns[0].Width = Math.Max(200, lstTonies.ClientSize.Width - 30);
                }
                finally { lstTonies.EndUpdate(); }
                lstTonies.Invalidate(true);
            };
            optionsToolStripMenuItem.DropDownItems.Add(listMode);

            ToolStripMenuItem redraw = new ToolStripMenuItem("Redraw library");
            redraw.ShortcutKeys = Keys.F5;
            redraw.Click += delegate { lstTonies.Invalidate(true); lstTonies.Update(); };
            optionsToolStripMenuItem.DropDownItems.Add(redraw);
        }

        private void AddLinuxLink(string title, string url)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(title);
            item.Click += delegate {
                try
                {
                    Process.Start(new ProcessStartInfo("xdg-open", url) { UseShellExecute = false });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not open the browser.\r\n\r\n" + url + "\r\n\r\n" + ex.Message,
                        "Open project link", MessageBoxButtons.OK);
                }
            };
            helpToolStripMenuItem.DropDownItems.Add(item);
        }

        private static void ApplyLinuxColors(Control root)
        {
            Color surface = Color.FromArgb(245, 247, 250);
            if (root is Form || root is Panel || root is GroupBox || root is SplitContainer)
            {
                root.BackColor = surface;
                root.ForeColor = Color.FromArgb(30, 41, 59);
            }

            Button button = root as Button;
            if (button != null)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.UseVisualStyleBackColor = false;
                button.BackColor = Color.White;
                button.ForeColor = Color.FromArgb(30, 41, 59);
                button.FlatAppearance.BorderColor = Color.FromArgb(180, 190, 205);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(225, 235, 250);
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(205, 220, 245);
            }

            foreach (Control child in root.Controls)
                ApplyLinuxColors(child);
        }
    }
}
