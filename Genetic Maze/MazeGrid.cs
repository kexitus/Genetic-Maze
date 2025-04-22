using System;
using System.Drawing;
using System.Windows.Forms;

namespace Genetic_Maze
{
    public class MazeGrid : Maze
    {
        private DataGridView _grid;
        private bool _isMouseDown;
        private Color _initialColor;
        private DataGridViewCell _startCell;
        private DataGridViewCell _endCell;
        private int _initialHeight = 800;
        private int _initialWidth = 800;
        private int _initialX = 10;
        private int _initialY = 10;
        private TextBox _startBox;
        private TextBox _endBox;

        public MazeGrid(DataGridView grid, int width, int height, TextBox startCoordinates, TextBox endCoordinates)
        {
            _grid = grid;
            _startBox = startCoordinates;
            _endBox = endCoordinates;

            //Изменение масштаба
            _grid.ColumnCount = width;
            _grid.RowCount = height;
            ResizeGrid();
            ResizeGrid();
            ResizeCells();
            
            //Сброс цветов
            foreach (DataGridViewRow row in _grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.BackColor = CustomColors.Background;
                }
            }

            Height = height;
            Width = width;
            Squares = new int[Width, Height];
            GenerateMaze();
            VisualizeMaze();
            
            //Присвоение действий
            _grid.SelectionChanged += Grid_SelectionChanged;
            _grid.MouseDown += Grid_MouseDown;
            _grid.MouseUp += Grid_MouseUp;
            _grid.CellMouseEnter += Grid_CellMouseEnter;
            _grid.SelectionChanged += Grid_SelectionChanged;
            _grid.CellMouseClick += Grid_CellMouseClick; 
        }

        public Maze GetMazeData()
        {
            Maze mazeData = new Maze(Height, Width, StartPosition, EndPosition);

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    mazeData[j, i] = Squares[j, i];
                }
            }

            return mazeData;
        }
        public void VisualizeMaze()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Squares[j, i] == 0)
                    {
                        _grid[j, i].Style.BackColor = CustomColors.Content;
                    } 
                }
            }
        }

        #region User interaction
        
        private void ChangeColor(DataGridViewCell cell)
        {
            if (cell.Style.BackColor == CustomColors.Background)
            {
                cell.Style.BackColor = CustomColors.Content;
                Squares[cell.ColumnIndex, cell.RowIndex] = 0;
            }
            else if (cell.Style.BackColor == CustomColors.Content)
            {
                cell.Style.BackColor = CustomColors.Background;
                Squares[cell.ColumnIndex, cell.RowIndex] = 1;
            }
        }
        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = true;
                DataGridView.HitTestInfo hitTestInfo = _grid.HitTest(e.X, e.Y);
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell)
                {
                    _initialColor = _grid[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex].Style.BackColor;
                    ChangeColor(_grid[hitTestInfo.ColumnIndex, hitTestInfo.RowIndex]);
                }
            }
        }
        private void Grid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMouseDown = false;
            }
        }
        private void Grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (_isMouseDown)
            {
                if (_grid[e.ColumnIndex, e.RowIndex].Style.BackColor == _initialColor)
                {
                    ChangeColor(_grid[e.ColumnIndex, e.RowIndex]);
                }
            }
        }
        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            _grid.ClearSelection();
        }
        
        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell clickedCell = _grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

                // Создание контекстного меню
                ContextMenuStrip menu = new ContextMenuStrip();

                // Создание пункта меню для выбора старта
                ToolStripMenuItem startMenuItem = new ToolStripMenuItem("Установить старт");
                startMenuItem.Click += (s, ev) => SetStartCell(clickedCell);
                menu.Items.Add(startMenuItem);

                // Создание пункта меню для выбора конца
                ToolStripMenuItem endMenuItem = new ToolStripMenuItem("Установить конец");
                endMenuItem.Click += (s, ev) => SetEndCell(clickedCell);
                menu.Items.Add(endMenuItem);

                // Отображение контекстного меню
                menu.Show(_grid, _grid.PointToClient(Cursor.Position));
            }
        }
        private void SetStartCell(DataGridViewCell cell)
        {
            if (_startCell != null)
            {
                // Возвращение предыдущей ячейки старта в состояние прохода
                _startCell.Style.BackColor = CustomColors.Content;
            }

            _startCell = cell;
            cell.Style.BackColor = CustomColors.Start;

            StartPosition = new Point(cell.ColumnIndex, cell.RowIndex);

            _startBox.Text = $@"({StartPosition.X}; {StartPosition.Y})";
        }
        private void SetEndCell(DataGridViewCell cell)
        {
            if (_endCell != null)
            {
                // Возвращение предыдущей ячейки конца в состояние прохода
                _endCell.Style.BackColor = CustomColors.Content;
            }

            _endCell = cell;
            cell.Style.BackColor = CustomColors.End;

            EndPosition = new Point(cell.ColumnIndex, cell.RowIndex);

            _endBox.Text = $@"({EndPosition.X}; {EndPosition.Y})";
        }

        #endregion User interaction

        #region Visual adjustments

        private void ResizeCells()
        {
            int cellWidth = _grid.Size.Width/_grid.ColumnCount;
            int cellHeight = _grid.Size.Height/_grid.RowCount;
            foreach (DataGridViewColumn column in _grid.Columns)
            {
                column.Width = cellWidth;
            }
            foreach (DataGridViewRow row in _grid.Rows)
            {
                row.Height = cellHeight;
            }
        }
        private void ResizeGrid()
        {
            int greatWidth = _initialWidth / _grid.ColumnCount * _grid.ColumnCount;
            int smallWidth = _initialWidth / _grid.RowCount * _grid.ColumnCount;
            
            int greatHeight = _initialHeight / _grid.RowCount * _grid.RowCount;
            int smallHeight = _initialHeight / _grid.ColumnCount * _grid.RowCount;

            int adaptiveX = _initialX + (_initialWidth - _grid.Width) / 2;
            int adaptiveY = _initialY + (_initialHeight - _grid.Height) / 2;
            
            if (_grid.RowCount > _grid.ColumnCount)
            {
                _grid.Size = new Size(smallWidth, greatHeight);
                _grid.Location = new Point(adaptiveX, _initialY);
            }
            else if (_grid.RowCount < _grid.ColumnCount)
            {
                _grid.Size = new Size(greatWidth, smallHeight);
                _grid.Location = new Point(_initialX, adaptiveY);
                
            }
            else
            {
                _grid.Size = new Size(greatWidth, greatHeight);
                _grid.Location = new Point(adaptiveX, adaptiveY);
            }
        }

        #endregion Visual adjustments
        
    }
}