﻿using System;
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
        private List<(int row, int col, int type)> traps = new List<(int row, int col, int type)>();
        private Random random = new Random();
        private bool skipNextTurn = false;
        private bool isPlayer2ImmuneToTrap = false;
        private int nextTurnMovesMultiplier = 1;
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
        // Método principal para manejar las habilidades especiales
        private void UseSpecialAbility()
        {
            if (remainingMoves != 5)
            {
                MessageBox.Show("¡La habilidad especial solo puede usarse cuando tienes exactamente 5 movimientos!");
                return;
            }

            switch (currentPlayerTurn + 1)
            {
                case 1:
                    Player1Ability();
                    break;
                case 2:
                    Player2Ability();
                    break;
                case 3:
                    Player3Ability();
                    break;
                case 4:
                    Player4Ability();
                    break;
                case 5:
                    Player5Ability();
                    break;
            }
        }

        private void Player1Ability()
        {
            int targetPlayer = (currentPlayerTurn + 0) % players.Count;
            remainingMoves += 3;

            if (remainingMoves < 0)
            {
                NextTurn();
            }
            MessageBox.Show($"¡Jugador 1 aumentó 3 movimientos del jugador {targetPlayer + 1}!");
        }

        private void Player2Ability()
        {
            isPlayer2ImmuneToTrap = true;
            MessageBox.Show("¡Jugador 2 es inmune a la siguiente trampa!");
        }

        private void Player3Ability()
        {
            int nextPlayer = (currentPlayerTurn + 1) % players.Count;
            Player nextPlayerObj = players[nextPlayer];

            maze[nextPlayerObj.Row, nextPlayerObj.Col] = ' ';
            nextPlayerObj.Row = 0;
            nextPlayerObj.Col = 0;
            maze[0, 0] = nextPlayerObj.Symbol;

            MessageBox.Show("¡El siguiente jugador fue enviado al inicio!");
            UpdateMazeDisplay(maze);
        }

        private void Player4Ability()
        {
            Player currentPlayer = players[currentPlayerTurn];
            string nearestTraps = FindNearestTraps(currentPlayer.Row, currentPlayer.Col);
            MessageBox.Show($"Las trampas más cercanas están en: {nearestTraps}");
        }

        private string FindNearestTraps(int playerRow, int playerCol)
        {
            var trapDistances = new List<(double distance, int row, int col)>();

            foreach (var trap in traps)
            {
                double distance = Math.Sqrt(Math.Pow(playerRow - trap.row, 2) + Math.Pow(playerCol - trap.col, 2));
                trapDistances.Add((distance, trap.row, trap.col));
            }

            var nearest = trapDistances.OrderBy(t => t.distance).Take(2);
            return $"[{nearest.First().row},{nearest.First().col}] y [{nearest.ElementAt(1).row},{nearest.ElementAt(1).col}]";
        }

        private void Player5Ability()
        {
            nextTurnMovesMultiplier = 2;
            MessageBox.Show("¡Los movimientos serán duplicados!");
        }


        // Agregar este método para generar las trampas
        private void GenerateTraps()
        {
            traps.Clear();
            List<(int row, int col)> validPositions = new List<(int row, int col)>();

            // Encontrar todas las posiciones válidas del camino
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (maze[i, j] == ' ' && !(i == 0 && j == 0) && !(i == maze.GetLength(0) - 1 && j == maze.GetLength(1) - 1))
                    {
                        validPositions.Add((i, j));
                    }
                }
            }

            // Generar 4 trampas de cada tipo
            for (int trapType = 1; trapType <= 4; trapType++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (validPositions.Count > 0)
                    {
                        int index = random.Next(validPositions.Count);
                        traps.Add((validPositions[index].row, validPositions[index].col, trapType));
                        validPositions.RemoveAt(index);
                    }
                }
            }
        }

        // Modificar el método MoveCurrentPlayer para incluir la verificación de trampas
        private void CheckForTrap(int row, int col)
        {
            var trap = traps.FirstOrDefault(t => t.row == row && t.col == col);

            if (currentPlayerTurn + 1 == 2 && isPlayer2ImmuneToTrap)
            {
                MessageBox.Show("¡El Jugador 2 es inmune a esta trampa!");
                isPlayer2ImmuneToTrap = false;
                return;
            }

            if (trap != default)
            {
                Player currentPlayer = players[currentPlayerTurn];

                switch (trap.type)
                {
                    case 1: // Volver al inicio
                        currentPlayer.Row = 0;
                        currentPlayer.Col = 0;
                        maze[row, col] = ' ';
                        maze[0, 0] = currentPlayer.Symbol;
                        MessageBox.Show("¡Has sido encontrado por una bruja!!! Vuelves al inicio de la misión");
                        break;

                    case 2: // Perder turno
                        skipNextTurn = true;
                        MessageBox.Show("¡Una bruja ha mejorado tu aura. El siguiente jugador no podrá moverse!!!");
                        break;

                    case 3: // Teletransporte aleatorio
                        List<(int row, int col)> validSpots = new List<(int row, int col)>();
                        for (int i = 0; i < maze.GetLength(0); i++)
                        {
                            for (int j = 0; j < maze.GetLength(1); j++)
                            {
                                if (maze[i, j] == ' ')
                                {
                                    validSpots.Add((i, j));
                                }
                            }
                        }

                        if (validSpots.Count > 0)
                        {
                            var newPos = validSpots[random.Next(validSpots.Count)];
                            maze[row, col] = ' ';
                            currentPlayer.Row = newPos.row;
                            currentPlayer.Col = newPos.col;
                            maze[newPos.row, newPos.col] = currentPlayer.Symbol;
                            MessageBox.Show("¡Has caído en un portal embrujado, ahora te irás a donde te envíe el destino!!!");
                        }
                        break;
                        
                }
                UpdateMazeDisplay(maze);
            }
        }
        private void MoveCurrentPlayer(Direction direction)
        {
            if (remainingMoves <= 0)
            {
                MessageBox.Show("No te quedan movimientos en este turno!");
                return;
            }

            Player currentPlayer = players[currentPlayerTurn];
            int newRow = currentPlayer.Row;
            int newCol = currentPlayer.Col;

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

            if (IsValidMove(newRow, newCol))
            {
                maze[currentPlayer.Row, currentPlayer.Col] = ' ';
                currentPlayer.Row = newRow;
                currentPlayer.Col = newCol;

                // Verificar si llegó a la meta antes de actualizar la posición
                if (maze[newRow, newCol] == 'E')
                {
                    MessageBox.Show($"¡El Jugador {currentPlayerTurn + 1} ({currentPlayer.Symbol}) ha ganado!,, pudiste salvar a la humanidad del ataque de las brujas");
                    maze[newRow, newCol] = currentPlayer.Symbol;
                    UpdateMazeDisplay(maze);
                    ResetGame();
                    return;
                }

                maze[newRow, newCol] = currentPlayer.Symbol;
                remainingMoves--;
                UpdateMazeDisplay(maze);
                CheckForTrap(newRow, newCol);

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
            // Verificar si algún jugador llegó a la meta
            Player currentPlayer = players[currentPlayerTurn];
            if (maze[currentPlayer.Row, currentPlayer.Col] == 'E')
            {
                MessageBox.Show($"¡El Jugador {currentPlayerTurn + 1} ({currentPlayer.Symbol}) ha ganado!, pudiste salvar a la humanidad del ataque de las brujas");
                ResetGame();
                return;
            }

            // Avanzar al siguiente jugador
            currentPlayerTurn = (currentPlayerTurn + 1) % players.Count;

            // Manejar la trampa de saltar turno
            if (skipNextTurn)
            {
                skipNextTurn = false;
                MessageBox.Show($"El Jugador {currentPlayerTurn + 1} pierde su turno por la trampa anterior");
                NextTurn(); // Salta inmediatamente al siguiente jugador
                return;
            }
            remainingMoves = Moves() * nextTurnMovesMultiplier;
            nextTurnMovesMultiplier = 1; // Reiniciar multiplicador

            // Establecer movimientos para el siguiente jugador
            remainingMoves = Moves();
            labelmoves.Text = remainingMoves.ToString();

            MessageBox.Show($"Turno del Jugador {currentPlayerTurn + 1} ({players[currentPlayerTurn].Symbol})\n" +
                           $"Movimientos disponibles: {remainingMoves}");
        
         }
        private void ResetGame()
        {
            players.Clear();
            maze = GenerateMultiPathMaze();
            DisplayMaze(maze);
            currentPlayerTurn = 0;
            remainingMoves = 0;
            labelmoves.Text = "0";
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

            char[] playerSymbols = { '♠', '♣', '♥', '♦', '$' };
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
            GenerateTraps();
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
            if (players.Count == 0)
            {
                MessageBox.Show("¡Primero debes generar los jugadores!");
                return;
            }

            currentPlayerTurn = 0;
            remainingMoves = Moves();
            labelmoves.Text = remainingMoves.ToString();
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
            remainingMoves = Moves();
            labelmoves.Text = remainingMoves.ToString();
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

        private void button3_Click(object sender, EventArgs e)
        {
            UseSpecialAbility();
        }
    }
   
}

