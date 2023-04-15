using Client.Core;
using Client.MVVM.Model;
using Client.Net;
using IotechTaskWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client.MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel _instance;
        public static MainViewModel Instance => _instance ??= new();
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand LightToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public string Username { get; set; }
        public string Message { get; set; }
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

        public string Light { get; set; }
        private readonly Server server;
        public MainViewModel()
        {

            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            server = new Server();


            server.ConnectedEvent += UserConnected;
            server.msgReceivedEvent += MessageReceived;
            server.lightSwitchEvent += UserLight;
            server.DisconnectedEvent += UserDisconnected;


            Guid guid = Guid.NewGuid();
            string str = guid.ToString();
            server.ConnectToServer(str);

            SendMessageCommand = new RelayCommand(o => server.SendMessageToServer(Message));
            LightToServerCommand = new RelayCommand(o => server.LightToServer(Light));
        }
        private void UserConnected()
        {

            var user = new UserModel
            {
                Username = server.Reader.ReadMessage(),
                UID = server.Reader.ReadMessage(),
            };
            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
        private void UserLight()
        {
            var light = server.Reader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
            if (light == "on")
            {
                ImagePath = "../../Images/Green.png";
            }
            else
            {
                ImagePath = "../../Images/Red.png";
            }
            });
        }

        private void MessageReceived()
        {
            var message = server.Reader.ReadMessage();
            getMessage = message;
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }
        private void UserDisconnected()
        {
            var uid = server.Reader.ReadMessage();
            var disconnectedUser = Users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(disconnectedUser));

        }






        /// <summary>
        /// ///////////////////////////////////////////////////
        /// </summary>
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
