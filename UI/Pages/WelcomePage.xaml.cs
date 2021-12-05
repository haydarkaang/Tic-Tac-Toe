using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var text = ServerUriTBX.Text.Trim();
            IPEndPoint? ep;
            var ctrl = IPEndPoint.TryParse(text, out ep);
            if (!ctrl)
            {
                MessageBox.Show("Please enter a valid endpoint in the relevant field...", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                return;
            }
            Globals.Frame.Navigate(new ConnectionPage(ep));
        }

        private void ServerUriTBX_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ServerUriTBX.SelectAll();
        }
    }
}
