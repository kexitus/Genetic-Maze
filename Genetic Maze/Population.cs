using System;
using System.Drawing;

namespace Genetic_Maze
{
    public class Population
    {
        public Individual[] Individuals { get; set; }

        public Population(int populationSize)
        {
            Individuals = new Individual[populationSize];
        }

        public Population(int populationSize, int chromosomeLength, Point startPosition)
        {
            Individuals = new Individual[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                Individuals[i] = new Individual(chromosomeLength);
                Individuals[i].RandomizeGenes(i + new Random().Next());
                Individuals[i].SetPosition(startPosition);
            }
        }

        public Individual GetFittestIndividual()
        {
            Individual fittest = Individuals[0];

            for (int i = 1; i < Individuals.Length; i++)
            {
                if (Individuals[i].Fitness > fittest.Fitness)
                {
                    fittest = Individuals[i];
                }
            }

            return fittest;
        }

        public Individual[] GetSeveralFittestIndividuals(int amount)
        {
            Individual[] fittestIndividuals = new Individual[amount];
            Individual fittest;
            bool repeatedIndividual;


            // Individual[] tempIndividuals = new Individual[Individuals.Length];
            //
            // for (int i = 0; i < Individuals.Length; i++)
            // {
            //     tempIndividuals[i] = Individuals[i];
            // }
            //
            //
            // for (int i = 0; i < amount; i++)
            // {
            //     fittest = tempIndividuals[0];
            //     int fittestIndex = 0;
            //     
            //     for (int j = 1; j < Individuals.Length; j++)
            //     {
            //         if (tempIndividuals[j] != null && tempIndividuals[j].Fitness > fittest.Fitness)
            //         {
            //             fittest = tempIndividuals[j];
            //             fittestIndex = j;
            //         }
            //     }
            //     
            //     fittestIndividuals[i] = fittest;
            //     tempIndividuals[fittestIndex] = null;
            // }
            

            // Перебор элементов fittestIndividuals
            for (int i = 0; i < amount; i++)
            {
                fittest = Individuals[0];
                
                // Перебор элементов Individuals
                for (int j = 1; j < Individuals.Length; j++)
                {
                    repeatedIndividual = false;
                    if (Individuals[j].Fitness >= fittest.Fitness)
                    {
                        // Перебор элементов fittestIndividuals для отсеивания повторений
                        for (int k = 0; k < amount; k++)
                        {
                            if (Individuals[j] == fittestIndividuals[k])
                            {
                                repeatedIndividual = true;
                                break;
                            }
                        }

                        if (!repeatedIndividual)
                        {
                            fittest = Individuals[j];
                        }
                    }
                }

                fittestIndividuals[i] = fittest;
            }
            
            return fittestIndividuals;
        }
    }
}