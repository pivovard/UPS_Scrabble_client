using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPS_Scrabble_client
{
    public static class Network
    {
        public const int msg_length = 1024;

        public static byte[] ip;
        public static int port;

        static string nick;
        static int n;

        public static byte[] buffer = new byte[64];
        public static byte[] buffer_in = new byte[msg_length];
        public static byte[] buffer_out = new byte[msg_length];

        public static Socket Socket { get; set; }

        public static Thread Listenner { get; set; }

        public static bool Connect(string ip, string port, string nick, int n)
        {
            string[] split = ip.Split('.');
            if (split.Count() != 4)
            {
                MessageBox.Show("IP address format must be 255.255.255.255 !");
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

            Network.nick = nick;
            Network.n = n;

            //IPHostEntry ipHostEntry = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = new IPAddress(ipHostEntry);
            IPAddress ipAddress = new IPAddress(Network.ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, Network.port);

            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Console.WriteLine("Socket created.");

                Socket.Connect(ipEndPoint);
                Console.WriteLine("Connected to " + Socket.RemoteEndPoint.ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("Couldn't connect to the server. Error:\n" + e);
                return false;
            }
            
            buffer = Encoding.ASCII.GetBytes(nick + ";" + n.ToString() + "\n");
            Socket.Send(buffer);

            Listenner = new Thread(Listen);
            Listenner.Start();

            return true;
        }

        public static void Disconnect()
        {
            Socket.Close();
            Console.WriteLine("Disconnected.");
        }

        private static void Listen()
        {
            int size;
            string msg = "";

            try
            {
                while ((size = Socket.Receive(buffer_in, msg_length, SocketFlags.None)) > 0)
                {
                    msg = Encoding.ASCII.GetString(buffer_in, 0, size);
                    int i = msg.IndexOf("\n");
                    if (i != -1)
                        msg = msg.Substring(0, i);
                    Console.WriteLine("Recv: " + msg);
                    Resolve(msg);
                }

                if (size < 1)
                {
                    MessageBox.Show("Server unavaible.");
                    Program.FormGame.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void Resolve(string msg)
        {
            string[] type = msg.Split(':');

            switch (type[0])
            {
                case "GAME":
                    Program.Game = new Game(type[1], type[2], nick, n);
                    Program.FormGame = new Form_Game(Program.Game);
                    Program.Game.Random();
                    
                    if (Program.FormMain.Btn_Start.InvokeRequired)
                    {
                        Program.FormMain.Btn_Start.Invoke(new Action(delegate() { Program.FormMain.Btn_Start.Enabled = true; }));
                    }
                    else
                    {
                        Program.FormMain.Btn_Start.Enabled = true;
                    }
                    
                    break;

                case "TURN":
                    if (Program.FormMain.Btn_Start.InvokeRequired)
                    {
                        Program.FormMain.Btn_Start.Invoke(new Action(() => { Program.FormGame.Btn_Turn.Enabled = true; Program.FormGame.turn = true; }));
                    }
                    else
                    {
                        Program.FormGame.Btn_Turn.Enabled = true;
                        Program.FormGame.turn = true;
                    }
                    break;

                case "TURNP":
                    Program.Game.RecvTurn(type[1], type[2]);
                    break;

                case "NICK":
                    MessageBox.Show("Nick allready in use.");
                    //Disconnect
                    Program.FormMain.Connect_Disconnect();
                    Disconnect();
                    break;

                case "RETURN":
                    DialogResult res = MessageBox.Show("Do you want to return to the existing game?", "Return", MessageBoxButtons.YesNo);
                    if (res == DialogResult.Yes)
                    {
                        Send("RETURN");
                    }
                    else if(res == DialogResult.No)
                    {
                        Send("NEW");
                    }
                    break;

                case "GAMER":
                    Program.Game = new Game(type[1], type[2], type[3], nick, n);
                    Program.FormGame = new Form_Game(Program.Game);
                    Program.FormGame.UpdateScore();
                    Program.Game.Random();
                    Program.Game.Reconnect();

                    if (Program.FormMain.Btn_Start.InvokeRequired)
                    {
                        Program.FormMain.Btn_Start.Invoke(new Action(delegate () { Program.FormMain.Btn_Start.Enabled = true; }));
                    }
                    else
                    {
                        Program.FormMain.Btn_Start.Enabled = true;
                    }
                    break;

                case "DISC":
                    MessageBox.Show("Player " + Program.Game.Players.Where(p => p.ID == int.Parse(type[1])).First().nick + " disconnected.");
                    break;

                case "RECN":
                    MessageBox.Show("Player " + Program.Game.Players.Where(p => p.ID == int.Parse(type[1])).First().nick + " reconnected.");
                    break;



                default:
                    break;
            }
        }

        public static void Send(string msg)
        {
            Console.WriteLine("Send: " + msg);
            buffer = Encoding.ASCII.GetBytes(msg + "\n");
            Socket.Send(buffer);
        }
    }
}
