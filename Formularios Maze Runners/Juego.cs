using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_csharp.Ejercicios
{
    
    public partial class Game : UserControl
    {
        public char[,] maze;
        public int playerCount;
        private List<Player> players = new List<Player>();
        private RichTextBox rtbMaze;
        private int currentPlayerTurn = 0;
        private int remainingMoves = 0;
        public class Player
        {
            public char Symbol { get; set; }
            public int Row { get; set; }
            public int Col { get; set; }
        }
        public Game()
        {
            InitializeComponent();
        }
        private void MoveCurrentPlayer(Direction direction)
        {
            // Verificar si hay movimientos disponibles
            if (remainingMoves <= 0)
            {
                MessageBox.Show("No te quedan movimientos en este turno!");
                return;
            }

            // Obtener el jugador actual
            if (currentPlayerTurn >= players.Count)
            {
                currentPlayerTurn = 0;
            }

            Player currentPlayer = players[currentPlayerTurn];
            int newRow = currentPlayer.Row;
            int newCol = currentPlayer.Col;

            // Calcular nueva posición según dirección
            switch (direction)
            {
                case Direction.Up:
                    newRow--;
                    break;
                case Direction.Down:
                    newRow++;
                    break;
                case Direction.Left:
                    newCol--;
                    break;
                case Direction.Right:
                    newCol++;
                    break;
            }

            // Verificar si el movimiento es válido
            if (IsValidMove(newRow, newCol))
            {
                // Guardar posición anterior
                maze[currentPlayer.Row, currentPlayer.Col] = ' ';

                // Actualizar posición del jugador
                currentPlayer.Row = newRow;
                currentPlayer.Col = newCol;
                maze[newRow, newCol] = currentPlayer.Symbol;

                remainingMoves--;
                UpdateMazeDisplay(maze);

                // Si no quedan movimientos, pasar al siguiente turno
                if (remainingMoves <= 0)
                {
                    NextTurn();
                }
            }
        }

        private bool IsValidMove(int row, int col)
        {
            // Verificar límites del laberinto
            if (row < 0 || row >= maze.GetLength(0) || col < 0 || col >= maze.GetLength(1))
                return false;

            // Verificar si la celda destino es un espacio válido o la meta
            return maze[row, col] == ' ' || maze[row, col] == 'E';
        }

        private void NextTurn()
        {
            currentPlayerTurn++;
            if (currentPlayerTurn >= players.Count)
            {
                currentPlayerTurn = 0;
            }

            // Obtener nuevos movimientos del label
            remainingMoves = int.Parse(labelmoves.Text);
            MessageBox.Show($"Turno del Jugador {currentPlayerTurn + 1} ({players[currentPlayerTurn].Symbol})\nMovimientos disponibles: {remainingMoves}");
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }
        private void GeneratePlayers(char[,] maze)
        {
            int playerCount = ValidateAndGetPlayerCount();
            if (playerCount == -1) return;

            char[] playerSymbols = { '♠', '♣', '♥', '♦', 't' };
            Random random = new Random();
            players.Clear();

            // Encontrar las posiciones válidas(lugares vacíos)
            List<(int x, int y)> validPositions = new List<(int x, int y)>();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (maze[i, j] == ' ')
                    {
                        validPositions.Add((i, j));
                    }
                }
            }

            // Posición de los jugadores
            for (int i = 0; i < playerCount; i++)
            {
                int posIndex = random.Next(validPositions.Count);
                var position = validPositions[posIndex];

                Player newPlayer = new Player
                {
                    Symbol = playerSymbols[i],
                    Row = position.x,
                    Col = position.y
                };

                players.Add(newPlayer);
                maze[position.x, position.y] = playerSymbols[i];
            }

            UpdateMazeDisplay(maze);
        }

        private void UpdateMazeDisplay(char[,] maze)
        {
            StringBuilder mazeText = new StringBuilder();
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    mazeText.Append(maze[i, j] + " . ");
                }
                mazeText.AppendLine();
            }

            if (rtbMaze != null)
                rtbMaze.Text = mazeText.ToString();
        }


        private char[,] GenerateMultiPathMaze()
        {
            char[,] maze = new char[15, 15];
            Random random = new Random();

            // Iniciar el laberinto con las paredes
            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                    maze[i, j] = '█';

            // Crear primer camino
            GeneratePath(maze, 0, 0, 14, 14, random);

            // Crear segundo camino
            int startX = random.Next(0, 5);
            int startY = random.Next(0, 5);
            GeneratePath(maze, startX, startY, 14, 14, random);

            // Marcar inicio y final
            maze[0, 0] = 'S';
            maze[14, 14] = 'E';

            return maze;
        }

        private void GeneratePath(char[,] maze, int startX, int startY, int endX, int endY, Random random)
        {
            Stack<(int x, int y)> path = new Stack<(int x, int y)>();
            path.Push((startX, startY));

            while (path.Count > 0)
            {
                var current = path.Peek();
                maze[current.x, current.y] = ' ';

                if (current.x == endX && current.y == endY)
                    break;

                List<(int x, int y)> neighbors = new List<(int x, int y)>();
                int[] dx = { 2, -2, 0, 0 };
                int[] dy = { 0, 0, 2, -2 };

                for (int i = 0; i < 4; i++)
                {
                    int newX = current.x + dx[i];
                    int newY = current.y + dy[i];

                    if (IsValidCell(newX, newY, maze))
                    {
                        neighbors.Add((newX, newY));
                    }
                }

                if (neighbors.Count > 0)
                {
                    var next = neighbors[random.Next(neighbors.Count)];
                    maze[(current.x + next.x) / 2, (current.y + next.y) / 2] = ' ';
                    path.Push(next);
                }
                else
                {
                    path.Pop();
                }
            }
        }

        private bool IsValidCell(int x, int y, char[,] maze)
        {
            return x >= 0 && x < maze.GetLength(0) &&
                   y >= 0 && y < maze.GetLength(1) &&
                   maze[x, y] == '█';
        }

        private void DisplayMaze(char[,] maze)
        {
            RichTextBox rtbMaze = new RichTextBox();
            rtbMaze.Name = "rtbMaze";
            rtbMaze.Font = new Font("Consolas", 15, FontStyle.Bold);
            rtbMaze.BackColor = Color.Black;
            rtbMaze.ForeColor = Color.Lime;
            rtbMaze.Dock = DockStyle.Fill;
            rtbMaze.ReadOnly = true;

            StringBuilder mazeText = new StringBuilder();
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    mazeText.Append(maze[i, j] + " . ");
                }
                mazeText.AppendLine();
            }

            rtbMaze.Text = mazeText.ToString();
            this.Controls.Add(rtbMaze);
        }

        private void Game_Load(object sender, EventArgs e)
        {
            MessageBox.Show("BIENVENIDO !!!");
            maze = GenerateMultiPathMaze();
            DisplayMaze(maze);
            rtbMaze = (RichTextBox)Controls.Find("rtbMaze", true).FirstOrDefault();
        }
        private int ValidateAndGetPlayerCount()
        {
            if (int.TryParse(textBox1.Text, out int playerCount))
            {
                if (playerCount >= 1 && playerCount <= 5)
                {
                    MessageBox.Show("EMPECEMOS !!!");
                    return playerCount;
                }
                else
                {
                    MessageBox.Show("El número de jugadores debe estar entre 1 y 5", "Número Incorrecto",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Clear();
                    textBox1.Focus();
                    return -1;
                }
            }
            else
            {
                MessageBox.Show("Por favor ingrese solo números", "Entrada Inválida",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Clear();
                textBox1.Focus();
                return -1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ValidateAndGetPlayerCount();
            GeneratePlayers(maze);
            

        }
        private void StartGame()
        {
            MessageBox.Show($"Comienza el Jugador 1 ({players[0].Symbol})\nMovimientos disponibles: {remainingMoves}");
        }
        private int Moves()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 6);
            return randomNumber;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int arg = Moves();
            labelmoves.Text = arg.ToString();
            StartGame();
        }

        private void up_Click(object sender, EventArgs e)
        {
            MoveCurrentPlayer(Direction.Up);
        }

        private void down_Click(object sender, EventArgs e)
        {
            MoveCurrentPlayer(Direction.Down);
        }

        private void right_Click(object sender, EventArgs e)
        {
            MoveCurrentPlayer(Direction.Right);
        }

        private void left_Click(object sender, EventArgs e)
        {
            MoveCurrentPlayer(Direction.Left);
        }
    }
   
}

