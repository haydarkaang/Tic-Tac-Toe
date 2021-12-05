using GameGrpcServiceV1;
using System.Timers;
using System.Windows.Controls;
using static GameGrpcServiceV1.Game.Types;

namespace UI.Pages
{
    /// <summary>
    /// Interaction logic for WinningPlayerPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        Timer _goBackTimer;
        Game _game;

        public ResultPage(Game game)
        {
            InitializeComponent();
            _game = game;

            _goBackTimer = new Timer(2000);
            _goBackTimer.Elapsed += _goBackTimer_Elapsed;

            var w = (int)_game.WinningPlayer;
            if (w == 2)
            {
                TitleTB.Text = "DRAW!";
            }
            else
            {
                TitleTB.Text = "WIN!";
                ShapeIMG.Source = Globals.GetDrawing((Shape)w);
            }

            _goBackTimer.Start();
        }

        private void _goBackTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _goBackTimer.Stop();
            Dispatcher.Invoke(() =>
            {
                Globals.Frame.Navigate(new GameListPage());
            });
        }
    }
}
