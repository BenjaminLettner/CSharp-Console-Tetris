#!/bin/bash

# Check if docker is installed
if ! command -v docker &> /dev/null; then
    echo "Docker is not installed. Please install Docker first."
    exit 1
fi

# Create empty highscore file if it doesn't exist
touch highscore.txt

# Check if running on Mac (different Docker parameters needed)
if [[ "$(uname)" == "Darwin" ]]; then
    echo "Detected macOS, using compatible Docker settings..."
    
    # For Mac, we need to explicitly set the TTY
    docker build -t tetris-game .
    
    # Run with extended terminal options for Mac
    docker run -it --rm \
      -e TERM=xterm-256color \
      -e DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION=true \
      -e DOTNET_CONSOLE_ANSI=true \
      -v "$(pwd)/highscore.txt:/app/highscore.txt" \
      tetris-game
else
    # Linux or other OS
    echo "Starting Tetris game with docker-compose..."
    docker-compose up --build
fi 