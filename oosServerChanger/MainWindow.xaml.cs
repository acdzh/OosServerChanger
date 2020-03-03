using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;


namespace oosServerChanger {

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class MainWindow : Window {

        public MainWindowData Datas = new MainWindowData();
        private List<Tuple<Task, CancellationTokenSource>> TaskTuples = new List<Tuple<Task, CancellationTokenSource>>();

        public MainWindow() {
            Utils.SetThemeUseingWindowsTheme();
            InitializeComponent();
            string serverName = Utils.GetServer();
            if (serverName == "") {
                MessageBox.Show("Can not find the value: photonRegion. Please check the registry.", "Error");
                Datas.CurrentServerName = "Error";
            } else {
                Datas.CurrentServerName = serverName;
            }
            DataContext = Datas;
        }

        private void InitTasks(bool isRecycle) {
            CancelAllTasks();
            TaskTuples.Clear();
            foreach (Server server in Datas.ServerList) {
                if (server.Name == "BEST") {
                    continue;
                }
                if (server.Delay == "-1 ms") server.Delay = "waiting";
                else server.Delay += "...";
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                TaskTuples.Add(
                    new Tuple<Task, CancellationTokenSource>(
                        new Task(() => {
                            while (!tokenSource.Token.IsCancellationRequested) {
                                string ip = server.IP;
                                long delayTime = Utils.MyUdpPing(ip);
                                server.Delay = delayTime == 0 ? "time out" : string.Format("{0} ms", delayTime);
                                if (!isRecycle || Datas.IsNotAutoRefesh) break;
                                Thread.Sleep(1000);
                            }
                        }, tokenSource.Token), 
                        tokenSource
                    )
                );
            }
            Console.WriteLine(TaskTuples.Count());
        }

        private void CancelAllTasks() {
            foreach(Tuple<Task, CancellationTokenSource> taskTuple in TaskTuples) {
                taskTuple.Item2.Cancel();
                Console.WriteLine(taskTuple.Item1.IsCanceled);
            }
        }

        private void Button_Refesh_Click(object sender, RoutedEventArgs e) {
            InitTasks(false);
            foreach (Tuple<Task, CancellationTokenSource> taskTuple in TaskTuples) {
                taskTuple.Item1.Start();
            }
        }

        private void Button_Change_Click(object sender, RoutedEventArgs e) {
            if (ListView_ServerList.SelectedItems.Count == 0) {
                return;
            }
            string server = ((Server)ListView_ServerList.SelectedItem).Name;
            if (Utils.SetServer(server)) {
                Datas.CurrentServerName = server;
                Utils.MyUdpPing(((Server)ListView_ServerList.SelectedItem).IP);
            } else {
                MessageBox.Show("Can not find the value: photonRegion. Please check the registry.", "Error");
            }
        }

        private void ToggleButton_AutoRefesh_Checked(object sender, RoutedEventArgs e) {
            if (Datas.IsAutoRefesh) {
                InitTasks(true);
                foreach (Tuple<Task, CancellationTokenSource> taskTuple in TaskTuples) {
                    taskTuple.Item1.Start();
                }
            }
        }
    }
}
