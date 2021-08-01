using System;

namespace Rhythm.TimeModel
{
    static class TimeModelFactory
    {
        public static IEventContainer Create(DrawContext ctx)
        {
            if (ctx.Model.ExerciseFirst > ctx.Model.ExerciseCount)
            {
                return new EmptyContainer(ctx, ctx.Model.VisibleRangeAfter.TotalMilliseconds);
            }

            try
            {
                var exercises = new IEventContainer[ctx.Model.ExerciseCount - ctx.Model.ExerciseFirst + 1];
                var oneTick = new OneTick(ctx);

                var seriesInterval = new EmptyContainer(ctx, ctx.Model.SeriesInterval * 1000.0);
                var nextSeries = new SeriesWithPrefix(seriesInterval, oneTick, ctx.Model.Length);

                for (int i = 0; i < exercises.Length; i++)
                {
                    var prefix = new LabelContainer(ctx, (i == 0 ? ctx.Model.StartDelay : ctx.Model.ExerciseInterval) * 1000.0, $"Упражнение {ctx.Model.ExerciseFirst + i}");
                    var firstSeries = new SeriesWithPrefix(prefix, oneTick, ctx.Model.Length);
                    exercises[i] = new SeriesWithPrefix(firstSeries, nextSeries, ctx.Model.SeriesCount - 1);
                }

                return new Sequence(exercises);
            }
            catch (Exception ex)
            {
                return new LabelContainer(ctx, ctx.Model.VisibleRangeAfter.TotalMilliseconds, ex.Message);
            }
        }
    }
}
