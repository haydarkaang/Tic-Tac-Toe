using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Globals.Frame = ContentFrame;
            Globals.Frame.Navigate(new Pages.WelcomePage());
        }
    }
}
