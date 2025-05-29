using System;
using System.Collections.Generic;

class Grammar
{
    public string NonTerminal { get; set; }
    public List<string> Productions { get; set; }

    public Grammar(string nonTerminal)
    {
        NonTerminal = nonTerminal;
        Productions = new List<string>();
    }

    public void AddProduction(string production)
    {
        Productions.Add(production);
    }

    public void Display()
    {
        Console.WriteLine($"{NonTerminal} → {string.Join(" | ", Productions)}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Create grammar rules
        var expr = new Grammar("Expr");
        expr.AddProduction("Expr + Term");
        expr.AddProduction("Term");

        var term = new Grammar("Term");
        term.AddProduction("Term * Factor");
        term.AddProduction("Factor");

        var factor = new Grammar("Factor");
        factor.AddProduction("( Expr )");
        factor.AddProduction("Number");

        var number = new Grammar("Number");
        number.AddProduction("0");
        number.AddProduction("1");
        number.AddProduction("2");
        number.AddProduction("3");
        number.AddProduction("4");
        number.AddProduction("5");
        number.AddProduction("6");
        number.AddProduction("7");
        number.AddProduction("8");
        number.AddProduction("9");

        // Store all grammars
        var grammarList = new List<Grammar> { expr, term, factor, number };

        // Display the grammar
        Console.WriteLine("Context-Free Grammar:");
        Console.WriteLine("----------------------");
        foreach (var g in grammarList)
        {
            g.Display();
        }

        // Display terminals
        Console.WriteLine("\nTerminals: +, *, (, ), 0-9");
        Console.WriteLine("Non-terminals: Expr, Term, Factor, Number");
    }
}
