using System;
using System.Drawing;

namespace Genetic_Maze
{
    public class Individual
    {
        public int Fitness { get; set; }
        public int[] Genes { get; set; } // 0 — вверх, 1 — вправо, 2 — вниз, 3 — влево
        public Point CurrentPosition { get; private set; }
        public Point PreviousPosition { get; private set; }
        public int ChromosomeLength => Genes.Length;
        public bool IsAlive { get; private set; }
        public bool HasSolved { get; private set; }
        public Point[] Path { get; set; }
        public int PathLength { get; set; }
        public IndividualSprite Sprite { get; set; }
        

        public Individual(int chromosomeLength)
        {
            Fitness = 0;
            IsAlive = true;
            HasSolved = false;
            Genes = new int[chromosomeLength];
            Path = new Point[chromosomeLength];
            PathLength = 0;
        }

        public void RandomizeGenes(int seed)
        {
            Random random = new Random(seed);
            for (int i = 0; i < ChromosomeLength; i++)
            {
                Genes[i] = random.Next(4);
            }
        }

        public void SetPosition(Point position)
        {
            PreviousPosition = new Point(CurrentPosition.X, CurrentPosition.Y);
            CurrentPosition = new Point(position.X, position.Y);
            Path[PathLength] = new Point(CurrentPosition.X, CurrentPosition.Y);
            PathLength++;
        }
        
        public void AddFitness(int value)
        {
            Fitness += value;
        }

        public void Die()
        {
            IsAlive = false;
        }

        public void Win()
        {
            HasSolved = true;
        }
    }
}
