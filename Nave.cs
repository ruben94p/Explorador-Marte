using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Explorador_Marte
{
    public class Nave
    {
        public PointF position { get; private set; }
        public int rocas = 0;

        public Nave(PointF position)
        {
            this.position = position;
        }

    }
}
