using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for GameListPage.xaml
    /// </summary>
    public partial class GameListPage : Page
    {
        public ObservableCollection<Entities.Game> Games { get; set; }

        public GameListPage()
        {
            InitializeComponent();
            Games = new ObservableCollection<Entities.Game>();
            DataContext = Games;
            GameLB.ItemsSource = Games;
            Loaded += GameListPage_Loaded;
        }

        private void ListDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (GameLB.SelectedItem == null)
                return;

            var gameEntity = GameLB.SelectedItem as Entities.Game;
            var json = JsonConvert.SerializeObject(gameEntity);
            var grpcGame = JsonConvert.DeserializeObject<GameGrpcServiceV1.Game>(json);
            Globals.Frame.Navigate(new PlaygroundPage(grpcGame));
        }

        private void ButtonClicked(object sender, MouseButtonEventArgs e)
        {
            var btn = sender as Button;
            if (btn.Name == "CreateBTN")
                Globals.Frame.Navigate(new PlaygroundPage(null));
            else if (btn.Name == "GoBackBTN")
                Globals.Frame.Navigate(new WelcomePage());
            else
                RefreshGameList();
        }

        private void GameListPage_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshGameList();
        }

        private async void RefreshGameList()
        {
            GameGrpcServiceV1.Games? games;
            try
            {
                games = await Globals.SrvClient.GetAllGamesAsync(new Empty(), deadline: Globals.Timeout);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Globals.Frame.Navigate(new ServiceErrorPage(ex));
                });
                return;
            }

            var json = JsonConvert.SerializeObject(games.GameList);
            var entities = JsonConvert.DeserializeObject<List<Entities.Game>>(json);

            if (entities.Count == 0)
            {
                WarningTB.Dispatcher.Invoke(() =>
                {
                    WarningTB.Text = "There is no available game to join. Please create new one.";
                });

                return;
            }

            Dispatcher.Invoke(() =>
            {
                Games.Clear();
                foreach (var item in entities)
                {
                    item.DateTime = item.DateTime.ToLocalTime();
                    Games.Add(item);
                }

                WarningTB.Visibility = Visibility.Collapsed;
                GameLB.Visibility = Visibility.Visible;
            });
        }
    }
}
