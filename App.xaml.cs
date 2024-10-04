using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using static GirlsStandards.Consowole;

namespace GirlsStandards
{
    public class App : Application
    {
        public static App AvaInstance { get; internal set; } = null;
        public static InvisibleWindow? InvisMain = null;
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            AvaInstance = this;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var inv = new InvisibleWindow();
            OpenWindow(inv);
            InvisMain = inv;
            base.OnFrameworkInitializationCompleted();
        }

        public void OpenWindow(Window window)
            => window.Show();

        public class InvisibleWindow : Window
        {
            internal InvisibleWindow()
            {
                this.ShowInTaskbar = false; 
                this.Opacity = 0;   
                this.WindowState = WindowState.Normal; 
                this.Width = 1; 
                this.Height = 1;
            }
        }
    }


}
