using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Console;

namespace H1GPDag3
{
    class Program
    {
        // Creates a filepath for the pool of words the system can pick from, variables to use throughout the program
        // and brings in Random to help with picking the word to be guessed

        readonly static string path = @"C:\Users\pc\Desktop\TEC\Grundlæggende programmering\H1GPDag3\";
        readonly static string fileName = "words.json";
        static Random random = new Random();
        
        static string _secretWord;
        static List<string> _hiddenWord = new List<string>();
        static List<string> _guessedLetters = new List<string>();
        static int _numberOfGuesses;
        static int _wrongGuesses;

        static void Main(string[] args)
        {
            Title = "Hangman";
            OutputEncoding = Encoding.UTF8;

            GameStart();
        }

        /// <summary>
        /// Resets variables and runs the setup and gamescreen methods
        /// </summary>
        static void GameStart()
        {
            _secretWord = "";
            _hiddenWord.Clear();
            _wrongGuesses = 5;
            _guessedLetters.Clear();

            Setup();
            DrawGameScreen();
        }

        static void DrawHiddenWord()
        {
            foreach (var letter in _hiddenWord)
            {
                Write(letter);
            }
        }

        static void DrawGameScreen()
        {
            while (true)
            {
                Clear();
                HangManTitle();
                DrawScaffold(_wrongGuesses);

                DrawHiddenWord();
                DrawGuessedLetters();
                WriteLine(Environment.NewLine + $"Du har nu { _numberOfGuesses } gæt til at finde det hemmelige ord. Gætter du forkert { _wrongGuesses } gange eller løber du tør for gæt, har du tabt!");
                UpdateHiddenWord(HandleGuess());
            }
        }

        /// <summary>
        /// Accepts a guess from the user as a string, increments the number of guesses used, and does datavalidation on the entered letter or string.
        /// If the guess is equal to the secretWord, the game ends in a win. Otherwise the output is used in updating the values in hiddenWord
        /// </summary>
        /// <returns>Single or multiletter string that the user has entered</returns>
        static string HandleGuess()
        {
            if (string.Join("", _hiddenWord.ToArray()) == _secretWord)
                GameOver("win");

            _numberOfGuesses--;
            if (_numberOfGuesses == 0)
                GameOver("lose");

            string guess;
            do
            {
                Write("Indtast næste gæt: ");
                guess = ReadLine().ToUpper();

                if(_guessedLetters.Contains(guess))
                    WriteLine("Du har allerede gættet på dette bogstav!");
            }
            while (_guessedLetters.Contains(guess));

            if (guess.Length == 1)
            {
                WriteLine(Environment.NewLine + $"Du har nu {_numberOfGuesses} gæt tilbage!" + Environment.NewLine);

                return guess;
            }
            else if (guess.Length == _hiddenWord.Count() && guess == _secretWord)
                GameOver("win");

            return null;
        }

        /// <summary>
        /// Accepts a string "guess" and uses that data to update the values in hiddenWord and the list of guessed letters. 
        /// Does data validation to ensure that the entered data fits with what is expected. 
        /// Also decrements the remaining guesses and ends the game if the counter reaches zero.
        /// </summary>
        /// <param name="guess"></param>

        static void UpdateHiddenWord(string guess)
        {
            if (string.IsNullOrEmpty(guess))
            {
                WriteLine("Det var ikke et brugbart gæt. Prøv igen!");
                UpdateHiddenWord(HandleGuess());
            }

            if (guess == null)
                return;

            if (_secretWord.Contains(guess))
            {
                for (int i = 0; i < _hiddenWord.Count; i++)
                {
                    if (_secretWord[i] == guess[0])
                        _hiddenWord[i] = guess;
                }
            }
            else
                _wrongGuesses--;

            if (_wrongGuesses == 0)
                GameOver("lose");
            
            if(guess.Length == 1)
                _guessedLetters.Add(guess);
        }

        static void DrawGuessedLetters()
        {
            WriteLine(Environment.NewLine);

            foreach (string c in _guessedLetters)
            {
                Write(c);
            }
        }

        /// <summary>
        /// Accepts a string and based on the input will end the game with a win or a loss
        /// </summary>
        /// <param name="winLose"></param>
        private static void GameOver(string winLose)
        {
            if (winLose == "win")
            {
                Clear();
                HangManTitle();
                WriteLine(Environment.NewLine + "Woooh, du vandt!");
            }
            else if (winLose == "lose")
            {
                Clear();
                HangManTitle();
                if (_wrongGuesses == 0)
                {
                    DrawScaffold(0);
                    WriteLine("Du har gættet forkert for mange gange, manden er hængt!");
                }
                else
                    WriteLine("Beklager, du har ikke flere gæt tilbage. Du tabte!");
            }

            WriteLine(Environment.NewLine + "Tryk en tast for at spille igen");
            ReadKey();
            Clear();
            GameStart();
        }

        #region Setup

        /// <summary>
        /// The user can choose the length of the secretWord and Setup will pick a random word from the list of available words, secretWordsModel, 
        /// and creates the hiddenWord. Also sets the number of guesses allowed by adding three to the length of the hiddenWord.
        /// </summary>
        static void Setup()
        {
            WriteLine("Velkommen til TEC Galgespil!" + Environment.NewLine);
            WriteLine("1. Tre bogstaver");
            WriteLine("2. Fire bogstaver");
            WriteLine("3. Fem bogstaver");

            Write(Environment.NewLine + "Indtast venligst den ønskede sværhedsgrad (længde på det hemmelige ord): ");

            int choice;

            while (!Int32.TryParse(ReadLine(), out choice) || choice < 1 || choice > 3)
            {
                Write("Du skal vælge et tal fra menuen, prøv igen: ");
            }

            SecretWordsModel secretWordsModel = GetSecretWords<SecretWordsModel>();
            int randomWordPick = random.Next(3);

            switch (choice)
            {
                case 1:
                    _secretWord = secretWordsModel.ThreeLetterWords[randomWordPick];
                    break;
                case 2:
                    _secretWord = secretWordsModel.FourLetterWords[randomWordPick];
                    break;
                case 3:
                    _secretWord = secretWordsModel.FiveLetterWords[randomWordPick];
                    break;
                default:
                    break;
            }

            for (int i = 0; i < _secretWord.Length; i++)
            {
                _hiddenWord.Add("_");
            }

            _numberOfGuesses = _hiddenWord.Count + 3;
        }

        /// <summary>
        /// Loads the wordlist from a file and deserializes it into an object, which is then used to create a new instance of the SecretWordsModel class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static T? GetSecretWords<T>()
        {
            if (!Directory.Exists(path)) return default;
            else if (!File.Exists(path + fileName)) return default;
            
            string secretWordsJson = File.ReadAllText(path + fileName);
            T? obj = JsonSerializer.Deserialize<T?>(secretWordsJson);

            return obj;
        }

        /// <summary>
        /// Draws the scaffolding to the level of completion expected from the number of wrong guesses (the int supplied)
        /// </summary>
        /// <param name="wrongGuesses"></param>
        static void DrawScaffold(int wrongGuesses)
        {
            if (wrongGuesses == 5)
            {
                WriteLine(@"
            _______
            |/     
            |
            |
            |
            |
        ___/|\___
        ");
            }
            else if (wrongGuesses == 4)
            {
                WriteLine(@"
            _______
            |/     |
            |
            |
            |
            |
        ___/|\___
        ");
            }
            else if (wrongGuesses == 3)
            {
                WriteLine(@"
            _______
            |/     |
            |     (_)
            |
            |
            |
        ___/|\___
        ");
            }
            else if (wrongGuesses == 2)
            {
                WriteLine(@"
            _______
            |/     |
            |     (_)
            |     \|/
            |
            |
        ___/|\___
        ");
            }
            else if (wrongGuesses == 1)
            {
                WriteLine(@"
            _______
            |/     |
            |     (_)
            |     \|/
            |      |
            |
        ___/|\___
        ");
            }
            else if (wrongGuesses == 0)
            {
                Clear();
                HangManTitle();

                WriteLine(@"
            _______
            |/     |
            |     (_)
            |     \|/
            |      |
            |     / \
        ___/|\___
        ");
            }
        }

        static void HangManTitle()
        {
            Write(" _                                             \r\n| |                                            \r\n| |__   __ _ _ __   __ _ _ __ ___   __ _ _ __  \r\n| '_ \\ / _` | '_ \\ / _` | '_ ` _ \\ / _` | '_ \\ \r\n| | | | (_| | | | | (_| | | | | | | (_| | | | |\r\n|_| |_|\\__,_|_| |_|\\__, |_| |_| |_|\\__,_|_| |_|\r\n                    __/ |                      \r\n                   |___/ " + Environment.NewLine);
        }
        #endregion

    }
}