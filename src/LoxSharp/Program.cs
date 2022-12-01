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
            runFile(args[0]);
        }
        else
        {
            runPrompt();
        }

    }

    private static void runFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        run(Encoding.Default.GetString(bytes));

        if (hadError) Environment.Exit(65);
    }

    private static void runPrompt()
    {
        using var streamReader = new StreamReader(Console.OpenStandardInput(), Encoding.Default, false, 8192);

        while (true)
        {
            Console.WriteLine("> ");
            string line = streamReader.ReadLine()!;
            if (line == null) break;
            run(line);
            hadError = false;
        }
    }

    private static void run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    internal static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}



