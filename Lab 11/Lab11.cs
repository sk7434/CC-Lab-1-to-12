using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

class Token
{
    public string Type { get; set; } = string.Empty;
    public string? Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public override string ToString()
    {
        return $"{Type} ({Line},{Column}){(string.IsNullOrEmpty(Value) ? "" : ": " + Value)}";
    }
}

class SymbolTableEntry
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public override string ToString()
    {
        return $"{Name}\t{Type}\t{Value}\t({Line},{Column})";
    }
}

class LexicalAnalyzer
{
    private static readonly Dictionary<string, string> Keywords = new Dictionary<string, string>
    {
        { "int", "INT" },
        { "float", "FLOAT" },
        { "char", "CHAR" },
        { "main", "MAIN" },
        { "if", "IF" },
        { "else", "ELSE" },
        { "while", "WHILE" },
        { "return", "RETURN" }
    };

    private static readonly Dictionary<string, string> Operators = new Dictionary<string, string>
    {
        { "+", "PLUS" },
        { "-", "MINUS" },
        { "*", "MUL" },
        { "/", "DIV" },
        { "=", "ASSIGN" },
        { "==", "EQ" },
        { "!=", "NEQ" },
        { "<", "LT" },
        { ">", "GT" },
        { "<=", "LE" },
        { ">=", "GE" }
    };

    private static readonly Dictionary<string, string> Punctuations = new Dictionary<string, string>
    {
        { "(", "LPAREN" },
        { ")", "RPAREN" },
        { "{", "LBRACE" },
        { "}", "RBRACE" },
        { ";", "SEMI" },
        { ",", "COMMA" }
    };

    private static readonly Regex IdentifierRegex = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
    private static readonly Regex IntegerRegex = new Regex(@"^\d+$");
    private static readonly Regex FloatRegex = new Regex(@"^\d+\.\d+$");
    private static readonly Regex CharRegex = new Regex(@"^'.'$");

    public static List<Token> Tokenize(string sourceCode)
    {
        List<Token> tokens = new List<Token>();
        string[] lines = sourceCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
        {
            string line = lines[lineNumber];
            int column = 0;

            while (column < line.Length)
            {
                // Skip whitespace
                if (char.IsWhiteSpace(line[column]))
                {
                    column++;
                    continue;
                }

                // Check for comments
                if (column < line.Length - 1 && line[column] == '/' && line[column + 1] == '/')
                {
                    // Skip the rest of the line for single-line comments
                    break;
                }

                // Check for operators (need to check multi-char operators first)
                bool foundOperator = false;
                foreach (var op in Operators)
                {
                    if (column <= line.Length - op.Key.Length && 
                        line.Substring(column, op.Key.Length) == op.Key)
                    {
                        tokens.Add(new Token
                        {
                            Type = op.Value,
                            Line = lineNumber + 1,
                            Column = column + 1
                        });
                        column += op.Key.Length;
                        foundOperator = true;
                        break;
                    }
                }
                if (foundOperator) continue;

                // Check for punctuations
                bool foundPunctuation = false;
                foreach (var punct in Punctuations)
                {
                    if (column < line.Length && line[column].ToString() == punct.Key)
                    {
                        tokens.Add(new Token
                        {
                            Type = punct.Value,
                            Line = lineNumber + 1,
                            Column = column + 1
                        });
                        column++;
                        foundPunctuation = true;
                        break;
                    }
                }
                if (foundPunctuation) continue;

                // Check for identifiers and keywords
                if (char.IsLetter(line[column]) || line[column] == '_')
                {
                    int start = column;
                    while (column < line.Length && (char.IsLetterOrDigit(line[column]) || line[column] == '_'))
                    {
                        column++;
                    }
                    string word = line.Substring(start, column - start);

                    if (Keywords.ContainsKey(word))
                    {
                        tokens.Add(new Token
                        {
                            Type = Keywords[word],
                            Line = lineNumber + 1,
                            Column = start + 1
                        });
                    }
                    else
                    {
                        tokens.Add(new Token
                        {
                            Type = "ID",
                            Value = word,
                            Line = lineNumber + 1,
                            Column = start + 1
                        });
                    }
                    continue;
                }

                // Check for numeric literals
                if (char.IsDigit(line[column]))
                {
                    int start = column;
                    bool isFloat = false;

                    while (column < line.Length && 
                          (char.IsDigit(line[column]) || line[column] == '.'))
                    {
                        if (line[column] == '.')
                        {
                            isFloat = true;
                        }
                        column++;
                    }

                    string number = line.Substring(start, column - start);
                    
                    if (isFloat)
                    {
                        tokens.Add(new Token
                        {
                            Type = "FLOAT_CONST",
                            Value = number,
                            Line = lineNumber + 1,
                            Column = start + 1
                        });
                    }
                    else
                    {
                        tokens.Add(new Token
                        {
                            Type = "INT_CONST",
                            Value = number,
                            Line = lineNumber + 1,
                            Column = start + 1
                        });
                    }
                    continue;
                }

                // Check for character literals
                if (line[column] == '\'')
                {
                    int start = column;
                    column++; // Skip the opening quote
                    
                    // Skip the character
                    if (column < line.Length)
                        column++;
                    
                    // Skip the closing quote if present
                    if (column < line.Length && line[column] == '\'')
                        column++;
                    
                    string charLiteral = line.Substring(start, column - start);
                    tokens.Add(new Token
                    {
                        Type = "CHAR_CONST",
                        Value = charLiteral,
                        Line = lineNumber + 1,
                        Column = start + 1
                    });
                    continue;
                }

                // If we get here, we encountered an unknown character
                Console.WriteLine($"Unrecognized character at line {lineNumber + 1}, column {column + 1}: {line[column]}");
                column++;
            }
        }

        tokens.Add(new Token { Type = "EOF", Line = lines.Length, Column = 1 });
        return tokens;
    }
}

class SemanticAnalyzer
{
    private List<SymbolTableEntry> symbolTable = new List<SymbolTableEntry>();
    private List<string> errors = new List<string>();

    public void AnalyzeTokens(List<Token> tokens)
    {
        // Process declarations and assignments
        for (int i = 0; i < tokens.Count - 2; i++)
        {
            // Check for variable declarations: [type] [identifier] [=] [value] [;]
            if ((tokens[i].Type == "INT" || tokens[i].Type == "FLOAT" || tokens[i].Type == "CHAR") &&
                i + 4 < tokens.Count &&
                tokens[i + 1].Type == "ID" &&
                tokens[i + 2].Type == "ASSIGN")
            {
                string varType = tokens[i].Type;
                string varName = tokens[i + 1].Value ?? string.Empty;
                Token valueToken = tokens[i + 3];
                
                // Check type compatibility
                bool typeError = false;
                
                if (varType == "INT" && valueToken.Type != "INT_CONST")
                    typeError = true;
                else if (varType == "FLOAT" && valueToken.Type != "FLOAT_CONST" && valueToken.Type != "INT_CONST")
                    typeError = true;
                else if (varType == "CHAR" && valueToken.Type != "CHAR_CONST")
                    typeError = true;
                
                if (typeError)
                {
                    errors.Add($"Type mismatch in declaration at line {tokens[i].Line}, column {tokens[i].Column}: " +
                              $"Cannot assign {valueToken.Type} to {varType} variable '{varName}'");
                }
                
                // Add to symbol table
                symbolTable.Add(new SymbolTableEntry
                {
                    Name = varName,
                    Type = varType,
                    Value = valueToken.Value ?? string.Empty,
                    Line = tokens[i + 1].Line,
                    Column = tokens[i + 1].Column
                });
            }
            
            // Check for variable assignments: [identifier] [=] [value] [;]
            if (tokens[i].Type == "ID" && 
                i + 3 < tokens.Count &&
                tokens[i + 1].Type == "ASSIGN")
            {
                string varName = tokens[i].Value ?? string.Empty;
                Token valueToken = tokens[i + 2];
                
                // Find variable in symbol table
                SymbolTableEntry? entry = symbolTable.Find(e => e.Name == varName);
                
                if (entry is null)
                {
                    errors.Add($"Undeclared variable '{varName}' at line {tokens[i].Line}, column {tokens[i].Column}");
                }
                else
                {
                    // Check type compatibility
                    bool typeError = false;
                    
                    if (entry.Type == "INT" && valueToken.Type != "INT_CONST" && valueToken.Type != "ID")
                        typeError = true;
                    else if (entry.Type == "FLOAT" && valueToken.Type != "FLOAT_CONST" && valueToken.Type != "INT_CONST" && valueToken.Type != "ID")
                        typeError = true;
                    else if (entry.Type == "CHAR" && valueToken.Type != "CHAR_CONST" && valueToken.Type != "ID")
                        typeError = true;
                    
                    if (typeError)
                    {
                        errors.Add($"Type mismatch in assignment at line {tokens[i].Line}, column {tokens[i].Column}: " +
                                  $"Cannot assign {valueToken.Type} to {entry.Type} variable '{varName}'");
                    }
                    
                    // Update symbol table
                    if (valueToken.Type == "ID")
                    {
                        // Look up the value from the other variable
                        SymbolTableEntry? sourceEntry = symbolTable.Find(e => e.Name == valueToken.Value);
                        if (sourceEntry != null)
                        {
                            entry.Value = sourceEntry.Value;
                        }
                        else
                        {
                            errors.Add($"Undeclared variable '{valueToken.Value}' used at line {valueToken.Line}, column {valueToken.Column}");
                        }
                    }
                    else
                    {
                        entry.Value = valueToken.Value;
                    }
                }
            }
            
            // Check for expression operations and their type compatibility
            if ((tokens[i].Type == "ID" || tokens[i].Type == "INT_CONST" || tokens[i].Type == "FLOAT_CONST") &&
                i + 3 < tokens.Count &&
                (tokens[i + 1].Type == "PLUS" || tokens[i + 1].Type == "MINUS" || 
                 tokens[i + 1].Type == "MUL" || tokens[i + 1].Type == "DIV") &&
                (tokens[i + 2].Type == "ID" || tokens[i + 2].Type == "INT_CONST" || tokens[i + 2].Type == "FLOAT_CONST"))
            {
                // Determine the types of operands
                string type1 = tokens[i].Type;
                string type2 = tokens[i + 2].Type;
                
                if (type1 == "ID")
                {
                    SymbolTableEntry? entry = symbolTable.Find(e => e.Name == tokens[i].Value);
                    if (entry == null)
                    {
                        errors.Add($"Undeclared variable '{tokens[i].Value}' at line {tokens[i].Line}, column {tokens[i].Column}");
                        continue;
                    }
                    if (entry != null)
                        type1 = entry.Type;
                }
                
                if (type2 == "ID")
                {
                    SymbolTableEntry? entry = symbolTable.Find(e => e.Name == tokens[i + 2].Value);
                    if (entry == null)
                    {
                        errors.Add($"Undeclared variable '{tokens[i + 2].Value}' at line {tokens[i + 2].Line}, column {tokens[i + 2].Column}");
                        continue;
                    }
                    if (entry != null)
                        type2 = entry.Type;
                }
                
                // Check for type compatibility in operations
                if ((type1 == "CHAR" || type2 == "CHAR") && 
                    tokens[i + 1].Type != "PLUS") // Only + allowed for char in this simple analyzer
                {
                    errors.Add($"Invalid operation on character type at line {tokens[i].Line}, column {tokens[i].Column}");
                }
            }
        }
    }

    public void PrintSymbolTable()
    {
        Console.WriteLine("\nSymbol Table:");
        Console.WriteLine("Name\tType\tValue\tPosition");
        foreach (var entry in symbolTable)
        {
            Console.WriteLine(entry);
        }
    }

    public void PrintErrors()
    {
        if (errors.Count == 0)
        {
            Console.WriteLine("\nNo semantic errors found.");
        }
        else
        {
            Console.WriteLine("\nSemantic errors found:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the source code file path or press Enter to use default test code:");
        string filePath = Console.ReadLine() ?? string.Empty;
        string sourceCode;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            // Default test code
            sourceCode = @"
Begin(){
int a=5;
int b=10;
int c=0;
c=a+b;
if(c>a)
print a;
else print c;
}end
";
        }
        else
        {
            try
            {
                sourceCode = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return;
            }
        }

        Console.WriteLine("Tokenizing source code...");
        List<Token> tokens = LexicalAnalyzer.Tokenize(sourceCode);

        Console.WriteLine("Output:");
        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }

        // Perform semantic analysis
        SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer();
        semanticAnalyzer.AnalyzeTokens(tokens);
        semanticAnalyzer.PrintSymbolTable();
        semanticAnalyzer.PrintErrors();
    }
}
