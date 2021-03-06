﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace genetico
{
    class Mochila
    {

        Individuo objetos;
        Opcion combinaciones;
        Padre padres; //variable que contendrá la lista de padres
        ElArca mejores; //variable que contendrá la lista de los resultados

        double capacidad;
        String respuesta;

        private static Mochila singleton = new Mochila();

        private Mochila()
        {
            objetos = null;
            combinaciones = null;
            capacidad = 0;
            respuesta = "";
        }

        public static Mochila getInstante()
        {
            return singleton;
        }

        public Individuo get_inicio()
        {
            return objetos;
        }

        public bool check_respuesta()
        {
            if (respuesta != "")
                return true;
            else
                return false;
        }

        public bool check_capacidad()
        {
            if (capacidad != 0)
                return true;
            else
                return false;
        }

        public bool check_inicio()
        {
            if (objetos != null)
                return true;
            else
                return false;
        }

        public void set_ganancias(String ganancias)
        {
            Individuo aux;

            if (objetos == null)
            {
                objetos = new Individuo();
                objetos.ganancia = Int32.Parse(ganancias);
            }
            else
            {

                aux = objetos;

                while (aux.siguiente != null)
                    aux = aux.siguiente;

                aux.siguiente = new Individuo();
                aux.siguiente.ganancia = Int32.Parse(ganancias);

            }

        }

        public void set_peso(String peso, int num)
        {
            Individuo aux = objetos;
            int cont = 1;

            for (; cont <= num; cont++)
                aux = aux.siguiente;

            if (aux != null)
            {
                aux.peso = Double.Parse(peso);
            }

        }

        public void set_capacidad(double _capacidad)
        {
            capacidad = _capacidad;
        }

        public void set_respuesta(String _respuesta)
        {
            respuesta = _respuesta;
        }

        public void iniciar_poblacion(int t_poblacion) //realizar paso uno
        {
            Random rand = new Random(); //objeto para generar numeros aleatorios
            String cuerpo = ""; //cuerpo del individuo (Grupo de 8 bits)

            int digito; //entero que estara entre 0 o 1

            Individuo cont = objetos;

            int cont_obj = 0;

            while (cont != null)
            {
                cont_obj++;
                cont = cont.siguiente;
            }


            for (int i = 0; i < t_poblacion; i++) //for para generar a todos los individuos
            {
                for (int j = 0; j < cont_obj; j++)
                {
                    digito = rand.Next(0, 2); //generar numero que sera 0 o 1
                    cuerpo += digito.ToString();//añadir el bit al cuerpo
                }

                Opcion nuevo = new Opcion(cuerpo); // generar individuo

                if (combinaciones == null) //comprobar si es el primer elemento
                {
                    combinaciones = nuevo;
                }
                else
                {
                    Opcion aux = combinaciones; //evitar que la lista se vea afectada

                    while (aux.siguiente != null)
                        aux = aux.siguiente; //conseguimos el ultimo elemento de la lista

                    aux.siguiente = nuevo; //guardamos el individuo
                }

                cuerpo = ""; //vaciamos el cuerpo para el siguiente individuo

            }
        }

        public void calculo_inicial()
        {
            double peso = 0;
            double ganancia = 0;
            double best_ganancia = 0;
            double p = 0;

            Opcion best = null;
            Opcion aux_o = combinaciones;
            Individuo objeto;
            ElArca aux_m = mejores;
            String aux = "";
            

            while (aux_o != null)
            {
                aux = aux_o.combinacion;
                objeto = objetos;

                for (int i = 0; i < aux.Length - 1; i++)
                {
                    peso += Int32.Parse("" + aux[i]) * objeto.peso;
                    ganancia += Int32.Parse("" + aux[i]) * objeto.ganancia;
                    objeto = objeto.siguiente;

                    if ((objeto.peso / objeto.ganancia) >= p)
                        p = (objeto.peso / objeto.ganancia);

                }

                aux_o.peso_total = peso;
                aux_o.ganancia_total = ganancia;
                aux_o.pmax = p;

                if(ganancia > best_ganancia && peso <= capacidad)
                {
                    best_ganancia = ganancia;
                    best = aux_o;
                }

                //reiniciar;
                ganancia = 0;
                peso = 0;
                p = 0;
                aux_o = aux_o.siguiente;
            }
            if(best != null)
            {
                if (mejores == null)
                    mejores = new ElArca(best.combinacion, best.ganancia_total, best.peso_total);
                else
                {
                    aux_m = mejores;

                    while (aux_m.siguiente != null)
                        aux_m = aux_m.siguiente;

                    aux_m.siguiente = new ElArca(best.combinacion, best.ganancia_total, best.peso_total);
                }
            }
        }

        public void cal_final() //calculamos fnom y acumulado
        {
            double facum = 0; //varibale donde se almacenará el acumulado respectivo del individuo

            Opcion calculo = combinaciones; //auxiliar para asigniar ultimos valores

            while (calculo != null)
            {
                calculo.fnom = calculo.ganancia_total / get_sumatoria(); //realizar fnom mediante formula por individuo
                facum += calculo.fnom; //calcular acumulado 
                calculo.acumulado = facum; //asignar el acumulado al respectivo individuo

                calculo = calculo.siguiente;
            }
        }

        public double get_sumatoria()
        {
            Opcion aux = combinaciones;
            double sum = 0;

            while (aux != null)
            {
                sum += aux.ganancia_total;
                aux = aux.siguiente;
            }

            return Math.Round(sum, 2);
        } //calculo de la sumatoria de aptitudes

        public void validar_ceros()
        {

            Opcion aux = combinaciones;

            while(aux != null)
            {
                if(aux.peso_total > capacidad)
                {
                    String cuerpo = aux.combinacion;
                    String aux2 = "";
                    for (int i = 0; i < cuerpo.Length - 1; i++)
                    {
                        aux2 += '0';
                    }
                    aux.combinacion = aux2;
                }

                aux = aux.siguiente;
            }

        }

        public void validar_uno()
        {
            Opcion aux = combinaciones;
            Individuo aux_o = objetos;

            while (aux != null)
            {
                if (aux.peso_total > capacidad)
                {
                    String cuerpo = aux.combinacion;
                    double pen = 0;
                    for (int i = 0; i < cuerpo.Length - 1; i++)
                    {
                        if(cuerpo[i] == '1')
                        {
                            pen += (aux_o.peso - capacidad);
                        }
                        aux_o = aux_o.siguiente;
                    }

                    pen *= aux.pmax;

                    aux.pen = pen;
                    
                }

                aux_o = objetos;
                aux = aux.siguiente;
            }
        }

        #region Denise

        private double[] generarNumerosAleatorios(int size)
        {
            double[] array = new double[size];
            Random random = new Random();

            for (int i = 0; i < array.Length; i++)
                array[i] = Math.Round(random.NextDouble(), 2);

            return array;
        }

        public void generarLosNuevosPadresDeLaPatria(int poblacion)
        {
            if (combinaciones == null)
                Console.WriteLine("No hay nada");
            else
            {
                double[] valores = generarNumerosAleatorios(poblacion);

                Opcion aux = combinaciones;
                Padre auxPadre, nuevo;

                for (int i = 0; i < poblacion; i++)
                {
                    while (aux != null)
                    {
                        if (aux.acumulado > valores[i])
                        {
                            if (padres == null)
                                padres = new Padre(aux.combinacion);
                            else
                            {
                                nuevo = new Padre(aux.combinacion);
                                auxPadre = padres;
                                while (auxPadre.siguiente != null)
                                    auxPadre = auxPadre.siguiente;
                                auxPadre.siguiente = nuevo;
                            }

                            break;
                        }
                        aux = aux.siguiente;
                    }
                    aux = combinaciones;
                }
                auxPadre = padres;
            }
        }

        #endregion

        public void cruze(double p_cruce, int poblacion) //metodo para realizar el cruce
        {
            //Console.WriteLine("\n\t\t\t\t ----Cruce: " + p_cruce + " | Nueva poblacion---- ");

            Padre auxPadre = padres,
                  padre1, padre2;

            Opcion aux,
                nuevo; //nodo para crear a los hijos

            String[] a_cambiar = new String[2]; //string para almacenar los bit a cambiar;
            Random aleatorio = new Random();

            double[] chance = generarNumerosAleatorios(poblacion / 2);
            int uno, dos; //numeros para indice de corte

            combinaciones = null;

            for (int i = 0; i < chance.Length; i++)
            {
                if (auxPadre != null)
                {
                    //asignacion de la pareja
                    padre1 = auxPadre;
                    auxPadre = auxPadre.siguiente;
                    padre2 = auxPadre;

                    if (chance[0] <= p_cruce) //se va a realizar el cruce
                    {
                        uno = aleatorio.Next(0, 8);
                        dos = aleatorio.Next(0, 8);

                        if (uno < dos)
                        {
                            //obtener bits de los padres
                            for (int j = 0; j < auxPadre.bite.Length - 1; j++)
                            {
                                if (j >= uno && j <= dos)
                                {
                                    a_cambiar[1] += padre1.bite[j];
                                    a_cambiar[0] += padre2.bite[j];
                                }
                                else
                                {
                                    a_cambiar[0] += padre1.bite[j];
                                    a_cambiar[1] += padre2.bite[j];
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < auxPadre.bite.Length - 1; j++)
                            {
                                if (j <= dos || j >= uno)
                                {
                                    a_cambiar[1] += padre1.bite[j];
                                    a_cambiar[0] += padre2.bite[j];
                                }

                                else
                                {
                                    a_cambiar[0] += padre1.bite[j];
                                    a_cambiar[1] += padre2.bite[j];
                                }
                            }
                        }

                        if (combinaciones == null)
                        {
                            nuevo = new Opcion(a_cambiar[0]);
                            combinaciones = nuevo;
                            nuevo.siguiente = new Opcion(a_cambiar[1]);
                        }
                        else
                        {
                            aux = combinaciones;

                            while (aux.siguiente != null)
                                aux = aux.siguiente;

                            nuevo = new Opcion(a_cambiar[0]);
                            aux.siguiente = nuevo;
                            nuevo.siguiente = new Opcion(a_cambiar[1]);
                        }
                    }
                    else //no hay cruce
                    {
                        if (combinaciones == null)
                        {
                            nuevo = new Opcion(padre1.bite);
                            combinaciones = nuevo;
                            nuevo.siguiente = new Opcion(padre2.bite);
                        }
                        else
                        {
                            aux = combinaciones;

                            while (aux.siguiente != null)
                                aux = aux.siguiente;

                            nuevo = new Opcion(padre1.bite);
                            aux.siguiente = nuevo;
                            nuevo.siguiente = new Opcion(padre2.bite);
                        }
                    }

                    a_cambiar[0] = "";
                    a_cambiar[1] = "";
                    auxPadre = auxPadre.siguiente;
                }
            }

            padres = null;

        }

        public void mute(double mutacion) //metodo para realizar la mutacion
        {
            Opcion aux = combinaciones;
            Random aleatorio = new Random();

            String cuerpo = "";

            int total_m = 0;
            double rand;
            char bit;

            while (aux != null)
            {
                for (int i = 0; i < aux.combinacion.Length; i++)
                {
                    bit = aux.combinacion[i];
                    rand = aleatorio.NextDouble();
                    if (rand < mutacion)
                    {
                        if (bit == '1')
                            cuerpo += '0';
                        else
                            cuerpo += '1';

                        total_m++;
                    }
                    else
                        cuerpo += bit;
                }
                aux.combinacion = cuerpo;
                cuerpo = "";
                aux = aux.siguiente;
            }
        }

        public string imprimir_ganadores()
        {
            ElArca aux = mejores;
            double peso = 0;
            double ganancia = 0;

            if (aux != null)
            {
                double mejor = aux.ganancia;
                String final = "";

                while (aux != null)
                {
                    if (aux.ganancia >= mejor && aux.peso <= capacidad)
                    {
                        final = aux.binario;
                        peso = aux.peso;
                        ganancia = aux.ganancia;
                    }

                    aux = aux.siguiente;
                }
                if (final == respuesta)
                    return "Se encontro la solucion establecida";
                else
                    return "Mejor combinacion encontrada:\n\n " + final + "\n Peso: " + peso + "\n Ganancia: " + ganancia;
            }
            else
                return "Sin combinaciones";
        }

    }
}
