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
            foreach (var b in _hiddenWord)
            {
                Write(b);
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

        static T? GetSecretWords<T>()
        {
            if (!Directory.Exists(path)) return default;
            else if (!File.Exists(path + fileName)) return default;
            
            string secretWordsJson = File.ReadAllText(path + fileName);
            T? obj = JsonSerializer.Deserialize<T?>(secretWordsJson);

            return obj;
        }

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