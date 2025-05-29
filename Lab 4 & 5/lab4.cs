using System;
using System.Collections.Generic;
using System.Text;

class LexicalAnalyzer
{
    private string sourceCode;
    private int bufferSize = 16; // Size of each buffer
    private char[] buffer1, buffer2;
    private int forward, lexemeStart;
    private bool isBuffer1Active;
    private HashSet<string> keywords = new HashSet<string> { "int", "float", "if", "else", "while", "return" };

    public LexicalAnalyzer(string source)
    {
        sourceCode = source + " "; // Ensure an ending space for termination
        buffer1 = new char[bufferSize];
        buffer2 = new char[bufferSize];
        forward = 0;
        lexemeStart = 0;
        isBuffer1Active = true;
        LoadBuffer();
    }

    private void LoadBuffer()
    {
        int start = isBuffer1Active ? 0 : bufferSize;
        int length = Math.Min(bufferSize, sourceCode.Length - start);
        char[] targetBuffer = isBuffer1Active ? buffer1 : buffer2;

        for (int i = 0; i < length; i++)
            targetBuffer[i] = sourceCode[start + i];
    }

    private char GetChar()
    {
        if (forward >= sourceCode.Length)
            return '\0'; // End of source

        char currentChar = sourceCode[forward++];
        if (forward % bufferSize == 0)
        {
            isBuffer1Active = !isBuffer1Active;
            LoadBuffer();
        }
        return currentChar;
    }

    public void Analyze()
    {
        StringBuilder token = new StringBuilder();
        char ch;
        while ((ch = GetChar()) != '\0')
        {
            if (char.IsWhiteSpace(ch))
            {
                if (token.Length > 0)
                {
                    ProcessToken(token.ToString());
                    token.Clear();
                }
            }
            else
            {
                token.Append(ch);
            }
        }
    }

    private void ProcessToken(string token)
    {
        if (keywords.Contains(token))
            Console.WriteLine($"Keyword: {token}");
        else if (int.TryParse(token, out _))
            Console.WriteLine($"Integer: {token}");
        else
            Console.WriteLine($"Identifier: {token}");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter source code:");
        string sourceCode = Console.ReadLine();
        LexicalAnalyzer lexer = new LexicalAnalyzer(sourceCode);
        lexer.Analyze();
    }
}

