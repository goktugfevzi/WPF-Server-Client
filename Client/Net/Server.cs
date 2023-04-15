using Client.Net.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using IotechTaskWpf;
using System.Windows.Controls;
using Client.MVVM.ViewModel;

namespace Client.Net
{
    class Server
    {
        TcpClient client;
        public PacketReader Reader;


        public event Action ConnectedEvent;
        public event Action msgReceivedEvent;
        public event Action lightSwitchEvent;
        public event Action DisconnectedEvent;

        public Server()
        {
            client = new TcpClient();
        }
        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var opcode = Reader.ReadByte();

                        switch (opcode)
                        {
                            case 1:
                                ConnectedEvent?.Invoke();
                                break;
                            case 5:
                                msgReceivedEvent?.Invoke();
                                break;
                            case 10:
                                DisconnectedEvent?.Invoke();
                                break;
                            case 15:
                                lightSwitchEvent?.Invoke();
                                break;
                            default:
                                MessageBox.Show("yanlis opcode");
                                break;
                        }
                    }
                    catch (Exception)
                    {
                    }

                }
            });
        }
        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {

                client.Connect("127.0.0.1", 1234);
                Reader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        public void SendMessageToServer(string message)
        {
            Random random = new Random();
            int number = random.Next(0, 100000);
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(number.ToString());
            client.Client.Send(messagePacket.GetPacketBytes());
        }
        public void LightToServer(string lightmsg)
        {
            try
            {
                lightmsg ??= "off";
                if (MainViewModel.Instance.ImagePath == "../../Images/Red.png")
                {
                    lightmsg = "on";
                }
                else
                {
                    lightmsg = "off";
                }
                var lightPacket = new PacketBuilder();
                lightPacket.WriteOpCode(15);
                lightPacket.WriteMessage(lightmsg);
                client.Client.Send(lightPacket.GetPacketBytes());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
