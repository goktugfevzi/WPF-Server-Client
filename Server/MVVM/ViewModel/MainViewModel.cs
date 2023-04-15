using Client.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Net;
using Server.Net.IO;
using System.Diagnostics;

namespace Server.MVVM.ViewModel
{

    public class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel _instance;

        public static MainViewModel Instance => _instance ??= new();
        static TcpListener listener;
        static List<Server.Net.Client> clients;
        public string _GetMessage;

        public string getMessage
        {
            get => _GetMessage;
            set
            {
                _GetMessage = value;
                OnPropertyChanged(nameof(getMessage));
            }
        }

        public static ObservableCollection<string> Messages { get; set; }
        public string Message { get; set; }

        public string Light { get; set; }

        private string _imagePath = "../../Images/Red.png";

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }
        private MainViewModel()
        {

            Messages = new ObservableCollection<string>();
            clients = new List<Server.Net.Client>();
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
            listener.Start();

            Task.Run(() =>
           {
               while (true)
               {
                   var client = new Server.Net.Client(listener.AcceptTcpClient());
                   clients.Add(client);
                   BroadcastConnection();
                   generateOldVariable();
               }
           });
        }

        public void generateOldVariable()
        {
            if (getMessage != null)
            {
                BroadcastMessage(getMessage);
            }

            if (ImagePath == "../../Images/Red.png")
            {
                BroadcastNewLight("off");
            }
            else
            {
                BroadcastNewLight("on");
            }
        }
        public void BroadcastMessage(string message)
        {
            foreach (var client in clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public void BroadcastNewLight(string message)
        {
            foreach (var client in clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(15);
                msgPacket.WriteMessage(message);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public void BroadcastLight(string message)
        {
            if (message == null || message == "on")
            {
                ImagePath = "../../Images/Green.png";
            }
            else if (message == "off")
            {
                ImagePath = ("../../Images/Red.png");
            }
            foreach (var client in clients)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(15);
                msgPacket.WriteMessage(message);
                client.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public void MessageReceived(string message)
        {
            getMessage = message;
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }
        public void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = clients.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            clients.Remove(disconnectedUser);


        }
        void BroadcastConnection()
        {
            foreach (var clnt in clients)
            {
                var BroadcastPacket = new PacketBuilder();
                BroadcastPacket.WriteOpCode(1);
                BroadcastPacket.WriteMessage(clnt.Username);
                BroadcastPacket.WriteMessage(clnt.UID.ToString());
                clnt.ClientSocket.Client.Send(BroadcastPacket.GetPacketBytes());
            }
        }










        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
