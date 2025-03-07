# C# Console Tetris Game

A feature-complete Tetris game implementation in C# using Spectre.Console for rich text UI in the console, with multithreading for responsive gameplay.

## Features

- Classic Tetris gameplay with all 7 tetromino shapes (I, J, L, O, S, T, Z)
- Multiple rotation states for each piece with wall kick functionality
- Scoring system based on the NES Tetris scoring rules
- Level progression with increasing speed
- Next piece preview
- High score tracking with top scores table
- Interactive main menu with game options
- Comprehensive instructions screen
- Line clearing animation with flashing effect
- Clean and consistent UI with proper borders and alignment
- Controls for movement, rotation, soft drop, and hard drop
- Pause functionality
- Docker support for easy deployment and play

## Requirements

### Standard Method Requirements
- Git (for cloning the repository)
- .NET SDK 8.0 or higher
- Terminal that supports ANSI color codes (like Terminal on macOS, PowerShell/Windows Terminal on Windows, or most Linux terminals)

### Docker Method Requirements
- Git (for cloning the repository)
- Docker Desktop (for Windows and macOS) or Docker Engine (for Linux)
- A terminal with adequate size to display the game board properly

## Installation and Running Guide

### 1. Clone the Repository

**Using HTTPS:**
```bash
git clone https://github.com/BenjaminLettner/CSharp-Console-Tetris.git
```

**Using SSH (if you have SSH keys set up):**
```bash
git clone git@github.com:BenjaminLettner/CSharp-Console-Tetris.git
```

**Using GitHub CLI:**
```bash
gh repo clone BenjaminLettner/CSharp-Console-Tetris
```

### 2. Navigate to the Project Directory

```bash
cd CSharp-Console-Tetris
```

### 3. Run the Game

#### Standard Method (.NET)

**Verify .NET Installation:**
First, ensure you have the correct .NET SDK version installed:
```bash
dotnet --version
```
The output should be 8.0.xxx or higher.

**Build and Run:**
```bash
dotnet build
dotnet run
```

**Or simply run directly:**
```bash
dotnet run
```

#### Docker Method

The Docker method allows you to run the game without installing .NET. The game runs inside a container with all necessary dependencies.

**Option 1: Using the provided script (Recommended)**

This script automatically detects your OS and runs with optimized settings:
```bash
# Make sure the script is executable
chmod +x run-tetris.sh

# Run the script
./run-tetris.sh
```

**Option 2: Using docker-compose**
```bash
docker-compose up --build
```

**Option 3: Using Docker commands directly**
```bash
# Build the Docker image
docker build -t tetris-game .

# Run the container
docker run -it --rm -v "$(pwd)/highscore.txt:/app/highscore.txt" tetris-game
```

### Platform-Specific Tips

**Windows:**
- Use Windows Terminal or PowerShell for best results
- If using Command Prompt, ANSI colors might not display correctly
- Make sure your terminal window is large enough (at least 80×30 characters)

**macOS:**
- Terminal.app or iTerm2 work well with the game
- The Docker script has specific optimizations for macOS

**Linux:**
- Most terminal emulators work well with both methods
- For Docker, the host network mode is used by default

## Main Menu

The game features a user-friendly main menu with the following options:

- **Start Game**: Begin a new Tetris game
- **High Scores**: View the top 10 highest scores
- **Instructions**: Learn how to play the game
- **Exit**: Quit the application

Navigate through the menu using the arrow keys and press Enter to select an option.

## Controls

- **←, →** or **A, D**: Move piece left/right
- **↑** or **W**: Rotate piece clockwise
- **↓** or **S**: Soft drop (move down faster and earn 1 point per cell)
- **Spacebar**: Hard drop (immediately drop the piece to the bottom and earn 2 points per cell)
- **P**: Pause/Resume game

## Game Rules

- The game ends when a new piece cannot be placed at the top of the grid
- Lines are cleared when they are completely filled with blocks
- Points are awarded based on the number of lines cleared simultaneously:
  - 1 line: 40 × (level + 1) points
  - 2 lines: 100 × (level + 1) points
  - 3 lines: 300 × (level + 1) points
  - 4 lines (Tetris): 1200 × (level + 1) points
- Level increases for every 10 lines cleared
- As the level increases, pieces fall faster

## Troubleshooting

### General Issues
- If the game's display looks odd, try increasing your terminal window size
- Make sure you're using a monospace font in your terminal
- Ensure your terminal supports ANSI color codes

### Docker Troubleshooting

1. **Input Problems**: If keyboard input doesn't work correctly:
   - Try using `run-tetris.sh` which contains specific optimizations for different operating systems
   - Ensure you're running with `-it` flags for interactive mode
   - Try a different terminal emulator

2. **Display Issues**: If the game display looks odd:
   - Make sure your terminal supports ANSI colors
   - Try increasing your terminal window size
   - Use a monospace font in your terminal

3. **MacOS Specific**: On Mac, use the `run-tetris.sh` script which handles Mac-specific Docker quirks

## Architecture

The game is implemented using object-oriented principles:

- **Menu class**: Handles the main menu system and user navigation
- **Game class**: Manages the game logic, including piece movement, collision detection, line clearing, and scoring
- **Tetromino class**: Represents a Tetris piece with position, rotation, and shape
- **Multithreading**: Uses separate threads for game loop and input handling for responsive gameplay
- **Spectre.Console**: Used for rendering the game board, menus, pieces, and UI elements with colors

## Docker Implementation

The Docker implementation uses a multi-stage build to keep the image size small:
1. First stage uses the .NET SDK image to build the application
2. Second stage uses the smaller .NET runtime image to run the game
3. Terminal settings are configured to ensure ANSI color support
4. A volume is used to persist high scores between container runs

## Credits

Developed as a programming exercise using Spectre.Console for console rendering.

## Contributing

If you'd like to contribute to this project, please feel free to open issues or submit pull requests on GitHub. 