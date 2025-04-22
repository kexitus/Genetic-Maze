using System;
using System.Drawing;

namespace Genetic_Maze
{
    public class Maze
    {
        protected int[,] Squares { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }

        public int this[int x, int y]
        {
            get => Squares[x, y];
            set => Squares[x, y] = value;
        }
        
        public Maze(){}
        
        public Maze(int height, int width, Point startPosition, Point endPosition)
        {
            Height = height;
            Width = width;
            Squares = new int[Width, Height];
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
        
        public void GenerateMaze() //1-999 — множества, 0 — проход, -(1-999) — стена снизу
        {
            Random random = new Random();
            int length = Width % 2 == 0 ? Width / 2 : Width / 2 + 1;
            int[] currentRow = new int[length];
            bool[] walls = new bool[length];
            int[] sets = new int[length];
            
            //Подготовка углов и крайних чётных стен
            for (int i = 1; i < Height; i += 2)
            {
                for (int j = 1; j < Width; j += 2)
                {
                    Squares[j, i] = 1;
                }
            }
            if (Height % 2 == 0)
            {
                for (int i = 0; i < Width; i++)
                {
                    Squares[i, Height - 1] = 1;
                }
            }
            if (Width % 2 == 0)
            {
                for (int i = 0; i < Height; i++)
                {
                    Squares[Width - 1, i] = 1;
                }
            }

            //Генерация стен
            for (int i = 0; i < Height; i += 2)
            {
                //Присвоение уникальных множеств
                for (int j = 0; j < length; j++)
                {
                    if (currentRow[j] == 0)
                        for (int k = 0; k < length; k++)
                        {
                            if (sets[k] == 0)
                            {
                                currentRow[j] = k + 1;
                                sets[k]++;
                                break;
                            }
                        }
                }
                
                //Расстановка вертикальных стен
                for (int j = 0; j < length - 1; j++)
                {
                    bool wantToPlace = random.Next(2) == 1;
                    
                    //Установка стены между общими сетами или по рандому
                    if (currentRow[j] == currentRow[j + 1] || wantToPlace)
                    {
                        walls[j] = true;
                    }
                    else //Поглощение следующего сета
                    {
                        int absorber = currentRow[j] - 1;
                        int absorbedSet = currentRow[j + 1] - 1;
                        for (int k = 0; k < length; k++)
                        {
                            if (currentRow[k] - 1 == absorbedSet)
                            {
                                currentRow[k] = absorber + 1;
                                sets[absorbedSet]--;
                                sets[absorber]++;
                            }
                        }
                    }
                }
                
                //Расстановка горизонтальных стен
                if (i < Height - 2)
                {
                    bool[] isSetIsolated = new bool[length];
                    for (int j = 0; j < length; j++) 
                    {
                        //Проверка на единственный элемент в сете
                        if (sets[currentRow[j] - 1] == 1)
                            continue;
                        //Проверка на отстутствие прохода в сете
                        if ((j == length - 1 || currentRow[j + 1] != currentRow[j]) && isSetIsolated[currentRow[j] - 1])
                        {
                            isSetIsolated[currentRow[j] - 1] = false;
                            continue;
                        }
                        //Принятие решения об установке стены
                        if (random.Next(2) == 1)
                        {
                            isSetIsolated[currentRow[j] - 1] = true;
                            currentRow[j] *= -1;
                        }
                        else
                        {
                            isSetIsolated[currentRow[j] - 1] = false;
                        }
                    }
                }
                else //Последний ряд
                {
                    int previousValue = currentRow[0];
                    for (int j = 0; j < length; j++)
                    {
                        if (walls[j] && previousValue != currentRow[j + 1])
                        {
                            walls[j] = false;
                            int absorber = currentRow[j] - 1;
                            int absorbedSet = currentRow[j + 1] - 1;
                            for (int k = 0; k < length; k++)
                            {
                                if (currentRow[k] - 1 == absorbedSet)
                                {
                                    currentRow[k] = absorber + 1;
                                    sets[absorbedSet]--;
                                    sets[absorber]++;
                                }
                            }
                        }
                    }
                }
                
                //Перенос строки в матрицу 
                for (int j = 0; j < length; j++)
                {
                    if (walls[j])
                    {
                        Squares[j * 2 + 1, i] = 1;
                        walls[j] = false;
                    }

                    if (currentRow[j] < 0)
                    {
                        Squares[j * 2, i + 1] = 1;
                        sets[currentRow[j] * -1 - 1]--;
                        currentRow[j] = 0;
                    }
                }
            }
        }
    }
}