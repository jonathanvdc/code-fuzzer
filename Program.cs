using System;
using CodeFuzzer;

namespace MiniCFuzzer
{
    public static class Program
    {
        private static Grammar grammar;
        private static Lexer lexer;
        private const string whitespaceSymbol = "WHITESPACE";
        private const string unknownSymbol = "UNKNOWN";

        public static Grammar MiniCGrammar { get { return grammar; } }
        public static Lexer MiniCLexer { get { return lexer; } }

        public static void Main(string[] Args)
        {
            var generator = new SyntaxGenerator(
                new Random(), grammar, whitespaceSymbol, lexer);

            string result = generator.GenerateSourceCode(grammar.StartSymbol);
            Console.Write(result);
        }

        static Program()
        {
            grammar = new Grammar();

            char[] escCodes = new char[]
            {
                 '\\', '\'', '\"', 'n', 'r', 't', 'f', 'b'
            };

            grammar.DefineToken("IDENTIFIER", new IdentifierDef(30));

            // ASSIGN, // '='
            grammar.DefineToken("ASSIGN", new SingletonTokenDef("="));

            // delimiters
            // LBRA, // '{' // left brace
            grammar.DefineToken("LBRA", new SingletonTokenDef("{"));
            // RBRA, // '}' // right brace
            grammar.DefineToken("RBRA", new SingletonTokenDef("}"));
            // LPAR, // '(' // left parenthesis
            grammar.DefineToken("LPAR", new SingletonTokenDef("("));
            // RPAR, // ')' // right parenthesis
            grammar.DefineToken("RPAR", new SingletonTokenDef(")"));
            // LSBR, // '[' // left square brace
            grammar.DefineToken("LSBR", new SingletonTokenDef("["));
            // RSBR, // ']' // left square brace
            grammar.DefineToken("RSBR", new SingletonTokenDef("]"));
            // SC, // ';'   // semicolon
            grammar.DefineToken("SC", new SingletonTokenDef(";"));
            // COMMA, // ','
            grammar.DefineToken("COMMA", new SingletonTokenDef(","));

            // types
            // INT,  // "int"
            grammar.DefineToken("INT", new SingletonTokenDef("int"));
            // VOID, // "void"
            grammar.DefineToken("VOID", new SingletonTokenDef("void"));
            // CHAR, // "char"
            grammar.DefineToken("CHAR", new SingletonTokenDef("char"));

            // keywords
            // IF,     // "if"
            grammar.DefineToken("IF", new SingletonTokenDef("if"));
            // ELSE,   // "else"
            grammar.DefineToken("ELSE", new SingletonTokenDef("else"));
            // WHILE,  // "while"
            grammar.DefineToken("WHILE", new SingletonTokenDef("while"));
            // RETURN, // "return"
            grammar.DefineToken("RETURN", new SingletonTokenDef("return"));
            // STRUCT, // "struct"
            grammar.DefineToken("STRUCT", new SingletonTokenDef("struct"));
            // SIZEOF, // "sizeof"
            grammar.DefineToken("SIZEOF", new SingletonTokenDef("sizeof"));

            // include
            // INCLUDE, // "#include"
            grammar.DefineToken("INCLUDE", new SingletonTokenDef("#include"));

            // literals
            grammar.DefineToken("STRING_LITERAL", new StringLiteralDef('\"', escCodes, 0, 50));
            grammar.DefineToken("CHAR_LITERAL", new StringLiteralDef('\'', escCodes, 1, 1));
            grammar.DefineToken("INT_LITERAL", new IntegerDef());

            // logical operators
            // AND, // "&&"
            grammar.DefineToken("AND", new SingletonTokenDef("&&"));
            // OR,  // "||"
            grammar.DefineToken("OR", new SingletonTokenDef("||"));

            // comparisons
            // EQ, // "=="
            grammar.DefineToken("EQ", new SingletonTokenDef("=="));
            // NE, // "!="
            grammar.DefineToken("NE", new SingletonTokenDef("!="));
            // LT, // '<'
            grammar.DefineToken("LT", new SingletonTokenDef("<"));
            // GT, // '>'
            grammar.DefineToken("GT", new SingletonTokenDef(">"));
            // LE, // "<="
            grammar.DefineToken("LE", new SingletonTokenDef("<="));
            // GE, // ">="
            grammar.DefineToken("GE", new SingletonTokenDef(">="));

            // operators
            // PLUS,  // '+'
            grammar.DefineToken("PLUS", new SingletonTokenDef("+"));
            // MINUS, // '-'
            grammar.DefineToken("MINUS", new SingletonTokenDef("-"));
            // ASTERISK, // '*'  // can be used for multiplication or pointers
            grammar.DefineToken("ASTERISK", new SingletonTokenDef("*"));
            // DIV,   // '/'
            grammar.DefineToken("DIV", new SingletonTokenDef("/"));
            // REM,   // '%'
            grammar.DefineToken("REM", new SingletonTokenDef("%"));

            // // struct member access
            // DOT, // '.'
            grammar.DefineToken("DOT", new SingletonTokenDef("."));

            grammar.DefineToken("WHITESPACE", new WhitespaceDef());
            grammar.MarkTrivia("WHITESPACE");
            grammar.DefineToken("SL_COMMENT", new SingleLineCommentDef("//", new char[] { '\n', '\r' }, 50));
            grammar.MarkTrivia("SL_COMMENT");
            grammar.DefineToken("ML_COMMENT", new MultiLineCommentDef("/*", "*/", 50));
            grammar.MarkTrivia("ML_COMMENT");

            // # #  comment
            // # () grouping
            // # [] optional
            // # *  zero or more
            // # +  one or more
            // # |  alternative
            //
            //
            // program    ::= (include)* (structdecl)* (vardecl)* (fundecl)* EOF
            grammar.AddProductionRule("program", new string[]
            {
                grammar.AddStarRule("include"),
                grammar.AddStarRule("structdecl"),
                grammar.AddStarRule("vardecl"),
                grammar.AddStarRule("fundecl")
            });

            // include    ::= "#include" STRING_LITERAL
            grammar.AddProductionRule("include", new string[]
            {
                "INCLUDE", "STRING_LITERAL"
            });

            // structdecl ::= structtype "{" (vardecl)+ "}" ";"    # structure declaration
            grammar.AddProductionRule("structdecl", new string[]
            {
                "structtype", "LBRA",
                grammar.AddStarRule("vardecl", 1),
                "RBRA", "SC"
            });

            // vardecl    ::= type IDENT ";"                       # normal declaration, e.g. int a;
            grammar.AddProductionRule("vardecl", new string[]
            {
                "type", "IDENTIFIER", "SC"
            });
            //              | type IDENT "[" INT_LITERAL "]" ";"   # array declaration, e.g. int a[2];
            grammar.AddProductionRule("vardecl", new string[]
            {
                "type", "IDENTIFIER", "LSBR", "INT_LITERAL", "RSBR", "SC"
            });

            // fundecl    ::= type IDENT "(" params ")" block    # function declaration
            grammar.AddProductionRule("fundecl", new string[]
            {
                "type", "IDENTIFIER", "LPAR", "params", "RPAR", "block"
            });

            // type       ::= ("int" | "char" | "void" | structtype) ["*"]
            grammar.AddProductionRule("type", new string[]
            {
                grammar.AddAltRule(new string[]
                {
                    "INT", "CHAR", "VOID", "structtype"
                }),
                grammar.AddOptionalRule("ASTERISK")
            });

            // structtype ::= "struct" IDENT
            grammar.AddProductionRule("structtype", new string[]
            {
                "STRUCT", "IDENTIFIER"
            });

            // params     ::= [ type IDENT ("," type IDENT)* ]
            grammar.AddProductionRule("params", new string[]
            {
                grammar.AddOptionalRule(grammar.AddConcatRule(new string[]
                {
                    "type", "IDENTIFIER",
                    grammar.AddStarRule(grammar.AddConcatRule(new string[]
                    {
                        "COMMA", "type", "IDENTIFIER"
                    }))
                }))
            });

            // stmt       ::= block
            grammar.AddProductionRule("stmt", new string[] { "block" });

            //              | "while" "(" exp ")" stmt              # while loop
            grammar.AddProductionRule("stmt", new string[]
            {
                "WHILE", "LPAR", "exp", "RPAR", "stmt"
            });
            //              | "if" "(" exp ")" stmt ["else" stmt]   # if then else
            grammar.AddProductionRule("stmt", new string[]
            {
                "IF", "LPAR", "exp", "RPAR", "stmt",
                grammar.AddOptionalRule(
                    grammar.AddConcatRule(new string[] { "ELSE", "stmt" }))
            });
            //              | "return" [exp] ";"                    # return
            grammar.AddProductionRule("stmt", new string[]
            {
                "RETURN",
                grammar.AddOptionalRule("exp"),
                "SC"
            });
            //              | exp "=" exp ";"                      # assignment
            grammar.AddProductionRule("stmt", new string[]
            {
                "exp", "ASSIGN", "exp", "SC"
            });
            //              | exp ";"                               # expression statement, e.g. a function call
            grammar.AddProductionRule("stmt", new string[]
            {
                "exp", "SC"
            });

            // block      ::= "{" (vardecl)* (stmt)* "}"
            grammar.AddProductionRule("block", new string[]
            {
                "LBRA",
                grammar.AddStarRule("vardecl"),
                grammar.AddStarRule("stmt"),
                "RBRA"
            });

            // exp        ::= "(" exp ")"
            grammar.AddProductionRule("exp", new string[]
            {
                "LPAR", "exp", "RPAR"
            });
            //              | ["-"] (IDENT | INT_LITERAL)
            grammar.AddProductionRule("exp", new string[]
            {
                grammar.AddOptionalRule("MINUS"),
                grammar.AddAltRule(new string[] { "IDENTIFIER", "INT_LITERAL" })
            });
            //              | CHAR_LITERAL
            grammar.AddProductionRule("exp", new string[] { "CHAR_LITERAL" });
            //              | STRING_LITERAL
            grammar.AddProductionRule("exp", new string[] { "STRING_LITERAL" });
            //              | exp (">" | "<" | ">=" | "<=" | "!=" | "==" | "+" | "-" | "/" | "*" | "%" | "||" | "&&") exp  # binary operators
            grammar.AddProductionRule("exp", new string[]
            {
                "exp",
                grammar.AddAltRule(new string[]
                {
                    "GT", "LT", "GE", "LE", "NE", "EQ", "PLUS", "MINUS", "DIV",
                    "ASTERISK", "REM", "OR", "AND"
                }),
                "exp"
            });
            //              | arrayaccess | fieldaccess | valueat | IDENT | funcall | sizeof | typecast
            grammar.AddProductionRule("exp", new string[]
            {
                grammar.AddAltRule(new string[]
                {
                    "arrayaccess", "fieldaccess", "valueat", "IDENTIFIER",
                    "funcall", "sizeof", "typecast"
                })
            });

            // funcall      ::= IDENT "(" [ exp ("," exp)* ] ")"
            grammar.AddProductionRule("funcall", new string[]
            {
                "IDENTIFIER", "LPAR",
                grammar.AddOptionalRule(grammar.AddConcatRule(new string[]
                {
                    "exp",
                    grammar.AddStarRule(grammar.AddConcatRule(new string[]
                    {
                        "COMMA", "exp"
                    }))
                })),
                "RPAR"
            });
            // arrayaccess  ::= exp "[" exp "]"                  # array access
            grammar.AddProductionRule("arrayaccess", new string[]
            {
                "exp", "LSBR", "exp", "RSBR"
            });
            // fieldaccess  ::= exp "." IDENT                    # structure field member access
            grammar.AddProductionRule("fieldaccess", new string[]
            {
                "exp", "DOT", "IDENTIFIER"
            });
            // valueat      ::= "*" exp                          # Value at operator (pointer indirection)
            grammar.AddProductionRule("valueat", new string[]
            {
                "ASTERISK", "exp"
            });
            // sizeof       ::= "sizeof" "(" type ")"            # size of type
            grammar.AddProductionRule("sizeof", new string[]
            {
                "SIZEOF", "LPAR", "type", "RPAR"
            });
            // typecast     ::= "(" type ")" exp                 # type casting
            grammar.AddProductionRule("typecast", new string[]
            {
                "LPAR", "type", "RPAR", "exp"
            });
            grammar.StartSymbol = "program";

            lexer = new Lexer(unknownSymbol, grammar);
            lexer.DefineTokenPrecedence("INT", 1);
            lexer.DefineTokenPrecedence("VOID", 1);
            lexer.DefineTokenPrecedence("CHAR", 1);
            lexer.DefineTokenPrecedence("IF", 1);
            lexer.DefineTokenPrecedence("ELSE", 1);
            lexer.DefineTokenPrecedence("WHILE", 1);
            lexer.DefineTokenPrecedence("RETURN", 1);
            lexer.DefineTokenPrecedence("STRUCT", 1);
            lexer.DefineTokenPrecedence("SIZEOF", 1);
        }
    }
}
