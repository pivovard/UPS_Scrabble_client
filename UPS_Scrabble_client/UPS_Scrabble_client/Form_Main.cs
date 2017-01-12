using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPS_Scrabble_client
{
    public partial class Form_Main : Form
    {
        public bool connected = false;

        public Form_Main()
        {
            InitializeComponent();
        }

        public void Button_Connect_Click(object sender, EventArgs e)
        {
            //lock on connected
            if (!connected)
            {
                int n;
                if (radioButton1.Checked) {
                    n = 2;
                }
                else if (radioButton2.Checked) {
                    n = 3;
                }
                else {
                    n = 4;
                }

                connected = Network.Connect(Tb_IP.Text, Tb_Port.Text, Tb_Nick.Text, n);
                if(connected)
                    if (Btn_Connect.InvokeRequired)
                    {
                        Btn_Connect.Invoke(new Action(delegate () { Btn_Connect.Text = "Disconnect"; }));
                    }
                    else
                    {
                        Btn_Connect.Text = "Disconnect";
                    }
            }
            else
            {
                Network.Disconnect();
                connected = false;
                if (Btn_Connect.InvokeRequired)
                {
                    Btn_Connect.Invoke(new Action(delegate () { Btn_Connect.Text = "Connect"; }));
                }
                else
                {
                    Btn_Connect.Text = "Connect";
                }
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            if (!connected) return;
            this.Hide();

            try
            {
                Program.FormGame.Show();
            }
            catch (Exception ex)
            {
                Program.FormGame = new Form_Game(Program.Game);
                Program.FormGame.UpdateScore();
                Program.Game.Random();
                Program.Game.Reconnect();
                Program.FormGame.Show();
            }
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (connected) Network.Disconnect();
        }
    }
}
