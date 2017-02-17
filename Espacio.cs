using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Explorador_Marte
{
    public class Espacio
    {
        private Form form;
        public float Width { get; set; }
        public float Height { get; set; }
        public PointF center { get; set; }
        public List<Thread> threads = new List<Thread>();

        public Nave nave;
        public List<Explorador> exploradores;
        public List<Roca> rocas;
        public List<Morona> moronas;

        public Espacio(Form form, int n_exploradores, int n_rocas, int capacidad, bool co_op)
        {
            this.form = form;
            Width = form.Size.Width - 20;
            Height = form.Size.Height - 20;
            center = new PointF(Width / 2, Height / 2);
            nave = new Nave(center);
            exploradores = new List<Explorador>();
            rocas = new List<Roca>();
            moronas = new List<Morona>();
            Random r = new Random();
            for(int i = 0; i < n_exploradores; i++)
            {
                exploradores.Add(new Explorador(this, center, r.Next(), capacidad, co_op));
            }
            for (int i = 0; i < n_rocas; i++)
            {
                rocas.Add(new Roca(new Point(r.Next(20, form.Size.Width-20), r.Next(20, form.Size.Height-20))));
            }
            iniciar();
        }

        public bool vacio
        {
            get
            {
                return nave.rocas == rocas.Count;
            }
        }

        public void iniciar()
        {
            foreach(Explorador e in exploradores)
            {
                Thread t = new Thread(e.iniciar);
                t.Start();
                threads.Add(t);
            }
            Thread d = new Thread(draw);
            d.Start();
        }

        private void draw()
        {
            do
            {
                if (form.IsDisposed)
                {
                    foreach(Thread t in threads)
                    {
                        try
                        {
                            t.Abort();
                        }
                        catch
                        {

                        }
                    }
                    return;
                }
                else
                {
                    try
                    {
                        Thread.Sleep(200);
                        form.Invoke((MethodInvoker)delegate
                        {
                            Graphics g = form.CreateGraphics();
                            g.Clear(form.BackColor);
                            g.DrawEllipse(new Pen(Color.Red, 3), nave.position.X, nave.position.Y, 3, 3);
                            foreach (Explorador e in exploradores)
                            {
                                g.DrawEllipse(new Pen(Color.Blue, 3), e.position.X, e.position.Y, 2, 2);
                                if(e.carga != 0)
                                {
                                    g.DrawString(e.carga.ToString(), new Font(FontFamily.GenericSerif, 8), Brushes.Black, e.position.X + 10, e.position.Y - 10);
                                }
                            }
                            foreach (Roca roca in rocas)
                            {
                                if (roca.active)
                                {
                                    g.DrawEllipse(new Pen(Color.Gray, 3), roca.position.X, roca.position.Y, 3, 3);
                                }
                            }
                            foreach (Morona morona in moronas)
                            {
                                if (morona.active)
                                {
                                    g.DrawEllipse(new Pen(Color.Black, 3), morona.position.X, morona.position.Y, 3, 3);
                                }
                            }
                            g.Dispose();
                        });
                    }catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            } while (true);
        }

    }
}
