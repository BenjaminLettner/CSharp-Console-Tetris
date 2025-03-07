FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy source code
COPY *.cs ./
COPY *.md ./

# Build the application
RUN dotnet publish -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .

# Make the container more colorful (support ANSI colors)
ENV TERM=xterm-256color

# Set entry point
ENTRYPOINT ["dotnet", "TetrisGame.dll"] 