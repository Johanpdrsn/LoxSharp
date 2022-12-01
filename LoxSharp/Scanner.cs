namespace LoxSharp;

internal class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new List<Token>();
    private int start = 0;
    private int current = 0;
    private int line = 1;

    private static readonly Dictionary<string, TokenType> keywords;

    static Scanner()
    {
        keywords = new Dictionary<string, TokenType>
        {
            { "and", TokenType.AND },
            { "class", TokenType.CLASS },
            { "else", TokenType.ELSE },
            { "false", TokenType.FALSE },
            { "for", TokenType.FOR },
            { "fun", TokenType.FUN },
            { "if", TokenType.IF },
            { "nil", TokenType.NIL },
            { "or", TokenType.OR },
            { "print", TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super", TokenType.SUPER },
            { "this", TokenType.THIS },
            { "true", TokenType.TRUE },
            { "var", TokenType.VAR },
            { "while", TokenType.WHILE }
        };
    }

    internal Scanner(string source)
    {
        this.source = source;
    }


    internal List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }
    private bool IsAtEnd()
    {
        return current >= source.Length;
    }
    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(':
                AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;

            case '/':
                if (match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;

            case '!':
                AddToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                break;
            case '=':
                AddToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                break;
            case '<':
                AddToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                break;
            case '>':
                AddToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                break;

            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                line++;
                break;


            case '"':
                StringT(); break;

            default:
                if (char.IsDigit(c))
                {
                    Number();
                }
                else if (char.IsLetterOrDigit(c))
                {
                    Identifier();
                }
                else
                {
                    LoxSharp.Error(line, "Unexpected character");
                }
                break;
        }
    }

    private void Identifier()
    {
        while (char.IsLetterOrDigit(Peek())) Advance();

        string text = source[start..current];
        bool exist = keywords.TryGetValue(text, out TokenType type);
        if (!exist) type = TokenType.IDENTIFIER;
        AddToken(type);
    }

    private void Number()
    {
        while (char.IsDigit(Peek())) Advance();

        // Fractional part
        if (Peek() == '.' && char.IsDigit(PeekNext()))
        {
            Advance();

            while (char.IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.NUMBER, double.Parse(source[start..current]));
    }
    private void StringT()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            LoxSharp.Error(line, "Unterminated string");
        }

        // Closing '"'"
        Advance();
        string value = source[(start + 1)..(current - 1)];
        AddToken(TokenType.STRING, value);
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current++;
        return true;
    }

    private char Advance()
    {
        return source[current++];
    }
    private void AddToken(TokenType type, object literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }






}
