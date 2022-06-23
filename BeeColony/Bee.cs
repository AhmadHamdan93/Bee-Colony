using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeeColony
{
    internal class Bee
    {
        double[,] population;
        int food;
        int iteration;
        int limit;
        int D;
        int[] trail;
        double[] probability;
        double[] OptimumSol;
        Random rand;
        int ub = 1;
        int lb = -1;

        public Bee(double[,] solutions)
        {
            food = solutions.GetLength(0);
            D = solutions.GetLength(1) - 1;
            iteration = 500;
            limit = food * D / 2;
            trail = new int[food];
            population = solutions;
            probability = new double[food];
            OptimumSol = new double[D + 1];
            rand = new Random();
        }

        public void Search()
        {
            SaveOptimumSol(0);
            SelectOptimumSolution();
            for (int iter = 0; iter < iteration; iter++)
            {
                WorkerBee();
                findProbability();
                LookerBee();
                SelectOptimumSolution();
                ScouterBee();
            }

            for (int i = 0; i < D; i++)
            {
                Console.WriteLine((i + 1) + " - " + OptimumSol[i]);
            }
            Console.WriteLine("Score Solution is: " + OptimumSol[D]);
        }

        public void LookerBee()
        {
            int i, t;
            i = 0;
            t = 0;
            /*onlooker Bee Phase*/
            while (t < food)
            {

                double r = rand.NextDouble();
                if (r < probability[i]) /*choose a food source depending on its probability to be chosen*/
                {
                    t++;
                    //--------------------------

                    int neighbor = rand.Next(food);
                    while (neighbor == i)
                    {
                        neighbor = rand.Next(food);
                    }
                    // detect current sol and new sol
                    double[] newRowSolution = LocalSearch(i, neighbor);
                    if (newRowSolution[D] < population[i, D])
                    {
                        replaceRow(newRowSolution, i);
                        trail[i] = 0;
                    }
                    else
                    {
                        trail[i] += 1;
                    }
                    //---------------------------
                }
                i++;
                if (i == food)
                    i = 0;
            }/*while*/

            /*end of onlooker bee phase     */
        }

        public void WorkerBee()
        {
            /*start of employed bee phase*/
            for (int i = 0; i < food; i++)
            {
                int neighbor = rand.Next(food);
                while (neighbor == i)
                {
                    neighbor = rand.Next(food);
                }
                // detect current sol and new sol
                double[] newRowSolution = LocalSearch(i, neighbor);
                if (newRowSolution[D] < population[i, D])
                {
                    replaceRow(newRowSolution, i);
                    trail[i] = 0;
                }
                else
                {
                    trail[i] += 1;
                }
            }
            /*end of employed bee phase*/
        }

        public void replaceRow(double[] sol, int idx)
        {
            for (int j = 0; j < D + 1; j++)
            {
                population[idx, j] = sol[j];
            }
        }

        public double[] LocalSearch(int currentIDX, int neighbor)
        {
            double[] newSol = new double[D + 1];
            for (int i = 0; i < D; i++)
            {
                double randomNumber = lb + rand.NextDouble() * (ub - lb);
                newSol[i] = population[currentIDX, i] + randomNumber * (population[currentIDX, i] - population[neighbor, i]);
            }
            // Evaluate solution
            newSol[D] = Evaluate(newSol);
            return newSol;
        }

        public void ScouterBee()
        {
            for (int i = 0; i < food; i++)
            {
                if (trail[i] >= limit)
                {
                    initSol(i);
                    trail[i] = 0;
                    // evaluate new solution and save it
                    // Evaluate solution
                    double[] row = fetchRow(i);
                    population[i, D] = Evaluate(row);
                }
            }
        }

        public double[] fetchRow(int idx)
        {
            double[] row = new double[D + 1];
            for (int j = 0; j < D + 1; j++)
            {
                row[j] = population[idx, j];
            }
            return row;
        }

        public void initSol(int idx)
        {
            for (int i = 0; i < D; i++)
            {
                population[idx, i] = lb + rand.NextDouble() * (ub - lb);
            }
        }

        public void SelectOptimumSolution()
        {
            double minFx = population[0, D];
            int minIndex = 0;
            if (minFx < OptimumSol[D])
                SaveOptimumSol(minIndex);
            for (int i = 1; i < food; i++)
            {
                if (population[i, D] < minFx)
                {
                    minFx = population[i, D];
                    minIndex = i;
                    if (minFx < OptimumSol[D])
                        SaveOptimumSol(minIndex);
                }
            }
        }

        public void SaveOptimumSol(int idx)
        {
            for (int i = 0; i < D + 1; i++)
            {
                OptimumSol[i] = population[idx, i];
            }
        }

        public void findProbability()
        {
            double sumFX = 0.0;
            for (int i = 0; i < food; i++)
            {
                sumFX += population[i, D];
            }
            for (int i = 0; i < food; i++)
            {
                probability[i] = population[i, D] / sumFX;
            }
        }

        public double Evaluate(double[] sol)
        {
            double result = 0.0;
            for (int i = 0; i < D; i++)
            {
                result += Fx(sol[i]);
            }
            return result;
        }

        public double Fx(double value)
        {
            return value * value;
        }
    }
}
