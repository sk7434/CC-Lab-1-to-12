using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace LexicalAnalyzerWithSymbolTable
{
    // Symbol Table Entry Class
    public class SymbolTableEntry
    {
        public int Index { get; set; }
        public string VariableName { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }

        public override string ToString()
        {
            return $"{Index,-5} {VariableName,-12} {Type,-8} {Value,-10} {LineNumber}";
        }
    }

    // Token Class
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }

        public override string ToString()
        {
            return $"<{Type}, {Value}>";
        }
    }

    // Main Lexical Analyzer Class
    public class LexicalAnalyzer
    {
        // Symbol Table
        private List<SymbolTableEntry> symbolTable;
        private int variableCounter = 1;

        // Regular Expressions
        private readonly Regex variableRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
        private readonly Regex intConstantRegex = new Regex(@"^[0-9]+$");
        private readonly Regex floatConstantRegex = new Regex(@"^[0-9]+\.[0-9]+([eE][+-]?[0-9]+)?$");
        private readonly Regex operatorRegex = new Regex(@"^[+\-*/=<>!&|%]$");
        private readonly Regex specialCharRegex = new Regex(@"^[;,(){}[\].]$");

        // Keywords
        private readonly HashSet<string> keywords = new HashSet<string>
        {
            "int", "float", "double", "char", "string", "bool",
            "if", "else", "while", "for", "do", "switch", "case",
            "break", "continue", "return", "void", "main",
            "true", "false", "null", "begin", "end", "print"
        };

        // Data types
        private readonly HashSet<string> dataTypes = new HashSet<string>
        {
            "int", "float", "double", "char", "string", "bool"
        };

        public LexicalAnalyzer()
        {
            symbolTable = new List<SymbolTableEntry>();
        }

        public void AnalyzeSourceCode(string sourceCode)
        {
            // Clear previous results
            symbolTable.Clear();
            variableCounter = 1;

            Console.WriteLine("=== LEXICAL ANALYZER WITH SYMBOL TABLE ===\n");
            Console.WriteLine("Source Code:");
            Console.WriteLine("------------");
            Console.WriteLine(sourceCode);
            Console.WriteLine();

            // Split into lines and process
            string[] lines = sourceCode.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<Token> allTokens = new List<Token>();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                var lineTokens = ProcessLine(lines[lineNumber].Trim(), lineNumber + 1);
                allTokens.AddRange(lineTokens);
            }

            // Display results
            DisplayTokens(allTokens);
            DisplaySymbolTable();
        }

        private List<Token> ProcessLine(string line, int lineNumber)
        {
            List<Token> tokens = new List<Token>();
            
            if (string.IsNullOrWhiteSpace(line)) 
                return tokens;

            List<string> lexemes = TokenizeLine(line);
            
            for (int i = 0; i < lexemes.Count; i++)
            {
                string lexeme = lexemes[i];
                Token token = GenerateToken(lexeme, lexemes, i, lineNumber);
                tokens.Add(token);
            }

            return tokens;
        }

        private List<string> TokenizeLine(string line)
        {
            List<string> tokens = new List<string>();
            string currentToken = "";
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (char.IsWhiteSpace(c))
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                }
                else if (IsOperatorOrSpecialChar(c))
                {
                    if (!string.IsNullOrEmpty(currentToken))
                    {
                        tokens.Add(currentToken);
                        currentToken = "";
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    currentToken += c;
                }
            }
            
            if (!string.IsNullOrEmpty(currentToken))
            {
                tokens.Add(currentToken);
            }
            
            return tokens;
        }

        private bool IsOperatorOrSpecialChar(char c)
        {
            return operatorRegex.IsMatch(c.ToString()) || specialCharRegex.IsMatch(c.ToString());
        }

        private Token GenerateToken(string lexeme, List<string> allLexemes, int currentIndex, int lineNumber)
        {
            // Check if it's a keyword
            if (keywords.Contains(lexeme.ToLower()))
            {
                return new Token { Type = "keyword", Value = lexeme, LineNumber = lineNumber };
            }
            
            // Check if it's an integer constant
            if (intConstantRegex.IsMatch(lexeme))
            {
                return new Token { Type = "const", Value = lexeme, LineNumber = lineNumber };
            }

            // Check if it's a float constant
            if (floatConstantRegex.IsMatch(lexeme))
            {
                return new Token { Type = "const", Value = lexeme, LineNumber = lineNumber };
            }
            
            // Check if it's an operator
            if (operatorRegex.IsMatch(lexeme))
            {
                return new Token { Type = "op", Value = lexeme, LineNumber = lineNumber };
            }
            
            // Check if it's a special character
            if (specialCharRegex.IsMatch(lexeme))
            {
                return new Token { Type = "special", Value = lexeme, LineNumber = lineNumber };
            }
            
            // Check if it's a variable
            if (variableRegex.IsMatch(lexeme))
            {
                return ProcessVariable(lexeme, allLexemes, currentIndex, lineNumber);
            }
            
            return new Token { Type = "unknown", Value = lexeme, LineNumber = lineNumber };
        }

        private Token ProcessVariable(string variable, List<string> allLexemes, int currentIndex, int lineNumber)
        {
            // Check if this is a variable declaration pattern
            if (IsVariableDeclaration(allLexemes, currentIndex))
            {
                string type = allLexemes[currentIndex - 1];
                string value = "undefined";
                
                // Check if there's an assignment
                if (currentIndex + 2 < allLexemes.Count && allLexemes[currentIndex + 1] == "=")
                {
                    value = allLexemes[currentIndex + 2];
                }
                
                // Add to symbol table
                var entry = new SymbolTableEntry
                {
                    Index = symbolTable.Count + 1,
                    VariableName = variable,
                    Type = type,
                    Value = value,
                    LineNumber = lineNumber
                };
                
                symbolTable.Add(entry);
                int varNum = variableCounter++;
                return new Token { Type = $"var{varNum}", Value = entry.Index.ToString(), LineNumber = lineNumber };
            }
            
            // Check if variable already exists in symbol table
            var existingEntry = FindVariableInSymbolTable(variable);
            if (existingEntry != null)
            {
                return new Token { Type = $"var{existingEntry.Index}", Value = existingEntry.Index.ToString(), LineNumber = lineNumber };
            }
            
            // If it's not a declaration and not in symbol table, treat as identifier
            return new Token { Type = "id", Value = variable, LineNumber = lineNumber };
        }

        private bool IsVariableDeclaration(List<string> lexemes, int currentIndex)
        {
            if (currentIndex == 0) return false;
            
            string previousLexeme = lexemes[currentIndex - 1];
            return dataTypes.Contains(previousLexeme.ToLower());
        }

        private SymbolTableEntry FindVariableInSymbolTable(string variableName)
        {
            return symbolTable.FirstOrDefault(entry => entry.VariableName == variableName);
        }

        private void DisplayTokens(List<Token> tokens)
        {
            Console.WriteLine("Generated Tokens:");
            Console.WriteLine("-----------------");
            
            int currentLine = 1;
            Console.Write($"Line {currentLine}: ");
            
            foreach (var token in tokens)
            {
                if (token.LineNumber != currentLine)
                {
                    Console.WriteLine();
                    currentLine = token.LineNumber;
                    Console.Write($"Line {currentLine}: ");
                }
                Console.Write($"{token} ");
            }
            Console.WriteLine("\n");
        }

        private void DisplaySymbolTable()
        {
            Console.WriteLine("Symbol Table:");
            Console.WriteLine("-------------");
            Console.WriteLine($"{"Index",-5} {"Variable",-12} {"Type",-8} {"Value",-10} {"Line#"}");
            Console.WriteLine(new string('-', 50));
            
            if (symbolTable.Count == 0)
            {
                Console.WriteLine("No variables found in symbol table.");
            }
            else
            {
                foreach (var entry in symbolTable)
                {
                    Console.WriteLine(entry.ToString());
                }
            }
            Console.WriteLine();
        }

        public void DisplayMenu()
        {
            Console.WriteLine("=== LEXICAL ANALYZER WITH SYMBOL TABLE ===");
            Console.WriteLine("1. Analyze predefined sample code");
            Console.WriteLine("2. Enter custom source code");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option (1-3): ");
        }

        public void RunSampleAnalysis()
        {
            string sampleCode = @"int x = 5;
float y = 3.14;
int z;
x = y + 10;
z = x * 2;
if x > 0
    print x;
end";
            
            AnalyzeSourceCode(sampleCode);
        }
    }

    // Main Program Class
    public class Program
    {
        public static void Main(string[] args)
        {
            LexicalAnalyzer analyzer = new LexicalAnalyzer();
            
            while (true)
            {
                Console.Clear();
                analyzer.DisplayMenu();
                
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        analyzer.RunSampleAnalysis();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                        
                    case "2":
                        Console.Clear();
                        Console.WriteLine("Enter your source code (press Enter twice to finish):");
                        Console.WriteLine("Example: int x = 5;");
                        Console.WriteLine("         float y = 3.14;");
                        Console.WriteLine();
                        
                        string userCode = "";
                        string line;
                        int emptyLineCount = 0;
                        
                        while (emptyLineCount < 2)
                        {
                            line = Console.ReadLine();
                            if (string.IsNullOrEmpty(line))
                            {
                                emptyLineCount++;
                            }
                            else
                            {
                                emptyLineCount = 0;
                                userCode += line + "\n";
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(userCode))
                        {
                            Console.Clear();
                            analyzer.AnalyzeSourceCode(userCode.Trim());
                        }
                        else
                        {
                            Console.WriteLine("No code entered.");
                        }
                        
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                        
                    case "3":
                        Console.WriteLine("Goodbye!");
                        return;
                        
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
