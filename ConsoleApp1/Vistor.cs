using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Vistor : TSqlFragmentVisitor
    {

        private List<Insert> _inserts;
        private string _text;
        public Vistor(string text)
        {
            _text = text;
            _inserts = new List<Insert>();
        }

        public string Text { get => _text; }
        internal List<Insert> Inserts { get => _inserts; }

        public override void ExplicitVisit(InsertStatement node)
        {
            var names = new List<Element>();
            var values = new List<List<Element>>();
            var target = node.InsertSpecification.Target as NamedTableReference;
            var insert = new Insert() { TableName = target?.SchemaObject.BaseIdentifier.Value };
            foreach (var name in node.InsertSpecification.Columns)
            {
                names.Add(new Element()
                {
                    Start = name.StartOffset,
                    Lenght = name.FragmentLength,
                    Text = _text.Substring(name.StartOffset, name.FragmentLength),
                });
            }

            if (node.InsertSpecification.InsertSource is SelectInsertSource selectInsertSource)
            {
                var row = new List<Element>();
                foreach (var value in (selectInsertSource.Select as QuerySpecification).SelectElements)
                {
                    row.Add(new Element()
                    {
                        Start = value.StartOffset,
                        Lenght = value.FragmentLength,
                        Text = _text.Substring(value.StartOffset, value.FragmentLength),
                    });
                }
                values.Add(row);
            }
            else if (node.InsertSpecification.InsertSource is ValuesInsertSource valuesInsertSource)
            {
                foreach (var row in valuesInsertSource.RowValues)
                {
                    var eleRow = new List<Element>();
                    foreach (var item in row.ColumnValues)
                    {
                        eleRow.Add(new Element() { Start = item.StartOffset, Lenght = item.FragmentLength, Text = _text.Substring(item.StartOffset, item.FragmentLength), });
                    }
                    values.Add(eleRow);
                }
            }
            for (int i = 0; i < names.Count; i++)
            {
                var mapping = new Mapping
                {
                    Field = names[i],
                    Values = new List<Element>()
                };

                foreach (var value in values)
                {
                    mapping.Values.Add(value[i]);
                }
                insert.Columns.Add(mapping);
            }
            _inserts.Add(insert);

            base.ExplicitVisit(node);
        }
    }
}
