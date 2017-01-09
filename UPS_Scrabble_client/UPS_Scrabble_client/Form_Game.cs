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
        public bool turn = false;


        public Form_Game(Game g)
        {
            InitializeComponent();

            Game = g;

            L_Player1.Text = Game.Players[0].nick;
            L_Player2.Text = Game.Players[1].nick;

            for (int i = 0; i < 15; i++)
            {
                Field_DataGridView.Rows.Add();
            }

            Field_DataGridView.Rows[7].Cells[7].Style.BackColor = Color.Khaki;
        }

        public void UpdateScore()
        {
            L_Score1.Text = Game.Players[0].score.ToString();
            L_Score2.Text = Game.Players[1].score.ToString();
        }

        private bool IsMove(int x, int y)
        {
            bool res = false;

            //test, jestli je prvni kamen na stredovem poli
            if (Game.field[7][7] != '\0' || (x == 7 && y == 7)) res = true;

            //test, jestli se kamen dotyka jineho
            if (x != 14 && Game.field[x + 1][y] != '\0') res = true;
            if (x !=  0 && Game.field[x - 1][y] != '\0') res = true;
            if (y != 14 && Game.field[x][y + 1] != '\0') res = true;
            if (y !=  0 && Game.field[x][y - 1] != '\0') res = true;

            return res;
        }


        private void Stack_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            c = e.ColumnIndex;
        }

        private void Field_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Game.stack[c] == '\0' || turn == false) return;
            if (!IsMove(e.RowIndex, e.ColumnIndex)) return;

            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Khaki;
            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Game.stack[c];
            Game.field[e.RowIndex][e.ColumnIndex] = Game.stack[c];

            Game.turn += ";" + e.RowIndex + "," + e.ColumnIndex + "," + Game.stack[c];
            Game.Player.score++;

            Game.stack[c] = '\0';
            Stack_DataGridView.Rows[0].Cells[c].Value = "";
        }

        private void Btn_Turn_Click(object sender, EventArgs e)
        {
            Btn_Turn.Enabled = false;
            turn = false;

            UpdateScore();
            
            Network.Send("TURN:" + Game.ID + ':' + Game.score + Game.turn);
            
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
