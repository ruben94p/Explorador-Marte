using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Explorador_Marte
{
    public partial class Inicio : Form
    {
        private int i = 1;

        public Inicio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int exploradores = (int)Math.Floor(numericUpDown1.Value);
            int rocas = (int)Math.Floor(numericUpDown2.Value);
            int x = (int)Math.Floor(numericUpDown3.Value);
            int y = (int)Math.Floor(numericUpDown4.Value);
            int capacidad = (int)Math.Floor(numericUpDown5.Value);
            bool co_op = checkBox1.Checked;
            Marte m = new Marte(i, exploradores, rocas, x, y, capacidad, co_op);
            m.Show();
            i++;
        }
    }
}
