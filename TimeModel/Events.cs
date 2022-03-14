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

    sealed class EmptyEvent : EventBase, IEvent
    {
        public EmptyEvent(double start, double duration) : base(start, duration) { }

        public void Draw(Graphics g, Rectangle eventRect) { }

        public static EventFactory Factory { get; } = (start, duration) => new EmptyEvent(start, duration);
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

    sealed class EventWithLabel : EventBase, IEvent
    {
        private readonly Font font;
        private readonly Brush fg;
        private readonly string label;

        private EventWithLabel(double start, double duration, Font font, Brush fg, string label) : base(start, duration)
        {
            this.font = font;
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
            g.DrawString(label, font, fg, eventRect, format);
        }

        public static EventFactory Factory(Font font, Brush fg, string label) => (start, duration) => new EventWithLabel(start, duration, font, fg, label);
    }
}
