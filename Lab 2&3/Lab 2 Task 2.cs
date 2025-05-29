using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string pattern = @"==|!=|>=|<=|>|<";
        string input = "a >= b && c != d || e < f";
        
        MatchCollection matches = Regex.Matches(input, pattern);
        
        foreach (Match match in matches)
        {
            Console.WriteLine($"Matched: {match.Value}");
        }
    }
}
