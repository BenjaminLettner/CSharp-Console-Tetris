using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace TetrisGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up console for Docker compatibility
            SetupConsole();
            
            // Hide cursor for cleaner display
            Console.CursorVisible = false;
            
            // Start with the menu
            var menu = new Menu();
            menu.Show();
            
            // Show cursor again before exiting
            Console.CursorVisible = true;
        }
        
        /// <summary>
        /// Set up the console for better compatibility, especially in Docker
        /// </summary>
        static void SetupConsole()
        {
            try
            {
                // Try to set input encoding to UTF8
                Console.InputEncoding = System.Text.Encoding.UTF8;
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                
                // Try to ensure ANSI colors work in Docker
                System.AppContext.SetSwitch("System.Console.AllowOutputColoring", true);
                
                // Try to disable quick edit mode on Windows (can cause input blocking)
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    // This is a no-op on other platforms
                    Console.WriteLine("Press any key to start...");
                    Console.ReadKey(true);
                }
            }
            catch (System.Exception ex)
            {
                // Just log and continue if there's an issue
                Console.WriteLine($"Console setup warning: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Main menu system for the Tetris game
    /// </summary>
    class Menu
    {
        private readonly string highScoreFile = "highscore.txt";
        private readonly List<string> menuOptions = new List<string>
        {
            "Start Game",
            "High Scores",
            "Instructions",
            "Exit"
        };
        
        /// <summary>
        /// Show the main menu and handle user selection
        /// </summary>
        public void Show()
        {
            bool exitProgram = false;
            
            while (!exitProgram)
            {
                // Clear console and draw title
                AnsiConsole.Clear();
                DrawTitle();
                
                // Show menu options
                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select an [green]option[/]:")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Green))
                        .AddChoices(menuOptions));
                
                // Handle selection
                switch (menuOptions.IndexOf(selection))
                {
                    case 0: // Start Game
                        var game = new Game();
                        game.Start();
                        break;
                        
                    case 1: // High Scores
                        ShowHighScores();
                        break;
                        
                    case 2: // Instructions
                        ShowInstructions();
                        break;
                        
                    case 3: // Exit
                        exitProgram = true;
                        break;
                }
            }
        }
        
        /// <summary>
        /// Draw the Tetris title with ASCII art
        /// </summary>
        private void DrawTitle()
        {
            string[] title = {
                @" _____ _____ _____ _____ _____ _____ ",
                @"|_   _|  ___|_   _|  _  |_   _|  ___|",
                @"  | | | |__   | | | | | | | | | |__  ",
                @"  | | |  __|  | | | | | | | |  __| ",
                @"  | | | |___  | | \ \_/ / | | | |___ ",
                @"  \_/ \____/  \_/  \___/  \_/ \____/ "
            };
            
            // Display title with random colors for each line
            string[] colors = { "red", "green", "yellow", "blue", "magenta", "cyan" };
            Random rand = new Random();
            
            AnsiConsole.WriteLine();
            for (int i = 0; i < title.Length; i++)
            {
                string color = colors[i % colors.Length];
                AnsiConsole.MarkupLine($"[{color}]{title[i]}[/]");
            }
            AnsiConsole.WriteLine();
        }
        
        /// <summary>
        /// Show high scores screen
        /// </summary>
        private void ShowHighScores()
        {
            AnsiConsole.Clear();
            DrawTitle();
            
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title("[yellow]HIGH SCORES[/]");
            
            // Add columns
            table.AddColumn(new TableColumn("Rank").Centered());
            table.AddColumn(new TableColumn("Score").Centered());
            
            // Load scores from file
            List<int> scores = LoadScores();
            
            if (scores.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No high scores yet![/]");
            }
            else
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    // Add row with rank and score
                    string medal = i switch
                    {
                        0 => "🥇",
                        1 => "🥈",
                        2 => "🥉",
                        _ => $"{i + 1}"
                    };
                    
                    table.AddRow(medal, scores[i].ToString());
                }
                
                AnsiConsole.Write(table);
            }
            
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu[/]");
            Console.ReadKey(true);
        }
        
        /// <summary>
        /// Show game instructions
        /// </summary>
        private void ShowInstructions()
        {
            AnsiConsole.Clear();
            DrawTitle();
            
            var panel = new Panel(
                "Tetris is a classic puzzle game where you must arrange falling tetrominos.\n" +
                "Clear lines by filling all cells in a horizontal row. As you clear more lines, " +
                "the level increases and pieces fall faster.\n\n" +
                "[bold]Controls:[/]\n" +
                "←, → or A, D: Move piece left/right\n" +
                "↑ or W: Rotate piece clockwise\n" +
                "↓ or S: Soft drop (move down faster)\n" +
                "Spacebar: Hard drop (immediately drop to bottom)\n" +
                "P: Pause/Resume game\n\n" +
                "[bold]Scoring:[/]\n" +
                "1 line: 40 × (level + 1) points\n" +
                "2 lines: 100 × (level + 1) points\n" +
                "3 lines: 300 × (level + 1) points\n" +
                "4 lines (Tetris): 1200 × (level + 1) points\n" +
                "Soft drop: 1 point per cell\n" +
                "Hard drop: 2 points per cell"
            )
            {
                Header = new PanelHeader("[bold]Game Instructions[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(panel);
            
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu[/]");
            Console.ReadKey(true);
        }
        
        /// <summary>
        /// Load high scores from file
        /// </summary>
        private List<int> LoadScores()
        {
            List<int> scores = new List<int>();
            
            try
            {
                if (File.Exists(highScoreFile))
                {
                    string[] lines = File.ReadAllLines(highScoreFile);
                    foreach (string line in lines)
                    {
                        if (int.TryParse(line, out int score))
                        {
                            scores.Add(score);
                        }
                    }
                    
                    // Sort scores in descending order
                    scores.Sort((a, b) => b.CompareTo(a));
                    
                    // Keep only top 10 scores
                    if (scores.Count > 10)
                    {
                        scores = scores.GetRange(0, 10);
                    }
                }
            }
            catch (Exception)
            {
                // If there's an error reading the file, return empty list
            }
            
            return scores;
        }
    }

    /// <summary>
    /// Represents a Tetromino (tetris piece) with position and rotation
    /// </summary>
    class Tetromino
    {
        // Each shape is a list of 4x4 matrices (rotations)
        private static readonly Dictionary<string, List<int[,]>> TetrominoShapes = new Dictionary<string, List<int[,]>>
        {
            // I-piece (cyan/aqua - 1)
            {
                "I", new List<int[,]>
                {
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 1, 0 },
                        { 0, 0, 1, 0 },
                        { 0, 0, 1, 0 },
                        { 0, 0, 1, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 1, 1, 1, 1 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 1, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 1, 0, 0 },
                        { 0, 1, 0, 0 }
                    }
                }
            },
            // J-piece (blue - 2)
            {
                "J", new List<int[,]>
                {
                    new int[4, 4] {
                        { 2, 0, 0, 0 },
                        { 2, 2, 2, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 2, 2, 0 },
                        { 0, 2, 0, 0 },
                        { 0, 2, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 2, 2, 2, 0 },
                        { 0, 0, 2, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 2, 0, 0 },
                        { 0, 2, 0, 0 },
                        { 2, 2, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            },
            // L-piece (orange - 3)
            {
                "L", new List<int[,]>
                {
                    new int[4, 4] {
                        { 0, 0, 3, 0 },
                        { 3, 3, 3, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 3, 0, 0 },
                        { 0, 3, 0, 0 },
                        { 0, 3, 3, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 3, 3, 3, 0 },
                        { 3, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 3, 3, 0, 0 },
                        { 0, 3, 0, 0 },
                        { 0, 3, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            },
            // O-piece (yellow - 4)
            {
                "O", new List<int[,]>
                {
                    new int[4, 4] {
                        { 0, 4, 4, 0 },
                        { 0, 4, 4, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 4, 4, 0 },
                        { 0, 4, 4, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 4, 4, 0 },
                        { 0, 4, 4, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 4, 4, 0 },
                        { 0, 4, 4, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            },
            // S-piece (green - 5)
            {
                "S", new List<int[,]>
                {
                    new int[4, 4] {
                        { 0, 5, 5, 0 },
                        { 5, 5, 0, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 5, 0, 0 },
                        { 0, 5, 5, 0 },
                        { 0, 0, 5, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 0, 5, 5, 0 },
                        { 5, 5, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 5, 0, 0, 0 },
                        { 5, 5, 0, 0 },
                        { 0, 5, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            },
            // T-piece (purple - 6)
            {
                "T", new List<int[,]>
                {
                    new int[4, 4] {
                        { 0, 6, 0, 0 },
                        { 6, 6, 6, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 6, 0, 0 },
                        { 0, 6, 6, 0 },
                        { 0, 6, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 6, 6, 6, 0 },
                        { 0, 6, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 6, 0, 0 },
                        { 6, 6, 0, 0 },
                        { 0, 6, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            },
            // Z-piece (red - 7)
            {
                "Z", new List<int[,]>
                {
                    new int[4, 4] {
                        { 7, 7, 0, 0 },
                        { 0, 7, 7, 0 },
                        { 0, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 7, 0 },
                        { 0, 7, 7, 0 },
                        { 0, 7, 0, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 0, 0, 0 },
                        { 7, 7, 0, 0 },
                        { 0, 7, 7, 0 },
                        { 0, 0, 0, 0 }
                    },
                    new int[4, 4] {
                        { 0, 7, 0, 0 },
                        { 7, 7, 0, 0 },
                        { 7, 0, 0, 0 },
                        { 0, 0, 0, 0 }
                    }
                }
            }
        };

        // Available shape keys
        public static readonly string[] ShapeKeys = { "I", "J", "L", "O", "S", "T", "Z" };

        // Current position and rotation
        public int X { get; set; }
        public int Y { get; set; }
        public int RotationIndex { get; private set; }
        public string ShapeKey { get; private set; }

        // Random generator for selecting shapes
        private static readonly Random Random = new Random();

        /// <summary>
        /// Create a new tetromino with the specified shape
        /// </summary>
        public Tetromino(string shapeKey, int x, int y)
        {
            ShapeKey = shapeKey;
            X = x;
            Y = y;
            RotationIndex = 0;
        }

        /// <summary>
        /// Create a new random tetromino
        /// </summary>
        public static Tetromino CreateRandom(int x, int y)
        {
            string randomShape = ShapeKeys[Random.Next(ShapeKeys.Length)];
            return new Tetromino(randomShape, x, y);
        }

        /// <summary>
        /// Get the current shape matrix based on rotation
        /// </summary>
        public int[,] GetCurrentShape()
        {
            return TetrominoShapes[ShapeKey][RotationIndex];
        }

        /// <summary>
        /// Rotate the piece clockwise
        /// </summary>
        public void Rotate()
        {
            RotationIndex = (RotationIndex + 1) % 4;
        }

        /// <summary>
        /// Get the color for this tetromino type
        /// </summary>
        public string GetColor()
        {
            return ShapeKey switch
            {
                "I" => "aqua",      // Cyan
                "J" => "blue",      // Blue
                "L" => "orange1",   // Orange
                "O" => "yellow",    // Yellow
                "S" => "green",     // Green
                "T" => "purple",    // Purple
                "Z" => "red",       // Red
                _ => "white"
            };
        }
    }

    /// <summary>
    /// Main game class that handles game logic and state
    /// </summary>
    class Game
    {
        // Game grid dimensions
        private const int GridWidth = 10;
        private const int GridHeight = 20;
        
        // Game state
        private int[,] grid;
        private Tetromino currentPiece;
        private Tetromino nextPiece;
        private bool gameOver;
        private bool isPaused;
        private object stateLock = new object();
        
        // Scoring and level
        private int score;
        private int level;
        private int totalLinesCleared;

        // High score file path
        private readonly string highScoreFile = "highscore.txt";
        private int highScore;

        /// <summary>
        /// Initialize a new game
        /// </summary>
        public Game()
        {
            // Initialize grid
            grid = new int[GridHeight, GridWidth];
            
            // Load high score if exists
            LoadHighScore();
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void Start()
        {
            // Hide cursor for cleaner display
            Console.CursorVisible = false;
            
            ResetGame();
            
            // Start input thread
            Thread inputThread = new Thread(ListenForInput) { IsBackground = true };
            inputThread.Start();
            
            // Main game loop
            GameLoop();
            
            // Game over, show final score
            AnsiConsole.Clear();
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold red]GAME OVER![/]");
            AnsiConsole.MarkupLine($"Final Score: [yellow]{score}[/]");
            
            if (score > highScore)
            {
                AnsiConsole.MarkupLine("[bold green]New High Score![/]");
            }
            
            // Save the score
            SaveScore(score);
            
            // Wait for a key press before returning to menu
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue[/]");
            Console.ReadKey(true);
            
            // Show cursor again
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Reset the game state for a new game
        /// </summary>
        private void ResetGame()
        {
            // Clear grid
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    grid[y, x] = 0;
                }
            }
            
            // Reset game state
            gameOver = false;
            isPaused = false;
            score = 0;
            level = 0;
            totalLinesCleared = 0;
            
            // Spawn initial pieces
            SpawnNewPiece();
        }

        /// <summary>
        /// Main game loop
        /// </summary>
        private void GameLoop()
        {
            int fallDelay = 500; // Initial delay in milliseconds
            
            while (!gameOver)
            {
                if (isPaused)
                {
                    Thread.Sleep(100);
                    continue;
                }
                
                long frameStart = DateTime.Now.Ticks;
                
                // Move the piece down automatically
                lock (stateLock)
                {
                    if (!TryMovePieceDown())
                    {
                        // If piece can't move down, lock it and spawn a new one
                        LockPiece();
                        
                        // Check for game over (if new piece can't be placed)
                        if (IsCollision(currentPiece))
                        {
                            gameOver = true;
                            continue;
                        }
                    }
                }
                
                // Render the game
                RenderGame();
                
                // Calculate level and adjust fall speed
                level = Math.Min(9, totalLinesCleared / 10);
                fallDelay = Math.Max(50, 500 - (level * 50));
                
                // Wait for next frame
                long frameTime = (DateTime.Now.Ticks - frameStart) / TimeSpan.TicksPerMillisecond;
                int sleepTime = Math.Max(1, fallDelay - (int)frameTime);
                Thread.Sleep(sleepTime);
            }
        }

        /// <summary>
        /// Listen for keyboard input on a separate thread
        /// </summary>
        private void ListenForInput()
        {
            while (!gameOver)
            {
                try
                {
                    // The key check needs to be more robust for Docker
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(intercept: true).Key;
                        
                        lock (stateLock)
                        {
                            if (gameOver)
                                continue;
                                
                            if (key == ConsoleKey.P)
                            {
                                isPaused = !isPaused;
                                RenderGame(); // Update display to show pause state
                                continue;
                            }
                            
                            if (isPaused)
                                continue;
                                
                            switch (key)
                            {
                                case ConsoleKey.LeftArrow:
                                case ConsoleKey.A:
                                    TryMovePieceLeft();
                                    break;
                                    
                                case ConsoleKey.RightArrow:
                                case ConsoleKey.D:
                                    TryMovePieceRight();
                                    break;
                                    
                                case ConsoleKey.DownArrow:
                                case ConsoleKey.S:
                                    // Soft drop - move down faster and add points
                                    if (TryMovePieceDown())
                                        score += 1; // Add 1 point per cell moved down
                                    break;
                                    
                                case ConsoleKey.UpArrow:
                                case ConsoleKey.W:
                                    TryRotatePiece();
                                    break;
                                    
                                case ConsoleKey.Spacebar:
                                    HardDrop();
                                    break;
                            }
                        }
                        
                        // Render after input
                        RenderGame();
                    }
                }
                catch (InvalidOperationException)
                {
                    // This can happen in Docker when the console state is unreliable
                    // Just continue - it's not critical
                }
                
                // Use a shorter sleep in Docker to improve responsiveness
                Thread.Sleep(5); // Smaller delay for better responsiveness
            }
        }

        /// <summary>
        /// Render the current game state
        /// </summary>
        private void RenderGame()
        {
            try
            {
                // Clear the console without relying on ANSI escape sequences
                Console.Clear();
                
                // Display score, level and next piece
                AnsiConsole.MarkupLine($"[bold]TETRIS[/]   [grey]P: Pause[/]");
                AnsiConsole.MarkupLine($"Score: [yellow]{score}[/]  Level: [yellow]{level}[/]  Lines: [yellow]{totalLinesCleared}[/]");
                
                // Create borders with consistent characters
                string topBorder = $"╔═{new string('═', GridWidth * 2)}═╦═{new string('═', 12)}═╗";
                string bottomBorder = $"╚═{new string('═', GridWidth * 2)}═╩═{new string('═', 12)}═╝";
                string midDivider = $"╠═{new string('═', GridWidth * 2)}═╬═{new string('═', 12)}═╣";
                
                // Draw top border
                AnsiConsole.Markup($"[grey]{topBorder}[/]");
                AnsiConsole.WriteLine();
                
                // Combined grid (board + current piece)
                int[,] displayGrid = GetDisplayGrid();
                
                // Draw each row
                for (int y = 0; y < GridHeight; y++)
                {
                    // Left border
                    AnsiConsole.Markup("[grey]║ [/]");
                    
                    // Draw row
                    for (int x = 0; x < GridWidth; x++)
                    {
                        int cell = displayGrid[y, x];
                        if (cell == 0)
                        {
                            AnsiConsole.Markup("[grey]· [/]"); // Empty cell
                        }
                        else
                        {
                            string color = GetColorForCell(cell);
                            AnsiConsole.Markup($"[{color}]■ [/]"); // Filled cell
                        }
                    }
                    
                    // Right border with next piece preview or other information
                    if (y == 1)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] [bold]Next:[/]        [grey]║[/]");
                    }
                    else if (y >= 2 && y <= 5)
                    {
                        AnsiConsole.Markup("[grey]║[/] ");
                        
                        // Draw next piece
                        if (nextPiece != null)
                        {
                            int previewY = y - 2;
                            int[,] nextShape = nextPiece.GetCurrentShape();
                            string color = nextPiece.GetColor();
                            
                            for (int px = 0; px < 4; px++)
                            {
                                if (previewY >= 0 && previewY < 4 && px < 4)
                                {
                                    int cell = nextShape[previewY, px];
                                    if (cell == 0)
                                    {
                                        AnsiConsole.Markup("  ");
                                    }
                                    else
                                    {
                                        AnsiConsole.Markup($"[{color}]■ [/]");
                                    }
                                }
                            }
                            
                            AnsiConsole.MarkupLine("      [grey]║[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("              [grey]║[/]");
                        }
                    }
                    else if (y == 7)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] [bold]High Score:[/]  [grey]║[/]");
                    }
                    else if (y == 8)
                    {
                        AnsiConsole.MarkupLine($"[grey]║[/] [yellow]{highScore,12}[/] [grey]║[/]");
                    }
                    else if (y == 10)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] [bold]Controls:[/]    [grey]║[/]");
                    }
                    else if (y == 11)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] ← → : Move     [grey]║[/]");
                    }
                    else if (y == 12)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] ↑   : Rotate   [grey]║[/]");
                    }
                    else if (y == 13)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] ↓   : Down     [grey]║[/]");
                    }
                    else if (y == 14)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] Space: Drop    [grey]║[/]");
                    }
                    else if (y == 15)
                    {
                        AnsiConsole.MarkupLine("[grey]║[/] P   : Pause    [grey]║[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[grey]║               ║[/]");
                    }
                }
                
                // Bottom border
                AnsiConsole.Markup($"[grey]{bottomBorder}[/]");
                AnsiConsole.WriteLine();
                
                // Display pause message if paused
                if (isPaused)
                {
                    AnsiConsole.MarkupLine("\n[bold]GAME PAUSED - Press P to continue[/]");
                }
            }
            catch (Exception)
            {
                // Ignore rendering errors in Docker - they're not critical
                // The next render will try again
            }
        }

        /// <summary>
        /// Get a combined grid of the current state and active piece
        /// </summary>
        private int[,] GetDisplayGrid()
        {
            // Create a copy of the grid
            int[,] display = new int[GridHeight, GridWidth];
            for (int y = 0; y < GridHeight; y++)
            {
                for (int x = 0; x < GridWidth; x++)
                {
                    display[y, x] = grid[y, x];
                }
            }
            
            // Add current piece to display
            if (currentPiece != null)
            {
                int[,] pieceShape = currentPiece.GetCurrentShape();
                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (pieceShape[y, x] != 0)
                        {
                            int gridY = currentPiece.Y + y;
                            int gridX = currentPiece.X + x;
                            
                            // Only draw if inside the grid
                            if (gridY >= 0 && gridY < GridHeight && gridX >= 0 && gridX < GridWidth)
                            {
                                display[gridY, gridX] = pieceShape[y, x];
                            }
                        }
                    }
                }
            }
            
            return display;
        }

        /// <summary>
        /// Get a color for a cell value
        /// </summary>
        private string GetColorForCell(int cell)
        {
            return cell switch
            {
                1 => "aqua",         // I piece
                2 => "blue",         // J piece
                3 => "orange1",      // L piece
                4 => "yellow",       // O piece
                5 => "green",        // S piece
                6 => "purple",       // T piece
                7 => "red",          // Z piece
                99 => "white",       // Special animation color (will be overridden)
                _ => "white"
            };
        }

        /// <summary>
        /// Spawn a new tetromino at the top of the grid
        /// </summary>
        private void SpawnNewPiece()
        {
            // If this is the first piece, create both current and next
            if (nextPiece == null)
            {
                nextPiece = Tetromino.CreateRandom(0, 0);
            }
            
            // Move next piece to current
            currentPiece = nextPiece;
            
            // Position the piece at the top center
            currentPiece.X = (GridWidth / 2) - 2;
            currentPiece.Y = 0;
            
            // Generate next piece
            nextPiece = Tetromino.CreateRandom(0, 0);
        }

        /// <summary>
        /// Lock the current piece into the grid
        /// </summary>
        private void LockPiece()
        {
            int[,] shape = currentPiece.GetCurrentShape();
            
            // Copy the piece to the grid
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (shape[y, x] != 0)
                    {
                        int gridY = currentPiece.Y + y;
                        int gridX = currentPiece.X + x;
                        
                        if (gridY >= 0 && gridY < GridHeight && gridX >= 0 && gridX < GridWidth)
                        {
                            grid[gridY, gridX] = shape[y, x];
                        }
                    }
                }
            }
            
            // Check for completed lines
            int linesCleared = ClearFullLines();
            
            // Update score based on number of lines cleared
            UpdateScore(linesCleared);
            
            // Spawn a new piece
            SpawnNewPiece();
        }

        /// <summary>
        /// Clear full lines and return the number of lines cleared
        /// </summary>
        private int ClearFullLines()
        {
            int linesCleared = 0;
            List<int> fullRows = new List<int>();
            
            // First pass: detect full lines
            for (int y = GridHeight - 1; y >= 0; y--)
            {
                bool isFullLine = true;
                
                // Check if line is full
                for (int x = 0; x < GridWidth; x++)
                {
                    if (grid[y, x] == 0)
                    {
                        isFullLine = false;
                        break;
                    }
                }
                
                // If line is full, add to the list
                if (isFullLine)
                {
                    fullRows.Add(y);
                    linesCleared++;
                }
            }
            
            // If we found full lines, animate and clear them
            if (fullRows.Count > 0)
            {
                // Animate line clearing
                AnimateClearLines(fullRows);
                
                // Actually clear the lines (starting from bottom to top)
                foreach (int row in fullRows.OrderByDescending(r => r))
                {
                    // Shift all lines above down
                    for (int y = row; y > 0; y--)
                    {
                        for (int x = 0; x < GridWidth; x++)
                        {
                            grid[y, x] = grid[y - 1, x];
                        }
                    }
                    
                    // Clear the top row
                    for (int x = 0; x < GridWidth; x++)
                    {
                        grid[0, x] = 0;
                    }
                }
                
                totalLinesCleared += linesCleared;
            }
            
            return linesCleared;
        }

        /// <summary>
        /// Animate the clearing of lines with a flashing effect
        /// </summary>
        private void AnimateClearLines(List<int> rows)
        {
            // Flash the lines a few times
            string[] flashColors = { "white", "yellow", "red", "white", "yellow", "red" };
            
            for (int flash = 0; flash < flashColors.Length; flash++)
            {
                // Create a temporary copy of the grid for rendering
                int[,] displayGrid = new int[GridHeight, GridWidth];
                for (int y = 0; y < GridHeight; y++)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        displayGrid[y, x] = grid[y, x];
                    }
                }
                
                // Change the color of the full rows to the flash color
                foreach (int row in rows)
                {
                    for (int x = 0; x < GridWidth; x++)
                    {
                        if (displayGrid[row, x] != 0)
                        {
                            // Set to a special value that will be rendered as the flash color
                            displayGrid[row, x] = 99;
                        }
                    }
                }
                
                // Render the modified grid
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[bold]TETRIS[/]   [grey]P: Pause[/]");
                AnsiConsole.MarkupLine($"Score: [yellow]{score}[/]  Level: [yellow]{level}[/]  Lines: [yellow]{totalLinesCleared}[/]");
                
                // Draw borders
                string topBorder = $"╔═{new string('═', GridWidth * 2)}═╦═{new string('═', 12)}═╗";
                string bottomBorder = $"╚═{new string('═', GridWidth * 2)}═╩═{new string('═', 12)}═╝";
                
                // Draw top border
                AnsiConsole.MarkupLine($"[grey]{topBorder}[/]");
                
                // Draw each row
                for (int y = 0; y < GridHeight; y++)
                {
                    // Left border
                    AnsiConsole.Markup("[grey]║ [/]");
                    
                    // Draw row
                    for (int x = 0; x < GridWidth; x++)
                    {
                        int cell = displayGrid[y, x];
                        if (cell == 0)
                        {
                            AnsiConsole.Markup("[grey]· [/]"); // Empty cell
                        }
                        else if (cell == 99)
                        {
                            // Flash color
                            AnsiConsole.Markup($"[{flashColors[flash]}]■ [/]");
                        }
                        else
                        {
                            string color = GetColorForCell(cell);
                            AnsiConsole.Markup($"[{color}]■ [/]"); // Filled cell
                        }
                    }
                    
                    // Right border (simplified for animation)
                    AnsiConsole.MarkupLine("[grey]║[/]");
                }
                
                // Bottom border
                AnsiConsole.MarkupLine($"[grey]{bottomBorder}[/]");
                
                // Short pause between animation frames
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Update score based on number of lines cleared
        /// </summary>
        private void UpdateScore(int linesCleared)
        {
            // Score based on NES Tetris scoring system
            switch (linesCleared)
            {
                case 1:
                    score += 40 * (level + 1);
                    break;
                case 2:
                    score += 100 * (level + 1);
                    break;
                case 3:
                    score += 300 * (level + 1);
                    break;
                case 4:
                    score += 1200 * (level + 1);
                    break;
            }
        }

        /// <summary>
        /// Try to move the piece down one cell
        /// </summary>
        private bool TryMovePieceDown()
        {
            // Temporarily move down
            currentPiece.Y++;
            
            // Check if this position is valid
            if (IsCollision(currentPiece))
            {
                // If not, move back up and return false
                currentPiece.Y--;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Try to move the piece left one cell
        /// </summary>
        private bool TryMovePieceLeft()
        {
            // Temporarily move left
            currentPiece.X--;
            
            // Check if this position is valid
            if (IsCollision(currentPiece))
            {
                // If not, move back right and return false
                currentPiece.X++;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Try to move the piece right one cell
        /// </summary>
        private bool TryMovePieceRight()
        {
            // Temporarily move right
            currentPiece.X++;
            
            // Check if this position is valid
            if (IsCollision(currentPiece))
            {
                // If not, move back left and return false
                currentPiece.X--;
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Try to rotate the piece
        /// </summary>
        private bool TryRotatePiece()
        {
            // Store the current rotation
            int originalRotation = currentPiece.RotationIndex;
            
            // Rotate the piece
            currentPiece.Rotate();
            
            // Check if the rotation is valid
            if (IsCollision(currentPiece))
            {
                // Try wall kick to the left
                currentPiece.X--;
                if (!IsCollision(currentPiece))
                {
                    return true;
                }
                
                // Try wall kick to the right
                currentPiece.X += 2;
                if (!IsCollision(currentPiece))
                {
                    return true;
                }
                
                // Revert position and rotation
                currentPiece.X--;
                
                // Reverse the rotation 3 times to get back to the original rotation
                for (int i = 0; i < 3; i++)
                {
                    currentPiece.Rotate();
                }
                
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Hard drop - move the piece down until it collides
        /// </summary>
        private void HardDrop()
        {
            int cellsMoved = 0;
            
            // Keep moving down until collision
            while (true)
            {
                currentPiece.Y++;
                
                if (IsCollision(currentPiece))
                {
                    // Move back up one cell
                    currentPiece.Y--;
                    break;
                }
                
                cellsMoved++;
            }
            
            // Add points for hard drop (2 points per cell)
            score += cellsMoved * 2;
            
            // Lock the piece
            LockPiece();
        }

        /// <summary>
        /// Check if the current piece collides with the grid or goes out of bounds
        /// </summary>
        private bool IsCollision(Tetromino piece)
        {
            int[,] shape = piece.GetCurrentShape();
            
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (shape[y, x] != 0)
                    {
                        int gridY = piece.Y + y;
                        int gridX = piece.X + x;
                        
                        // Check boundaries
                        if (gridX < 0 || gridX >= GridWidth || gridY >= GridHeight)
                        {
                            return true;
                        }
                        
                        // Skip checks above the grid
                        if (gridY < 0)
                        {
                            continue;
                        }
                        
                        // Check collision with existing blocks
                        if (grid[gridY, gridX] != 0)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

        /// <summary>
        /// Load high score from file (highest score only)
        /// </summary>
        private void LoadHighScore()
        {
            highScore = 0;
            
            try
            {
                if (File.Exists(highScoreFile))
                {
                    string[] lines = File.ReadAllLines(highScoreFile);
                    
                    foreach (string line in lines)
                    {
                        if (int.TryParse(line, out int score) && score > highScore)
                        {
                            highScore = score;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If there's an error reading the file, keep highScore at 0
            }
        }

        /// <summary>
        /// Save score to the high scores file
        /// </summary>
        private void SaveScore(int newScore)
        {
            // Only save scores above 0
            if (newScore <= 0)
                return;
            
            try
            {
                // Load existing scores
                List<int> scores = new List<int>();
                
                if (File.Exists(highScoreFile))
                {
                    string[] lines = File.ReadAllLines(highScoreFile);
                    foreach (string line in lines)
                    {
                        if (int.TryParse(line, out int score))
                        {
                            scores.Add(score);
                        }
                    }
                }
                
                // Add new score
                scores.Add(newScore);
                
                // Sort scores in descending order
                scores.Sort((a, b) => b.CompareTo(a));
                
                // Write all scores back to file
                File.WriteAllLines(highScoreFile, scores.Select(s => s.ToString()));
                
                // Update highScore if necessary
                if (newScore > highScore)
                {
                    highScore = newScore;
                }
            }
            catch (Exception)
            {
                // If there's an error writing the file, silently fail
            }
        }
    }
}
