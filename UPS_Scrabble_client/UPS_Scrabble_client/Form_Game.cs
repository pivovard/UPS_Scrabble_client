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
    public partial class Form_Game : Form
    {
        Game Game { get; set; }

        int c = 0;


        public Form_Game(Game g)
        {
            InitializeComponent();

            Game = g;

            L_Player1.Text = Game.players[0];
            L_Player2.Text = Game.players[1];

            for (int i = 0; i < 15; i++)
            {
                Field_DataGridView.Rows.Add();
            }

            Field_DataGridView.Rows[7].Cells[7].Style.BackColor = Color.Khaki;
        }

        private void Stack_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            c = e.ColumnIndex;
        }

        private void Field_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Game.stack[c] == '*') return;

            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Khaki;
            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Game.stack[c];
            Game.field[e.RowIndex][e.ColumnIndex] = Game.stack[c];

            Game.turn += ";" + e.RowIndex + "," + e.ColumnIndex;
            Game.score++;

            Game.stack[c] = '*';
            Stack_DataGridView.Rows[0].Cells[c].Value = "";
        }

        private void Btn_Turn_Click(object sender, EventArgs e)
        {
            Btn_Turn.Enabled = false;

            Network.Send("TURN:" + Game.score + Game.turn);
            
            Game.Random();
            Game.turn = "";
        }



        private void Btn_End_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form_Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Hide();
            Program.FormMain.Show();
        }
    }
}
