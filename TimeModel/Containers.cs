using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhythm.TimeModel
{
    interface IEventContainer
    {
        double Duration { get; }

        IEnumerable<IEvent> GetEvents(double start, double shift = default);
    }

    class OneTick : IEventContainer
    {
        private readonly double halfTick;
        private readonly EventFactory onEventFactory, offEventFactory;

        public OneTick(DrawContext ctx)
        {
            halfTick = 30000.0 / ctx.Model.Frequency;
            onEventFactory = EventWithoutBorder.Factory(ctx.ForegroundBrush);
            offEventFactory = EventWithoutBorder.Factory(ctx.ComplementaryBrush);
        }

        public double Duration => halfTick + halfTick;

        public IEnumerable<IEvent> GetEvents(double start, double shift = default)
        {
            if (shift < halfTick)
                yield return onEventFactory(start, halfTick);

            yield return offEventFactory(start + halfTick, halfTick);
        }
    }

    class LabelContainer : IEventContainer
    {
        private readonly double duration;
        private readonly EventFactory eventFactory;

        public LabelContainer(DrawContext ctx, double duration, string label)
        {
            this.duration = duration;
            eventFactory = EventWithLabel.Factory(ctx.TextFont, ctx.TextBrush, label);
        }

        public double Duration => duration;

        public IEnumerable<IEvent> GetEvents(double start, double shift = 0)
        {
            yield return eventFactory(start, duration);
        }
    }

    class EmptyContainer : IEventContainer
    {
        private readonly double duration;
        private readonly EventFactory eventFactory;

        public EmptyContainer(DrawContext ctx, double duration)
        {
            this.duration = duration;
            eventFactory = EmptyEvent.Factory;
        }

        public double Duration => duration;

        public IEnumerable<IEvent> GetEvents(double start, double shift = 0)
        {
            yield return eventFactory(start, duration);
        }
    }

    class SeriesWithPrefix : IEventContainer
    {
        private readonly IEventContainer prefix, repeated;
        private readonly int repeatCount;

        public SeriesWithPrefix(IEventContainer prefix, IEventContainer repeated, int repeatCount)
        {
            this.prefix = prefix;
            this.repeated = repeated;
            this.repeatCount = repeatCount;

            Duration = prefix.Duration + repeated.Duration * repeatCount;
        }

        public double Duration { get; }

        public IEnumerable<IEvent> GetEvents(double start, double shift = default)
        {
            if (shift < prefix.Duration)
            {
                foreach (var e in prefix.GetEvents(start, shift))
                    yield return e;
            }
            start += prefix.Duration;
            shift -= prefix.Duration;

            for (int i = 0; i < repeatCount; i++)
            {
                if (shift < repeated.Duration * (i + 1))
                {
                    foreach (var e in repeated.GetEvents(start + repeated.Duration * i, shift - repeated.Duration * i))
                        yield return e;
                }
            }
        }
    }

    class Sequence : IEventContainer
    {
        private readonly IEventContainer[] items;

        public Sequence(IEventContainer[] items)
        {
            this.items = items;

            Duration = items.Sum(x => x.Duration);
        }

        public double Duration { get; }

        public IEnumerable<IEvent> GetEvents(double start, double shift = 0)
        {
            foreach (var item in items)
            {
                if (shift < item.Duration)
                {
                    foreach (var e in item.GetEvents(start, shift))
                        yield return e;
                }
                start += item.Duration;
                shift -= item.Duration;
            }
        }
    }
}
