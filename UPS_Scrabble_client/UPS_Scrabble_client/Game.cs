using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace UPS_Scrabble_client
{
    public class Game
    {
        public int ID;
        public int N;

        public Player[] Players;
        public Player Player;

        public char[][] field;
        public char[] stack;

        public string turn = "";

        private string vocals = "AEIOUY";
        private int[] multiplier = { 1, 3, 3, 2, 1, 3, 3, 3, 1, 2, 2, 2, 2, 2, 1, 3, 10, 2, 2, 3, 1, 3, 10, 10, 1, 3 };


        public Game(string id, string pl, string nick, int n)
        {
            ID = int.Parse(id);
            N = n;

            //init hracu
            string[] pls = pl.Split(';');
            Players = new Player[pls.Count()];

            for(int i = 0; i < pls.Count(); i++)
            {
                Players[i] = new Player(pls[i]);
            }

            //mistni hrac
            Player = Players.Where(p => p.nick == nick).First(); ;

            field = new char[15][];
            stack = new char[7];

            for (int i = 0; i < 15; i++)
            {
                field[i] = new char[15];
                for(int j = 0; j < 15; j++)
                {
                    field[i][j] = '\0';
                }
            }
        }

        public Game(string id, string pl, string turn, string nick, int n)
        {
            ID = int.Parse(id);
            N = n;

            //init hracu
            string[] pls = pl.Split(';');
            Players = new Player[pls.Count()];

            for (int i = 0; i < pls.Count(); i++)
            {
                Players[i] = new Player(pls[i], true);
            }

            //mistni hrac
            Player = Players.Where(p => p.nick == nick).First(); ;

            field = new char[15][];
            stack = new char[7];

            for (int i = 0; i < 15; i++)
            {
                field[i] = new char[15];
                for (int j = 0; j < 15; j++)
                {
                    field[i][j] = '\0';
                }
            }

            int x;
            int y;
            string[] c;
            string[] t = turn.Split(';');
            //jednotlive tahy: x,y,char
            for (int i = 1; i < t.Count(); i++)
            {
                c = t[i].Split(',');

                x = int.Parse(c[0]);
                y = int.Parse(c[1]);

                field[x][y] = c[2].ElementAt(0);

                //Program.FormGame.Field_DataGridView.Rows[x].Cells[y].Value = c[2].ElementAt(0);
                //Program.FormGame.Field_DataGridView.Rows[x].Cells[y].Style.BackColor = System.Drawing.Color.Khaki;
            }
        }

        public void Random()
        {
            Random r = new Random();
            for (int i = 0; i < 7; i++)
            {
                if (r.Next() % 3 == 0) stack[i] = (char)(65 + r.Next(26));
                else stack[i] = vocals.ElementAt(r.Next(6));
            }

            Program.FormGame.Stack_DataGridView.Rows.Clear();
            Program.FormGame.Stack_DataGridView.Rows.Add(stack[0], stack[1], stack[2], stack[3], stack[4], stack[5], stack[6]);
        }

        public int Points(char c)
        {
            int i = c - 65;
            return multiplier[i];
        }

        public void RecvTurn(string player, string turn)
        {
            int id = int.Parse(player);
            Player pl = Players.Where(p => p.ID == id).First();

            //rozdeleni na tahy - [0] je score
            string[] t = turn.Split(';');
            pl.score = int.Parse(t[0]);
            Program.FormGame.UpdateScore();

            int x;
            int y;
            string[] c;

            //jednotlive tahy: x,y,char
            for(int i = 1; i < t.Count(); i++)
            {
                c = t[i].Split(',');

                x = int.Parse(c[0]);
                y = int.Parse(c[1]);

                field[x][y] = c[2].ElementAt(0);

                Program.FormGame.Field_DataGridView.Rows[x].Cells[y].Value = c[2].ElementAt(0);
                Program.FormGame.Field_DataGridView.Rows[x].Cells[y].Style.BackColor = System.Drawing.Color.Khaki;
            }
        }

        public void Reconnect()
        {
            for(int i = 0; i < 15; i++)
            {
                for(int j = 0; j < 15; j++)
                {
                    if(field[i][j] != '\0')
                    {
                        Program.FormGame.Field_DataGridView.Rows[i].Cells[j].Value = field[i][j];
                        Program.FormGame.Field_DataGridView.Rows[i].Cells[j].Style.BackColor = System.Drawing.Color.Khaki;
                    }
                }
            }
        }

    }
}
