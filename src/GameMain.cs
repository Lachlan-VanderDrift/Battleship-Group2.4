using System;
using MyGame.src;
using SwinGameSDK;

namespace MyGame
{
    public class GameMain
    {
        public static void Main()
        {
            //Opens a new Graphics Window
            SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

            MenuController menuController = new MenuController();
            DiscoveryController discoveryController = new DiscoveryController();
            EndingGameController endingGameController = new EndingGameController();
            GameResources gameResources = new GameResources();
            UtilityFunctions utilityFunctions = new UtilityFunctions();
            DeploymentController deploymentController = new DeploymentController();
            HighScoreController highScoreController = new HighScoreController();
            GameController gameController = new GameController(highScoreController, endingGameController, discoveryController, deploymentController, utilityFunctions, gameResources, menuController);

            utilityFunctions.UtilityFunctionsAdd(gameResources, gameController);
            highScoreController.HighScoreControllerAdd(utilityFunctions, gameController, gameResources);
            menuController.MenuControllerAdd(utilityFunctions, gameResources, gameController);
            discoveryController.DiscoveryControllerAdd(gameResources, utilityFunctions, gameController);
            endingGameController.EndingGameControllerAdd(highScoreController, utilityFunctions, gameController);
            deploymentController.DeploymentControllerAdd(gameResources, utilityFunctions, gameController);

            gameController.UpdateGameControl(highScoreController, endingGameController, discoveryController, deploymentController, utilityFunctions, gameResources, menuController);

            //Load Resources
            gameResources.LoadResources();

            SwinGame.PlayMusic(gameResources.GameMusic("Background"));

            //Game Loop
            do
            {
                gameController.HandleUserInput();
                gameController.DrawScreen();
            } while (!(SwinGame.WindowCloseRequested() == true | gameController.CurrentState == GameState.Quitting));

            SwinGame.StopMusic();

            //Free Resources and Close Audio, to end the program.
            gameResources.FreeResources();
        
        }
    }
}