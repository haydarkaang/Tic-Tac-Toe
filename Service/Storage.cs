using GameGrpcServiceV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using static GameGrpcServiceV1.Game.Types;

namespace Service
{
    internal class Storage
    {
        Timer _timer;
        public Dictionary<string, Game> GameDict { get; set; }

        public Storage()
        {
            GameDict = new Dictionary<string, Game>();

            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var garbage = GameDict.Where(x => (DateTime.Now - DateTime.Parse(x.Value.DateTime)).TotalSeconds >= 30
            && x.Value.Status == GameStatus.Completed);
            
            if (garbage.Any())
            {
                Log.Prt($"Service: Storage: {garbage.Count()} Game(s) cleared from game dictionary",
                    Log.LogType.Message);
                foreach (var item in garbage)
                    GameDict.Remove(item.Key);
            }
        }
    }
}
