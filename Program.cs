using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Pokedex;

namespace pokedex;

public class Program
{
    public static bool IsLoggedIn { get; set; } // Offentlig egenskab til at spore, om brugeren er logget ind

    public static void Main(string[] args)
    {
        string userDataFile = "userdata.csv";
        HashPasswordsInFile(userDataFile); // Kald funktion for at hashe adgangskoder i filen
        ShowMainMenu(); // Vis hovedmenuen
    }

    // Denne funktion hasher alle adgangskoder i filen, hvis de ikke allerede er hashede
    public static void HashPasswordsInFile(string inputFile)
    {
        if (!File.Exists(inputFile)) return; // Hvis filen ikke findes, gør intet

        string[] lines = File.ReadAllLines(inputFile); // Læs alle linjer i filen
        if (lines.Length == 0) return; // Hvis filen er tom, gør intet

        string[] updatedLines = new string[lines.Length];
        updatedLines[0] = lines[0]; // Kopier header linje (brugernavn, adgangskode)

        // Loop gennem alle linjer undtagen headeren og tjek om adgangskoden er hashet
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(","); // Split linjen i brugernavn og adgangskode
            if (parts.Length == 2)
            {
                string username = parts[0].Trim(); // Trim eventuelle mellemrum
                string password = parts[1].Trim(); // Trim eventuelle mellemrum

                // Hvis adgangskoden ikke allerede er hashet, hasher vi den
                if (!IsHashed(password))
                {
                    string hashedPassword = HashingUtils.ComputeSha256Hash(password); // Hash adgangskoden
                    updatedLines[i] = $"{username},{hashedPassword}"; // Erstat den gamle adgangskode med den hashede
                }
                else
                {
                    updatedLines[i] = lines[i]; // Hvis den er hashet, behold den originale linje
                }
            }
        }

        File.WriteAllLines(inputFile, updatedLines); // Skriv de opdaterede linjer tilbage til filen
    }

    // Beregn SHA-256 hash af en given streng
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData)); // Beregn hash
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // Konverter hash bytes til hex-form
            }
            return builder.ToString(); // Returner den genererede hash
        }
    }

    // Tjek om adgangskoden er allerede hashet (kontrollerer længde og tegn)
    public static bool IsHashed(string password)
    {
        return password.Length == 64 && System.Text.RegularExpressions.Regex.IsMatch(password, @"\A\b[0-9a-fA-F]+\b\Z");
    }

    // Vis hovedmenuen med muligheder for brugeren
    public static void ShowMainMenu()
    {
        do
        {
            Console.Clear(); // Ryd skærmen

            // Hvis brugeren ikke er logget ind, tilbydes login som mulighed
            if (!IsLoggedIn)
            {
                Console.WriteLine("1. Login");
            }
            else
            {
                Console.WriteLine("1. Du er logget ind, men du kan stadig skifte brugere!");
            }
            Console.WriteLine("2. Se alle Pokémons");
            Console.WriteLine("3. Søg efter en Pokémon");

            // Hvis brugeren er logget ind, kan de redigere Pokédexen
            if (!IsLoggedIn)
            {
                Console.WriteLine("4. Rediger i Pokédexet (Login først)");
            }
            else
            {
                Console.WriteLine("4. Rediger i Pokédexet");
            }
            Console.WriteLine("5. Afslut programmet");
            Console.Write("Tast en tast mellem 1-5: ");
            string menuChoice = Console.ReadLine()?.Trim(); // Hent brugerens valg som en streng

            // Tjek om brugerens input er en af de gyldige muligheder
            switch (menuChoice)
            {
                case "1":
                    Login login = new();
                    new Login().LoginMenu(); // Kald login-menuen
                    break;
                case "2":
                    new ViewPokémons().DisplayPokémons(); // Vis alle Pokémon
                    break;
                case "3":
                    new Search().Searching(); // Søg efter Pokémon
                    break;
                case "4":
                    if (IsLoggedIn)
                    {
                        new EditMenu().ViewEditMenu(); // Rediger Pokédex
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Du skal logge ind først!"); // Meddel bruger, hvis de ikke er logget ind
                    }
                    break;
                case "5":
                    Console.Clear();
                    Console.WriteLine("Programmet afsluttes!");
                    Thread.Sleep(3000); // Vent et par sekunder før afslutning
                    Environment.Exit(0); // Luk programmet
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Ugyldigt valg. Indtast venligst et nummer fra 1 til 5. Tryk på en tast for at prøve igen");
                    Console.ReadKey();
                    break;
            }
        } while (true); // Fortsæt med at vise menuen
    }
}

public static class HashingUtils
{
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData)); // Beregn hash
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // Konverter til hex-streng
            }
            return builder.ToString(); // Returner hashen
        }
    }
}