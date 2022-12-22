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
          "Assign   : Token name, Expr value",
          "Binary   : Expr left, Token operaTor, Expr right",
          "Call     : Expr callee, Token paren, List<Expr> arguments",
          "Grouping : Expr expression",
          "Literal  : object? value",
          "Logical  : Expr left, Token operaTor, Expr right",
          "Unary    : Token operaTor, Expr right",
          "Variable : Token name"
        });

        DefineAST(outputDir, "Stmt", new List<string>
        {
            "Block      : List<Stmt> statements",
            "Expression : Expr expression",
            "Function   : Token name, List<Token> parameters, List<Stmt> body",
            "If         : Expr condition, Stmt thenBranch, Stmt? elseBranch",
            "Print      : Expr expression",
            "Return     : Token keyword, Expr? value",
            "Var        : Token name, Expr? initializer",
            "While      : Expr condition, Stmt body"
        });
    }


    private static void DefineAST(string outputDir, string baseName, List<string> types)
    {
        string path = $"{outputDir}/{baseName}.cs";
        StreamWriter writer = new(path);

        writer.WriteLine("using LoxSharp.Model;");
        writer.WriteLine("namespace LoxSharp.Model;");
        writer.WriteLine();
        writer.WriteLine($"public abstract class {baseName} {{");

        DefineVisitor(writer, baseName, types);

        foreach (string type in types)
        {
            string className = type.Split(":").First().Trim();
            string fields = type.Split(':')[1].Trim();
            DefineType(writer, baseName, className, fields);
        }

        writer.WriteLine();
        writer.WriteLine("    public abstract T Accept<T>(Visitor<T> visitor);");

        writer.WriteLine("}");
        writer.Close();


    }

    private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
    {
        writer.WriteLine($"   public class {className} : {baseName} {{");

        // Constructor
        writer.WriteLine($"        public {className}({fieldList}) {{");

        // Store paramters in fields
        string[] fields = fieldList.Split(", ");
        foreach (string field in fields)
        {
            string name = field.Split(" ")[1];
            writer.WriteLine($"            this.{name} = {name};");
        }
        writer.WriteLine("        }");

        writer.WriteLine();
        writer.WriteLine("    public override T Accept<T>(Visitor<T> visitor) {");
        writer.WriteLine($"        return visitor.Visit{className}{baseName}(this);");
        writer.WriteLine("    }");
        // Fields
        writer.WriteLine();
        foreach (string field in fields)
        {
            writer.WriteLine($"        public readonly {field};");
        }
        writer.WriteLine("    }");
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("    public interface Visitor<T> {");

        foreach (string type in types)
        {
            string typename = type.Split(":").First().Trim();
            writer.WriteLine($"        public T Visit{typename}{baseName}({typename} {baseName.ToLower()});");
        }
        writer.WriteLine("    }");
    }
}