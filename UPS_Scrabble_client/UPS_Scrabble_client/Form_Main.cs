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
        bool connected = false;

        public Form_Main()
        {
            InitializeComponent();
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                connected = Network.Connect(Tb_IP.Text, Tb_Port.Text, Tb_Nick.Text);
                if(connected)
                    Btn_Connect.Text = "Disconnect";
            }
            else
            {
                Network.Disconnect();
                connected = false;
                Btn_Connect.Text = "Connect";
            }
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            //if (!connected) return;
            this.Hide();
            Program.FormGame.Show();
        }

        private void Form_Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (connected) Network.Disconnect();
        }
    }
}
