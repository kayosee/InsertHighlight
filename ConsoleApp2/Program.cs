using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sql = @"D:\Users\HHF\Documents\SQL Server Management Studio\SQLQuery7.sql";
            using (TextReader str = new StreamReader(sql))
            {
                var lexer = new InsertLexer(new AntlrInputStream(str));
                var tokenStream = new CommonTokenStream(lexer);
                var parser = new InsertParser(tokenStream);
                var tree = parser.insert();
                ParseTreeWalker.Default.Walk(new Class1(),tree);
            }
        }
    }
}

