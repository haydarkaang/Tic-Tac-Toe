using System.Windows;
using System.Windows.Media;
using static GameGrpcServiceV1.Game.Types;

namespace UI
{
    public class Globals
    {
        public static System.Windows.Controls.Frame? Frame { get; set; }
        public static GameGrpcServiceV1.TicTacToeSrv.TicTacToeSrvClient? SrvClient { get; set; }
        public static System.DateTime Timeout { get { return System.DateTime.UtcNow.AddSeconds(1); } }

        public static DrawingImage GetDrawing(Shape shape)
        {
            return (DrawingImage)Application.Current.Resources[$"{shape}"];
        }
    }
}
