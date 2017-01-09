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
        public Player[] Players;
        public Player Player;

        public char[][] field;
        public char[] stack;

        public int score = 0;

        public string turn = "";


        public Game(string id, string pl, string nick)
        {
            ID = int.Parse(id);

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

        public void Random()
        {
            Random r = new Random();
            for (int i = 0; i < 7; i++)
            {
                stack[i] = (char)(65 + r.Next(26));
            }

            Program.FormGame.Stack_DataGridView.Rows.Clear();
            Program.FormGame.Stack_DataGridView.Rows.Add(stack[0], stack[1], stack[2], stack[3], stack[4], stack[5], stack[6]);
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
            }
        }

    }
}
