using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace pokedex;

class EditMenu
{
    // Vist menu for at redigere Pokémon
    public void ViewEditMenu()
    {
        string menuChoice;

        do
        {
            Console.Clear();
            Console.WriteLine("1. Tilføj en Pokémon");
            Console.WriteLine("2. Slet en Pokémon");
            Console.WriteLine("3. Redigere i en Pokémon");
            Console.Write("4. Gå tilbage til hovedmenuen: ");
            menuChoice = Console.ReadLine();

            // Valg af menu funktioner
            switch (menuChoice)
            {
                case "1":
                    {
                        Add add = new();
                        add.AddPokemon(); // Tilføj en Pokémon
                        break;
                    }
                case "2":
                    {
                        Delete delete = new();
                        delete.DeletePokemon(); // Slet en Pokémon
                        break;
                    }
                case "3":
                    {
                        Edit edit = new();
                        edit.EditPokemon(); // Rediger en Pokémon
                        break;
                    }
                case "4":
                    {
                        Program.ShowMainMenu(); // Gå tilbage til hovedmenuen
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Ugyldig indtastning, prøv igen!");
                        break;
                    }
            }
        } while (menuChoice != "4"); // Looper indtil brugeren vælger 4
    }
}

class Add
{
    // Funktion til at tilføje en Pokémon
    public void AddPokemon()
    {
        string filePath = "Pokedex.csv";
        string? name = "", type = "", strength = "";
        int nextId = 1;

        // Find den højeste ID i filen
        var lines = File.ReadLines(filePath).Skip(1).ToArray(); // Spring headeren over
        if (lines.Any())
        {
            // Få den maksimale ID
            int maxId = lines
                .Select(line => int.Parse(line.Split(",")[0])) // Få ID fra hver linje
                .Max(); // Find max ID værdi

            nextId = maxId + 1; // Sæt næste ID til at være én større end max ID
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Tilføj en Pokémon:");

            // Indtast Pokémon oplysninger
            Console.Write("Indtast Pokémons Navn: ");
            name = Console.ReadLine();

            Console.Write("Indtast Pokémons Type: ");
            type = Console.ReadLine();

            Console.Write("Indtast Pokémons Styrke: ");
            strength = Console.ReadLine();

            // Hvis styrken er et gyldigt tal, fortsæt
            if (int.TryParse(strength, out _))
            {
                string newPokemon = $"{nextId},{name},{type},{strength}";

                // Kontroller om den sidste linje allerede ender med et nyt linjeskift
                if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
                {
                    var lastLine = File.ReadLines(filePath).Last();
                    if (!lastLine.EndsWith(Environment.NewLine))
                    {
                        File.AppendAllText(filePath, Environment.NewLine); // Tilføj et linjeskift hvis nødvendigt
                    }
                }

                // Tilføj den nye Pokémon korrekt
                File.AppendAllText(filePath, newPokemon);

                Console.WriteLine("\nPokémon er blevet tilføjet!");
                break; // Gå tilbage til ViewEditMenu efter succesfuld tilføjelse
            }
            else
            {
                Console.WriteLine("\nStyrken kan kun være et tal. Prøv venligst igen.");
            }

            Console.ReadKey(); // Vent på brugerens input før vi fortsætter
        }
    }
}

class Delete
{
    // Funktion til at slette en Pokémon
    public void DeletePokemon()
    {
        string filePath = "Pokedex.csv";
        string[] lines = File.ReadAllLines(filePath);

        // Spring headeren over og få Pokémon data
        var pokemons = lines.Skip(1).ToArray();

        // Bed brugeren om at søge efter en Pokémon
        Console.Clear();
        Console.Write("Søg efter Pokémon at slette: ");
        string? input = Console.ReadLine();

        // Find Pokémon baseret på navn
        var matchingPokemons = pokemons
            .Where(pokemon => pokemon.Split(",")[1].Contains(input, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matchingPokemons.Any())
        {
            // Vis alle fundne Pokémon
            Console.WriteLine("Fundne Pokémons:");
            for (int i = 0; i < matchingPokemons.Length; i++)
            {
                string[] parts = matchingPokemons[i].Split(",");
                Console.WriteLine($"{i + 1}. Navn: {parts[1]}, Type: {parts[2]}, Styrke: {parts[3]}");
            }

            // Bed brugeren vælge hvilken Pokémon at slette
            int choice = -1;
            bool validChoice = false;

            // Fortsæt indtil brugeren laver et gyldigt valg
            while (!validChoice)
            {
                Console.Write("Vælg Pokémon at slette (nummer): ");
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out choice) && choice >= 1 && choice <= matchingPokemons.Length)
                {
                    choice--; // Juster til 0-baseret index
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Ugyldigt valg. Indtast venligst et gyldigt nummer.");
                }
            }

            string selectedPokemon = matchingPokemons[choice];

            // Fjern den valgte Pokémon fra listen
            pokemons = pokemons.Where(pokemon => pokemon != selectedPokemon).ToArray();

            // Byg filen med de resterende Pokémon og opdater ID'erne
            var updatedPokemons = new List<string> { lines[0] }; // Behold headeren

            int newId = 1;
            foreach (var pokemon in pokemons)
            {
                var parts = pokemon.Split(",");
                updatedPokemons.Add($"{newId},{parts[1]},{parts[2]},{parts[3]}");
                newId++; // Inkrementer ID
            }

            // Skriv de opdaterede data tilbage til filen uden ekstra nye linjer
            File.WriteAllText(filePath, string.Join(Environment.NewLine, updatedPokemons));

            Console.WriteLine("Pokémon slettet!");
        }
        else
        {
            Console.WriteLine("Ingen Pokémon fundet.");
        }

        Console.ReadKey();
    }
}


class Edit
{
    // Funktion til at redigere en Pokémon
    public void EditPokemon()
    {
        string filePath = "Pokedex.csv";
        string[] lines = File.ReadAllLines(filePath);

        // Spring headeren over
        var pokemons = lines.Skip(1).ToArray();

        // Bed brugeren om at søge efter en Pokémon
        Console.Clear();
        Console.Write("Søg efter Pokémon at redigere: ");
        string? input = Console.ReadLine();

        var matchingPokemons = pokemons
            .Where(pokemon => pokemon.Split(",")[1].Contains(input, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (matchingPokemons.Any())
        {
            // Vis alle fundne Pokémon
            Console.Clear();
            Console.WriteLine("Fundne Pokémons:");
            for (int i = 0; i < matchingPokemons.Length; i++)
            {
                string[] parts = matchingPokemons[i].Split(",");
                Console.WriteLine($"{i + 1}. Navn: {parts[1]}, Type: {parts[2]}, Styrke: {parts[3]}");
            }

            // Bed brugeren vælge hvilken Pokémon at redigere
            int choice = -1;
            bool validChoice = false;

            // Fortsæt indtil brugeren laver et gyldigt valg
            while (!validChoice)
            {
                Console.Write("\nVælg Pokémon at redigere (nummer): ");
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out choice) && choice >= 1 && choice <= matchingPokemons.Length)
                {
                    choice--; // Juster til 0-baseret index
                    validChoice = true;
                }
                else
                {
                    Console.WriteLine("Ugyldigt valg. Indtast venligst et gyldigt nummer.");
                }
            }

            string selectedPokemon = matchingPokemons[choice];
            string[] selectedPokemonParts = selectedPokemon.Split(",");

            // Start redigering af den valgte Pokémon
            bool editing = true;

            while (editing)
            {
                Console.Clear();
                Console.WriteLine("\nHvilken del af Pokémonen vil du redigere?");
                Console.WriteLine("1. Navn");
                Console.WriteLine("2. Type");
                Console.WriteLine("3. Styrke");
                Console.WriteLine("4. Afslut");
                Console.Write("Vælg en mulighed: ");

                string editChoiceInput = Console.ReadLine();
                int editChoice;

                if (int.TryParse(editChoiceInput, out editChoice))
                {
                    switch (editChoice)
                    {
                        case 1:
                            Console.Clear();
                            Console.Write("Indtast det nye navn: ");
                            selectedPokemonParts[1] = Console.ReadLine();
                            break;

                        case 2:
                            Console.Clear();
                            Console.Write("Indtast den nye type: ");
                            selectedPokemonParts[2] = Console.ReadLine();
                            break;

                        case 3:
                            Console.Clear();
                            bool validStrength = false;
                            while (!validStrength)
                            {
                                Console.Write("Indtast den nye styrke (kun tal): ");
                                string strengthInput = Console.ReadLine();
                                if (int.TryParse(strengthInput, out int strength))
                                {
                                    selectedPokemonParts[3] = strength.ToString();
                                    validStrength = true;
                                }
                                else
                                {
                                    Console.Clear();
                                    Console.WriteLine("Ugyldigt input. Styrken skal være et tal.");
                                }
                            }
                            break;

                        case 4:
                            editing = false;
                            break;

                        default:
                            Console.Clear();
                            Console.WriteLine("Ugyldigt valg, prøv igen.");
                            continue; // Fortsæt til næste iteration uden at spørge om fortsættelse
                    }

                    if (editing)
                    {
                        Console.Clear();
                        // Spørg om brugeren vil redigere noget andet
                        Console.WriteLine("\nVil du redigere noget andet? (Ja / Nej)");
                        string continueEditingInput = Console.ReadLine()?.ToLower();

                        if (continueEditingInput == "nej")
                        {
                            editing = false;
                        }
                        else if (continueEditingInput == "ja")
                        {
                            // Hvis brugeren skriver 'ja', gentages valgene for redigering
                            continue;
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Ugyldigt valg, prøv igen.");
                        }
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Ugyldigt valg, prøv igen.");
                }
            }

            // Opdater den valgte Pokémon med de nye detaljer
            string updatedPokemon = string.Join(",", selectedPokemonParts);
            pokemons = pokemons.Select(pokemon => pokemon == selectedPokemon ? updatedPokemon : pokemon).ToArray();

            // Byg filen uden ekstra linjer i slutningen
            using (var writer = new StreamWriter(filePath, false))
            {
                writer.Write(lines[0] + "\n"); // Skriv header med eksplicit linjeskift

                for (int i = 0; i < pokemons.Length; i++)
                {
                    if (i == pokemons.Length - 1)
                    {
                        // Undgå at tilføje ekstra linjeskift i slutningen af filen
                        writer.Write(pokemons[i]);
                    }
                    else
                    {
                        writer.WriteLine(pokemons[i]);
                    }
                }
            }

            Console.Clear();
            Console.WriteLine("\nPokémon er blevet opdateret!");
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Ingen Pokémon fundet.");
        }

        Console.ReadKey();
    }
}