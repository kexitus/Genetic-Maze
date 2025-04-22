using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Genetic_Maze
{
    public class GeneticAlgorithm
    {
        private Control.ControlCollection _controls;
        private Size _spriteSize;
        private Point _spriteStartLocation;
        
        private const int RewardSolved = 1000;
        private const int RewardAlreadySolved = 50;
        private const int RewardExploration = 10;
        private const int PenaltyCollision = -10;
        private const int PenaltyAlreadyDeadEnd = -50;
        private const int PenaltyDeadEnd = -1000;

        private Maze _maze;
        private int _populationSize;
        private double _mutationRate;
        private int _elitesCount;
        private int _tournamentSize;
        private int _chromosomeLength;
        private Random _random;
        private bool _foundSolution;
        private bool _alreadySolved;
        private int _pathLength;

        public GeneticAlgorithm(Maze maze, int populationSize, double mutationRate, int elitesCount, int tournamentSize, Size cellSize, Control.ControlCollection controls)
        {
            _controls = controls;
            
            _spriteSize = new Size(Convert.ToInt32(cellSize.Width / 100.0 * 60),
                Convert.ToInt32(cellSize.Height / 100.0 * 60));
            int spriteOffsetX = Convert.ToInt32(_spriteSize.Width / 60 * 20);
            int spriteOffsetY = Convert.ToInt32(_spriteSize.Height / 60 * 20);
            _spriteStartLocation = new Point(21 + spriteOffsetX + maze.StartPosition.X * (spriteOffsetX * 2 + _spriteSize.Width), 
                21 + spriteOffsetY + maze.StartPosition.Y * (spriteOffsetY * 2 + _spriteSize.Height));
            
            
            _maze = maze;
            _populationSize = populationSize;
            _mutationRate = mutationRate;
            _elitesCount = elitesCount;
            _tournamentSize = tournamentSize;

            _chromosomeLength = _maze.Height * _maze.Width;

            _foundSolution = false;
            
            _random = new Random();
        }

        #region Simulation

            // Основная функция запуска симуляции
            public void Simulate(int minimumIterations, RichTextBox console)
            {
                console.Text = "";
                int iteration = 0;
                Population currentPopulation = new Population(_populationSize, _chromosomeLength, _maze.StartPosition);

                void LogSolution()
                {
                    if (_foundSolution)
                    {
                        string output = $"Итерация {iteration}: {currentPopulation.GetFittestIndividual().Fitness}\n";
                        output += "!!!Решение найдено\n";
    
                        Individual solver = currentPopulation.GetFittestIndividual();
                        string direction = "00000";
                        
                        output += "[";
                        for (int i = 0; i < _chromosomeLength; i++)
                        {
                            switch (solver.Genes[i])
                            {
                                case 0:
                                    direction = "down";
                                    break;
                                case 1:
                                    direction = "right";
                                    break;
                                case 2:
                                    direction = "up";
                                    break;
                                case 3:
                                    direction = "left";
                                    break;
                            }
                            output += direction + (i < _chromosomeLength - 1 ? ", " : "");
                        }
                    
                        output += "]\n";
    
                        console.Text = output + console.Text;
                    }
                }
                
                
                
                while ((!_foundSolution || iteration < minimumIterations || _chromosomeLength > _pathLength) && iteration < minimumIterations*2)
                {
                    //SpawnSprites(currentPopulation);
                    
                    CyclePopulation(currentPopulation);
                    
                    //RemoveSprites(currentPopulation);
                    
                    LogSolution();
                    
                    iteration++;
                    _foundSolution = false;
                    _alreadySolved = false;
                    
                    currentPopulation = EvolvePopulation(currentPopulation);
                }
                
            }
    
            // Выполнение цикла одной популяции
            public void CyclePopulation(Population population)
            {
                for (int i = 0; i < _chromosomeLength; i++)
                {
                    PopulationMove(i, population);

                    //AnimateMove(i, population);
                }

                for (int i = 0; i < _populationSize; i++)
                {
                    int distancePoints = Convert.ToInt32(-1 * Math.Sqrt((_maze.EndPosition.X - population.Individuals[i].CurrentPosition.X) * (_maze.EndPosition.X - population.Individuals[i].CurrentPosition.X)
                                                   + (_maze.EndPosition.Y - population.Individuals[i].CurrentPosition.Y) * (_maze.EndPosition.Y - population.Individuals[i].CurrentPosition.Y)));
                    population.Individuals[i].AddFitness(distancePoints);
                }
            }
    
            // Выполнение одного хода всеми индивидами
            public void PopulationMove(int turn, Population population)
            {
                for (int i = 0; i < _populationSize; i++)
                {
                    IndividualMove(turn, population.Individuals[i]);
                }
            }
    
            // Ход одного индивида
            private void IndividualMove(int turn, Individual individual)
            {
                // Пропуск мёртвых (в тупике)
                if (individual.IsAlive == false)
                {
                    individual.AddFitness(PenaltyAlreadyDeadEnd);
                    return;
                }
                // Пропуск уже решивших
                if (individual.HasSolved)
                {
                    individual.AddFitness(RewardAlreadySolved);
                    return;
                }

                Point direction = GetDirection(individual.Genes[turn]);
                Point nextPosition = new Point(individual.CurrentPosition.X + direction.X, individual.CurrentPosition.Y + direction.Y);
                
                // Проверка неверных ходов
                if (nextPosition.X < 0 || nextPosition.Y < 0 || nextPosition.X >= _maze.Width || nextPosition.Y >= _maze.Height || // Выход за пределы лабиринта
                    _maze[nextPosition.X, nextPosition.Y] == 1 ||                                                                  // Столкновение со стеной
                    (nextPosition.X == individual.PreviousPosition.X && nextPosition.Y == individual.PreviousPosition.Y))          // Шаг назад
                {
                    individual.AddFitness(PenaltyCollision);
                }
                else
                {
                    // Успешный сдвиг
                    individual.SetPosition(nextPosition);
                    individual.AddFitness(RewardExploration);
                    
                    /*// Анимация
                    void AnimateStep()
                    {
                        switch (individual.Genes[turn])
                        {
                            case 0:
                                individual.Sprite.MoveDown();
                                break;
                            case 1:
                                individual.Sprite.MoveRight();
                                break;
                            case 2:
                                individual.Sprite.MoveUp();
                                break;
                            case 3:
                                individual.Sprite.MoveLeft();
                                break;
                        }
                    }
                    
                    new Thread(AnimateStep).Start();*/
    
                    // Проверка на финиш или тупик
                    CheckPosition(individual, direction);

                    if (_foundSolution && !_alreadySolved)
                    {
                        _chromosomeLength = turn + 1;
                        _alreadySolved = true;
                        _pathLength = individual.PathLength;
                    }
                }
            }
    
            // Проверка конечной позиции индивида
            private void CheckPosition(Individual individual, Point previousMoveDirection)
            {
                // Проверка финиша
                if (individual.CurrentPosition.X == _maze.EndPosition.X &&
                    individual.CurrentPosition.Y == _maze.EndPosition.Y)
                {
                    individual.Win();
                    individual.AddFitness(RewardSolved);
                    _foundSolution = true;
                    return;
                }
                    
                // Проверка тупика
                Point checkForward = new Point(individual.CurrentPosition.X + previousMoveDirection.X, 
                                                individual.CurrentPosition.Y + previousMoveDirection.Y);
                Point checkRight = new Point(individual.CurrentPosition.X + previousMoveDirection.Y, 
                                                individual.CurrentPosition.Y + previousMoveDirection.X);
                Point checkLeft = new Point(individual.CurrentPosition.X - previousMoveDirection.Y, 
                                                individual.CurrentPosition.Y - previousMoveDirection.X);
    
                bool isWallForward = checkForward.X < 0 || checkForward.Y < 0 || checkForward.X >= _maze.Width || checkForward.Y >= _maze.Height || 
                                     _maze[checkForward.X, checkForward.Y] == 1;
                bool isWallRight = checkRight.X < 0 || checkRight.Y < 0 || checkRight.X >= _maze.Width || checkRight.Y >= _maze.Height || 
                                   _maze[checkRight.X, checkRight.Y] == 1;
                bool isWallLeft = checkLeft.X < 0 || checkLeft.Y < 0 || checkLeft.X >= _maze.Width || checkLeft.Y >= _maze.Height || 
                                  _maze[checkLeft.X, checkLeft.Y] == 1;
    
                if (isWallForward && isWallRight && isWallLeft)
                {
                    individual.Die();
                    individual.AddFitness(PenaltyDeadEnd);
                }
            }
    
            // Преобразование гена в направление движения
            private Point GetDirection(int gene)
            {
                Point direction = new Point(0, 0);
                
                switch (gene)
                {
                    case 0:
                        direction.Y += 1;
                        break;
                    case 1:
                        direction.X += 1;
                        break;
                    case 2:
                        direction.Y += -1;
                        break;
                    case 3:
                        direction.X += -1;
                        break;
                }
    
                return direction;
            }

        #endregion

        #region Evolution

            // Создание следующего поколения
            private Population EvolvePopulation(Population oldPopulation)
            {
                Population newPopulation = new Population(_populationSize);
                
                // Создание потомства
                for (int i = _elitesCount; i < _populationSize; i++)
                {
                    Individual parent1 = TournamentSelection(oldPopulation);
                    Individual parent2 = TournamentSelection(oldPopulation);
    
                    Individual child = Crossover(parent1, parent2);
                    
                    Mutate(child);
    
                    newPopulation.Individuals[i] = child;
                }
                
                // Сохранение элиты
                Individual[] eliteIndividuals = oldPopulation.GetSeveralFittestIndividuals(_elitesCount);
                for (int i = 0; i < _elitesCount; i++)
                {
                    newPopulation.Individuals[i] = new Individual(_chromosomeLength);
                    for (int j = 0; j < _chromosomeLength; j++)
                    {
                        newPopulation.Individuals[i].Genes[j] = eliteIndividuals[i].Genes[j];
                    }
                    
                }

                return newPopulation;
            }
    
            // Скрещивание двух индивидов
            private Individual Crossover(Individual parent1, Individual parent2)
            {
                Individual child = new Individual(_chromosomeLength);
    
                int crossoverPoint = _random.Next(_chromosomeLength);
    
                for (int i = 0; i < crossoverPoint; i++)
                {
                    child.Genes[i] = parent1.Genes[i];
                }
                
                for (int i = crossoverPoint; i < _chromosomeLength; i++)
                {
                    child.Genes[i] = parent2.Genes[i];
                }
                
                return child;
            }
    
            // Мутация индивида
            private void Mutate(Individual individual)
            {
                Random tempRandom = new Random(_random.Next());
                for (int i = 0; i < _chromosomeLength; i++)
                {
                    if (tempRandom.NextDouble() < _mutationRate)
                    {
                        individual.Genes[i] = _random.Next(4);
                    }
                }
            }
    
            // Отбор индивида методом турнира
            private Individual TournamentSelection(Population population)
            {
                Population tournament = new Population(_tournamentSize);
                bool repeatedIndividual;
    
                for (int i = 0; i < _tournamentSize; i++)
                {
                    Individual randomIndividual;
                    
                    // Предотвращение повторов
                    do
                    {
                        repeatedIndividual = false;
                        
                        randomIndividual = population.Individuals[_random.Next(_populationSize)];
                        
                        for (int j = 0; j < i; j++)
                        {
                            if (tournament.Individuals[j] == randomIndividual)
                            {
                                repeatedIndividual = true;
                                break;
                            }
                        }
                    } while (repeatedIndividual);
    
                    tournament.Individuals[i] = randomIndividual;
                }
    
                return tournament.GetFittestIndividual();
            }

        #endregion

        #region Animation

            private void AnimateMove(int turn, Population population)
            {
                Thread[] individualThread = new Thread[_populationSize];
                
                void IndividualStep(object individualIndex)
                {
                    int index = (int)individualIndex;

                    switch (population.Individuals[index].Genes[turn])
                    {
                        case 0:
                            population.Individuals[index].Sprite.MoveDown();
                            break;
                        case 1:
                            population.Individuals[index].Sprite.MoveRight();
                            break;
                        case 2:
                            population.Individuals[index].Sprite.MoveUp();
                            break;
                        case 3:
                            population.Individuals[index].Sprite.MoveLeft();
                            break;
                    }
                }
                
                for (int i = 0; i < _populationSize; i++)
                {
                    individualThread[i] = new Thread(IndividualStep);
                    individualThread[i].Start(i);
                }

                for (int i = 0; i < _populationSize; i++)
                {
                    individualThread[i].Join();
                }
            }

            private void SpawnSprites(Population population)
            {
                for (int i = 0; i < _populationSize; i++)
                {
                    population.Individuals[i].Sprite = new IndividualSprite(_spriteStartLocation, _spriteSize);
                    _controls.Add(population.Individuals[i].Sprite);
                    population.Individuals[i].Sprite.BringToFront();
                }
            }

            private void RemoveSprites(Population population)
            {
                for (int i = 0; i < _populationSize; i++)
                {
                    _controls.Remove(population.Individuals[i].Sprite);
                }
            }

        #endregion
        
    }
}