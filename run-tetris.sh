#!/bin/bash

# Check if docker is installed
if ! command -v docker &> /dev/null; then
    echo "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if docker-compose is installed
if command -v docker-compose &> /dev/null; then
    echo "Starting Tetris game with docker-compose..."
    docker-compose up --build
else
    echo "docker-compose not found. Using docker commands instead..."
    echo "Building Tetris game Docker image..."
    docker build -t tetris-game .
    
    echo "Running Tetris game..."
    # Create empty highscore file if it doesn't exist
    touch highscore.txt
    
    # Run the container with the highscore volume
    docker run -it --rm \
      -v "$(pwd)/highscore.txt:/app/highscore.txt" \
      tetris-game
fi 