using System;
using System.ComponentModel;
using System.Drawing;

namespace Rhythm
{
    public sealed class ViewModel : INotifyPropertyChanged
    {
        private const string CAT_TIMINGS = "1. Упражнение";
        private const string CAT_APPEARANCE = "2. Отображение";

        [DisplayName("Частота, 1/мин"), Category(CAT_TIMINGS)]
        public int Frequency { get; set; } = Properties.Settings.Default.Frequency;

        [DisplayName("Длина серии"), Category(CAT_TIMINGS)]
        public int Length { get; set; } = Properties.Settings.Default.Length;

        [DisplayName("Интервал после серии, сек"), Category(CAT_TIMINGS)]
        public int SeriesInterval { get; set; } = Properties.Settings.Default.SeriesInterval;

        [DisplayName("Количество серий"), Category(CAT_TIMINGS)]
        public int SeriesCount { get; set; } = Properties.Settings.Default.SeriesCount;

        [DisplayName("Интервал между упражнениями, сек"), Category(CAT_TIMINGS)]
        public int ExerciseInterval { get; set; } = Properties.Settings.Default.ExerciseInterval;

        [DisplayName("Количество упражнений"), Category(CAT_TIMINGS)]
        public int ExerciseCount { get; set; } = Properties.Settings.Default.ExerciseCount;

        [DisplayName("Номер первого упражнения"), Category(CAT_TIMINGS)]
        public int ExerciseFirst { get; set; } = Properties.Settings.Default.ExerciseFirst;

        [DisplayName("Начальная задержка, сек"), Category(CAT_TIMINGS)]
        public int StartDelay { get; set; } = Properties.Settings.Default.StartDelay;


        [DisplayName("Видимое будущее"), Category(CAT_APPEARANCE)]
        public TimeSpan VisibleRangeAfter { get; set; } = Properties.Settings.Default.VisibleRangeAfter;

        [DisplayName("Видимое прошлое"), Category(CAT_APPEARANCE)]
        public TimeSpan VisibleRangeBefore { get; set; } = Properties.Settings.Default.VisibleRangeBefore;

        [DisplayName("Цвет фона"), Category(CAT_APPEARANCE)]
        public Color BackgroundColor { get; set; } = Properties.Settings.Default.BackgroundColor;

        [DisplayName("Цвет прямоугольников"), Category(CAT_APPEARANCE)]
        public Color ForegroundColor { get; set; } = Properties.Settings.Default.ForegroundColor;

        [DisplayName("Дополнительный цвет"), Category(CAT_APPEARANCE)]
        public Color ComplementaryColor { get; set; } = Properties.Settings.Default.ComplementaryColor;

        [DisplayName("Цвет текста"), Category(CAT_APPEARANCE)]
        public Color TextColor { get; set; } = Properties.Settings.Default.TextColor;

        [DisplayName("Шрифт текста"), Category(CAT_APPEARANCE)]
        public Font TextFont { get; set; } = Properties.Settings.Default.TextFont;

        [DisplayName("Цвет указателя"), Category(CAT_APPEARANCE)]
        public Color PointerColor { get; set; } = Properties.Settings.Default.PointerColor;

        [DisplayName("Размер указателя, px"), Category(CAT_APPEARANCE)]
        public int PointerSize { get; set; } = Properties.Settings.Default.PointerSize;

        [DisplayName("Размер рамки, px"), Category(CAT_APPEARANCE)]
        public int BorderWidth { get; set; } = Properties.Settings.Default.BorderWidth;

        public void Save()
        {
            var settings = Properties.Settings.Default;
            settings.Frequency = Frequency;
            settings.Length = Length;
            settings.SeriesCount = SeriesCount;
            settings.SeriesInterval = SeriesInterval;
            settings.StartDelay = StartDelay;
            settings.VisibleRangeAfter = VisibleRangeAfter;
            settings.VisibleRangeBefore = VisibleRangeBefore;
            settings.BackgroundColor = BackgroundColor;
            settings.ForegroundColor = ForegroundColor;
            settings.ComplementaryColor = ComplementaryColor;
            settings.PointerColor = PointerColor;
            settings.PointerSize = PointerSize;
            settings.TextColor = TextColor;
            settings.TextFont = TextFont;
            settings.ExerciseCount = ExerciseCount;
            settings.ExerciseInterval = ExerciseInterval;
            settings.BorderWidth = BorderWidth;
            settings.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
