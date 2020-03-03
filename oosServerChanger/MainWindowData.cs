using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oosServerChanger {
    public class Server : INotifyPropertyChanged {
        public string Name { get; set; }
        public string IP { get; set; }
        public string Domain { get; set; }
        private string _Delay;
        public string Delay { 
            get { return _Delay; } 
            set { _Delay = value; OnPropertyChanged("Delay"); } }
        protected internal virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class MainWindowData : INotifyPropertyChanged {
        private string _CurrentServerName = "";
        public string CurrentServerName {
            get { return _CurrentServerName; }
            set { _CurrentServerName = value; OnPropertyChanged("CurrentServerName"); }
        }

        private bool _IsAutoRefesh;
        public bool IsAutoRefesh {
            get { return _IsAutoRefesh; }
            set { _IsAutoRefesh = value; OnPropertyChanged("IsAutoRefesh"); OnPropertyChanged("IsNotAutoRefesh"); }
        }
        public bool IsNotAutoRefesh {
            get { return !_IsAutoRefesh; }
        }


        public ObservableCollection<Server> _ServerList = new ObservableCollection<Server>() {
            { new Server(){ Name = "CN", IP = "123.206.109.115", Domain="", Delay="-1 ms"} },
            { new Server(){ Name = "ASIA", IP = "92.38.183.13", Domain="gcsing019.exitgames.com", Delay="-1 ms"} },
            { new Server(){ Name = "EU", IP = "92.38.154.34", Domain="gcams049.exitgames.com", Delay="-1 ms"} },
            { new Server(){ Name = "SA", IP = "5.188.239.5", Domain ="gcsp001.exitgames.com",Delay="-1 ms"} },
            { new Server(){ Name = "US", IP = "92.223.82.17",Domain="gcash013.exitgames.com", Delay="-1 ms"} },
            { new Server(){ Name = "BEST", IP = "", Domain="", Delay=""} }
        };
        public ObservableCollection<Server> ServerList {
            get { return _ServerList; }
            set { _ServerList = value; OnPropertyChanged("ServerList"); }
        }




        protected internal virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
