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

            if (Game.N == 3)
            {
                L_Player3.Text = Game.Players[2].nick;
            }
            if (Game.N == 4)
            {
                L_Player4.Text = Game.Players[3].nick;
            }

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

            if(Game.N == 3){
                L_Score3.Text = Game.Players[2].score.ToString();
            }
            if (Game.N == 4)
            {
                L_Score4.Text = Game.Players[3].score.ToString();
            }
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
            
            Network.Send("TURN:" + Game.ID + ':' + Game.Player.score + Game.turn);
            
            Game.Random();
            Game.turn = "";
        }

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            if (Game.turn == "") return;

            string last = Game.turn.Split(';').Last();

            string[] c = last.Split(',');
            int x = int.Parse(c[0]);
            int y = int.Parse(c[1]);

            Game.field[x][y] = '\0';
            Field_DataGridView.Rows[x].Cells[y].Value = "";
            Field_DataGridView.Rows[x].Cells[y].Style.BackColor = Color.White;

            for(int i = 0; i < 7; i++)
            {
                if(Game.stack[i] == '\0')
                {
                    Game.stack[i] = c[2].First();
                    Stack_DataGridView.Rows[0].Cells[i].Value = c[2].First();
                    break;
                }
            }

            Game.Player.score--;

            Game.turn = Game.turn.Substring(0, Game.turn.Length - last.Length - 1);
        }

        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            if (Game.turn == "") return;

            string[] turn = Game.turn.Split(';');

            foreach(var t in turn)
            {
                if (t == "") continue;  //turn[0] je "" -> score

                string[] c = t.Split(',');
                int x = int.Parse(c[0]);
                int y = int.Parse(c[1]);

                Game.field[x][y] = '\0';
                Field_DataGridView.Rows[x].Cells[y].Value = "";
                Field_DataGridView.Rows[x].Cells[y].Style.BackColor = Color.White;

                for (int i = 0; i < 7; i++)
                {
                    if (Game.stack[i] == '\0')
                    {
                        Game.stack[i] = c[2].First();
                        Stack_DataGridView.Rows[0].Cells[i].Value = c[2].First();
                        break;
                    }
                }

                Game.Player.score--;
            }
            
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
