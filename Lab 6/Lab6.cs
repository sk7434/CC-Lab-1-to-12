using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class JavaGrammarChecker
{
    // Example Java keywords
    private static HashSet<string> javaKeywords = new HashSet<string>
    {
        "class", "public", "static", "void", "int", "double", "if", "else", "while", "for", "return"
    };

    // Regex for valid Java identifiers
    private static Regex identifierRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");

    // Example grammar (can be replaced with input)
    private static Dictionary<string, string> sampleGrammar = new Dictionary<string, string>
    {
        {"S", "public static void main ( String [ ] args ) { BODY }"},
        {"BODY", "int identifier = 0 ; BODY | return ;"}
    };

    public static void Main(string[] args)
    {
        Console.WriteLine("Checking the provided grammar for Java language compliance...\n");

        foreach (var rule in sampleGrammar)
        {
            string lhs = rule.Key;
            string rhs = rule.Value;

            Console.WriteLine($"Rule: {lhs} -> {rhs}");
            CheckRule(lhs, rhs);
            Console.WriteLine();
        }
    }

    private static void CheckRule(string lhs, string rhs)
    {
        // Check LHS identifier
        if (!IsValidIdentifier(lhs))
        {
            Console.WriteLine($"❌ Invalid non-terminal name: {lhs}");
        }
        else
        {
            Console.WriteLine($"✅ Valid non-terminal: {lhs}");
        }

        // Check RHS tokens
        string[] tokens = rhs.Split(new char[] { ' ', '|', ';', '[', ']', '(', ')', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            if (javaKeywords.Contains(token))
            {
                Console.WriteLine($"✅ Recognized Java keyword: {token}");
            }
            else if (token == "identifier")
            {
                Console.WriteLine($"✅ Placeholder for identifier: {token}");
            }
            else if (IsValidIdentifier(token))
            {
                Console.WriteLine($"✅ Valid identifier: {token}");
            }
            else if (int.TryParse(token, out _))
            {
                Console.WriteLine($"✅ Recognized integer literal: {token}");
            }
            else
            {
                Console.WriteLine($"⚠️ Unrecognized or invalid token: {token}");
            }
        }
    }

    private static bool IsValidIdentifier(string token)
    {
        return identifierRegex.IsMatch(token) && !javaKeywords.Contains(token);
    }
}
