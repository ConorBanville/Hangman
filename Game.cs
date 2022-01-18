using System;
using System.IO;
using Newtonsoft.Json;

namespace Console_menu
{
    class Game
    {
        static string[] items;  //The items to be displayed in the menu
        static string save_file_path;    // The save file name 
        public Menu menu;       // Menu object 
        public Hangman hangman; // Hangman game object
        string banner = // Title banner
        @"
  ▄████     ▄▄▄          ███▄ ▄███▓   ▓█████           ███▄ ▄███▓   ▓█████     ███▄    █     █    ██ 
 ██▒ ▀█▒   ▒████▄       ▓██▒▀█▀ ██▒   ▓█   ▀          ▓██▒▀█▀ ██▒   ▓█   ▀     ██ ▀█   █     ██  ▓██▒
▒██░▄▄▄░   ▒██  ▀█▄     ▓██    ▓██░   ▒███            ▓██    ▓██░   ▒███      ▓██  ▀█ ██▒   ▓██  ▒██░
░▓█  ██▓   ░██▄▄▄▄██    ▒██    ▒██    ▒▓█  ▄          ▒██    ▒██    ▒▓█  ▄    ▓██▒  ▐▌██▒   ▓▓█  ░██░
░▒▓███▀▒    ▓█   ▓██▒   ▒██▒   ░██▒   ░▒████▒         ▒██▒   ░██▒   ░▒████▒   ▒██░   ▓██░   ▒▒█████▓ 
 ░▒   ▒     ▒▒   ▓▒█░   ░ ▒░   ░  ░   ░░ ▒░ ░         ░ ▒░   ░  ░   ░░ ▒░ ░   ░ ▒░   ▒ ▒    ░▒▓▒ ▒ ▒ 
  ░   ░      ▒   ▒▒ ░   ░  ░      ░    ░ ░  ░         ░  ░      ░    ░ ░  ░   ░ ░░   ░ ▒░   ░░▒░ ░ ░ 
░ ░   ░      ░   ▒      ░      ░         ░            ░      ░         ░         ░   ░ ░     ░░░ ░ ░ 
      ░          ░  ░          ░         ░  ░                ░         ░  ░            ░       ░     
                                                                                                    ";

        //Constructor
        public Game()
        {
            save_file_path = File.ReadAllText("./game_save/last_save_location.txt");
            if(save_file_path == ""){
                items = new string []{"  PLAY  ","  ABOUT  ","  EXIT  "};
            }
            else
            {
                items = new string []{"  CONTINUE  ","  PLAY  ","  ABOUT  ","  EXIT  "};
            }
            menu = new Menu(items);     // Create a new menu item
        }

        //Run the game
        public void Run()
        {
            bool gameRunning = true;
            // Loop runs while game is on
            while(gameRunning)
            {
                // In every tick of the game we,
                Console.Clear();                                // Clear the console
                Console.CursorVisible = false;                  // Turn off the cursor
                Console.WriteLine(banner);                      // Print the banner 
                menu.display();                                 // Display the menu

                System.ConsoleKeyInfo key = Console.ReadKey();  // Read in a key from the user
                menu.SetActive(key);                            // Update the menu 
                // Check if the enter key was pressed, and if so we will preform an action on the active menu item
                if(key.Key == ConsoleKey.Enter)
                {
                    switch(menu.GetActiveItem())
                    {
                        case "  CONTINUE  ":
                            string save_file_json = File.ReadAllText($"./game_save/{save_file_path}.json");
                            Hangman_Save save_object = JsonConvert.DeserializeObject<Hangman_Save>(save_file_json);
                            hangman = new Hangman(save_object);
                            hangman.Play();
                        break;

                        case "  PLAY  ":
                            hangman = new Hangman();    // Create a new hangman game
                            hangman.Play();
                        break;

                        case "  ABOUT  ":
                        break;

                        case "  EXIT  ":
                            gameRunning = false;
                            Console.Clear();
                        break;
                    }
                }
            }
        }
    }
}