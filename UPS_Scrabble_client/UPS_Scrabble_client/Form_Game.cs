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

        int score = 0;

        int c = 0;


        public Form_Game()
        {
            InitializeComponent();
            
            init();
            Random();
        }

        private void init()
        {
            field = new char[15][];
            stack = new char[7];

            for(int i = 0; i < 15; i++)
            {
                field[i] = new char[15];
                Field_DataGridView.Rows.Add();
            }

            Field_DataGridView.Rows[7].Cells[7].Style.BackColor = Color.Khaki;
        }


        private void Random()
        {
            Random r = new Random();
            for (int i = 0; i < 7; i++)
            {
                stack[i] = (char)(65 + r.Next(26));
            }

            Stack_DataGridView.Rows.Clear();
            Stack_DataGridView.Rows.Add(stack[0], stack[1], stack[2], stack[3], stack[4], stack[5], stack[6]);
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

        private void Stack_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            c = e.ColumnIndex;
        }

        private void Field_DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (stack[c] == '*') return;

            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Khaki;
            Field_DataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = stack[c];

            score++;

            stack[c] = '*';
            Stack_DataGridView.Rows[0].Cells[c].Value = "";
        }
    }
}
