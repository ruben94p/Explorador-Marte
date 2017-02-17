using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Explorador_Marte
{
    public class Morona
    {
        public PointF position;
        public bool active;

        public Morona(PointF position)
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
                }else
                {
                    return false;
                }
            }
        }
    }
}
