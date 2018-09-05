using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public class EndingGameController
    {
        private UtilityFunctions _utilityFunctions;
        private GameController _gameController;
        private HighScoreController _highScoreController;

        public void EndingGameControllerAdd(HighScoreController pHighScoreControl, UtilityFunctions pUtilityFunctions, GameController pGameController)
        {
            _utilityFunctions = pUtilityFunctions;
            _gameController = pGameController;
            _highScoreController = pHighScoreControl;
        }

        /// <summary>
        /// Draw the end of the game screen, shows the win/lose state
        /// </summary>
        public void DrawEndOfGame()
        {

            Rectangle toDraw = new Rectangle();
            string whatShouldIPrint;

            _utilityFunctions.DrawField(_gameController.ComputerPlayer.PlayerGrid, _gameController.ComputerPlayer, true);
            _utilityFunctions.DrawSmallField(_gameController.HumanPlayer.PlayerGrid, _gameController.HumanPlayer);

            toDraw.X = 0;
            toDraw.Y = 250;
            toDraw.Width = SwinGame.ScreenWidth();
            toDraw.Height = SwinGame.ScreenHeight();

            if (_gameController.HumanPlayer.IsDestroyed)
            {
                whatShouldIPrint = "YOU LOSE!";
            }
            else
            {
                whatShouldIPrint = "-- WINNER --";
            }

            string strHolder = "ArialLarge";
            GameResources gameResources = new GameResources();

            //SwinGame.DrawTextLines(whatShouldIPrint, Color.White, Color.Transparent, gameResources.GameFont(strHolder), FontAlignment.AlignCenter, toDraw);

            SwinGame.DrawText(whatShouldIPrint, Color.White, Color.Transparent, gameResources.GameFont(strHolder), FontAlignment.AlignCenter, toDraw);
        }

        /// <summary>
        /// Handle the input during the end of the game. Any interaction
        /// will result in it reading in the highsSwinGame.
        /// </summary>
        public void HandleEndOfGameInput()
        {
            if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.ReturnKey) || SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                _highScoreController.ReadHighScore(_gameController.HumanPlayer.Score);
                _gameController.EndCurrentState();
            }
        }
    }
}
