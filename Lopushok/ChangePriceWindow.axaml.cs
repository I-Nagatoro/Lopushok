using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Globalization;

namespace Lopushok
{
    public partial class ChangePriceWindow : Window
    {
        public decimal? EnteredPrice { get; private set; }

        public ChangePriceWindow(decimal suggestedPrice)
        {
            InitializeComponent();
            PriceBox.Text = suggestedPrice.ToString("0.##", CultureInfo.InvariantCulture);
        }

        public void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            Close(null);
        }

        public void Apply_Click(object? sender, RoutedEventArgs e)
        {
            var text = PriceBox.Text?.Trim()
                .Replace(',', '.');

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var price) && price >= 0)
            {
                EnteredPrice = Math.Round(price, 2);
                Close(EnteredPrice);
            }
            else
            {
                MessageBoxAvalon.Show(this, "Введите корректное число (например: 10.50).", "Ошибка", MessageBoxAvalon.MessageBoxButtons.Ok);
            }
        }
    }
}