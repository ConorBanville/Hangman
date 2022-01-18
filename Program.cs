namespace Console_menu
{
    /*  This is a hangman game that uses a menu system and tracks 
        highscores and allows the user top save a game.
    */
    class Program
    {
        public static Game game = new Game();   // Create a new Game object 
        static void Main(string[] args)
        {
            game.Run(); // Run the game
        }
    }
}
