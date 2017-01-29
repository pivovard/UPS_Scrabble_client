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
        private object _lock = new object();
        public bool connected = false;

        public Form_Main()
        {
            InitializeComponent();
        }

        public void Connect_Disconnect()
        {
            lock (_lock)
            {
                if (!connected)
                {
                    int n;
                    if (radioButton1.Checked)
                    {
                        n = 2;
                    }
                    else if (radioButton2.Checked)
                    {
                        n = 3;
                    }
                    else
                    {
                        n = 4;
                    }

                    connected = Network.Connect(Tb_IP.Text, Tb_Port.Text, Tb_Nick.Text, n);
                    if (connected)
                        if (Btn_Connect.InvokeRequired)
                        {
                            Btn_Connect.Invoke(new Action(delegate () {
                                Btn_Connect.Text = "Disconnect";
                                radioButton1.Enabled = false;
                                radioButton2.Enabled = false;
                                radioButton3.Enabled = false;
                            }));
                        }
                        else
                        {
                            Btn_Connect.Text = "Disconnect";
                            radioButton1.Enabled = false;
                            radioButton2.Enabled = false;
                            radioButton3.Enabled = false;
                        }
                }
                else
                {
                    Network.Disconnect();
                    connected = false;
                    if (Btn_Connect.InvokeRequired)
                    {
                        Btn_Connect.Invoke(new Action(delegate () {
                            Btn_Connect.Text = "Connect";
                            radioButton1.Enabled = true;
                            radioButton2.Enabled = true;
                            radioButton3.Enabled = true;
                        }));
                    }
                    else
                    {
                        Btn_Connect.Text = "Connect";
                        radioButton1.Enabled = true;
                        radioButton2.Enabled = true;
                        radioButton3.Enabled = true;
                    }
                } 
            }
        }

        public void Button_Connect_Click(object sender, EventArgs e)
        {
            this.Connect_Disconnect();
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
