using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;
using GameLibrary;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Core;
using Windows.Media.Playback;
using System.Linq;

namespace GameInterface
{
    public sealed partial class MainPage : Page
    {

        #region variables
        private static GamePiece player;
        private List<GamePiece> dots = new List<GamePiece>();
        private List<GamePiece> enemies = new List<GamePiece>();
        private int score = 0;
        private int level = 1;
        private int dotsPerLevel = 10;
        private int scoreThreshold = 100;
        private DispatcherTimer gameLoopTimer;
        private DispatcherTimer levelTimer;
        private double screenWidth;
        private double screenHeight;
        private int timeRemaining = 30;
        private List<int> highScores;
        private HighScoreManager highScoreManager = new HighScoreManager();
        #endregion

        public MainPage()
        {
            this.InitializeComponent();
            highScores = highScoreManager.ReadHighScores();
            UpdateHighScoreDisplay();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Initialize the game loop timer
            gameLoopTimer = new DispatcherTimer();
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameLoopTimer.Tick += GameLoopTimer_Tick;
            gameLoopTimer.Start();

            // Initialize the level timer
            levelTimer = new DispatcherTimer();
            levelTimer.Interval = TimeSpan.FromSeconds(1);
            levelTimer.Tick += LevelTimer_Tick;
            levelTimer.Start();

            // Get the screen dimensions
            screenWidth = Window.Current.Bounds.Width;
            screenHeight = Window.Current.Bounds.Height;

            CreateDots(dotsPerLevel);
            player = CreatePiece("player", 100, 50, 50);
            CreateEnemies(1);


            // Initialize the sound player
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/pacman_beginning.wav"));
            // Play the sound
            mediaPlayer.Play();

        }
        #region Updates
        private void UpdateHighScoreDisplay()
        {
            highScores = highScores.OrderByDescending(score => score).ToList();

            // Update the high score text block
            txtHighScores.Text = "High Scores:\n";
            for (int i = 0; i < Math.Min(10, highScores.Count); i++)
            {
                txtHighScores.Text += (i + 1) + ". " + highScores[i] + "\n";
            }
        }
        #endregion

        #region Timer 
        private void LevelTimer_Tick(object sender, object e)
        {
            // Update the timer text
            txtTimer.Text = "Time Left: " + timeRemaining + "s";

            if (timeRemaining > 0)
            {
                timeRemaining--;
            }
            else
            {
                
                Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
                EndGame();
                return;
            }

            if (score >= scoreThreshold)
            {
                // Advance to the next level and update the score threshold
                level++;
                scoreThreshold += 100;

                int levelTimeLimit = 30 + (level - 1) * 10;
                timeRemaining = levelTimeLimit;

                CreateDots(dotsPerLevel);
                CreateEnemies(level);
                UpdateLevelText();
            }
        }

        private void GameLoopTimer_Tick(object sender, object e)
        {
            // Update the positions of dots
            foreach (var dot in dots)
            {
                dot.MoveObject();
            }

            // Update the positions of enemies
            foreach (var enemy in enemies)
            {
                enemy.MoveObject();
            }
        }
        #endregion

        #region CreatePisce
        private GamePiece CreatePiece(string imgSrc, int size, int left, int top)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{imgSrc}.png"));
            img.Width = 30;
            img.Height = 30;
            img.Name = $"img{imgSrc}";
            img.Margin = new Thickness(left, top, 0, 0);
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;

            gridMain.Children.Add(img);

            return new GamePiece(img);
        }
        #endregion

        #region Create the dots and the enemies 
        private void CreateDots(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int dotSize = 30;
                int left = new Random().Next(0, (int)(Window.Current.Bounds.Width - dotSize));
                int top = new Random().Next(0, (int)(Window.Current.Bounds.Height - dotSize));

                left = Math.Max(left, 0);
                top = Math.Max(top, 0);

                var dot = CreatePiece("dot", dotSize, left, top);
                dots.Add(dot);
            }
        }

        private void CreateEnemies(int count)
        {
           
            foreach (var enemy in enemies)
            {
                gridMain.Children.Remove(enemy.onScreen);
            }
            enemies.Clear();

            for (int i = 0; i < count; i++)
            {
                int left = new Random().Next(0, (int)Window.Current.Bounds.Width - 30);
                int top = new Random().Next(0, (int)Window.Current.Bounds.Height - 30);
                left = Math.Max(left, 0);
                top = Math.Max(top, 0);
                var enemy = CreatePiece("enemy", 50, left, top);
                enemies.Add(enemy);

                enemy.MoveObject();
            }
        }
        #endregion

        #region CoreWindow_KeyDown
        private void CoreWindow_KeyDown(object sender, Windows.UI.Core.KeyEventArgs e)
        {
            if (timeRemaining <= 0)
            {
                return;
            }
            // Calculate new location for the player character
            player.Move(e.VirtualKey);

            for (int i = dots.Count - 1; i >= 0; i--)
            {
                double playerCenterX = player.Location.Left + player.onScreen.Width / 2;
                double playerCenterY = player.Location.Top + player.onScreen.Height / 2;

                double dotCenterX = dots[i].Location.Left + dots[i].onScreen.Width / 2;
                double dotCenterY = dots[i].Location.Top + dots[i].onScreen.Height / 2;

                double distance = Math.Sqrt(Math.Pow(playerCenterX - dotCenterX, 2) + Math.Pow(playerCenterY - dotCenterY, 2));

                if (distance < (player.onScreen.Width / 2) + (dots[i].onScreen.Width / 2))
                {
                    // Play the "pacman_chomp.wav" sound when a dot is hit
                    PlayChompSound();

                    // Remove the collected dot from the tree
                    gridMain.Children.Remove(dots[i].onScreen);
                    dots.RemoveAt(i);

                    score += 10;
                }
            }

            foreach (var enemy in enemies)
            {
                double playerEnemyDistance = Math.Sqrt(Math.Pow(player.Location.Left - enemy.Location.Left, 2) +
                    Math.Pow(player.Location.Top - enemy.Location.Top, 2));

                if (playerEnemyDistance < (player.onScreen.Width / 2) + (enemy.onScreen.Width / 2))
                {
                    // Play the "pacman_death.wav" sound when an enemy is hit
                    PlayDeathSound();

                    EndGame();
                    return;
                }
            }
            txtScore.Text = "Score: " + score;
        }
        #endregion

        #region sounds track for the game
        private void PlayChompSound()
        {
            MediaPlayer chompSoundPlayer = new MediaPlayer();
            chompSoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/pacman_chomp.wav"));
            chompSoundPlayer.Play();
        }

        private void PlayDeathSound()
        {
            MediaPlayer deathSoundPlayer = new MediaPlayer();
            deathSoundPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/pacman_death.wav"));
            deathSoundPlayer.Play();
        }
        #endregion

        #region EndGame Method
        private async void EndGame()
        {
            Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;

            // Display a game over message
            var dialog = new MessageDialog("Game Over! Your final score: " + score);
            dialog.Commands.Add(new UICommand("Restart", new UICommandInvokedHandler(async (cmd) =>
            {
                // Stop the game loop timer and level timer
                gameLoopTimer.Stop();
                levelTimer.Stop();

                // Reset game variables to their initial state
                score = 0;
                level = 1;
                scoreThreshold = 100;
                dotsPerLevel = 10;
                timeRemaining = 30;

                // Clear the dots and enemies
                foreach (var dot in dots)
                {
                    gridMain.Children.Remove(dot.onScreen);
                }
                dots.Clear();

                foreach (var enemy in enemies)
                {
                    gridMain.Children.Remove(enemy.onScreen);
                }
                enemies.Clear();

               
                CreateDots(dotsPerLevel);
                CreateEnemies(1);

               
                player.ResetPosition(100, 50);
 
                UpdateLevelText();

                Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

                // Restart the game loop timer and level timer
                gameLoopTimer.Start();
                levelTimer.Start();
            })));

            highScores.Add(score); 
            highScoreManager.WriteHighScores(highScores); 
            UpdateHighScoreDisplay(); 

            dialog.Commands.Add(new UICommand("Quit", new UICommandInvokedHandler(async (cmd) =>
            {
                Application.Current.Exit();
            })));

            await dialog.ShowAsync();
        }
        #endregion

        private void UpdateLevelText()
        {
            txtLevel.Text = "Level " + level;
        }

    }
}
