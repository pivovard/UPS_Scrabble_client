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
        /// <summary>
        /// Instance of Game (same as Program.Game)
        /// </summary>
        Game Game { get; set; }

        //Selected char in stack
        int c = 0;
        //Player on turn
        public bool turn = false;


        /// <summary>
        /// Create and init new Game Form
        /// </summary>
        /// <param name="g"></param>
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

            //middle field
            Field_DataGridView.Rows[7].Cells[7].Style.BackColor = Color.Khaki;
        }

        /// <summary>
        /// Update score of all players
        /// </summary>
        public void UpdateScore()
        {
            SetText(L_Score1, Game.Players[0].score.ToString());
            SetText(L_Score2, Game.Players[1].score.ToString());

            if (Game.N == 3){
                SetText(L_Score3, Game.Players[2].score.ToString());
            }
            if (Game.N == 4)
            {
                SetText(L_Score4, Game.Players[3].score.ToString());
            }
        }

        /// <summary>
        /// Set text to the label (cross-thread)
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        private void SetText(Label label, string value)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(delegate () { label.Text = value; }));
            }
            else
            {
                label.Text = value;
            }
        }

        /// <summary>
        /// Actualize Game Form
        /// </summary>
        public void UpdateTurns()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (Game.field[i][j] != '\0')
                    {
                        Program.FormGame.Field_DataGridView.Rows[i].Cells[j].Value = Game.field[i][j];
                        Program.FormGame.Field_DataGridView.Rows[i].Cells[j].Style.BackColor = System.Drawing.Color.Khaki;
                    }
                }
            }
        }

        /// <summary>
        /// Test if the place of char is valid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool IsMove(int x, int y)
        {
            bool res = false;

            //test, jestli je prvni kamen na stredovem poli
            if (Game.field[7][7] == '\0')
            {
                if (x == 7 && y == 7) return true;
                else return false;
            }

            //test, jestli je uz pole obsazeno
            if (Game.field[x][y] != '\0') return false;

            //test, jestli se kamen dotyka jineho
            if (x != 14 && Game.field[x + 1][y] != '\0') res = true;
            if (x !=  0 && Game.field[x - 1][y] != '\0') res = true;
            if (y != 14 && Game.field[x][y + 1] != '\0') res = true;
            if (y !=  0 && Game.field[x][y - 1] != '\0') res = true;
            
            return res;
        }

        /// <summary>
        /// Selects char from the stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stack_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            c = e.ColumnIndex;
        }

        /// <summary>
        /// Place char to the gameplan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Field_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //test
            if (Game.stack[c] == '\0' || turn == false) return;
            if (!IsMove(e.RowIndex, e.ColumnIndex)) return;

            //set char
            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Khaki;
            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Game.stack[c];
            Game.field[e.RowIndex][e.ColumnIndex] = Game.stack[c];

            //add move to turn
            Game.turn += ";" + e.RowIndex + "," + e.ColumnIndex + "," + Game.stack[c];

            //update score
            Game.Player.score += Game.Points(Game.stack[c]);
            UpdateScore();

            //update stack
            Game.stack[c] = '\0';
            Stack_DataGridView.Rows[0].Cells[c].Value = "";

            Field_DataGridView.ClearSelection();
        }

        /// <summary>
        /// Send turn to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Turn_Click(object sender, EventArgs e)
        {
            UpdateScore();
            Btn_Turn.Enabled = false;
            turn = false;
            
            //send
            Network.Send("TURN:" + Game.ID + ':' + Game.Player.score + Game.turn);
            
            //new stack
            Game.Random();
            Game.turn = "";
        }

        /// <summary>
        /// Return last placed char
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Back_Click(object sender, EventArgs e)
        {
            if (Game.turn == "") return;

            //get last move
            string last = Game.turn.Split(';').Last();

            string[] c = last.Split(',');
            int x = int.Parse(c[0]);
            int y = int.Parse(c[1]);

            //sub char from field
            Game.field[x][y] = '\0';
            Field_DataGridView.Rows[x].Cells[y].Value = "";
            Field_DataGridView.Rows[x].Cells[y].Style.BackColor = Color.White;

            //add char back to stack on free position
            for(int i = 0; i < 7; i++)
            {
                if(Game.stack[i] == '\0')
                {
                    Game.stack[i] = c[2].First();
                    Stack_DataGridView.Rows[0].Cells[i].Value = c[2].First();
                    break;
                }
            }

            //sub score
            Game.Player.score -= Game.Points(c[2].First());
            UpdateScore();

            //sub move from turn
            Game.turn = Game.turn.Substring(0, Game.turn.Length - last.Length - 1);
        }

        /// <summary>
        /// Return all placed chars of current turn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Reset_Click(object sender, EventArgs e)
        {
            if (Game.turn == "") return;

            string[] turn = Game.turn.Split(';');

            //all moves
            foreach(var t in turn)
            {
                if (t == "") continue;  //turn[0] is "" -> score

                string[] c = t.Split(',');
                int x = int.Parse(c[0]);
                int y = int.Parse(c[1]);

                //sub char from field
                Game.field[x][y] = '\0';
                Field_DataGridView.Rows[x].Cells[y].Value = "";
                Field_DataGridView.Rows[x].Cells[y].Style.BackColor = Color.White;

                //add char back to stack on free position
                for (int i = 0; i < 7; i++)
                {
                    if (Game.stack[i] == '\0')
                    {
                        Game.stack[i] = c[2].First();
                        Stack_DataGridView.Rows[0].Cells[i].Value = c[2].First();
                        break;
                    }
                }

                //sub score
                Game.Player.score -= Game.Points(c[2].First());
                UpdateScore();
            }
            
            //clear turn
            Game.turn = "";
        }

        /// <summary>
        /// Close Game Form -> Disconnect from the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_End_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Leave game?", "Exit", MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                Network.Send("END");
                this.Close();
            }
        }

        /// <summary>
        /// Disconnect from the server on close.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
            Program.Game = null;

            //Disconnect
            Program.FormMain.Connect_Disconnect();
            Program.FormMain.Show();
        }

        
    }
}
