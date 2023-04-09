using Irony.Parsing;

namespace Parakeet.Net.FormulaGrammars
{
    public class ParserReference
    {
        public ReferenceType ReferenceType { get; set; }
        public string LocationString { get; set; }
        public string Name { get; private set; }

        public ParserReference(ReferenceType referenceType, string locationString = null, string name = null)
        {
            ReferenceType = referenceType;
            LocationString = locationString;
            Name = name;
        }

        public ParserReference(ParseTreeNode node)
        {
            InitializeReference(node);
        }

        /// <summary>
        ///     Initializes the current object based on the input ParseTreeNode
        /// </summary>
        /// <remarks>
        ///     For Reference nodes (Prefix ReferenceItem), it initialize the values derived from the Prefix node and
        ///     is re-invoked for the ReferenceItem node.
        /// </remarks>
        public void InitializeReference(ParseTreeNode node)
        {
            switch (node.Type())
            {
                case GrammarNames.Reference:
                    InitializeReference(node.ChildNodes[1]);
                    break;
                case GrammarNames.NamedRange:
                    ReferenceType = ReferenceType.UserDefinedName;
                    Name = node.ChildNodes[0].Token.ValueString;
                    break;
                case GrammarNames.RefError:
                    ReferenceType = ReferenceType.RefError;
                    break;
            }

            LocationString = node.Print();
        }
    }
}
