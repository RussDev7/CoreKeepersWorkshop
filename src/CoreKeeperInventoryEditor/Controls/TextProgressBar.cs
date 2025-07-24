﻿using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System;

namespace CoreKeepersWorkshop
{
    // Taken from github: https://github.com/ukushu/TextProgressBar
    public enum ProgressBarDisplayMode
    {
        NoText,
        Percentage,
        CurrProgress,
        CustomText,
        TextAndPercentage,
        TextAndCurrProgress
    }

    public class TextProgressBar : ProgressBar
    {
        [Description("Font of the text on ProgressBar"), Category("Additional Options")]
        public Font TextFont { get; set; }
        
        private SolidBrush _textcolorBrush = (SolidBrush)Brushes.Black;
        [Category("Additional Options")]
        public Color TextColor
        {
            get
            {
                return _textcolorBrush.Color;
            }
            set
            {
                _textcolorBrush.Dispose();
                _textcolorBrush = new SolidBrush(value);
            }
        }

        private SolidBrush _progresscolorBrush = (SolidBrush)Brushes.LightGreen;
        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color ProgressColor
        {
            get
            {
                return _progresscolorBrush.Color;
            }
            set
            {
                _progresscolorBrush.Dispose();
                _progresscolorBrush = new SolidBrush(value);
            }
        }

        private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;
        [Category("Additional Options"), Browsable(true)]
        public ProgressBarDisplayMode VisualMode
        {
            get
            {
                return _visualMode;
            }
            set
            {
                _visualMode = value;
                Invalidate(); //redraw component after change value from VS Properties section.
            }
        }

        private string _text = string.Empty;

        [Description("If it's empty, % will be shown"), Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string CustomText
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Invalidate(); //redraw component after change value from VS Properties section.
            }
        }

        private string TextToDraw
        {
            get
            {
                string text = CustomText;

                switch (VisualMode)
                {
                    case (ProgressBarDisplayMode.Percentage):
                        text = PercentageStr;
                        break;
                    case (ProgressBarDisplayMode.CurrProgress):
                        text = CurrProgressStr;
                        break;
                    case (ProgressBarDisplayMode.TextAndCurrProgress):
                        text = CustomText + ": " + CurrProgressStr;
                        break;
                    case (ProgressBarDisplayMode.TextAndPercentage):
                        text = CustomText + ": " + PercentageStr;
                        break;
                }

                return text;
            }
            set { }
        }

        private string PercentageStr { get { return ((int)((float)Value - Minimum) / ((float)Maximum - Minimum) * 100) + "%"; } }

        private string CurrProgressStr
        {
            get
            {
                return Value + "/" + Maximum;
            }
        }

        public TextProgressBar()
        {
            TextFont = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold | FontStyle.Italic);
            Value = Minimum;
            FixComponentBlinking();
        }

        private void FixComponentBlinking()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            DrawProgressBar(g);

            DrawStringIfNeeded(g);
        }

        private void DrawProgressBar(Graphics g)
        {
            Rectangle rect = ClientRectangle;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);

            rect.Inflate(-3, -3);

            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);

                g.FillRectangle(_progresscolorBrush, clip);
            }
        }

        private void DrawStringIfNeeded(Graphics g)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {

                string text = TextToDraw;

                SizeF len = g.MeasureString(text, TextFont);

                Point location = new Point(((Width / 2) - (int)len.Width / 2), ((Height / 2) - (int)len.Height / 2));

                g.DrawString(text, TextFont, (Brush)_textcolorBrush, location);
            }
        }

        public new void Dispose()
        {
            _textcolorBrush.Dispose();
            _progresscolorBrush.Dispose();
            base.Dispose();
        }
    }
}
