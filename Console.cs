using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GirlsStandards
{
    public class Consowole
    {
        public static Consowole Instance { get; private set; }
        internal Window consoleWindow;
        private ListBox logListBox;
        private ObservableCollection<string> logMessages = new ObservableCollection<string>();
        private bool isConsoleOpen;
        private DispatcherPriority priority = DispatcherPriority.Normal;
        private ScrollViewer scrollViewer;
        private double _fontsize = 10;
        private static string UID => Initial.instance?.UID ?? "NoUID";
        
        public bool DisplayTime { get; set; } = false;
        
        public double FontSize { get => _fontsize; set => _fontsize = Math.Min(Math.Max(10, value), 30); }

        public void TogglePriority()
            => priority = priority == DispatcherPriority.Normal ? DispatcherPriority.Send : DispatcherPriority.Normal;

        public async Task OpenConsole()
        {
            if (Instance is not null && Instance.isConsoleOpen)
                await Instance.CloseConsole();
            Instance = this;
            App.AvaInstance?.OpenWindow(CreateMainWindow());
        }

        public Consowole() { }

        internal Window CreateMainWindow()
        {
            consoleWindow = new Window
            {
                Title = UID,
                Width = 800,
                Height = 600,
                Content = CreateConsoleContent(),
                Background = Brushes.Black
            };
            isConsoleOpen = true;

            consoleWindow.Closed += (sender, e) =>
            {
                isConsoleOpen = false;
            };

            return consoleWindow;
        }

        private ScrollViewer CreateConsoleContent()
        {
            logListBox = new ListBox
            {
                ItemsSource = logMessages,
                ItemTemplate = new FuncDataTemplate<string>((item, _) =>
                {
                    var tb = new TextBlock
                    {
                        Text = item,
                        FontSize = _fontsize,
                        FontFamily = new FontFamily("Consolas"),
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(0, 0),
                        Padding = new Thickness(0),
                        Background = Brushes.Transparent
                    };

                    tb.PointerPressed += async (sender, e) =>
                    {
                        if (e.ClickCount == 2)
                        {
                            var listBoxItem = sender as TextBlock;
                            await DialogWindow.ShowAsync(consoleWindow, listBoxItem?.Text ?? "No message to display.");
                        }
                    };

                    return tb;
                })
            };

            scrollViewer = new ScrollViewer
            {
                Content = logListBox,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };
            return scrollViewer;
        }

        private static class DialogWindow
        {
            public static async Task ShowAsync(Window parent, string message)
            {
                var tb = new TextBox
                {
                    Text = message,
                    IsReadOnly = true,
                    FontFamily = new FontFamily("Consolas"),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    SelectedText = message,
                };

                var dialog = new Window
                {
                    Width = 300,
                    Height = 150,
                    Content = tb,
                    CanResize = false,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                await dialog.ShowDialog(parent);
            }
        }


        public void PrintLog(string message)
        {
            if (isConsoleOpen)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    logMessages.Add($"{(DisplayTime ? $"{DateTime.Now:HH:mm:ss:fff} -- " : "")}{message}");
                    scrollViewer.ScrollToEnd();
                    logListBox.InvalidateVisual();
                }, priority);
            }
        }

        public async Task CloseConsole()
        {
            if (isConsoleOpen)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    consoleWindow.Close();
                    consoleWindow = null;
                    logMessages.Clear();
                    isConsoleOpen = false;
                }, priority);
            }
        }
    }
}
