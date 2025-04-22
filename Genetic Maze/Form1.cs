using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Genetic_Maze
{
    public partial class Form1 : Form
    {
        public MazeGrid MazeGrid { get; set; }
        public Form1()
        {
            InitializeComponent();
            MazeGrid = new MazeGrid(Grid, InputHandler.GetInt(inputMazeWidth), InputHandler.GetInt(inputMazeHeight), inputMazeStart, inputMazeEnd);
            
            Grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            MazeGrid = new MazeGrid(Grid, InputHandler.GetInt(inputMazeWidth), InputHandler.GetInt(inputMazeHeight), inputMazeStart, inputMazeEnd);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Thread threadWorker = new Thread(GaThreadMethod);
            
            threadWorker.Start();
        }

        private void GaThreadMethod()
        {
            int populationSize = InputHandler.GetInt(inputPopulationSize);
            double mutationRate = InputHandler.GetInt(inputMutationChance)/100.0;
            int elitesCount = InputHandler.GetInt(inputElitesCount);
            int tournamentSize = InputHandler.GetInt(inputTournamentSize);
            int minimumIterations = 250;

            GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(MazeGrid.GetMazeData(), populationSize, mutationRate, elitesCount, tournamentSize, Grid[0,0].Size, Controls);

            geneticAlgorithm.Simulate(minimumIterations, outputBox);
        }
    }
}