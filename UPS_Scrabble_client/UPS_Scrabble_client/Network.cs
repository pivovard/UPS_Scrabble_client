using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPS_Scrabble_client
{
    public static class Network
    {
        public const int msg_length = 1024;

        public static byte[] ip;
        public static int port;

        public static byte[] buffer = new byte[msg_length];

        public static Socket socket;

        public static bool Connect(string ip, string port, string nick)
        {
            string[] split = ip.Split('.');
            if (split.Count() != 4)
            {
                MessageBox.Show("IP address format must be 1.1.1.1 !");
                return false;
            }

            Network.ip = new byte[4];
            Network.ip[0] = byte.Parse(split[0]);
            Network.ip[1] = byte.Parse(split[1]);
            Network.ip[2] = byte.Parse(split[2]);
            Network.ip[3] = byte.Parse(split[3]);

            if (int.TryParse(port, out Network.port))
            {
                MessageBox.Show("Port must be a number!");
                return false;
            }

            if(nick.Length == 0)
            {
                MessageBox.Show("Nick must be at least 1 character long!");
                return false;
            }

            //IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = new IPAddress(ipHostEntry);
            IPAddress ipAddress = new IPAddress(Network.ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, Network.port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine($"Socket created.");

            socket.Connect(ipEndPoint);
            Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}");

            buffer = Encoding.UTF8.GetBytes("Pivis");
            socket.Send(buffer);

            return true;
        }

        public static void Disconnect()
        {
            socket.Close();
        }
    }
}
