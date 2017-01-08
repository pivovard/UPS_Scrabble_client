using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPS_Scrabble_client
{
    public class Game
    {
        public string[] players;

        public char[][] field;
        public char[] stack;

        public int score = 0;

        public string turn = "";


        public Game(string pl)
        {
            players = pl.Split(';');

            field = new char[15][];
            stack = new char[7];

            for (int i = 0; i < 15; i++)
            {
                field[i] = new char[15];
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

    }
}
