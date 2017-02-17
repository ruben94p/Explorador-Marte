using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace Explorador_Marte
{

    public enum Estado
    {
        HACIENDO_NADA,
        ENCONTRO_ROCA,
        RECOLECTAR_MORONA,
        IR_A_NAVE
    }

    public class Explorador
    {
        private readonly Espacio espacio;
        private readonly int seed;
        private readonly Random r;

        private static readonly float distanciaBusqueda = 90;
        private static readonly float distanciaMorona = 180;
        private static readonly float velocidad = 4;
        private static readonly float cercania = 9;
        private bool co_op = false;
        private Stopwatch stopwatch;
        private Roca roca = null;
        private Morona morona = null;

        public PointF position;
        private int capacidad;
        public int carga;
        private int it = 0;
        public Estado estado;

        private bool lleno
        {
            get
            {
                return capacidad == carga;
            }
        }

        public void cargarRoca()
        {
            carga++;
            stopwatch.Reset();
            stopwatch.Start();
            if (co_op)
            {
                if (lleno)
                {
                    lock (espacio.moronas)
                    {
                        espacio.moronas.Add(new Morona(position));
                    }
                }
            }
        }

        public Explorador(Espacio espacio, PointF position, int seed, int capacidad, bool co_op)
        {
            this.espacio = espacio;
            this.position = position;
            this.seed = seed;
            this.capacidad = capacidad;
            this.carga = 0;
            this.co_op = co_op;
            this.r = new Random(seed);
            estado = Estado.HACIENDO_NADA;
            stopwatch = new Stopwatch();
        }

        public void iniciar()
        {
            if (co_op)
            {
                iniciarCo_op();
            }else
            {
                iniciarIndividual();
            }
        }

        public void iniciarIndividual()
        {
            while (true) {
                switch (estado)
                {
                    case Estado.HACIENDO_NADA:
                        moverseAlAzar();
                        if(carga != 0 && stopwatch.ElapsedMilliseconds >= 30000)
                        {
                            irANave();
                        }
                        buscarRocas();
                        break;
                    case Estado.ENCONTRO_ROCA:
                        irARoca();
                        break;
                    case Estado.IR_A_NAVE:
                        irANave();
                        break;
                }
                Thread.Sleep(200);
            }
        }

        public void iniciarCo_op()
        {
            while (true)
            {
                switch (estado)
                {
                    case Estado.HACIENDO_NADA:
                        moverseAlAzar();
                        buscarMoronas();
                        if (carga != 0 && stopwatch.ElapsedMilliseconds >= 30000)
                        {
                            irANave();
                        }
                        buscarRocas();
                        break;
                    case Estado.ENCONTRO_ROCA:
                        buscarMoronas();
                        irARoca();
                        break;
                    case Estado.RECOLECTAR_MORONA:
                        irAMorona();
                        break;
                    case Estado.IR_A_NAVE:
                        irANave();
                        it++;
                        if(it == 5)
                        {
                            lock (espacio.moronas)
                            {
                                espacio.moronas.Add(new Morona(position));
                            }
                        }
                        break;
                }
                Thread.Sleep(200);
            }
        }

        public void buscarRocas()
        {
            double min = -1;
            foreach(Roca roca in espacio.rocas)
            {
                if (roca.active)
                {
                    if(min == -1)
                    {
                        min = Math.Sqrt(Math.Pow(roca.position.X - position.X, 2) + Math.Pow(roca.position.Y - position.Y, 2));
                    }else
                    {
                        min = Math.Min(min, Math.Sqrt(Math.Pow(roca.position.X - position.X, 2) + Math.Pow(roca.position.Y - position.Y, 2)));
                    }
                    if (Math.Sqrt(Math.Pow(roca.position.X - position.X ,2) + Math.Pow(roca.position.Y - position.Y, 2))<= distanciaBusqueda && min == Math.Sqrt(Math.Pow(roca.position.X - position.X, 2) + Math.Pow(roca.position.Y - position.Y, 2)))
                    {
                        estado = Estado.ENCONTRO_ROCA;
                        this.roca = roca;
                    }
                }
            }
        }

        public void buscarMoronas()
        {
            lock (espacio.moronas)
            {
                double min = -1;
                foreach (Morona morona in espacio.moronas)
                {
                    if (morona.active)
                    {
                        if (min == -1)
                        {
                            min = Math.Sqrt(Math.Pow(morona.position.X - position.X, 2) + Math.Pow(morona.position.Y - position.Y, 2));
                        }
                        else
                        {
                            min = Math.Min(min, Math.Sqrt(Math.Pow(morona.position.X - position.X, 2) + Math.Pow(morona.position.Y - position.Y, 2)));
                        }
                        if (Math.Sqrt(Math.Pow(morona.position.X - position.X, 2) + Math.Pow(morona.position.Y - position.Y, 2)) <= distanciaBusqueda && min == Math.Sqrt(Math.Pow(morona.position.X - position.X, 2) + Math.Pow(morona.position.Y - position.Y, 2)))
                        {
                            estado = Estado.RECOLECTAR_MORONA;
                            this.morona = morona;
                        }
                    }
                }
            }
        }

        public void irARoca()
        {
            if (roca.active)
            {
                if (position.X <= roca.position.X + cercania && position.X >= roca.position.X - cercania && position.Y <= roca.position.Y + cercania && position.Y >= roca.position.Y - cercania)
                {
                    if (roca.recoger())
                    {
                        cargarRoca();
                        it = 0;
                        if (lleno)
                        {
                            estado = Estado.IR_A_NAVE;
                        }
                        else
                        {
                            estado = Estado.HACIENDO_NADA;
                        }
                    }else
                    {
                        estado = Estado.HACIENDO_NADA;
                    }
                    roca = null;
                }
                else
                {
                    if (position.X < roca.position.X)
                    {
                        position.X += velocidad;
                    }
                    if (position.X > roca.position.X)
                    {
                        position.X -= velocidad;
                    }
                    if (position.Y < roca.position.Y)
                    {
                        position.Y += velocidad;
                    }
                    if (position.Y > roca.position.Y)
                    {
                        position.Y -= velocidad;
                    }
                }
            }else
            {
                //Otro la recogio
                estado = Estado.HACIENDO_NADA;
                roca = null;
            }
        }

        public void irAMorona()
        {
            if (morona.active)
            {
                if (position.X <= morona.position.X + cercania && position.X >= morona.position.X - cercania && position.Y <= morona.position.Y + cercania && position.Y >= morona.position.Y - cercania)
                {
                    morona.recoger();
                    estado = Estado.HACIENDO_NADA;
                    morona = null;
                }
                else
                {
                    if (position.X < morona.position.X)
                    {
                        position.X += velocidad;
                    }
                    if (position.X > morona.position.X)
                    {
                        position.X -= velocidad;
                    }
                    if (position.Y < morona.position.Y)
                    {
                        position.Y += velocidad;
                    }
                    if (position.Y > morona.position.Y)
                    {
                        position.Y -= velocidad;
                    }
                }
            }
            else
            {
                //Otro la recogio
                estado = Estado.HACIENDO_NADA;
                morona = null;
            }
        }

        public void irANave()
        {
            if (position.X <= espacio.nave.position.X + cercania && position.X >= espacio.nave.position.X - cercania && position.Y <= espacio.nave.position.Y + cercania && position.Y >= espacio.nave.position.Y - cercania)
            {
                estado = Estado.HACIENDO_NADA;
                espacio.nave.rocas += carga;
                carga = 0;
            }
            else
            {
                if (position.X < espacio.nave.position.X)
                {
                    position.X += velocidad;
                }
                if (position.X > espacio.nave.position.X)
                {
                    position.X -= velocidad;
                }
                if (position.Y < espacio.nave.position.Y)
                {
                    position.Y += velocidad;
                }
                if (position.Y > espacio.nave.position.Y)
                {
                    position.Y -= velocidad;
                }
            }
        }

        public void moverse(float x, float y)
        {
            position.X += x;
            position.Y += y;
        }

        public void moverseAlAzar()
        {
            float x = (velocidad * r.Next(-2, 3));
            float y = (velocidad * r.Next(-2, 3));
            if(position.X + x > espacio.Width + 10 || position.X + x < -10)
            {
                x *= -1;
            }
            if (position.Y + y > espacio.Height + 10 || position.Y + y < -10)
            {
                y *= -1;
            }
            moverse(x, y);
        }


    }
}
