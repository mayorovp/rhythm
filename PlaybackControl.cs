using Rhythm.TimeModel;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Rhythm
{
    public sealed class PlaybackControl : Control
    {
        private double visibleRange, timeShift, timePerPixel, elapsedTime;
        private DrawContext drawContext;
        private IEventContainer timeModel;
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
            DoubleBuffered = true;
        }

        private void Reconfigure()
        {
            this.visibleRange = (Config.VisibleRangeAfter + Config.VisibleRangeBefore).TotalMilliseconds;
            this.timeShift = Config.VisibleRangeBefore.TotalMilliseconds;

            drawContext?.Dispose();
            drawContext = new DrawContext(Config);
            timeModel = TimeModelFactory.Create(drawContext);

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
            e.Graphics.FillRectangle(drawContext.BackgroundBrush, 0, 0, Width, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var clipRect = e.ClipRectangle;

            if (clipRect.Top <= ybase + yheight && ybase <= clipRect.Bottom)
            {
                var elapsedPx = (int)(elapsedTime / timePerPixel);
                var time = (elapsedPx - xbase + clipRect.Left) * timePerPixel;
                var lastEvent = int.MinValue;

                foreach (var ev in timeModel.GetEvents(0, time))
                {
                    var left = xbase + (int)((ev.Start) / timePerPixel) - elapsedPx;
                    var right = xbase + (int)((ev.Start + ev.Duration) / timePerPixel) - elapsedPx;
                    if (left < lastEvent) left = lastEvent;
                    lastEvent = right + 1;

                    if (left > clipRect.Right)
                        break;

                    var eventRect = Rectangle.FromLTRB(left: left, right: right, top: ybase, bottom: ybase + yheight);
                    var eventClipRect = Rectangle.Intersect(eventRect, clipRect);

                    e.Graphics.SetClip(eventClipRect);
                    ev.Draw(e.Graphics, eventRect);
                }

                if (lastEvent == int.MinValue)
                    lastEvent = clipRect.Left;
            }

            e.Graphics.SetClip(clipRect);
            e.Graphics.FillRectangle(drawContext.PointerBrush, xbase, 0, Config.PointerSize, Height);
        }

        private void Invalidate2() => Invalidate(new Rectangle(0, ybase, Width, yheight));

        public bool IsEnded => elapsedTime > timeModel.Duration;

        private void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => Reconfigure();
    }
}
