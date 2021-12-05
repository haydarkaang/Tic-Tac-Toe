using System;
using System.ComponentModel;

namespace UI.Entities
{
    public class Game : INotifyPropertyChanged
    {
        public enum Shape
        {
            Circle,
            Cross
        }

        public enum GameStatus
        {
            WaitingForOpponent,
            InProgress,
            Completed
        }

        public enum Winner
        {
            PlayerCircle,
            PlayerCross,
            Draw
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private string _id;
        private DateTime _dateTime;
        private Winner _winningPlayer;
        private GameSteps? _steps;
        private GameStatus _status;
        private Shape _turn;

        public string Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("Id"); }
        }

        public DateTime DateTime
        {
            get { return _dateTime; }
            set { _dateTime = value; OnPropertyChanged("DateTime"); }
        }

        public Winner WinningPlayer
        {
            get { return _winningPlayer; }
            set { _winningPlayer = value; OnPropertyChanged("WinningPlayer"); }
        }

        public GameSteps? GameSteps
        {
            get { return _steps; }
            set { _steps = value; OnPropertyChanged("GameSteps"); }
        }

        public GameStatus Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged("Status"); }
        }

        public Shape Turn
        {
            get { return _turn; }
            set { _turn = value; OnPropertyChanged("Turn"); }
        }

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
