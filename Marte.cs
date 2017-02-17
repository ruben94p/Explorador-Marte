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
    public partial class Marte : Form
    {
        private Espacio espacio;
        public Marte()
        {
            InitializeComponent();
            this.Close();
        }

        public Marte(int n, int exploradores, int rocas, int x, int y, int capacidad, bool co_op)
        {
            InitializeComponent();
            this.Text = "Ambiente " + n;
            this.Size = new Size(x, y);
            espacio = new Espacio(this, exploradores, rocas, capacidad, co_op);
        }
    }
}
