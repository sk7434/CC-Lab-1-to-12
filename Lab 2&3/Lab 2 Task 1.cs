using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string pattern = @"&&|\|\||!";
        string input = "x && y || !z";
        
        MatchCollection matches = Regex.Matches(input, pattern);
        
        foreach (Match match in matches)
        {
            Console.WriteLine($"Matched: {match.Value}");
        }
    }
}
