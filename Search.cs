using System;
using System.IO;
using System.Linq;

namespace pokedex;

class Search
{
    public void Searching()
    {
        Console.Clear();
        string[] searchPokémons = File.ReadAllLines("Pokedex.csv").Skip(1).ToArray(); // Læs Pokémon data fra filen
        int results = 0;

        Console.Write("Søg efter Pokémon navn eller type: ");
        string? input = Console.ReadLine(); // Hent brugerens søgning

        // Gennemse alle Pokémon og vis de matchende
        foreach (var item in searchPokémons)
        {
            string[] x = item.Split(",");
            if (x[1].Contains(input) || x[2].Contains(input))
            {
                Console.WriteLine($"Navn: {x[1]} Type: {x[2]} Styrke: {x[3]}");
                results++;
            }
        }
        if (results == 0) Console.WriteLine("Ingen Pokémon fundet."); // Hvis ingen matchninger findes
        Console.ReadKey(); // Vent på brugerinput
    }
}