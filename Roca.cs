using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Explorador_Marte
{
    public class Roca
    {

        public Point position { get; set; }
        public bool active;

        public Roca(Point position)
        {
            this.position = position;
            active = true;
        }

        public bool recoger()
        {
            lock (this)
            {
                if (active)
                {
                    active = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
