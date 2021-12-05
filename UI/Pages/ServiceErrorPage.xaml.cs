using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for ServiceErrorPage.xaml
    /// </summary>
    public partial class ServiceErrorPage : Page
    {
        Exception? _exception;

        public ServiceErrorPage(Exception exception)
        {
            InitializeComponent();
            _exception = exception;
            Loaded += ServiceErrorPage_Loaded;
            GoWelcomePageBTN.Click += GoWelcomePageBTN_Click;
        }

        private void GoWelcomePageBTN_Click(object sender, RoutedEventArgs e)
        {
            Globals.Frame.Navigate(new WelcomePage());
        }

        private void ServiceErrorPage_Loaded(object sender, RoutedEventArgs e)
        {
            MessageTBX.Text = _exception.ToString();
        }
    }
}
