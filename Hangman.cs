using System;
using System.IO;
using Newtonsoft.Json;

namespace Console_menu
{
    class Hangman
    {
        string banner = 
        @"
 ██░ ██     ▄▄▄          ███▄    █      ▄████     ███▄ ▄███▓    ▄▄▄          ███▄    █    
▓██░ ██▒   ▒████▄        ██ ▀█   █     ██▒ ▀█▒   ▓██▒▀█▀ ██▒   ▒████▄        ██ ▀█   █    
▒██▀▀██░   ▒██  ▀█▄     ▓██  ▀█ ██▒   ▒██░▄▄▄░   ▓██    ▓██░   ▒██  ▀█▄     ▓██  ▀█ ██▒   
░▓█ ░██    ░██▄▄▄▄██    ▓██▒  ▐▌██▒   ░▓█  ██▓   ▒██    ▒██    ░██▄▄▄▄██    ▓██▒  ▐▌██▒   
░▓█▒░██▓    ▓█   ▓██▒   ▒██░   ▓██░   ░▒▓███▀▒   ▒██▒   ░██▒    ▓█   ▓██▒   ▒██░   ▓██░   
 ▒ ░░▒░▒    ▒▒   ▓▒█░   ░ ▒░   ▒ ▒     ░▒   ▒    ░ ▒░   ░  ░    ▒▒   ▓▒█░   ░ ▒░   ▒ ▒    
 ▒ ░▒░ ░     ▒   ▒▒ ░   ░ ░░   ░ ▒░     ░   ░    ░  ░      ░     ▒   ▒▒ ░   ░ ░░   ░ ▒░   
 ░  ░░ ░     ░   ▒         ░   ░ ░    ░ ░   ░    ░      ░        ░   ▒         ░   ░ ░    
 ░  ░  ░         ░  ░            ░          ░           ░            ░  ░            ░    
                                                                                          ";
        static string [] drawing = {
        @"
          + === +
                |
                |
                |
                |
        || = = = ||",
        @"
          + === +
          O     |
          |     |
                |
                |
        || = = = ||",
        @"
          + === +
          O     |
         /|     |
                |
                |
        || = = = ||",
        @"
          + === +
          O     |
         /|\    |
                |
                |
        || = = = ||",
        @"
          + === +
          O     |
         /|\    |
         /      |
                |
        || = = = ||",
        @"
          + === +
          O     |
         /|\    |
         / \    |
                |
        || = = = ||"
        };             // The hangman drawing is made up of 6 string literals
        private static string path = "./dictionary/1025 17.12.48.txt"; // Dictionary path
        string[] dict = File.ReadAllLines(path); // Dictionary for the game 
        string active_word {get; set;}                     // The guess word for this game
        string guessCard;                       // Guess card contains underscore, _ inplace of letters in the active word
        
        string previousGuesses;                 // Remember the guesses played
        Random GetRandom = new Random();        // Random number generator
        int gameStage;
        char guess, lastKey;
        bool gameRunning, gameStageLocked;

        // Constructor
        public Hangman()
        {
                active_word = dict[GetRandom.Next(dict.Length + 1)];    // Set the active word
                //active_word = "word";
                guessCard = GetGuessCard(active_word);                  // Get the guess card
                gameStage = 0;          // Track what stage of the game we are at
                guess = ' ';        // Set the last guess, blank when the first guess is yet to be played
                gameRunning = true;
                gameStageLocked = false;
                previousGuesses = "";
        }

        // Constructor for game continue
        public Hangman(Hangman_Save game_save)
        {
                active_word = game_save.active_word;
                guessCard = game_save.guess_card;
                gameStage = game_save.game_stage;
                previousGuesses = game_save.previous_guesses;
                guess = ' ';
                gameRunning  = true;
                gameStageLocked = false;
        }

        // Play the game
        public void Play()
        {
                // The loop runs while the player still has guesses left, and the game hasn't been won
                while(gameStage < drawing.Length && gameRunning)
                {       
                        /*      1. Draw the board
                                2. Get a key from player
                                3. If the key == ESC then open a menu
                                4. If the key was enter then use that key */

                        lastKey = ' ';
                        while(true)
                        {
                                Console.Clear();        // Clear the console                                   
                                DrawBanner();         // Draw the banner                                     
                                DrawHangman();         // Draw the hangman sketch                                      
                                DrawGamePieces();       // Draw the guess card and last guess                                      
                                Console.WriteLine(lastKey); // Draw the last key


                                ConsoleKeyInfo key = Console.ReadKey(); // Read a new key
                                if(key.Key == ConsoleKey.Enter && lastKey != ' ') break; // If the ENTR key is pressed then break the loop, accepting the last char pressed as the new key
                                else if(key.Key == ConsoleKey.Escape) gameRunning = GameMenu();      /*   If Escape key then display a menu,
                                                                                                          Menu returns false if the game should be closed */
                                // Check to see if game is still running
                                if(!gameRunning) break;
                                lastKey = key.KeyChar;   // Set the last guess
                        }

                        // The last key pressed has now been confirmed by the player

                        guess = lastKey;    // Update the last guess
                        gameStageLocked = false;  // GameStagedLocked is true when then player cant loes a life

                        // If the player's guess has not been played before then we add it to the previous guessed array
                        if(!previousGuesses.Contains(guess)) previousGuesses += $" {guess} ";

                        /* If the players guess has been played before then they can't loes a life again, 
                           otherwise they loes a life if the guess was incorrect */
                        else gameStageLocked = true;
                        if(gameStage < 0) gameStage = 0; // Game stage is indexer for drawing array, must never be < 0

                        // Player guessed a letter correctly
                        if(active_word.Contains(guess))
                        {      
                                guessCard = UpdateGuessCard();  // Fill in the correct guess on the guess card

                                // Check if the entire card is filled in
                                if(guessCard.Replace(" ","").Equals(active_word))
                                {
                                        // Player has guessed the entire word
                                        
                                        Console.Clear();        // Redraw the screen so the full word is show
                                        DrawBanner();
                                        DrawHangman();
                                        DrawGamePieces();

                                        // Print a congrats message
                                        Console.WriteLine("\tCongratulations you have won the game!\n\tPress any key to continue");
                                        Console.ReadKey();
                                        gameRunning = false;    // Turn off the hangman game
                                        return;
                                }
                        }
                        else
                        {
                                // Player guessed incorrectly
                                if(!gameStageLocked)gameStage ++;       // Player loes a life
                                if(gameStage == drawing.Length - 1) 
                                {
                                        gameStage = drawing.Length - 1;    // Game Stage is indexer for drawing, so must never be greater than drawing.Length
                                        EndGame();
                                }
                        }                        
                }

                // If the player has lost
                void EndGame()
                {
                        // Redraw the board with a Game over message and wait for a key press
                        Console.Clear();
                        DrawBanner();
                        DrawHangman();
                        DrawGamePieces();
                        Console.WriteLine("\tGame Over! You lost\n\tPress any key to continue");
                        Console.ReadKey();
                        gameRunning = false;
                }
        }

        public bool GameMenu()
        {
                Menu menu = new Menu(new string[]{"  RESUME  ","  SAVE  ","  EXIT  "});
                ConsoleKeyInfo key;
                bool gameON = true;
                bool menuRunning = true;

                while(menuRunning)
                {
                        Console.Clear();                                // Clear the console
                        Console.CursorVisible = false;                  // Turn off the cursor
                        DrawBanner();
                        menu.display();
                        key = Console.ReadKey();                        // Read in a key from the user
                        menu.SetActive(key);                            // Update the menu 

                        // Check if the ENTR key was pressed, and if so we will preform an action on the active menu item
                        if(key.Key == ConsoleKey.Enter)
                        {
                                // GetActiveItem() returns the name of the current selected item in the menu
                                switch(menu.GetActiveItem())
                                {
                                        case "  RESUME  ":
                                                menuRunning = false;
                                        return true;

                                        case "  SAVE  ":
                                                SaveGame();
                                        return true;

                                        case "  EXIT  ":
                                                menuRunning = ConfirmExit();
                                                gameON = menuRunning;
                                        break;

                                        default:
                                        return true;
                                }
                        }
                }

                bool ConfirmExit()
                {
                        Console.WriteLine("Are you sure you want to exit. Any unsaved data will be lost [Y]es?");
                        switch(Console.ReadLine().ToLower())
                        {
                                case "y":
                                return false;

                                case "yes":
                                return false;

                                default:
                                return true;
                        }
                }

                return gameON;
                
        }

        // Save the game
        public void SaveGame()
        {
                string save_path = DateTime.Now.ToString("dddd, dd MM yyyy HH-mm-ss");
                Hangman_Save save = new Hangman_Save
                {
                    active_word = this.active_word,
                    game_stage = this.gameStage,
                    guess_card = this.guessCard,
                    previous_guesses = this.previousGuesses 
                };

                string json = JsonConvert.SerializeObject(save,Formatting.Indented);
                File.WriteAllText($"./game_save/{save_path}.json", json);
                File.WriteAllText("./game_save/last_save_location.txt", save_path);
        }

        // Draw the banner
        public void DrawBanner()
        {
                Console.WriteLine(banner);
        }

        // Draw game pieces
        public void DrawGamePieces()
        {
                string gamePieces = @$"
     {PadGuessCard(guessCard)}                       Guesses: {previousGuesses}

                ";

                Console.WriteLine(gamePieces);
        }
        
        // Draw the hangman
        public void DrawHangman()
        {
                Console.WriteLine(drawing[gameStage]);
        }

        // Update guess card
        public string UpdateGuessCard()
        {
                string newGuessCard = "";

                for(int i=0; i<active_word.Length; i++)
                {
                        if(active_word[i] == guess)
                        {
                                newGuessCard += guess;
                        }
                        else if(guessCard[i] == '_')
                        {
                                newGuessCard += "_";
                        }
                        else
                        {
                                newGuessCard += guessCard[i];
                        }
                }

                return newGuessCard;
        }
        // Returns a guess card
        public string GetGuessCard(string fullWord)
        {
                string guessCard = "";

                foreach(char letter in fullWord)
                {
                        guessCard += "_";
                }

                return guessCard;
        }
        // Returns a string with a space around every character in guessCard 
        public string PadGuessCard(string guessCard)
        {
                string padded = "";

                foreach(char letter in guessCard)
                {
                        padded += $" {letter} "; 
                }

                return padded;
        }
    }
}