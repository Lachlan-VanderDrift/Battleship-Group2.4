﻿using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public static class EndingGameController
    {
        /// <summary>
        /// Draw the end of the game screen, shows the win/lose state
        /// </summary>
        public static void DrawEndOfGame()
        {

            Rectangle toDraw = new Rectangle();
            string whatShouldIPrint;

            UtilityFunctions.DrawField(GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
            UtilityFunctions.DrawSmallField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

            toDraw.X = 0;
            toDraw.Y = 250;
            toDraw.Width = SwinGame.ScreenWidth();
            toDraw.Height = SwinGame.ScreenHeight();

            if (GameController.HumanPlayer.IsDestroyed)
            {
                whatShouldIPrint = "YOU LOSE!";
            }
            else
            {
                whatShouldIPrint = "-- WINNER --";
            }

            string strHolder = "ArialLarge";
            //GameResources gameResources = new GameResources();

            //SwinGame.DrawTextLines(whatShouldIPrint, Color.White, Color.Transparent, gameResources.GameFont(strHolder), FontAlignment.AlignCenter, toDraw);

            SwinGame.DrawText(whatShouldIPrint, Color.White, Color.Transparent, GameResources.GameFont(strHolder), FontAlignment.AlignCenter, toDraw);
            SwinGame.DrawText("Click to Continue", Color.White, Color.Transparent, GameResources.GameFont(strHolder), FontAlignment.AlignCenter - 5, toDraw);
        }

        /// <summary>
        /// Handle the input during the end of the game. Any interaction
        /// will result in it reading in the highsSwinGame.
        /// </summary>
        public static void HandleEndOfGameInput()
        {
            if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.ReturnKey) || SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                HighScoreController.ReadHighScore(GameController.HumanPlayer.Score);
                GameController.EndCurrentState();
            }
        }
    }
}
