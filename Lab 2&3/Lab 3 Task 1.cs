using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string pattern = @"^[+-]?(\d{1,5}(\.\d{1,5})?|\.\d{1,5})$";
        string[] testInputs = { "123", "12.34", ".456", "-0.12", "+45.6", "123.456", "+12.345" };

        foreach (string input in testInputs)
        {
            Console.WriteLine($"{input} -> {Regex.IsMatch(input, pattern)}");
        }
    }
}
