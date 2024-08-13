# GameInterface

## Description
GameInterface is a 2D game built using the Universal Windows Platform (UWP) framework. The game features a player character that collects dots while avoiding enemies on the screen. The game includes several levels, a timer, a score system, and a high score manager. The game uses sound effects to enhance the gameplay experience.

## Features
- **Player Movement:** Move the player character using the W, A, S, D keys.
- **Enemies and Dots:** Collect dots to increase your score while avoiding enemies that move randomly on the screen.
- **Levels:** The game advances through multiple levels as the player collects dots and reaches score thresholds.
- **Timer:** Each level has a time limit, requiring the player to collect dots and avoid enemies within the allotted time.
- **High Scores:** The game keeps track of the highest scores and displays them on the main screen.

## Installation
2. Open the solution in Visual Studio.
3. Restore any NuGet packages if required.
4. Build and run the project on your local machine.

## How to Play
- Use the **W, A, S, D** keys to move your character around the screen.
- Collect dots to increase your score.
- Avoid enemies, as colliding with them will end the game.
- Progress through levels by reaching the score threshold for each level.
- Keep track of your high scores on the main screen.

## Files Overview
- **MainPage.xaml.cs:** The main game logic, including player movement, enemy behavior, and game loop management.
- **GamePiece.cs:** Defines the game piece class, including movement logic for the player and enemies.
- **HighScoreManager.cs:** Manages reading and writing high scores to a file.
- **AssemblyInfo.cs:** Metadata about the assembly.
- **GameLibrary.rd.xml & Default.rd.xml:** Runtime directives for the .NET Native optimizer.

## Credits
- **Original Code:** Melissa VanderLely
- **Modifications:** Qasem Yafeai

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

