using System.Text;

namespace LoxSharp;

class LoxSharp
{
    private static readonly Interpreter interpreter = new();
    static bool hadError = false;
    static bool hadRuntimeError = false;

    static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: loxc [script]");
            System.Environment.Exit(64);
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

        if (hadError) System.Environment.Exit(65);
        if (hadRuntimeError) System.Environment.Exit(70);

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
        List<Stmt> statements = parser.Parse();

        if (hadError) return;

        interpreter.Interpret(statements);
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

    public static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine($"{error.Message}\n[line {error.token.line}]");
        hadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}



