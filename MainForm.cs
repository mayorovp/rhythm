using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rhythm
{
    public partial class MainForm : Form
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();

            propertyGrid.SelectedObject = playback.Config;
        }

        private void playback_TimePerPixelChanged(object sender, System.EventArgs e)
        {
            timer.Interval = playback.TimePerPixel;
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            stopwatch.Restart();
            timer.Enabled = true;
            playback.ElapsedTime = 0;

            propertyGrid.Visible = false;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            stopwatch.Stop();
            timer.Enabled = false;

            propertyGrid.Visible = true;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            playback.ElapsedTime = stopwatch.ElapsedMilliseconds;

            if (playback.IsEnded)
            {
                stopwatch.Stop();
                timer.Enabled = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            playback.Config.Save();

            base.OnClosed(e);
        }
    }
}
