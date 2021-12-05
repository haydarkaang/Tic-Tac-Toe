using GameGrpcServiceV1;
using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static GameGrpcServiceV1.Game.Types;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for PlaygroundPage.xaml
    /// </summary>
    public partial class PlaygroundPage : Page
    {
        Shape _currentPlayer;
        Game? _game;

        public PlaygroundPage(Game? game)
        {
            _game = game;

            InitializeComponent();
            Loaded += PlaygroundPage_Loaded;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void PlaygroundPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_game == null)
            {
                _currentPlayer = Shape.Circle;

                _game = new Game()
                {
                    Id = Guid.NewGuid().ToString(),
                    DateTime = DateTime.UtcNow.ToString(),
                    Status = GameStatus.WaitingForOpponent,
                    GameSteps = new GameSteps()
                };

                try
                {
                    var r = Globals.SrvClient.CreateGame(_game, deadline: Globals.Timeout);
                    if (r.Status == Response.Types.call_status.Err)
                    {
                        Globals.Frame.Navigate(new Exception(r.Message));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Globals.Frame.Navigate(new ServiceErrorPage(ex));
                    return;
                }
            }
            else
            {
                _currentPlayer = Shape.Cross;
                _game.Status = GameStatus.Inprogress;
                try
                {
                    Globals.SrvClient.UpdateGame(_game);
                }
                catch (Exception ex)
                {
                    Globals.Frame.Navigate(new ServiceErrorPage(ex));
                    return;
                }
            }

            CurrentPlayerIMG.Source = Globals.GetDrawing(_currentPlayer);

            PreparePlayground();
            StreamLoop();
        }

        private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
        {
            _game.Status = GameStatus.Completed;

            try
            {
                Globals.SrvClient.UpdateGame(_game);
            }
            catch (Exception ex)
            {
                Globals.Frame.Navigate(new ServiceErrorPage(ex));
            }
        }

        private void PlaygroundBTNClicked(object sender, MouseButtonEventArgs e)
        {
            GameBoardGrid.IsEnabled = false;

            var btn = sender as Button;
            var val = uint.Parse(btn.Name.Split('_')[1]);

            if (_game.GameSteps.Circle.Contains(val))
                return;

            if (_game.GameSteps.Cross.Contains(val))
                return;

            if (_currentPlayer == Shape.Circle)
                _game.GameSteps.Circle.Add(val);
            else
                _game.GameSteps.Cross.Add(val);

            try
            {
                Globals.SrvClient.UpdateGame(_game);
            }
            catch (Exception ex)
            {
                Globals.Frame.Navigate(new ServiceErrorPage(ex));
                return;
            }
        }

        private async Task StreamLoop()
        {
            try
            {
                using (var call = Globals.SrvClient.GameStream())
                {
                    await call.RequestStream.WriteAsync(new GameStreamRequest() { Id = _game.Id });
                    while (_game.Status != GameStatus.Completed && await call.ResponseStream.MoveNext())
                    {
                        if (_game != null)
                        {
                            await call.RequestStream.WriteAsync(new GameStreamRequest() { Id = _game.Id });
                            await Task.Delay(50);
                        }
                        else
                        {
                            break;
                        }
                        _game = call.ResponseStream.Current;
                        Dispatcher.Invoke(() => { UpdatePlayground(); });
                    }
                }
            }
            catch (Exception ex)
            {
                _game.Status = GameStatus.Completed;
                Dispatcher.Invoke(() => { Globals.Frame.Navigate(new ServiceErrorPage(ex)); });
            }
        }

        private void PreparePlayground()
        {
            GameBoardGrid.IsEnabled = false;

            var value = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var img = new Image() { Margin = new Thickness(10) };
                    var btn = new Button();
                    btn.Name = $"BTN_{++value}";
                    btn.PreviewMouseDown += PlaygroundBTNClicked;
                    btn.SetValue(Grid.ColumnProperty, c);
                    btn.SetValue(Grid.RowProperty, r);
                    btn.Content = img;
                    GameBoardGrid.Children.Add(btn);
                }
            }
        }

        private void UpdatePlayground()
        {
            if (ShowWinner())
                return;
            UpdateTurn();
            UpdateButtons();
        }

        private bool ShowWinner()
        {
            var r = _game.Status == GameStatus.Completed;
            if (r) Globals.Frame.Navigate(new ResultPage(_game));
            return r;
        }

        private void UpdateTurn()
        {
            TurnIMG.Source = Globals.GetDrawing(_game.Turn);
        }

        private void UpdateButtons()
        {
            var isEnabled = _game.Turn == _currentPlayer &&
               _game.Status == GameStatus.Inprogress;

            GameBoardGrid.IsEnabled = isEnabled;

            for (int i = 1; i <= 9; i++)
            {
                var btn = LogicalTreeHelper.FindLogicalNode(GameBoardGrid, $"BTN_{i}") as Button;
                var img = btn.Content as Image;
                if (_game.GameSteps.Circle.Where(x => x == i).Any())
                    img.Source = Globals.GetDrawing(Shape.Circle);
                if (_game.GameSteps.Cross.Where(x => x == i).Any())
                    img.Source = Globals.GetDrawing(Shape.Cross);
            }
        }
    }
}
