using System.Text;

namespace LoxSharp;

class LoxSharp
{
    static bool hadError = false;
    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: loxc [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }

    }

    private static void RunFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.Default.GetString(bytes));

        if (hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
        using var streamReader = new StreamReader(Console.OpenStandardInput(), Encoding.Default, false, 8192);

        while (true)
        {
            Console.WriteLine("> ");
            string line = streamReader.ReadLine()!;
            if (line == null) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new Parser(tokens);
        Expr expression = parser.Parse();

        if (hadError) return;

        Console.WriteLine(new ASTPrinter().Print(expression));
    }

    internal static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    internal static void Error(Token token, string message)
    {
        if (token.type == TokenType.EOF)
        {
            Report(token.line, "at end", message);
        }
        else
        {
            Report(token.line, $"at '{token.lexeme}'", message);
        }
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}



