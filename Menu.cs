using System;

namespace Console_menu
{
    class Menu
    {
        
        string [] items;    // Array of items in the menu
        int active = 0;     // Track the active item

        // Constructor
        public Menu(string[] items)
        {
            this.items = items; // Set the menu items
        } 

        //Display the menu items
        public void display()
        {
            for(int i=0; i<items.Length; i++)
            {
                if(i == active)
                {
                    Console.Write("> ");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(items[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write("  ");
                    Console.WriteLine(items[i]);
                }
            }
        }

        //Update the active item
        public bool SetActive(System.ConsoleKeyInfo key)
        {
            if(key.Key == ConsoleKey.DownArrow)
            {
                active ++;
                if(active >= items.Length - 1){ active = items.Length - 1;}
            }

            else if(key.Key == ConsoleKey.UpArrow) 
            {
                active --;
                if(active <= 0){ active = 0;}
            }

            return false;
        }
        
        /*  Need to rewrite this method for each different menu
            This method is called when the enter key is pressed,
            depending what menu item was active the game will
            change in some way. */
        public string GetActiveItem()
        {
            return items[active];
        }
    }
}