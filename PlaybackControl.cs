using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rhythm
{
    public sealed class PlaybackControl : Control
    {
        private int pulseCount, seriesCount;
        private double visibleRange, timeShift, startDelay, seriesLength, seriesInterval, pulseLength, timePerPixel, elapsedTime;
        private int ybase, yheight;
        private int xbase;
        private ViewModel _config;

        public ViewModel Config
        {
            get => _config;
            set
            {
                if (_config != null)
                {
                    _config.PropertyChanged -= Config_PropertyChanged;
                }
                _config = value;
                if (_config != null)
                {
                    _config.PropertyChanged += Config_PropertyChanged;
                    Reconfigure();
                }
            }
        }
        public int TimePerPixel => (int)timePerPixel;
        public event EventHandler TimePerPixelChanged;

        public double ElapsedTime
        {
            get => elapsedTime;
            set
            {
                elapsedTime = value;
                Invalidate2();
            }
        }

        public PlaybackControl()
        {
            Config = new ViewModel();
        }

        private void Reconfigure()
        {
            this.pulseCount = Config.Length;
            this.pulseLength = TimeSpan.FromMinutes(1.0 / Config.Frequency).TotalMilliseconds;

            this.seriesCount = Config.SeriesCount;
            this.seriesInterval = TimeSpan.FromSeconds(Config.SeriesInterval).TotalMilliseconds;
            this.seriesLength = pulseLength * pulseCount + seriesInterval;

            this.startDelay = TimeSpan.FromSeconds(Config.StartDelay).TotalMilliseconds;
            this.visibleRange = (Config.VisibleRangeAfter + Config.VisibleRangeBefore).TotalMilliseconds;
            this.timeShift = Config.VisibleRangeBefore.TotalMilliseconds;

            RecalculateWidth();
            Invalidate();
        }

        private void RecalculateWidth()
        {
            if (Width > 0)
            {
                timePerPixel = visibleRange / Width;
                TimePerPixelChanged?.Invoke(this, EventArgs.Empty);

                xbase = (int)(timeShift / timePerPixel) - Config.PointerSize;
            }
            else
                timePerPixel = double.PositiveInfinity;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            RecalculateWidth();

            ybase = 2 * Height / 5;
            yheight = Height / 5;

            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using var brush = new SolidBrush(Config.BackgroundColor);

            e.Graphics.FillRectangle(brush, 0, 0, Width, ybase);
            e.Graphics.FillRectangle(brush, 0, ybase + yheight, Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (e.ClipRectangle.Left <= xbase + Config.PointerSize && xbase <= e.ClipRectangle.Right)
            {
                using var brush = new SolidBrush(Config.PointerColor);
                e.Graphics.FillRectangle(brush, xbase, 0, Config.PointerSize, Height);
            }

            if (e.ClipRectangle.Top <= ybase + yheight && ybase <= e.ClipRectangle.Bottom)
            {
                using var foreBrush = new SolidBrush(Config.ForegroundColor);
                using var backBrush = new SolidBrush(Config.BackgroundColor);

                for (int x = e.ClipRectangle.Left; x < e.ClipRectangle.Right;)
                {
                    var (active, nextEvent) = GetEventData(x);
                    if (x < xbase)
                    {
                        if (nextEvent > xbase - x)
                            nextEvent = xbase - x;
                    }
                    else
                    {
                        if (nextEvent > e.ClipRectangle.Right - x)
                            nextEvent = e.ClipRectangle.Right;
                    }

                    e.Graphics.FillRectangle(active ? foreBrush : backBrush, x, ybase, nextEvent, yheight);

                    x += nextEvent + 1;

                    if (x >= xbase && x <= xbase + Config.PointerSize)
                        x = xbase + Config.PointerSize + 1;
                }
            }
        }

        private void Invalidate2() => Invalidate(new Rectangle(0, ybase, Width, yheight));

        private (bool active, int nextEvent) GetEventData(int x)
        {
            var time = elapsedTime - timeShift + x * timePerPixel;

            time -= startDelay;
            if (time < 0)
                return (false, -(int)(time / timePerPixel));

            int seriesNo = (int)Math.Floor(time / seriesLength);
            time %= seriesLength;
            if (seriesNo >= seriesCount)
                return (false, int.MaxValue / 2);

            int pulseNo = (int)Math.Floor(time / pulseLength);
            if (pulseNo >= pulseCount)
                return (false, (int)((seriesLength - time) / timePerPixel));

            time %= pulseLength;
            if (time < pulseLength * 0.5)
                return (true, (int)((pulseLength * 0.5 - time) / timePerPixel));
            else
                return (false, (int)((pulseLength - time) / timePerPixel));
        }

        public bool IsEnded => elapsedTime > startDelay + seriesLength * seriesCount - seriesInterval - pulseLength * 0.5;

        private void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Reconfigure();
    }
}
