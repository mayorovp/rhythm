using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Rhythm
{
    public partial class MainForm : Form
    {
        private readonly Stopwatch stopwatch = new Stopwatch();
        private TimeSpan rewind;

        public MainForm()
        {
            InitializeComponent();

            propertyGrid.SelectedObject = playback.Config;
            State = PlaybackState.Stopped;
        }

        private void playback_TimePerPixelChanged(object sender, System.EventArgs e)
        {
            timer.Interval = playback.TimePerPixel;
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            if (State == PlaybackState.Stopped)
            {
                stopwatch.Restart();
                rewind = TimeSpan.Zero;
                playback.ElapsedTime = 0;
            }
            else
            {
                stopwatch.Start();
            }
            State = PlaybackState.Started;
        }

        private void btnStop_Click(object sender, System.EventArgs e)
        {
            stopwatch.Stop();
            State = PlaybackState.Stopped;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            stopwatch.Stop();
            State = PlaybackState.Paused;
        }

        private void btnPast1_Click(object sender, EventArgs e)
        {
            rewind += TimeSpan.FromSeconds(1);
            playback.ElapsedTime = stopwatch.ElapsedMilliseconds - rewind.TotalMilliseconds;
        }

        private void btnPast10_Click(object sender, EventArgs e)
        {
            rewind += TimeSpan.FromSeconds(10);
            playback.ElapsedTime = stopwatch.ElapsedMilliseconds - rewind.TotalMilliseconds;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            playback.ElapsedTime = stopwatch.ElapsedMilliseconds - rewind.TotalMilliseconds;

            if (playback.IsEnded)
            {
                stopwatch.Stop();
                State = PlaybackState.Stopped;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            playback.Config.Save();

            base.OnClosed(e);
        }

        enum PlaybackState
        {
            Stopped, Started, Paused
        }
        private PlaybackState _state;
        private PlaybackState State
        {
            get => _state;
            set
            {
                _state = value;

                timer.Enabled = value == PlaybackState.Started;

                btnStart.Visible = value != PlaybackState.Started;
                btnPause.Visible = value == PlaybackState.Started;
                btnStart.Visible = true;
                btnPast1.Visible = value != PlaybackState.Stopped;
                btnPast10.Visible = value != PlaybackState.Stopped;

                btnStart.Enabled = true;
                btnPause.Enabled = true;
                btnPast1.Enabled = true;
                btnPast10.Enabled = true;
                btnStop.Enabled = value != PlaybackState.Stopped;

                propertyGrid.Visible = value == PlaybackState.Stopped;
            }
        }
    }
}
