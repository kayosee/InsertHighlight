using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Parse(@"D:\Users\HHF\Documents\SQL Server Management Studio\SQLQuery7.sql");
        }

        static string NiBoLan(string expression)
        {
            string result = "";
            Stack<string> oper = new Stack<string>();
            for (int i=0;i<expression.Length;i++)
            {
                if ("0123456789".IndexOf(expression[i]) >= 0)
                {
                    var num = "";
                    do
                    {
                        num += expression[i++];
                    } while (i < expression.Length && "0123456789".IndexOf(expression[i]) >= 0);
                    i--;
                    if (!string.IsNullOrEmpty(num))
                    {
                        result+=(num);
                    }
                }
                else
                {
                    if ("+-*/".IndexOf(expression[i]) >= 0)
                    {
                        if (oper.Count > 0)
                        {
                            if (Compare(oper.Peek(), expression[i].ToString()))
                            {
                                do
                                {
                                    var op = oper.Pop();
                                    result += (op);
                                } while (oper.Count > 0 && Compare(oper.Peek(), expression[i].ToString()));
                            }
                        }
                        oper.Push(expression[i].ToString());

                    }
                    else if (expression[i] == '(')
                    {
                        oper.Push(expression[i].ToString());
                    }
                    else if (expression[i] == ')')
                    {
                        do
                        {
                            var op = oper.Pop();
                            result += (op);
                        } while (oper.Peek() != "(");
                        oper.Pop();

                    }
                }
            }

            while(oper.Count > 0)
            {
                result+=(oper.Pop());
            }
            return result;
        }

        static bool Compare(string left, string right)
        {
            Dictionary<string, int> map = new Dictionary<string, int>
            {
                { "+", 1 },
                { "-", 1 },
                { "*", 2 },
                { "/", 2 },
                { "(", 0 },
                { ")", 0 },
            };

            return map[left] > map[right];
        }

        static void Parse(string input)
        {
            Microsoft.SqlServer.TransactSql.ScriptDom.TSql150Parser sql150Parser = new Microsoft.SqlServer.TransactSql.ScriptDom.TSql150Parser(true);
            var result=sql150Parser.Parse(new StreamReader(input), out var errors);
            result.Accept(new Vistor(File.ReadAllText(input)));
        }
    }
}
