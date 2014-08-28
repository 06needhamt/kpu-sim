using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KyleHughes.CIS2118.KPUSim.Assembly;

namespace KyleHughes.CIS2118.KPUSim.Views
{
    // ReSharper disable once InconsistentNaming
    public partial class KPUSyntaxBox : INotifyPropertyChanged
    {
        private DrawingControl _renderCanvas;

        public KPUSyntaxBox()
        {
            this.InitializeComponent();
            TextBlock.SetLineHeight(this, FontSize*1.3);

            Loaded += (s, e) =>
            {
                _renderCanvas = (DrawingControl) Template.FindName("PART_RenderCanvas", this);
                ScrollViewer scrollBox = (ScrollViewer) Template.FindName("PART_ContentHost", this);

                scrollBox.ScrollChanged += (ss, ee) => InvalidateVisual();
                FormatText();
                InvalidateVisual();
            };

            SizeChanged += (s, e) =>
            {
                if (e.HeightChanged == false)
                    return;
                InvalidateVisual();
            };

            TextChanged += (s, e) =>
            {
                int index = SelectionStart;
                FormatText();
                InvalidateVisual();
                SelectionStart = index;
            };
            TextChanged += NotifyRowAndCol;
            KeyUp += NotifyRowAndCol;
            MouseMove += NotifyRowAndCol;
        }


        public int CurrentLine
        {
            get
            {
                int line = 0;
                char[] chars = Text.ToCharArray();
                for (int i = 0; i < CaretIndex; i++)
                {
                    if (chars[i].Equals('\n'))
                        line++;
                }
                return line + 1;
            }
        }

        public int CurrentColumn
        {
            get
            {
                int linePos = 0;
                char[] chars = Text.ToCharArray();
                for (int i = 0; i < CaretIndex; i++)
                {
                    linePos++;
                    if (chars[i].Equals('\n'))
                    {
                        linePos = 0;
                    }
                }
                return linePos + 1;
            }
        }

        public double LineHeight
        {
            get
            {
                TextBlock.SetLineHeight(this, FontSize*1.3);
                return FontSize*1.3d;
            }
        }

        public FormattedText FormattedText { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyRowAndCol(object e, object e1)
        {
            Notify("CurrentLine");
            Notify("CurrentColumn");
        }

        private void FormatText()
        {
            var ft = new FormattedText(
                Text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                Brushes.Black) {Trimming = TextTrimming.None, LineHeight = LineHeight};

            FormattedText = ft;

            ThreadPool.QueueUserWorkItem(param =>
            {
                FormattedText.SetFontWeight(FontWeights.Bold);
                FormattedText.SetFontStyle(FontStyles.Normal);
                FormattedText.SetForegroundBrush(Brushes.Red);
                foreach (WordKind v in WordKinds.AllItems)
                {
                    v.Format(FormattedText);
                }
            });
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Render();
            base.OnRender(drawingContext);
        }

        private void Render()
        {
            try
            {
                if (!IsLoaded || _renderCanvas == null)
                    return;
                using (DrawingContext dc = _renderCanvas.GetContext())
                {
                    dc.DrawRectangle(Background, null,
                        new Rect(HorizontalOffset, VerticalOffset, ActualWidth, ActualHeight));
                    dc.DrawText(FormattedText, new Point(2 - HorizontalOffset, -VerticalOffset));
                }
            }
            catch (ArgumentException)
            {
                // Sometimes the rectangle object fails to generate. I can't reproduce the issue
                // and I don't know any side-effects of it
            }
        }

        protected void Notify([CallerMemberName] string prop = null)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}