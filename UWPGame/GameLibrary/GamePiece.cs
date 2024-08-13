using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


/* UWP Game Library
 * Written By: Melissa VanderLely
 * Modified By: Qasem yafeai
 */

namespace GameLibrary
{
    public class GamePiece
    {
        private Thickness objectMargins;            //represents the location of the piece on the game board
        public Image onScreen;
        public double Left { get; set; }
        public double Top { get; set; }
        public Thickness Location                     //get access only - can not directly modify the location of the piece
        {
            get { return onScreen.Margin; }
        }

        public GamePiece(Image img)                 //constructor creates a piece and a reference to its associated image
        {                                           //use this to set up other GamePiece properties
            onScreen = img;
            objectMargins = img.Margin;
        }

        public void ResetPosition(double x, double y)
        {
            // Set the new position for the player
            Left = x;
            Top = y;

            // Update the visual position of the image
            onScreen.Margin = new Thickness(Left, Top, 0, 0);
        }

        #region Player Movement
        public bool Move(Windows.System.VirtualKey direction)
        {
            double moveDistance = 20;
            double moveX = 0;
            double moveY = 0;

            // Calculate the movement based on the key
            if (direction == Windows.System.VirtualKey.W)
            {
                moveY = -moveDistance;
            }
            if (direction == Windows.System.VirtualKey.S)
            {
                moveY = moveDistance;
            }
            if (direction == Windows.System.VirtualKey.A)
            {
                moveX = -moveDistance;
            }
            if (direction == Windows.System.VirtualKey.D)
            {
                moveX = moveDistance;
            }

            // Update the position
            objectMargins.Left += moveX;
            objectMargins.Top += moveY;

            // Wrap around the screen horizontally
            if (objectMargins.Left < 0)
            {
                objectMargins.Left = Window.Current.Bounds.Width;
            }
            else if (objectMargins.Left > Window.Current.Bounds.Width)
            {
                objectMargins.Left = 0;
            }

            // Wrap around the screen vertically
            if (objectMargins.Top < 0)
            {
                objectMargins.Top = Window.Current.Bounds.Height;
            }
            else if (objectMargins.Top > Window.Current.Bounds.Height)
            {
                objectMargins.Top = 0;
            }

            onScreen.Margin = objectMargins;

            return true;
        }
        #endregion


        #region Object Movement
        public void MoveObject()
        {
            Random random = new Random();
            double moveDistance = 20;
            double moveX = random.NextDouble() * moveDistance * 2 - moveDistance;
            double moveY = random.NextDouble() * moveDistance * 2 - moveDistance;

            // Update the position
            objectMargins.Left += moveX;
            objectMargins.Top += moveY;

            //enemies stay within the screen bounds
            objectMargins.Left = Math.Max(0, Math.Min(Window.Current.Bounds.Width - onScreen.Width, objectMargins.Left));
            objectMargins.Top = Math.Max(0, Math.Min(Window.Current.Bounds.Height - onScreen.Height, objectMargins.Top));

            onScreen.Margin = objectMargins;
        }
        #endregion

    }
}
