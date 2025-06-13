using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System.Threading.Tasks;

public static class MessageBoxAvalon
{
    public enum MessageBoxButtons { Ok }

    public static async Task Show(Window owner, string message, string title, MessageBoxButtons buttons)
    {
        var dlg = new Window
        {
            Title = title,
            Width = 300,
            Height = 150,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var okButton = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };
        okButton.Click += (_, _) => dlg.Close();

        dlg.Content = new StackPanel
        {
            Margin = new Thickness(10),
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                okButton
            }
        };

        await dlg.ShowDialog(owner);
    }
}