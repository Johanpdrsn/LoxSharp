namespace GenerateAST;

internal class GenerateAST
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {

            Console.Error.WriteLine("Usage: generate_ast <output_directory>");
            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            Environment.Exit(64);
        }

        string outputDir = args[0];

        DefineAST(outputDir, "Expr", new List<string>{
          "Binary   : Expr left, Token operaTor, Expr right",
          "Grouping : Expr expression",
          "Literal  : Object value",
          "Unary    : Token operaTor, Expr right" }
        );
    }


    private static void DefineAST(string outputDir, string baseName, List<string> types)
    {

        string path = $"{outputDir}/{baseName}.cs";
        StreamWriter writer = new(path);

        writer.WriteLine("namespace LoxSharp;");
        writer.WriteLine();
        writer.WriteLine($"abstract class {baseName} {{");

        foreach (string type in types)
        {
            string className = type.Split(":").First().Trim();
            string fields = type.Split(':')[1].Trim();
            DefineType(writer, baseName, className, fields);
        }

        writer.WriteLine("}");
        writer.Close();


    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine($"   class {className} : {baseName} {{");

        // Constructor
        writer.WriteLine($"        {className}({fieldList}) {{");

        // Store paramters in fields
        string[] fields = fieldList.Split(", ");
        foreach (string field in fields)
        {
            string name = field.Split(" ")[1];
            writer.WriteLine($"            this.{name} = {name};");
        }
        writer.WriteLine("        }");

        // Fields
        writer.WriteLine();
        foreach (string field in fields)
        {
            writer.WriteLine($"        readonly {field};");
        }
        writer.WriteLine("    }");
    }
}