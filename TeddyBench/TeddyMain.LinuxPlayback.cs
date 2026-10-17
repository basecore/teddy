using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TonieFile;

namespace TeddyBench
{
    public partial class TeddyMain
    {
        private CancellationTokenSource linuxPlaybackCancellation;
        private bool linuxPlaybackClosing;
        private System.Windows.Forms.Timer linuxPlaybackLabels;

        internal void ConfigureLinuxPlayback()
        {
            btnPlay.Click -= btnPlay_Click;
            btnPlay.Click += LinuxPlayClick;
            lstTonies.DoubleClick -= lstTonies_DoubleClick;
            lstTonies.DoubleClick += LinuxPlayClick;
            btnPlay.UseCompatibleTextRendering = false;
            btnPlay.AccessibleName = "Play or stop selected audio";
            trackPlayPosition.Enabled = false;
            btnPlay.Text = "Play";
            linuxPlaybackLabels = new System.Windows.Forms.Timer { Interval = 250 };
            linuxPlaybackLabels.Tick += delegate {
                btnPlay.Text = linuxPlaybackCancellation == null ? "Play" : "Stop";
                trackPlayPosition.Enabled = false;
                lblPlayTime.Text = linuxPlaybackCancellation == null ? "--:-- / --:--" : "Playing";
            };
            linuxPlaybackLabels.Start();
            FormClosing += delegate {
                linuxPlaybackClosing = true;
                linuxPlaybackLabels.Stop();
                linuxPlaybackLabels.Dispose();
                linuxPlaybackCancellation?.Cancel();
            };
        }

        private async void LinuxPlayClick(object sender, EventArgs e)
        {
            if (linuxPlaybackCancellation != null)
            {
                linuxPlaybackCancellation.Cancel();
                return;
            }
            if (lstTonies.SelectedItems.Count != 1) return;
            var tag = lstTonies.SelectedItems[0].Tag as ListViewTag;
            if (tag == null) return;
            string filename = tag.FileName;
            var cancellation = new CancellationTokenSource();
            linuxPlaybackCancellation = cancellation;
            btnPlay.Text = "Stop";
            try
            {
                await Task.Run(() => PlayLinuxPcm(filename, cancellation.Token));
            }
            catch (Exception ex)
            {
                if (!cancellation.IsCancellationRequested && !linuxPlaybackClosing)
                    MessageBox.Show("Linux playback failed: " + ex.Message +
                        "\n\nPlayback requires ffplay (usually supplied by the ffmpeg package).\n" +
                        "On Linux Mint: sudo apt install ffmpeg\n" +
                        "Seeking is not supported by this playback backend.", "Linux playback");
            }
            finally
            {
                linuxPlaybackCancellation = null;
                cancellation.Dispose();
                if (!linuxPlaybackClosing && !IsDisposed)
                {
                    btnPlay.Text = "Play";
                    lblPlayTime.Text = "--:-- / --:--";
                }
            }
        }

        private static void PlayLinuxPcm(string filename, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var audio = TonieAudio.FromFile(filename);
            cancellation.ThrowIfCancellationRequested();
            using (var input = new MemoryStream(audio.Audio, false))
            using (var decoder = new OpusWaveStream(input, 48000, 2))
            using (var player = new Process())
            {
                player.StartInfo = new ProcessStartInfo
                {
                    FileName = "ffplay",
                    Arguments = "-nodisp -autoexit -loglevel error -f s16le -ar 48000 -ac 2 -i pipe:0",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                player.Start();
                var errors = player.StandardError.ReadToEndAsync();
                using (cancellation.Register(() => {
                    try { if (!player.HasExited) player.Kill(); } catch (InvalidOperationException) { }
                }))
                {
                    try
                    {
                        byte[] buffer = new byte[19200];
                        while (true)
                        {
                            cancellation.ThrowIfCancellationRequested();
                            int count = decoder.Read(buffer, 0, buffer.Length);
                            if (count <= 0) break;
                            player.StandardInput.BaseStream.Write(buffer, 0, count);
                        }
                        player.StandardInput.Close();
                        player.WaitForExit();
                        cancellation.ThrowIfCancellationRequested();
                        string error = errors.GetAwaiter().GetResult();
                        if (player.ExitCode != 0)
                            throw new IOException("ffplay exited with code " + player.ExitCode + ": " + error);
                    }
                    finally
                    {
                        try
                        {
                            if (!player.HasExited) player.Kill();
                            player.WaitForExit();
                        }
                        catch (InvalidOperationException) { }
                    }
                }
            }
        }
    }
}
