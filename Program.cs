using System.Text;
using static System.Console;

namespace H1GPDag3
{
    class Program
    {
        static Random random = new Random();

        static string[][] words = new string[][]
        {
            new string[] {"BEN", "KOP", "API"},
            new string[] {"STOL", "FISK", "HØNE"},
            new string[] {"TRIST", "TRIST", "TRIST"},
        };
        
        static string secretWord;
        static List<string> hiddenWord = new List<string>();
        static List<string> guessedLetters = new List<string>();
        static int numberOfGuesses;
        static int wrongGuesses;

        static void Main(string[] args)
        {
            Title = "Hangman";
            OutputEncoding = Encoding.UTF8;

            GameStart();
        }

        static void GameStart()
        {
            secretWord = "";
            hiddenWord.Clear();
            wrongGuesses = 0;

            Setup();
            DrawGameScreen();
        }

        static void DrawHiddenWord()
        {
            foreach (var b in hiddenWord)
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
                DrawScaffold(wrongGuesses);

                DrawHiddenWord();
                DrawGuessedLetters();
                WriteLine(Environment.NewLine + $"Du har nu { numberOfGuesses } gæt til at finde det hemmelige ord. Gætter du forkert fem gange eller løber tør for gæt, har du tabt!");
                UpdateHiddenWord(HandleGuess());
            }
        }

        static string HandleGuess()
        {
            if (string.Join("", hiddenWord.ToArray()) == secretWord)
                GameWon();

            string guess;
            do
            {
                Write("Indtast næste gæt: ");
                guess = ReadLine().ToUpper();

                if(guessedLetters.Contains(guess))
                    WriteLine("Du har allerede gættet på dette bogstav!");
            }
            while (guessedLetters.Contains(guess));

            numberOfGuesses--;
            if (numberOfGuesses == 0)
                GameOver();

            if (guess.Length == 1)
            {
                WriteLine(Environment.NewLine + $"Du har nu { numberOfGuesses } gæt tilbage!" + Environment.NewLine);

                return guess;
            }
            else if (guess.Length == hiddenWord.Count() && guess == secretWord)
            {
                GameWon();
            }
            return null;
        }

        static void UpdateHiddenWord(string guess)
        {
            if (string.IsNullOrEmpty(guess))
            {
                WriteLine("Det var ikke et brugbart gæt. Prøv igen!");
                UpdateHiddenWord(HandleGuess());
            }

            if (secretWord.Contains(guess))
            {
                for (int i = 0; i < hiddenWord.Count; i++)
                {
                    if (secretWord[i] == guess[0])
                        hiddenWord[i] = guess;
                }
            }
            else
                wrongGuesses++;

            if(guess.Length == 1)
                guessedLetters.Add(guess);
        }
        
        static void DrawGuessedLetters()
        {
            WriteLine(Environment.NewLine);

            foreach (string c in guessedLetters)
            {
                Write(c);
            }
        }

        private static void GameWon()
        {
            Clear();
            HangManTitle();
            WriteLine(Environment.NewLine + "Woooh, du vandt! Tryk en tast for at spille igen...");
            ReadKey();
            Clear();
            GameStart();
        }

        private static void GameOver()
        {
            Clear();
            HangManTitle();

            WriteLine("Beklager, du har ikke flere gæt tilbage. Du tabte!");
            WriteLine("Tryk en tast for at spille igen");
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
            Int32.TryParse(ReadLine(), out int choice);

            secretWord = words[choice - 1][random.Next(3)];

            for (int i = 0; i < secretWord.Length; i++)
            {
                hiddenWord.Add("_");
            }

            numberOfGuesses = hiddenWord.Count + 3;
        }

        static void DrawScaffold(int wrongGuesses)
        {
            if (wrongGuesses == 0)
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
            else if (wrongGuesses == 1)
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
            else if (wrongGuesses == 2)
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
            else if (wrongGuesses == 3)
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
            else if (wrongGuesses == 4)
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
            else if (wrongGuesses == 5)
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
                GameOver();
            }
        }

        static void HangManTitle()
        {
            Write(" _                                             \r\n| |                                            \r\n| |__   __ _ _ __   __ _ _ __ ___   __ _ _ __  \r\n| '_ \\ / _` | '_ \\ / _` | '_ ` _ \\ / _` | '_ \\ \r\n| | | | (_| | | | | (_| | | | | | | (_| | | | |\r\n|_| |_|\\__,_|_| |_|\\__, |_| |_| |_|\\__,_|_| |_|\r\n                    __/ |                      \r\n                   |___/ " + Environment.NewLine);
        }
        #endregion

    }
}