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

## Requirements

- .NET SDK 6.0 or higher
- Terminal that supports ANSI color codes

## How to Run

1. Clone this repository
2. Navigate to the TetrisGame directory
3. Run the following command:

```
dotnet run
```

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

## Credits

Developed as a programming exercise using Spectre.Console for console rendering. 