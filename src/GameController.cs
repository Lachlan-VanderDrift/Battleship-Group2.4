﻿using MyGame.src.Model;
using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public class GameController
    {
        private BattleShipsGame _theGame;
        private Player _human;

        private AIPlayer _ai;

        private Stack<GameState> _state = new Stack<GameState>();

        private AIOptions _aiSetting;

        private UtilityFunctions _utilityFunctions;
        private GameResources _gameResources;
        private MenuController _menuController;
        private DeploymentController _deploymentController;
        private DiscoveryController _discoveryController;
        private EndingGameController _endingGameController;
        private HighScoreController _highScoreController;
        /// <summary>
        /// Returns the current state of the game, indicating which screen is
        /// currently being used
        /// </summary>
        /// <value>The current state</value>
        /// <returns>The current state</returns>
        public GameState CurrentState
        {
            get
            {
                return _state.Peek();
            }
        }

        /// <summary>
        /// Returns the human player.
        /// </summary>
        /// <value>the human player</value>
        /// <returns>the human player</returns>
        public Player HumanPlayer
        {
            get
            {
                return _human;
            }
        }

        /// <summary>
        /// Returns the computer player.
        /// </summary>
        /// <value>the computer player</value>
        /// <returns>the conputer player</returns>
        public Player ComputerPlayer
        {
            get
            {
                return _ai;
            }
        }

        public GameController(HighScoreController pHighScoreController, EndingGameController pEndGameController, DiscoveryController pDiscoveryController, DeploymentController pDeploymentController, UtilityFunctions pUtilityFunctions, GameResources pGameResources, MenuController pMenuController)
        {
            //this method creates all the controllers throughout the program to make them useable in this GameController Class
            _utilityFunctions = pUtilityFunctions;
            _gameResources = pGameResources;
            _menuController = pMenuController;
            _deploymentController = pDeploymentController;
            _discoveryController = pDiscoveryController;
            _endingGameController = pEndGameController;
            _highScoreController = pHighScoreController;

            //bottom state will be quitting. If player exits main menu then the game is over
            _state.Push(GameState.Quitting);

            //at the start the player is viewing the main menu
            _state.Push(GameState.ViewingMainMenu);
        }

        public void UpdateGameControl(HighScoreController pHighScoreController, EndingGameController pEndGameController, DiscoveryController pDiscoveryController, DeploymentController pDeploymentController, UtilityFunctions pUtilityFunctions, GameResources pGameResources, MenuController pMenuController)
        {
            //this was designed to update classes after they have been updated
            _utilityFunctions = pUtilityFunctions;
            _gameResources = pGameResources;
            _menuController = pMenuController;
            _deploymentController = pDeploymentController;
            _discoveryController = pDiscoveryController;
            _endingGameController = pEndGameController;
            _highScoreController = pHighScoreController;
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// <remarks>
        /// Creates an AI player based upon the _aiSetting.
        /// </remarks>
        public void StartGame()
        {
            if (_theGame != null)
                EndGame();

            //Create the game
            _theGame = new BattleShipsGame();

            //create the players
            switch (_aiSetting)
            {
                case AIOptions.Medium:
                    _ai = new AIMediumPlayer(_theGame);
                    break;
                case AIOptions.Hard:
                    _ai = new AIHardPlayer(_theGame);
                    break;
                default:
                    _ai = new AIHardPlayer(_theGame);
                    break;
            }

            _human = new Player(_theGame);

            //AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
            _ai.PlayerGrid.Changed += GridChanged;
            _theGame.AttackCompleted += AttackCompleted;

            AddNewState(GameState.Deploying);
        }

        /// <summary>
        /// Stops listening to the old game once a new game is started
        /// </summary>

        private void EndGame()
        {
            //RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
            _ai.PlayerGrid.Changed -= GridChanged;
            _theGame.AttackCompleted -= AttackCompleted;
        }

        /// <summary>
        /// Listens to the game grids for any changes and redraws the screen
        /// when the grids change
        /// </summary>
        /// <param name="sender">the grid that changed</param>
        /// <param name="args">not used</param>
        private void GridChanged(object sender, EventArgs args)
        {
            DrawScreen();
            SwinGame.RefreshScreen();
        }

        private void PlayHitSequence(int row, int column, bool showAnimation)
        {
            //this method is designed so that once a ship has been hit, it adds an animation and a sound
            if (showAnimation)
            {
                _utilityFunctions.AddExplosion(row, column);
            }

            Audio.PlaySoundEffect(_gameResources.GameSound("Hit"));

            _utilityFunctions.DrawAnimationSequence();
        }

        private void PlayMissSequence(int row, int column, bool showAnimation)
        {
            //this method is designed so that once a player has missed, it adds an animation and a sound
            if (showAnimation)
            {
                _utilityFunctions.AddSplash(row, column);
            }

            Audio.PlaySoundEffect(_gameResources.GameSound("Miss"));

            _utilityFunctions.DrawAnimationSequence();
        }

        /// <summary>
        /// Listens for attacks to be completed.
        /// </summary>
        /// <param name="sender">the game</param>
        /// <param name="result">the result of the attack</param>
        /// <remarks>
        /// Displays a message, plays sound and redraws the screen
        /// </remarks>
        private void AttackCompleted(object sender, AttackResult result)
        {
            bool isHuman;
            isHuman = object.ReferenceEquals(_theGame.Player, HumanPlayer);

            if (isHuman)
            {
                _utilityFunctions.Message = "You " + result.ToString();
            }
            else
            {
                _utilityFunctions.Message = "The AI " + result.ToString();
            }

            switch (result.Value)
            {
                case ResultOfAttack.Destroyed:
                    PlayHitSequence(result.Row, result.Column, isHuman);

                    Audio.PlaySoundEffect(_gameResources.GameSound("Sink"));
                    break;
                case ResultOfAttack.GameOver:
                    PlayHitSequence(result.Row, result.Column, isHuman);
                    Audio.PlaySoundEffect(_gameResources.GameSound("Sink"));


                    while (Audio.SoundEffectPlaying(_gameResources.GameSound("Sink")))
                    {
                        SwinGame.Delay(10);
                        SwinGame.RefreshScreen();
                    }

                    if (HumanPlayer.IsDestroyed)
                    {
                        Audio.PlaySoundEffect(_gameResources.GameSound("Lose"));
                    }
                    else
                    {
                        Audio.PlaySoundEffect(_gameResources.GameSound("Winner"));

                    }
                    break;
                case ResultOfAttack.Hit:
                    PlayHitSequence(result.Row, result.Column, isHuman);
                    break;
                case ResultOfAttack.Miss:
                    PlayMissSequence(result.Row, result.Column, isHuman);
                    break;
                case ResultOfAttack.ShotAlready:
                    Audio.PlaySoundEffect(_gameResources.GameSound("Error"));
                    break;
            }
        }

        /// <summary>
        /// Completes the deployment phase of the game and
        /// switches to the battle mode (Discovering state)
        /// </summary>
        /// <remarks>
        /// This adds the players to the game before switching
        /// state.
        /// </remarks>
        public void EndDeployment()
        {
            //deploy the players
            _theGame.AddDeployedPlayer(_human);
            _theGame.AddDeployedPlayer(_ai);

            SwitchState(GameState.Discovering);
        }

        /// <summary>
        /// Gets the player to attack the indicated row and column.
        /// </summary>
        /// <param name="row">the row to attack</param>
        /// <param name="col">the column to attack</param>
        /// <remarks>
        /// Checks the attack result once the attack is complete
        /// </remarks>
        public void Attack(int row, int col)
        {
            AttackResult result;
            result = _theGame.Shoot(row, col);
            CheckAttackResult(result);
        }

        /// <summary>
        /// Gets the AI to attack.
        /// </summary>
        /// <remarks>
        /// Checks the attack result once the attack is complete.
        /// </remarks>
        private void AIAttack()
        {
            AttackResult result;
            result = _theGame.Player.Attack();
            CheckAttackResult(result);
        }

        /// <summary>
        /// Checks the results of the attack and switches to
        /// Ending the Game if the result was game over.
        /// </summary>
        /// <param name="result">the result of the last
        /// attack</param>
        /// <remarks>Gets the AI to attack if the result switched
        /// to the AI player.</remarks>
        private void CheckAttackResult(AttackResult result)
        {
            switch (result.Value)
            {
                case ResultOfAttack.Miss:
                    if (object.ReferenceEquals(_theGame.Player, ComputerPlayer))
                        AIAttack();

                    break;
                case ResultOfAttack.GameOver:
                    SwitchState(GameState.EndingGame);

                    break;
            }
        }

        /// <summary>
        /// Handles the user SwinGame.
        /// </summary>
        /// <remarks>
        /// Reads key and mouse input and converts these into
        /// actions for the game to perform. The actions
        /// performed depend upon the state of the game.
        /// </remarks>
        public void HandleUserInput()
        {
            //Read incoming input events
            SwinGame.ProcessEvents();

            switch (CurrentState)
            {
                case GameState.ViewingMainMenu:
                    _menuController.HandleMainMenuInput();

                    break;
                case GameState.ViewingGameMenu:
                    _menuController.HandleGameMenuInput();

                    break;
                case GameState.AlteringSettings:
                    _menuController.HandleSetupMenuInput();

                    break;
                case GameState.Deploying:
                    _deploymentController.HandleDeploymentInput();

                    break;
                case GameState.Discovering:
                    _discoveryController.HandleDiscoveryInput();

                    break;
                case GameState.EndingGame:
                    _endingGameController.HandleEndOfGameInput();

                    break;
                case GameState.ViewingHighScores:
                    _highScoreController.HandleHighScoreInput();

                    break;
            }

            _utilityFunctions.UpdateAnimations();
        }

        /// <summary>
        /// Draws the current state of the game to the screen.
        /// </summary>
        /// <remarks>
        /// What is drawn depends upon the state of the game.
        /// </remarks>
        public void DrawScreen()
        {
            _utilityFunctions.DrawBackground();

            switch (CurrentState)
            {
                case GameState.ViewingMainMenu:
                    _menuController.DrawMainMenu();

                    break;
                case GameState.ViewingGameMenu:
                    _menuController.DrawGameMenu();

                    break;
                case GameState.AlteringSettings:
                    _menuController.DrawSettings();

                    break;
                case GameState.Deploying:
                    _deploymentController.DrawDeployment();

                    break;
                case GameState.Discovering:
                    _discoveryController.DrawDiscovery();

                    break;
                case GameState.EndingGame:
                    _endingGameController.DrawEndOfGame();

                    break;
                case GameState.ViewingHighScores:
                    _highScoreController.DrawHighScores();

                    break;
            }

            _utilityFunctions.DrawAnimations();

            SwinGame.RefreshScreen();
        }

        /// <summary>
        /// Move the game to a new state. The current state is maintained
        /// so that it can be returned to.
        /// </summary>
        /// <param name="state">the new game state</param>
        public void AddNewState(GameState state)
        {
            _state.Push(state);
            _utilityFunctions.Message = "";
        }

        /// <summary>
        /// End the current state and add in the new state.
        /// </summary>
        /// <param name="newState">the new state of the game</param>
        public void SwitchState(GameState newState)
        {
            EndCurrentState();
            AddNewState(newState);
        }

        /// <summary>
        /// Ends the current state, returning to the prior state
        /// </summary>
        public void EndCurrentState()
        {
            _state.Pop();
        }

        /// <summary>
        /// Sets the difficulty for the next level of the game.
        /// </summary>
        /// <param name="setting">the new difficulty level</param>
        public void SetDifficulty(AIOptions setting)
        {
            _aiSetting = setting;
        }
    }
}
