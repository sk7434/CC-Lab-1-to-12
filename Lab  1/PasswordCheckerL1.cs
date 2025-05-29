using System;
using System.Text.RegularExpressions;

class PasswordChecker
{

    public static bool IsValidPassword(string password)
    {
        string pattern = @"^(?=(.*\d.*\d)) (?=(.*[A-Z]))(?=(.*[!@#$%^&*()\-_+=\[\]{}|;:,.<>?/~`]))
        (?=(.*[a-z].*[a-z].*[a-z].*[a-z]))[^\s]{1,12}$ ";


        Regex regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);


        return regex.IsMatch(password);
    }

    static void Main(string[] args)
    {

        Console.Write("Enter your password: ");
        string password = "Haid56@#er";


        bool isValid = IsValidPassword(password);

        if (isValid)
        {
            Console.WriteLine("Password is valid!");
        }
        else
        {
            Console.WriteLine("Password is not valid.");
        }
    }
}

