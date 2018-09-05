using MyGame.src.Model;
using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public class DeploymentController
    {
        private const int SHIPS_TOP = 98;
        private const int SHIPS_LEFT = 20;
        private const int SHIPS_HEIGHT = 90;

        private const int SHIPS_WIDTH = 300;
        private const int TOP_BUTTONS_TOP = 72;

        private const int TOP_BUTTONS_HEIGHT = 46;
        private const int PLAY_BUTTON_LEFT = 693;

        private const int PLAY_BUTTON_WIDTH = 80;
        private const int UP_DOWN_BUTTON_LEFT = 410;

        private const int LEFT_RIGHT_BUTTON_LEFT = 350;
        private const int RANDOM_BUTTON_LEFT = 547;

        private const int RANDOM_BUTTON_WIDTH = 51;

        private const int DIR_BUTTONS_WIDTH = 47;

        private const int TEXT_OFFSET = 5;
        private Direction _currentDirection = Direction.UpDown;

        private ShipName _selectedShip = ShipName.Tug;

        private GameController _gameController;
        private UtilityFunctions _utilityFunctions;
        private GameResources _gameResources;

        public void DeploymentControllerAdd(GameResources pGameResources, UtilityFunctions pUtilityFunctions, GameController pGameController)
        {
            _gameController = pGameController;
            _utilityFunctions = pUtilityFunctions;
            _gameResources = pGameResources;
        }

        //public Player HumanPlayer = new Player(new BattleShipsGame());

        /// <summary>
        /// Handles user input for the Deployment phase of the game.
        /// </summary>
        /// <remarks>
        /// Involves selecting the ships, deloying ships, changing the direction
        /// of the ships to add, randomising deployment, end then ending
        /// deployment
        /// </remarks>
        public void HandleDeploymentInput()
        {
            if (SwinGame.KeyTyped(KeyCode.EscapeKey))
            {
                _gameController.AddNewState(GameState.ViewingGameMenu);
            }

            if (SwinGame.KeyTyped(KeyCode.UpKey) | SwinGame.KeyTyped(KeyCode.DownKey))
            {
                _currentDirection = Direction.UpDown;
            }
            if (SwinGame.KeyTyped(KeyCode.LeftKey) | SwinGame.KeyTyped(KeyCode.RightKey))
            {
                _currentDirection = Direction.LeftRight;
            }

            if (SwinGame.KeyTyped(KeyCode.RKey))
            {
                _gameController.HumanPlayer.RandomizeDeployment();
            }

            if (SwinGame.MouseClicked(MouseButton.LeftButton))
            {
                ShipName selected;
                selected = GetShipMouseIsOver();
                if (selected != ShipName.None)
                {
                    _selectedShip = selected;
                }
                else
                {
                    DoDeployClick();
                }

                if (_gameController.HumanPlayer.ReadyToDeploy & _utilityFunctions.IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                {
                    _gameController.EndDeployment();
                }
                else if (_utilityFunctions.IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                {
                    _currentDirection = Direction.LeftRight;
                }
                else if (_utilityFunctions.IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                {
                    _currentDirection = Direction.LeftRight;
                }
                else if (_utilityFunctions.IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                {
                    _gameController.HumanPlayer.RandomizeDeployment();
                }
            }
        }

        /// <summary>
        /// The user has clicked somewhere on the screen, check if its is a deployment and deploy
        /// the current ship if that is the case.
        /// </summary>
        /// <remarks>
        /// If the click is in the grid it deploys to the selected location
        /// with the indicated direction
        /// </remarks>
        private void DoDeployClick()
        {
            Point2D mouse;

            mouse = SwinGame.MousePosition();

            //Calculate the row/col clicked
            int row;
            int col;
            row = Convert.ToInt32(Math.Floor((mouse.Y) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
            col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FIELD_LEFT) / (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP)));

            if (row >= 0 & row < _gameController.HumanPlayer.PlayerGrid.Height)
            {
                if (col >= 0 & col < _gameController.HumanPlayer.PlayerGrid.Width)
                {
                    //if in the area try to deploy
                    try
                    {
                        _gameController.HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection);
                    }

                    catch (Exception ex)
                    {
                        Audio.PlaySoundEffect(_gameResources.GameSound("Error"));
                        _utilityFunctions.Message = ex.Message;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the deployment screen showing the field and the ships
        /// that the player can deploy.
        /// </summary>
        public void DrawDeployment()
        {
            _utilityFunctions.DrawField(_gameController.HumanPlayer.PlayerGrid, _gameController.HumanPlayer, true);

            //Draw the Left/Right and Up/Down buttons
            if (_currentDirection == Direction.LeftRight)
            {
                SwinGame.DrawBitmap(_gameResources.GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
                //SwinGame.DrawText("U/D", Color.Gray, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
                //SwinGame.DrawText("L/R", Color.White, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
            }

            else
            {
                SwinGame.DrawBitmap(_gameResources.GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
                //SwinGame.DrawText("U/D", Color.White, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
                //SwinGame.DrawText("L/R", Color.Gray, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
            }

            var varShipNameLoop = Enum.GetValues(typeof(ShipName));

            //DrawShips
            foreach (ShipName sn in varShipNameLoop)
            {
                int i;
                i = (int) sn - 1;
                if (i >= 0)
                {
                    if (sn == _selectedShip)
                    {
                        SwinGame.DrawBitmap(_gameResources.GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT);
                        //    SwinGame.FillRectangle(Color.LightBlue, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                        //Else
                        //    SwinGame.FillRectangle(Color.Gray, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                    }

                    //SwinGame.DrawRectangle(Color.Black, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                    //SwinGame.DrawText(sn.ToString(), Color.Black, GameFont("Courier"), SHIPS_LEFT + TEXT_OFFSET, SHIPS_TOP + i * SHIPS_HEIGHT)

                }
            }

            if (_gameController.HumanPlayer.ReadyToDeploy)
            {
                SwinGame.DrawBitmap(_gameResources.GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP);
                //SwinGame.FillRectangle(Color.LightBlue, PLAY_BUTTON_LEFT, PLAY_BUTTON_TOP, PLAY_BUTTON_WIDTH, PLAY_BUTTON_HEIGHT)
                //SwinGame.DrawText("PLAY", Color.Black, GameFont("Courier"), PLAY_BUTTON_LEFT + TEXT_OFFSET, PLAY_BUTTON_TOP)
            }

            SwinGame.DrawBitmap(_gameResources.GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP);

            _utilityFunctions.DrawMessage();
        }

        /// <summary>
        /// Gets the ship that the mouse is currently over in the selection panel.
        /// </summary>
        /// <returns>The ship selected or none</returns>
        private ShipName GetShipMouseIsOver()
        {
            var varShipNameLoop = Enum.GetValues(typeof(ShipName));

            foreach (ShipName sn in varShipNameLoop)
            {
                int i;
                i = (int) sn - 1;

                if (_utilityFunctions.IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT))
                {
                    return sn;
                }
            }

            return ShipName.None;
        }
    }
}
