using SwinGameSDK;

namespace battleships
{
    class GameLogic
    {
        public void Main()
        {
            //Opens a new Graphics Window
            SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

            GameResources gameResources = new GameResources();
            GameController gameController = new GameController();
            UtilityFunctions utilityFunctions = new UtilityFunctions();

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