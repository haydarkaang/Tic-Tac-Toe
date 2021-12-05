using Client.Library;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for ConnectionPage.xaml
    /// </summary>
    public partial class ConnectionPage : Page
    {
        IPEndPoint _serviceEP;

        public ConnectionPage(IPEndPoint serviceEP)
        {
            InitializeComponent();
            _serviceEP = serviceEP;
            Loaded += ConnectionPage_Loaded;
        }

        private void ConnectionPage_Loaded(object sender, RoutedEventArgs e)
        {
            var host = _serviceEP.Address.ToString();
            var port = _serviceEP.Port;
            new Manager().SetConnection(host, port).ContinueWith(_ =>
            {
                if (_.Result == null)
                {
                    var ex = new Exception($"Connection can not be established @ {host}:{port}");
                    Dispatcher.Invoke(() =>
                    {
                        Globals.Frame.Navigate(new ServiceErrorPage(ex));
                    });
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    Globals.SrvClient = _.Result;
                    Globals.Frame.Navigate(new GameListPage());
                });
            });
        }
    }
}
