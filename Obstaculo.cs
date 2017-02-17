using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Explorador_Marte
{
    public class Obstaculo
    {
        public PointF position;
        public float r;

        public Obstaculo(PointF position, float r)
        {
            this.position = position;
            this.r = r;
        }


    }
}
