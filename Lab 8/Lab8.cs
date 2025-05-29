using System;

class LL1Parser
{
    private string input;
    private int index;
    private char lookahead;

    public LL1Parser(string input)
    {
        this.input = input.Replace(" ", ""); 
        this.index = 0;
        this.lookahead = this.input.Length > 0 ? this.input[this.index] : '$'; 
    }

    private void Match(char expected)
    {
        if (lookahead == expected)
        {
            index++;
            lookahead = index < input.Length ? input[index] : '$'; 
        }
        else
        {
            throw new Exception($"Syntax Error: Expected '{expected}', found '{lookahead}'");
        }
    }

    public void Parse()
    {
        S(); 
        if (lookahead == '$')
            Console.WriteLine("✅ Parsing Successful!");
        else
            throw new Exception("❌ Parsing Failed: Unexpected characters at end.");
    }

    private void S()
    {
        if (lookahead == 'd' || lookahead == 'e' || lookahead == 'b')
        {
            A();
            Match('y');
        }
        else if (lookahead == 'b')
        {
            Match('b');
            A();
        }
        else
        {
            throw new Exception($"Syntax Error: Unexpected token '{lookahead}' in 'S' production");
        }
    }

    private void A()
    {
        if (lookahead == 'd' || lookahead == 'e')
        {
            Match(lookahead); 
        }
        else
        {
            throw new Exception($"Syntax Error: Unexpected token '{lookahead}' in 'A' production");
        }
    }
}

class Program
{
    static void Main()
    {
        try
        {
            string input = "d y"; 
            var parser = new LL1Parser(input);
            parser.Parse();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
