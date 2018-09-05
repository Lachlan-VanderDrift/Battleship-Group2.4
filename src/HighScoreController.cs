using SwinGameSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.src
{
    public class HighScoreController
    {
        private const int NAME_WIDTH = 3;

        private const int SCORES_LEFT = 490;

        private GameResources _gameResources;
        private GameController _gameController;
        private UtilityFunctions _utilityFunctions;

        public void HighScoreControllerAdd(UtilityFunctions pUtilityFunctions, GameController pGameControl, GameResources pGameResources)
        {
            _gameResources = pGameResources;
            _gameController = pGameControl;
            _utilityFunctions = pUtilityFunctions;
        }

        /// <summary>
        /// The score structure is used to keep the name and
        /// score of the top players together.
        /// </summary>
        private struct Score : IComparable
        {
            public string Name;

            public int Value;
            /// <summary>
            /// Allows scores to be compared to facilitate sorting
            /// </summary>
            /// <param name="obj">the object to compare to</param>
            /// <returns>a value that indicates the sort order</returns>
            public int CompareTo(object obj)
            {
                if (obj is Score)
                {
                    Score other = (Score)obj;

                    return other.Value - this.Value;
                }

                else
                {
                    return 0;
                }
            }
        }


        private List<Score> _Scores = new List<Score>();
        /// <summary>
        /// Loads the scores from the highscores text file.
        /// </summary>
        /// <remarks>
        /// The format is
        /// # of scores
        /// NNNSSS
        /// 
        /// Where NNN is the name and SSS is the score
        /// </remarks>
        private void LoadScores()
        {
            string filename;
            filename = SwinGame.PathToResource("highscores.txt");

            StreamReader input;
            input = new StreamReader(filename);

            //Read in the # of scores
            int numScores;
            numScores = Convert.ToInt32(input.ReadLine());

            _Scores.Clear();

            int i;

            for (i = 1; i <= numScores; i++)
            {
                Score s;
                string line;

                line = input.ReadLine();

                s.Name = line.Substring(0, NAME_WIDTH);
                s.Value = Convert.ToInt32(line.Substring(NAME_WIDTH));
                _Scores.Add(s);
            }
            input.Close();
        }

        /// <summary>
        /// Saves the scores back to the highscores text file.
        /// </summary>
        /// <remarks>
        /// The format is
        /// # of scores
        /// NNNSSS
        /// 
        /// Where NNN is the name and SSS is the score
        /// </remarks>
        private void SaveScores()
        {
            string filename;
            filename = SwinGame.PathToResource("highscores.txt");

            StreamWriter output;
            output = new StreamWriter(filename);

            output.WriteLine(_Scores.Count);

            foreach (Score s in _Scores)
            {
                output.WriteLine(s.Name + s.Value);
            }

            output.Close();
        }

        /// <summary>
        /// Draws the high scores to the screen.
        /// </summary>
        public void DrawHighScores()
        {
            const int SCORES_HEADING = 40;
            const int SCORES_TOP = 80;
            const int SCORE_GAP = 30;

            if (_Scores.Count == 0)
                LoadScores();

            SwinGame.DrawText("   High Scores   ", Color.White, _gameResources.GameFont("Courier"), SCORES_LEFT, SCORES_HEADING);

            //For all of the scores
            int i;
            for (i = 0; i <= _Scores.Count - 1; i++)
            {
                Score s;

                s = _Scores[i];

                //for scores 1 - 9 use 01 - 09
                if (i < 9)
                {
                    SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, _gameResources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
                }

                else
                {
                    SwinGame.DrawText(i + 1 + ":   " + s.Name + "   " + s.Value, Color.White, _gameResources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
                }
            }
        }

        /// <summary>
        /// Handles the user input during the top score screen.
        /// </summary>
        /// <remarks></remarks>
        public void HandleHighScoreInput()
        {
            if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.EscapeKey) || SwinGame.KeyTyped(KeyCode.ReturnKey))
            {
                _gameController.EndCurrentState();
            }
        }

        /// <summary>
        /// Read the user's name for their highsSwinGame.
        /// </summary>
        /// <param name="value">the player's sSwinGame.</param>
        /// <remarks>
        /// This verifies if the score is a highsSwinGame.
        /// </remarks>
        public void ReadHighScore(int value)
        {
            const int ENTRY_TOP = 500;

            if (_Scores.Count == 0)
                LoadScores();

            //is it a high score
            if (value > _Scores[_Scores.Count - 1].Value)
            {
                Score s = new Score();
                s.Value = value;

                _gameController.AddNewState(GameState.ViewingHighScores);

                int x;
                x = SCORES_LEFT + SwinGame.TextWidth(_gameResources.GameFont("Courier"), "Name: ");

                SwinGame.StartReadingText(Color.White, NAME_WIDTH, _gameResources.GameFont("Courier"), x, ENTRY_TOP);

                //Read the text from the user
                while (SwinGame.ReadingText())
                {
                    SwinGame.ProcessEvents();

                    _utilityFunctions.DrawBackground();
                    DrawHighScores();
                    SwinGame.DrawText("Name: ", Color.White, _gameResources.GameFont("Courier"), SCORES_LEFT, ENTRY_TOP);
                    SwinGame.RefreshScreen();
                }

                s.Name = SwinGame.TextReadAsASCII();

                while (s.Name.Length < 3)
                {
                    s.Name = s.Name + " ";
                }

                _Scores.RemoveAt(_Scores.Count - 1);
                _Scores.Add(s);
                _Scores.Sort();

                _gameController.EndCurrentState();
            }
        }
    }
}
