# AyBreak

A breakout game made in Windows Forms and C#.

## What is it?

AyBreak is a simple breakout game made in Windows Forms and C#.
It has been made as a school project for Ynov in about 2 weeks.
It has been chosen between 6 different projects like platformer, pong, etc, and I chose to make a breakout game because I thought it would be the funniest to make and the most interesting to do.

### Main objectives

- [x] Make a breakout game
- [x] Make a menu
- [x] Make a score system
- [x] Make a level system and selection screen
- [x] Have a level system working with a file
- [x] Make a pause menu
- [x] Make a lives system
- [x] Make a powerups system
- [ ] Make a settings menu
- [ ] Have perfect collisions
- [ ] Make a level editor
- [ ] Have different themes

### Difficulties encountered

The main difficulty encountered was the collision detection.
It was hard to make it work with the ball and the bricks.
It is currently not perfect, but it works most of the time, and it's not a big problem while playing.

The second main difficulty was to polish the game while still having the problems of Windows Forms (like the flickering or the lack of transparency).
It was fixed by not using a lot of sprites (just colored rectangles) and by having a simple black background.

## How to play

Use the arrow keys to move the paddle and hit the ball. The ball will bounce off the paddle and the bricks. The game ends when the there are no more bricks.
If the ball hits the bottom of the screen, you lose a life. You have 5 lives.
You can spawn random powerups by breaking bricks.
The powerups are:
- Ball no-clip
- Ball speed up
- Ball speed down
- Increase paddle size
- More ball
- More life
- Score add
- Random

### Controls

- Left arrow key: Move paddle left
- Right arrow key: Move paddle right
- Mouse: Move paddle
- Space: Launch balls
- R: Restart balls
- Escape: Pause game
- B: Accelerate balls

## How to build

1. Clone the repository
2. Open the solution in Visual Studio or Rider
3. Build the solution using the build button
4. Run the game using the run button

### Debug specificities

When the game is run in debug mode, you will start with 2 balls and a label will be shown for each brick, showing their health.
