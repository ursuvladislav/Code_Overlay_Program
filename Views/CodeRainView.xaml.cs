using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace CodeOverlayRunner.Views
{
    public partial class CodeRainView : UserControl
    {
        private readonly DispatcherTimer _timer = new();
        private readonly Random _random = new();
        private readonly List<TextBlock> _symbols = new();

        private readonly string[] _chars =
        {
            "{", "}", "<", ">", "/", "*", "+", "-", "0", "1",
            "C++", "AI", "RUN", "STD", "INT", "MAIN", "🥪",
            "</>", "С", "[]", "CTRL", "AUTO", "==", "&&", "IF",
            "#", "=>", "WPF", "LLM", "FOR", "0", "0", "0", "0", 
            "0", "0", "0", "0", "0", "1", "1", "1", "1", "1", "1",
             "1", "1", "1"
        };

        public CodeRainView()
        {
            InitializeComponent();

            Loaded += (_, _) => InitSymbols();

            _timer.Interval = TimeSpan.FromMilliseconds(40);                       // плавность/скорость
            _timer.Tick += (_, _) => Animate();
            _timer.Start();
        }

        private void InitSymbols()
        {
            RainCanvas.Children.Clear();
            _symbols.Clear();

            for (int i = 0; i < 60; i++)                                            // количество символов
            {
                var text = new TextBlock
                {
                    Text = _chars[_random.Next(_chars.Length)],
                    FontSize = _random.Next(10, 22),                                // случайный размер шрифта
                    Foreground = new SolidColorBrush(Color.FromRgb(                 // здесь задается цвет
                        (byte)_random.Next(80, 120),
                        (byte)_random.Next(180, 255),
                        (byte)_random.Next(180, 255)))
                };

                Canvas.SetLeft(text, _random.Next(0, Math.Max(1, (int)ActualWidth)));
                Canvas.SetTop(text, _random.Next(0, Math.Max(1, (int)ActualHeight)));

                RainCanvas.Children.Add(text);
                _symbols.Add(text);
            }
        }

        private void Animate()
        {
            foreach (var symbol in _symbols)
            {
                double top = Canvas.GetTop(symbol);
                top += _random.Next(1, 4);

                if (top > ActualHeight)
                {
                    top = -20;
                    Canvas.SetLeft(symbol, _random.Next(0, Math.Max(1, (int)ActualWidth)));
                    symbol.Text = _chars[_random.Next(_chars.Length)];
                }

                Canvas.SetTop(symbol, top);
            }
        }
    }
}