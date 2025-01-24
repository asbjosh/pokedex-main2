using System;
using System.IO;
using pokedex;

namespace Pokedex;

public class Login
{
    Program program = new();
    public void LoginMenu()
    {
        User user = new();

        do
        {
            Console.Clear();
            Console.Write("Indtast brugernavn: ");
            string? username = Console.ReadLine();

            Console.Clear();
            Console.Write("Indtast kodeord: ");
            string? password = Console.ReadLine();

            // Kontroller loginoplysninger
            user.LoggedIn = LoginCheck(username, password);

            if (user.LoggedIn == false)
            {
                Console.Clear();
                Console.WriteLine("Brugernavn eller adgangskode er forkert!"); // Hvis login fejler
                Console.ReadKey();
            }
        } while (!user.LoggedIn); // Gentag indtil login er succesfuldt

        Program.IsLoggedIn = true;  // Opdater global login status
    }

    // Funktion til at kontrollere loginoplysninger mod filer
    public bool LoginCheck(string username, string password)
    {
        string[] file = File.ReadAllLines("userdata.csv").Skip(1).ToArray(); // Læs filen og ignorér headeren

        foreach (var item in file)
        {
            string[] x = item.Split(","); // Split linjen i brugernavn og adgangskode

            if (x[0].ToLower() == username.ToLower()) // Hvis brugernavnet findes
            {
                string hashedPassword = HashingUtils.ComputeSha256Hash(password); // Hash indtastet password

                // Hvis hashede adgangskoder matcher
                if (x[1] == hashedPassword)
                {
                    return true;
                }
            }
        }

        return false; // Returner false, hvis login fejler
    }
}


public class User
{
    public bool LoggedIn { get; set; } // Brugerens loginstatus
}