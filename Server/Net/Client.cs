using Server.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Server.MVVM.ViewModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Server.Net
{
    class Client
    {
        public event Action lightSwitchEvent;

        private MainViewModel mainViewModel;

        public int id;
        public string Username { get; set; }
        public string Light { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }



        PacketReader Reader { get; set; }
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            Reader = new PacketReader(ClientSocket.GetStream());
            var opcode = Reader.ReadByte();
            Username = Reader.ReadMessage();
            Task.Run(() => Process());
        }




        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = Reader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = Reader.ReadMessage();
                            MainViewModel.Instance.MessageReceived(msg);
                            MainViewModel.Instance.BroadcastMessage(msg);
                            break;
                        case 15:
                            var lightmsg = Reader.ReadMessage();
                            MainViewModel.Instance.BroadcastLight(lightmsg);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Disconnected");
                    MainViewModel.Instance.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
