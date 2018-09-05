using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public class DiscoveryController
    {
        private GameController _gameController;
        private UtilityFunctions _utilityFunctions;
        private GameResources _gameResources;

        public void DiscoveryControllerAdd(GameResources pGameResources, UtilityFunctions pUtilityFunctions, GameController pGameController)
        {
            _gameController = pGameController;
            _utilityFunctions = pUtilityFunctions;
            _gameResources = pGameResources;
        }

        /// <summary>
        /// Handles input during the discovery phase of the game.
        /// </summary>
        /// <remarks>
        /// Escape opens the game menu. Clicking the mouse will
        /// attack a location.
        /// </remarks>
        public void HandleDiscoveryInput()
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                _gameController.AddNewState(GameState.ViewingGameMenu);
            }

            if (SwinGame.MouseClicked(MouseButton.LeftButton))
            {
                DoAttack();
            }
        }

        /// <summary>
        /// Attack the location that the mouse if over.
        /// </summary>
        private void DoAttack()
        {
            Point2D mouse;

            mouse = SwinGame.MousePosition();

            //Calculate the row/col clicked
            int row;
            int col;
            row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FIELD_TOP) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
            col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 & row < _gameController.HumanPlayer.EnemyGrid.Height)
            {
                if (col >= 0 & col < _gameController.HumanPlayer.EnemyGrid.Width)
                {
                    _gameController.Attack(row, col);
                }
            }
        }

        /// <summary>
        /// Draws the game during the attack phase.
        /// </summary>s
        public void DrawDiscovery()
        {
            const int SCORES_LEFT = 172;
            const int SHOTS_TOP = 157;
            const int HITS_TOP = 206;
            const int SPLASH_TOP = 256;

            if ((SwinGame.KeyDown(KeyCode.LeftShiftKey) | SwinGame.KeyDown(KeyCode.RightShiftKey)) & SwinGame.KeyDown(KeyCode.CKey))
            {
                _utilityFunctions.DrawField(_gameController.HumanPlayer.EnemyGrid, _gameController.ComputerPlayer, true);
            }
            else
            {
                _utilityFunctions.DrawField(_gameController.HumanPlayer.EnemyGrid, _gameController.ComputerPlayer, false);
            }

            _utilityFunctions.DrawSmallField(_gameController.HumanPlayer.PlayerGrid, _gameController.HumanPlayer);
            _utilityFunctions.DrawMessage();

            SwinGame.DrawText(_gameController.HumanPlayer.Shots.ToString(), Color.White, _gameResources.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
            SwinGame.DrawText(_gameController.HumanPlayer.Hits.ToString(), Color.White, _gameResources.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
            SwinGame.DrawText(_gameController.HumanPlayer.Missed.ToString(), Color.White, _gameResources.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
        }
    }
}
