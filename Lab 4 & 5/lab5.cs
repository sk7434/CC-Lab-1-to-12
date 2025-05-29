using System;
using System.Collections.Generic;

class Symbol
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Address { get; set; }

    public Symbol(string name, string type, int address)
    {
        Name = name;
        Type = type;
        Address = address;
    }

    public override string ToString()
    {
        return $"{Name} -> Type: {Type}, Address: {Address}";
    }
}

class SymbolTable
{
    private Dictionary<string, Symbol> table;

    public SymbolTable()
    {
        table = new Dictionary<string, Symbol>();
    }

    private int HashFunction(string key)
    {
        return key.GetHashCode(); // Hash function using built-in hash code
    }

    public void Insert(string name, string type, int address)
    {
        int hash = HashFunction(name);
        if (!table.ContainsKey(name))
        {
            table[name] = new Symbol(name, type, address);
            Console.WriteLine($"Inserted: {name} at Hash: {hash}");
        }
        else
        {
            Console.WriteLine($"Error: {name} already exists in symbol table.");
        }
    }

    public Symbol Lookup(string name)
    {
        if (table.ContainsKey(name))
        {
            return table[name];
        }
        return null;
    }

    public void Display()
    {
        Console.WriteLine("Symbol Table:");
        foreach (var entry in table)
        {
            Console.WriteLine(entry.Value);
        }
    }
}

class Program
{
    static void Main()
    {
        SymbolTable symbolTable = new SymbolTable();
        symbolTable.Insert("x", "int", 100);
        symbolTable.Insert("y", "float", 104);
        symbolTable.Insert("func", "void", 200);

        Console.WriteLine("\nLookup Result:");
        Symbol foundSymbol = symbolTable.Lookup("x");
        if (foundSymbol != null)
            Console.WriteLine(foundSymbol);
        else
            Console.WriteLine("Symbol not found.");

        Console.WriteLine("\nFinal Symbol Table:");
        symbolTable.Display();
    }
}
