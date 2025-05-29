using System;
using System.Collections;
using System.Collections.Generic;

class SLRParser
{
    static void Main()
    {
        // Initialize the input string (tokenized version)
        List<string> finalArray = new List<string> {
            "begin", "(", ")", "{", 
            "int", "a", "=", "5", ";", 
            "int", "b", "=", "10", ";", 
            "int", "c", "=", "0", ";", 
            "c", "=", "a", "+", "b", ";", 
            "if", "(", "c", ">", "a", ")", "{", "print", "a", ";", "}", 
            "else", "{", "print", "c", ";", "}", 
            "}", "end", "$"
        };

        // Define the columns (terminals and non-terminals)
        string[] Col = { 
            "begin", "(", ")", "{", "int", "a", "b", "c", "=", "5", "10", "0", ";", 
            "if", ">", "print", "else", "$", "}", "+", "end", 
            "Program", "DecS", "AssS", "IffS", "PriS", "Var", "Const" 
        };

        // Define the states (productions)
        List<string> States = new List<string> {
            "Program.begin ( ) { DecS DecS DecS AssS IffS } end",
            "DecS.int Var = Const ;",
            "AssS.Var = Var + Var ;",
            "IffS.if ( Var > Var ) { PriS } else { PriS }",
            "PriS.print Var ;",
            "Var.a",
            "Var.b",
            "Var.c",
            "Const.5",
            "Const.10",
            "Const.0"
        };

        // Initialize the stack
        Stack<string> Stack = new Stack<string>();
        Stack.Push("0");

        // Add end marker to input
        finalArray.Add("$");

        // Initialize the parse table as a dictionary
        var dict = new Dictionary<string, Dictionary<string, string>>();

        // State 0
        dict.Add("0", new Dictionary<string, string> {
            {"begin", "S2"}, {"(", ""}, {")", ""}, {"{", ""}, {"int", ""}, 
            {"a", ""}, {"b", ""}, {"c", ""}, {"=", ""}, {"5", ""}, {"10", ""}, {"0", ""}, 
            {";", ""}, {"if", ""}, {">", ""}, {"print", ""}, {"else", ""}, {"$", ""}, 
            {"}", ""}, {"+", ""}, {"end", ""}, 
            {"Program", "1"}, {"DecS", ""}, {"AssS", ""}, {"IffS", ""}, {"PriS", ""}, 
            {"Var", ""}, {"Const", ""}
        });

        // State 1 (Accept state)
        dict.Add("1", new Dictionary<string, string> {
            {"begin", ""}, {"(", ""}, {")", ""}, {"{", ""}, {"int", ""}, 
            {"a", ""}, {"b", ""}, {"c", ""}, {"=", ""}, {"5", ""}, {"10", ""}, {"0", ""}, 
            {";", ""}, {"if", ""}, {">", ""}, {"print", ""}, {"else", ""}, {"$", "Accept"}, 
            {"}", ""}, {"+", ""}, {"end", ""}, 
            {"Program", ""}, {"DecS", ""}, {"AssS", ""}, {"IffS", ""}, {"PriS", ""}, 
            {"Var", ""}, {"Const", ""}
        });

        // State 2
        dict.Add("2", new Dictionary<string, string> {
            {"begin", ""}, {"(", "S3"}, {")", ""}, {"{", ""}, {"int", ""}, 
            {"a", ""}, {"b", ""}, {"c", ""}, {"=", ""}, {"5", ""}, {"10", ""}, {"0", ""}, 
            {";", ""}, {"if", ""}, {">", ""}, {"print", ""}, {"else", ""}, {"$", ""}, 
            {"}", ""}, {"+", ""}, {"end", ""}, 
            {"Program", ""}, {"DecS", ""}, {"AssS", ""}, {"IffS", ""}, {"PriS", ""}, 
            {"Var", ""}, {"Const", ""}
        });

        // Continue adding all states from the parse table in the lab manual...
        // (For brevity, I'm showing a partial implementation. In a real solution,
        // you would include all states and their transitions as shown in the manual)

        // State 3
        dict.Add("3", new Dictionary<string, string> {
            {"begin", ""}, {"(", ""}, {")", "S4"}, {"{", ""}, {"int", ""}, 
            // ... rest of state 3 transitions
        });

        // ... continue with all other states

        // Parsing algorithm
        int pointer = 0;
        while (true)
        {
            string currentToken = finalArray[pointer];
            
            // Check if token is valid
            if (!((IList)Col).Contains(currentToken))
            {
                Console.WriteLine("Unable to Parse: Unknown Input '" + currentToken + "'");
                break;
            }

            string currentState = Stack.Peek();
            string action = dict[currentState][currentToken];

            if (action == "")
            {
                Console.WriteLine("Unable to Parse: No action for state " + currentState + " and token " + currentToken);
                break;
            }

            if (action == "Accept")
            {
                Console.WriteLine("Input successfully parsed!");
                break;
            }

            if (action.StartsWith("S"))
            {
                // Shift operation
                string newState = action.Substring(1);
                Stack.Push(currentToken);
                Stack.Push(newState);
                pointer++;
                Console.WriteLine("Shift: " + currentToken + ", goto state " + newState);
            }
            else if (action.StartsWith("R"))
            {
                // Reduce operation
                int productionNum = int.Parse(action.Substring(1)) - 1;
                string production = States[productionNum];
                string[] parts = production.Split('.');
                string lhs = parts[0];
                string rhs = parts[1].Trim();

                int popCount = rhs.Split(' ').Length * 2;
                for (int i = 0; i < popCount; i++)
                {
                    Stack.Pop();
                }

                string gotoState = Stack.Peek();
                Stack.Push(lhs);
                string newState = dict[gotoState][lhs];
                Stack.Push(newState);

                Console.WriteLine("Reduce: " + production);
            }
        }
    }
}
