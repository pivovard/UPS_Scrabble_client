using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPS_Scrabble_client
{
    public static class Network
    {
        public const int msg_length = 1024;

        public static byte[] ip;
        public static int port;

        public static byte[] buffer = new byte[64];
        public static byte[] buffer_in = new byte[msg_length];
        public static byte[] buffer_out = new byte[msg_length];

        public static Socket Socket { get; set; }

        public static Thread Listenner { get; set; }

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

            if (!int.TryParse(port, out Network.port))
            {
                MessageBox.Show("Port must be a number!" + Network.port);
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

            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine($"Socket created.");

                Socket.Connect(ipEndPoint);
                Console.WriteLine($"Connected to {Socket.RemoteEndPoint.ToString()}");
            }
            catch (Exception e)
            {
                MessageBox.Show("Couldn't connect to the server. Error:\n" + e);
                return false;
            }
            
            buffer = Encoding.UTF8.GetBytes(nick);
            Socket.Send(buffer);

            Listenner = new Thread(Listen);
            Listenner.Start();

            return true;
        }

        public static void Disconnect()
        {
            Socket.Close();
            Console.WriteLine($"Disconnected.");
        }

        private static void Listen()
        {
            int size;
            string msg;

            try
            {
                while ((size = Socket.Receive(buffer_in, msg_length, SocketFlags.None)) > 0)
                {
                    msg = Encoding.UTF8.GetString(buffer_in);
                    Console.WriteLine(msg);
                }

                if(size < 1)
                {
                    MessageBox.Show("Server unreachable.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
