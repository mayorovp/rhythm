using System.Drawing;

namespace Rhythm.TimeModel
{
    interface IEvent
    {
        double Start { get; }
        double Duration { get; }

        void Draw(Graphics g, Rectangle eventRect);
    }

    delegate IEvent EventFactory(double start, double duration);

    abstract class EventBase
    {
        public double Start { get; }
        public double Duration { get; }

        public EventBase(double start, double duration)
        {
            Start = start;
            Duration = duration;
        }
    }

    sealed class EventWithoutBorder : EventBase, IEvent
    {
        private readonly Brush bg;

        private EventWithoutBorder(double start, double duration, Brush bg) : base(start, duration)
        {
            this.bg = bg;
        }

        public void Draw(Graphics g, Rectangle eventRect)
        {
            g.FillRectangle(bg, eventRect);
        }

        public static EventFactory Factory(Brush bg) => (start, duration) => new EventWithoutBorder(start, duration, bg);
    }

    sealed class EventWithBorder : EventBase, IEvent
    {
        private readonly Pen pen;
        private readonly Brush bg;

        private EventWithBorder(double start, double duration, Pen pen, Brush bg) : base(start, duration)
        {
            this.pen = pen;
            this.bg = bg;
        }

        public void Draw(Graphics g, Rectangle eventRect)
        {
            RectangleF rect2 = eventRect;
            rect2.Inflate(0, -pen.Width);
            g.FillRectangle(bg, rect2);

            rect2.Inflate(0, pen.Width / 2);
            g.DrawLine(pen, rect2.Left, rect2.Top, rect2.Right, rect2.Top);
            g.DrawLine(pen, rect2.Left, rect2.Bottom, rect2.Right, rect2.Bottom);
        }

        public static EventFactory Factory(Pen pen, Brush bg) => (start, duration) => new EventWithBorder(start, duration, pen, bg);
    }

    sealed class EventWithLabel : EventBase, IEvent
    {
        private readonly Font font;
        private readonly Brush bg, fg;
        private readonly string label;

        private EventWithLabel(double start, double duration, Font font, Brush bg, Brush fg, string label) : base(start, duration)
        {
            this.font = font;
            this.bg = bg;
            this.fg = fg;
            this.label = label;
        }

        private static readonly StringFormat format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            Trimming = StringTrimming.None,
        };

        public void Draw(Graphics g, Rectangle eventRect)
        {
            g.FillRectangle(bg, eventRect);
            g.DrawString(label, font, fg, eventRect, format);
        }

        public static EventFactory Factory(Font font, Brush bg, Brush fg, string label) => (start, duration) => new EventWithLabel(start, duration, font, bg, fg, label);
    }
}
