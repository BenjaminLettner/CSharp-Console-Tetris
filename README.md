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

- .NET SDK 8.0 or higher
- Terminal that supports ANSI color codes
- OR Docker (see Docker section below)

## How to Run

### Standard Method
1. Clone this repository
2. Navigate to the TetrisGame directory
3. Run the following command:

```
dotnet run
```

### Docker Method
You can also run the game using Docker, which doesn't require .NET to be installed on your system:

1. Clone this repository
2. Navigate to the TetrisGame directory
3. Build and run the Docker container:

```bash
# Using the provided script (recommended)
./run-tetris.sh

# OR using docker-compose
docker-compose up --build

# OR using Docker directly
docker build -t tetris-game .
docker run -it --rm tetris-game
```

Note: The Docker version uses a volume to persist high scores between container runs.

### Docker Troubleshooting

If you encounter issues running the game in Docker:

1. **Input Problems**: If keyboard input doesn't work correctly:
   - Try using `run-tetris.sh` which contains specific optimizations for different operating systems
   - Ensure you're running with `-it` flags for interactive mode
   - Try a different terminal emulator

2. **Display Issues**: If the game display looks odd:
   - Make sure your terminal supports ANSI colors
   - Try increasing your terminal window size
   - Use a monospace font in your terminal

3. **MacOS Specific**: On Mac, use the `run-tetris.sh` script which handles Mac-specific Docker quirks

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