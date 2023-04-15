using Server.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Threading;
using Client.Core;
using Server.Net;
using Server.MVVM.ViewModel;

namespace Server
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
                InitializeComponent();
            DataContext = MainViewModel.Instance;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            int number = random.Next(0,100000);
            MainViewModel.Instance.MessageReceived(number.ToString());
            MainViewModel.Instance.BroadcastMessage(number.ToString());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.Instance.ImagePath == "../../Images/Red.png")
            {
                MainViewModel.Instance.BroadcastLight("on");
                MainViewModel.Instance.ImagePath = "../../Images/Green.png";
            }
            else 
            {
                MainViewModel.Instance.BroadcastLight("off");
                MainViewModel.Instance.ImagePath = "../../Images/Red.png";

            }

        }
    }
}