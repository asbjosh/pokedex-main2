using System;
using System.IO;

namespace pokedex;
public class ViewPokémons
{
    // Vis alle Pokémon på en sideniveau
    public void DisplayPokémons()
    {
        Console.Clear();
        string[] displayPokémons = File.ReadAllLines("Pokedex.csv").Skip(1).ToArray(); // Læs Pokémon data fra filen

        int currentPage = 0;
        int pokesPerPage = 5; // Antal Pokémon vist pr. side
        int totalPages = (int)Math.Ceiling(displayPokémons.Length / (double)pokesPerPage); // Beregn det totale antal sider

        bool loop = true;
        while (loop)
        {
            Console.Clear();
            Console.WriteLine($"Page {currentPage + 1} of {totalPages}"); // Vis nuværende side og antal sider

            // Vis Pokémon på den nuværende side
            foreach (var item in displayPokémons.Skip(currentPage * pokesPerPage).Take(pokesPerPage))
            {
                string[] x = item.Split(",");
                Console.WriteLine($"Navn: {x[1]} Type: {x[2]} Styrke: {x[3]}");
            }

            Console.WriteLine("\n N for næste, P for forrige, Q for hovedmenu.");
            var key = Console.ReadKey(true).Key; // Læs brugerens tastetryk
            if (key == ConsoleKey.N && currentPage < totalPages - 1)
                currentPage++; // Gå til næste side
            else if (key == ConsoleKey.P && currentPage > 0)
                currentPage--; // Gå til forrige side
            else if (key == ConsoleKey.Q)
                loop = false; // Afslut loopet og gå tilbage til hovedmenuen
        }
    }
}