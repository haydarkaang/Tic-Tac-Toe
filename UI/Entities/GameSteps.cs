using System.Collections.Generic;
using System.ComponentModel;

namespace UI.Entities
{
    public class GameSteps : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<uint>? _circle;
        private List<uint>? _cross;

        public List<uint>? Circle
        {
            get { return _circle; }
            set { _circle = value; OnPropertyChanged("Circle"); }
        }

        public List<uint>? Cross
        {
            get { return _cross; }
            set { _cross = value; OnPropertyChanged("Cross"); }
        }


        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
