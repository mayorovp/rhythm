using System;
using System.Drawing;

namespace Rhythm.TimeModel
{
    sealed class DrawContext : IDisposable
    {
        public Brush BackgroundBrush { get; }
        public Brush ForegroundBrush { get; }
        public Brush ComplementaryBrush { get; }
        public Brush TextBrush { get; }
        public Brush PointerBrush { get; }
        public Font TextFont { get; }
        public Pen BorderPen { get; }
        public ViewModel Model { get; }

        public DrawContext(ViewModel model)
        {
            BackgroundBrush = new SolidBrush(model.BackgroundColor);
            ForegroundBrush = new SolidBrush(model.ForegroundColor);
            ComplementaryBrush = new SolidBrush(model.ComplementaryColor);
            TextBrush = new SolidBrush(model.TextColor);
            PointerBrush = new SolidBrush(model.PointerColor);
            TextFont = model.TextFont;
            BorderPen = new Pen(ForegroundBrush, model.BorderWidth);
            Model = model;
        }

        public void Dispose()
        {
            BorderPen.Dispose();
            TextBrush.Dispose();
            ForegroundBrush.Dispose();
            BackgroundBrush.Dispose();
            ComplementaryBrush.Dispose();
            PointerBrush.Dispose();
        }
    }
}
