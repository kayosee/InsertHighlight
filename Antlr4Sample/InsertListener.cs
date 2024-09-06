using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antlr4Sample
{
    internal class InsertListener : InsertBaseListener
    {
        public override void EnterInsertTableName([NotNull] InsertParser.InsertTableNameContext context)
        {
            Console.WriteLine(context.GetText());
        }
        public override void EnterInsertField([NotNull] InsertParser.InsertFieldContext context)
        {
            Console.WriteLine(context.GetText());
        }
        public override void EnterInsertValueFromConstant([NotNull] InsertParser.InsertValueFromConstantContext context)
        {
            base.EnterInsertValueFromConstant(context);
        }
        public override void EnterInsertValueFromSelect([NotNull] InsertParser.InsertValueFromSelectContext context)
        {
            var fieldsExpr = context.GetChild(0);
            int i = 0;
            while (fieldsExpr != null && i < fieldsExpr.ChildCount)
            {
                if (fieldsExpr.GetChild(i).GetType() == typeof(InsertParser.FieldsContext))
                {
                    var fields = fieldsExpr.GetChild(i);
                    for (int j = 0; j < fields.ChildCount; j++)
                    {
                        var field = fields.GetChild(j);
                        if (field.GetType() != typeof(TerminalNodeImpl))
                            Console.WriteLine(field.GetText());
                    }
                }
                i++;
            }

        }
    }
}
