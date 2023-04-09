using Irony.Parsing;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Parakeet.Net.FormulaGrammars
{
    /// <summary>
    /// 常用公式解析 This class can do some simple analysis on the trees produced by the parser.
    /// </summary>
    public class GeneralFormulaAnalyzer
    {
        public ParseTreeNode Root { get; private set; }

        private List<ParseTreeNode> _allNodes;

        /// <summary>
        /// Lazy cached version of all nodes
        /// </summary>
        public List<ParseTreeNode> AllNodes
        {
            get
            {
                return _allNodes ??= Root.AllNodes().ToList();
            }
        } 

        /// <summary>
        /// Provide formula analysis functions on a tree
        /// </summary>
        public GeneralFormulaAnalyzer(ParseTreeNode root)
        {
            Root = root;
        }

        /// <summary>
        /// 公式解析 Provide formula analysis functions
        /// </summary>
        public GeneralFormulaAnalyzer(string formula) : this(GeneralFormulaParser.Parse(formula))
        {}

        /// <summary>
        /// 获取所有单引用节点 Get all references that aren't part of another reference expression
        /// </summary>
        public IEnumerable<ParseTreeNode> References()
        {
            return Root.GetReferenceNodes();
        }

        public IEnumerable<string> Functions()
        {
            return AllNodes
                .Where(node => node.IsFunction())
                .Select(GeneralFormulaParser.GetFunction);
        }

        /// <summary>
        /// 常量
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Constants()
        {
            return Root.AllNodesConditional(GeneralFormulaParser.IsNumberWithSign)
                .Where(node => node.Is(GrammarNames.Constant) || node.IsNumberWithSign())
                .Select(GeneralFormulaParser.Print);
        } 

        ///<summary>
        /// 返回此公式中使用的所有常量 Return all constant numbers used in this formula
        ///</summary>
        public IEnumerable<double> Numbers()
        {
            // Excel numbers can be a double, short or signed int. double can fully represent all of these
            return Root.AllNodesConditional(GeneralFormulaParser.IsNumberWithSign)
                .Where(node => node.Is(GrammarNames.Number) || node.IsNumberWithSign())
                .Select(node => double.Parse(node.Print(), NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 返回解析树的深度、嵌套公式的数量 Return the depth of the parse tree, the number of nested Formulas
        /// </summary>
        public int Depth()
        {
            return Depth(Root);
        }

        /// <summary>
        /// 嵌套公式的深度 Depth of nested formulas
        /// </summary>
        private static int Depth(ParseTreeNode node)
        {
            // Get the maximum depth of the childnodes
            int depth = node.ChildNodes.Count == 0 ? 0 : node.ChildNodes.Max(n => Depth(n));

            // If this is a formula node, add one to the depth
            if (node.Is(GrammarNames.Formula))
            {
                depth++;
            }

            return depth;
        }

        /// <summary>
        /// 获取函数/运算符深度 Get function/operator depth
        /// </summary>
        /// <param name="operators">If not null, count only specific functions/operators</param>
        public int OperatorDepth(ISet<string> operators = null)
        {
            return OperatorDepth(Root, operators);
        }

        private int OperatorDepth(ParseTreeNode node, ISet<string> operators = null)
        {
            // 子节点最大深度 Get the maximum depth of the childnodes
            int depth = node.ChildNodes.Count == 0 ? 0 : node.ChildNodes.Max(n => OperatorDepth(n, operators));

            // 目标函数之一，则将深度增加1 If this is one of the target functions, increase depth by 1
            if(node.IsFunction()
               && (operators == null || operators.Contains(node.GetFunction())))
            {
                depth++;
            }

            return depth;
        }

        private static readonly ISet<string> _conditionalFunctions = new HashSet<string>()
                {
                    "IF",
                    "COUNTIF",
                    "COUNTIFS",
                    "SUMIF",
                    "SUMIFS",
                    "AVERAGEIF",
                    "AVERAGEIFS",
                    "IFERROR"
                };
        /// <summary>
        /// 获取公式的条件复杂性 Get the conditional complexity of the formula
        /// </summary>
        public int ConditionalComplexity()
        {
            return OperatorDepth(Root, _conditionalFunctions);
        }

        /// <summary>
        /// 获取公式中包含的所有引用 Get all references included in the formula
        /// </summary>
        public IEnumerable<ParserReference> ParserReferences()
        {
            return References().SelectMany(x => x.GetParserReferences());
        }
    }
}
