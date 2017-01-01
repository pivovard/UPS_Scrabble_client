using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPS_Scrabble_client
{
    public partial class Form_Game : Form
    {
        char[][] field;
        char[] stack;


        public Form_Game()
        {
            InitializeComponent();
            
            init();
        }

        private void init()
        {
            field = new char[15][];
            stack = new char[7];

            for(int i = 0; i < 15; i++)
            {
                field[i] = new char[15];
                DataGridView.Rows.Add();
            }

            DataGridView.Rows[8].Cells[8].Style.BackColor = Color.Khaki;
        }

        private void Btn_End_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
            Program.FM.Show();
        }
    }
}
