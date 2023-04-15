using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace Server.Net.IO
{
    class PacketReader : BinaryReader
    {
        private NetworkStream ms;
        public PacketReader(NetworkStream ms) : base(ms)
        {
            this.ms = ms;

        }
        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            ms.Read(msgBuffer, 0, length);
            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
    }
}
