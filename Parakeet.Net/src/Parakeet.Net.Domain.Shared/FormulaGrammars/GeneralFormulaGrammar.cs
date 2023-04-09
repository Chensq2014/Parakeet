using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Parakeet.Net.FormulaGrammars
{
    /// <summary>
    /// 常规公式语法
    /// </summary>
    [Language("常规公式语法", "1.0", "Grammar for General Formulas")]
    public class GeneralFormulaGrammar : Grammar
    {
        #region 1-Terminals

        #region 操作标识符 Symbols and operators

        public Terminal at => ToTerm("@");
        public Terminal comma => ToTerm(",");
        //public Terminal colon => ToTerm(":");
        //public Terminal hash => ToTerm("#");
        //public Terminal semicolon => ToTerm(";");
        public Terminal OpenParen => ToTerm("(");
        public Terminal CloseParen => ToTerm(")");
        public Terminal CloseSquareParen => ToTerm("]");
        public Terminal OpenSquareParen => ToTerm("[");

        //public Terminal exclamationMark => ToTerm("!");
        public Terminal CloseCurlyParen => ToTerm("}");
        public Terminal OpenCurlyParen => ToTerm("{");
        public Terminal QuoteS => ToTerm("'");

        public Terminal plusop => ToTerm("+");
        public Terminal minop => ToTerm("-");
        public Terminal mulop => ToTerm("*");
        public Terminal divop => ToTerm("/");
        public Terminal concatop => ToTerm("&");
        public Terminal expop => ToTerm("^");
        public Terminal andop => ToTerm("&&");
        public Terminal orop => ToTerm("||");

        // Intersect op is a single space, which cannot be parsed normally so we need an ImpliedSymbolTerminal
        // Attention: ImpliedSymbolTerminal seems to break if you assign it a priority, and its default priority is low
        public Terminal intersectop { get; } = new ImpliedSymbolTerminal(GrammarNames.TokenIntersect);

        //public Terminal percentop => ToTerm("%");
        public Terminal mod => ToTerm("%");

        public Terminal gtop => ToTerm(">");
        public Terminal eqop => ToTerm("=");
        public Terminal ltop => ToTerm("<");
        //public Terminal neqop => ToTerm("<>");
        public Terminal neqop => ToTerm("!=");
        public Terminal gteop => ToTerm(">=");
        public Terminal lteop => ToTerm("<=");

        #endregion

        #region FieldToken 自定义类型

        private const string FieldNameRegex = @"[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*";//@"[a-zA-Z_](\w|\.)+[a-zA-Z_]\w+";//@"\w+";//

        public Terminal FieldNameToken { get; } = new RegexBasedTerminal(GrammarNames.FieldName, FieldNameRegex)
        { Priority = TerminalPriority.Name };

        #endregion

        #region 常量 Literals

        public Terminal BoolToken { get; } = new RegexBasedTerminal(GrammarNames.TokenBool, "TRUE|FALSE")
        {
            Priority = TerminalPriority.Bool
        };

        public Terminal NumberToken { get; } = new NumberLiteral(GrammarNames.TokenNumber, NumberOptions.None)
        {
            DefaultIntTypes = new[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt }
        };

        public Terminal TextToken { get; } = new StringLiteral(GrammarNames.TokenText, "\"",
            StringOptions.AllowsDoubledQuote | StringOptions.AllowsLineBreak | StringOptions.NoEscapes);

        #region 自定义函数常量Token

        public Terminal MonthNumberToken { get; } = new RegexBasedTerminal(GrammarNames.MonthNumberToken, string.Join("|", Enumerable.Range(1, 12)))
        {
            Priority = 0
        };
        public Terminal DayNumberToken { get; } = new RegexBasedTerminal(GrammarNames.DayNumberToken, string.Join("|", Enumerable.Range(1, 31)))
        {
            Priority = 0
        };

        public Terminal HourNumberToken { get; } = new RegexBasedTerminal(GrammarNames.DayNumberToken, string.Join("|", Enumerable.Range(0, 24)))
        {
            Priority = 0
        };

        public Terminal MinuteOrSecondNumberToken { get; } = new RegexBasedTerminal(GrammarNames.DayNumberToken, string.Join("|", Enumerable.Range(0, 59)))
        {
            Priority = 0
        };

        #endregion

        public Terminal SingleQuotedStringToken { get; } = new StringLiteral(GrammarNames.TokenSingleQuotedString, "'",
            StringOptions.AllowsDoubledQuote | StringOptions.AllowsLineBreak | StringOptions.NoEscapes)
        { Priority = TerminalPriority.SingleQuotedString };

        public Terminal ErrorToken { get; } = new RegexBasedTerminal(GrammarNames.TokenError, "#NULL!|#DIV/0!|#VALUE!|#NAME\\?|#NUM!|#N/A|#GETTING_DATA|#SPILL!");
        public Terminal RefErrorToken => ToTerm("#REF!", GrammarNames.TokenRefError);

        #endregion

        #region Functions
        private const string SpecialUdfChars = "¡¢£¤¥¦§¨©«¬­®¯°±²³´¶·¸¹»¼½¾¿×÷"; // Non-word characters from ISO 8859-1 that are allowed in VBA identifiers
        private const string AllUdfChars = SpecialUdfChars + @"\\.\w";
        private const string UdfPrefixRegex = @"('[^<>""/\|?*]+)";
        //private const string UdfPrefixRegex = @"('[^<>""/\|?*]+\.xla'!|_xll\.)";

        // The following regex uses the rather exotic feature Character Class Subtraction
        // https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#CharacterClassSubtraction
        private static readonly string UdfTokenRegex = $@"([{AllUdfChars}-[CcRr]]|{UdfPrefixRegex}[{AllUdfChars}]|{UdfPrefixRegex}?[{AllUdfChars}]{{2,1023}})\(";

        public Terminal UDFToken { get; } = new RegexBasedTerminal(GrammarNames.TokenUDF, UdfTokenRegex) { Priority = TerminalPriority.UDF };
        //public Terminal UDFToken { get; } = new RegexBasedTerminal(GrammarNames.TokenUDF, @"\w+") { Priority = TerminalPriority.UDF };
        public Terminal AksoRefFunctionToken { get; } = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, "(INDEX|OFFSET|INDIRECT)\\(")
        { Priority = TerminalPriority.AksoRefFunction };

        public Terminal AksoFunction { get; } = new RegexBasedTerminal(GrammarNames.AksoFunction, "(" + string.Join("|", _aksoFunctionList) + ")\\(")
        { Priority = TerminalPriority.AksoFunction };
        // Using this instead of Empty allows a more accurate tree
        public Terminal EmptyArgumentToken { get; } = new ImpliedSymbolTerminal(GrammarNames.TokenEmptyArgument);

        #region 日期

        /// <summary>
        /// Date(year, month, day)
        /// </summary>
        public Terminal DateFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Date\(")
        { Priority = TerminalPriority.AksoRefFunction };


        /// <summary>
        /// DateValue(DateTime) 或者 DateValue(DateTime, Timezone)
        /// </summary>
        public Terminal DateValueFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"DateValue\(")
        { Priority = TerminalPriority.AksoRefFunction };


        /// <summary>
        /// Day(date)
        /// </summary>
        public Terminal DayFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Day\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Days(number)
        /// </summary>
        public Terminal DaysFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Days\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Hour(dateTime)或者Hour()
        /// </summary>
        public Terminal HourFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Hour\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// TotalHours(interval)
        /// </summary>
        public Terminal TotalHoursFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"TotalHours\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Minute(DateTime) or Minute()
        /// </summary>
        public Terminal MinuteFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Minute\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Month(date), Month(DateTime)
        /// </summary>
        public Terminal MonthFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Month\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Months(number)
        /// </summary>
        public Terminal MonthsFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Months\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// NetWorkdays(start_date/datetime, number_of_days, weekend_number, holiday_schedule)
        /// </summary>
        public Terminal NetWorkdaysFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"NetWorkdays\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Now()
        /// </summary>
        public Terminal NowWorkdaysFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Now\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Second(date), Second()
        /// </summary>
        public Terminal SecondFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Second\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// StartOfDay(date, timezone)
        /// </summary>
        public Terminal StartOfDayFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"StartOfDay\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Time(hour, minute, second)
        /// </summary>
        public Terminal TimeFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Time\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Today(), Today(timezone)
        /// </summary>
        public Terminal TodayFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Today\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Weekday(date), Weekday(datetime)
        /// </summary>
        public Terminal WeekdayFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Weekday\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Year(date)或者Year(DateTime)
        /// </summary>
        public Terminal YearFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Year\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Years(number)
        /// </summary>
        public Terminal YearsFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Years\(")
        { Priority = TerminalPriority.AksoRefFunction };


        #endregion

        #region 数学函数

        /// <summary>
        /// Abs(number)
        /// </summary>
        public Terminal AbsFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Abs\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Average(number1, number2, ...)
        /// </summary>
        public Terminal AverageFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Average\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Ceiling(number)
        /// </summary>
        public Terminal CeilingFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Ceiling\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Floor(number)
        /// </summary>
        public Terminal FloorFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Floor\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Max(value1, value2, …)
        /// </summary>
        public Terminal MaxFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Max\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Min(value1, value2, …)
        /// </summary>
        public Terminal MinFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Min\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Power(number, number)
        /// </summary>
        public Terminal PowerFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Power\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Round(number, number) Round(number, number_of_digits, 'significant') OR Round(number, number_of_digits, 'significant-astm')
        /// </summary>
        public Terminal RoundFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Round\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Sqrt(number)
        /// </summary>
        public Terminal SqrtFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Sqrt\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Sum(number1, number2, ...)
        /// </summary>
        public Terminal SumFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Sum\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Value(text)
        /// </summary>
        public Terminal ValueFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Value\(")
        { Priority = TerminalPriority.AksoRefFunction };

        #endregion

        #region 逻辑函数

        /// <summary>
        /// And(expression1, expression2, …)
        /// </summary>
        public Terminal AndFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"And\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Case(expression1, value1, result1, value2, result2, else_result)
        /// </summary>
        public Terminal CaseFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Case\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// If(expression, value_if_true, value_if_false)
        /// </summary>
        public Terminal IfFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"If\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Includes(multi-value picklist, string) 或者Includes(multi-value picklist, single-value picklist)
        /// </summary>
        public Terminal IncludesFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Includes\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// IsBlank(expression)
        /// </summary>
        public Terminal IsBlankFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"IsBlank\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// IsNumber(text)
        /// </summary>
        public Terminal IsNumberFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"IsNumber\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Not(expression)
        /// </summary>
        public Terminal NotFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Not\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// (expression1) || (expression2)注意：用户可以使用 || 运算符代替 Or()
        /// </summary>
        public Terminal OrFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Or\(")
        { Priority = TerminalPriority.AksoRefFunction };


        #endregion

        #region 文本函数

        /// <summary>
        /// Concat(text1, text2, …) 注意：用户可以使用 & 代替 Concat()
        /// </summary>
        public Terminal ConcatFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Concat\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Find(find_text, within_text)
        /// </summary>
        public Terminal FindFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Find\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Left(text, position)
        /// </summary>
        public Terminal LeftFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Left\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Length(text)
        /// </summary>
        public Terminal LengthFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Length\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Lower(text)
        /// </summary>
        public Terminal LowerFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Lower\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Middle(text, start_position, end_position)
        /// </summary>
        public Terminal MiddleFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Middle\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Right(text, number)
        /// </summary>
        public Terminal RightFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Right\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Substitute(text, old_text, new_text)
        /// </summary>
        public Terminal SubstituteFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Substitute\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Text(value, 'format') 或者Text(lifecycle_state/picklist_value/number)
        /// </summary>
        public Terminal TextFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Text\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Trim(text)
        /// </summary>
        public Terminal TrimFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Trim\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// Upper(text)
        /// </summary>
        public Terminal UpperFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Upper\(")
        { Priority = TerminalPriority.AksoRefFunction };

        #endregion

        #region 其它功能

        /// <summary>
        /// Hyperlink(href, label, target, connection)
        /// </summary>
        public Terminal HyperlinkFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"Hyperlink\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// RecordByLabel() RecordByLabel (“Cholecap”) 返回Cholecap产品的对象引用
        /// </summary>
        public Terminal RecordByLabelFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"RecordByLabel\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// state__v
        /// </summary>
        public Terminal StateFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"state__v\(")
        { Priority = TerminalPriority.AksoRefFunction };

        /// <summary>
        /// UrlEncode(text)
        /// </summary>
        public Terminal UrlEncodeFunctionToken = new RegexBasedTerminal(GrammarNames.TokenAksoRefFunction, @"UrlEncode\(")
        { Priority = TerminalPriority.AksoRefFunction };


        #endregion

        #endregion

        #endregion

        #region 2-NonTerminals
        // Most non-terminals are first defined here, so they can be used anywhere in the rules
        // Otherwise you can only use non-terminals that have been defined previously

        public NonTerminal Argument { get; } = new NonTerminal(GrammarNames.Argument);
        public NonTerminal Arguments { get; } = new NonTerminal(GrammarNames.Arguments);

        #region 自定义Argument
        public NonTerminal NumberArguments { get; } = new NonTerminal(GrammarNames.NumberArguments);
        public NonTerminal FieldNumberArgument { get; } = new NonTerminal(GrammarNames.FieldNumberArgument);
        public NonTerminal FieldDateTimeArgument { get; } = new NonTerminal(GrammarNames.FieldDateTimeArgument);
        public NonTerminal DateYearArgument { get; } = new NonTerminal(GrammarNames.DateYearArgument);
        public NonTerminal DateMonthArgument { get; } = new NonTerminal(GrammarNames.DateMonthArgument);
        public NonTerminal DateDayArgument { get; } = new NonTerminal(GrammarNames.DateDayArgument);
        public NonTerminal DateValueArgument { get; } = new NonTerminal(GrammarNames.DateValueArgument);
        public NonTerminal DateValueTimezoneArgument { get; } = new NonTerminal(GrammarNames.DateValueTimezoneArgument);
        public NonTerminal HourArgument { get; } = new NonTerminal(GrammarNames.HourArgument);
        public NonTerminal MinuteMonthArgument { get; } = new NonTerminal(GrammarNames.MinuteMonthArgument);
        public NonTerminal SecondArgument { get; } = new NonTerminal(GrammarNames.SecondArgument);
        public NonTerminal StrListArguments { get; } = new NonTerminal(GrammarNames.StrListArguments);
        public NonTerminal TextArrayArguments { get; } = new NonTerminal(GrammarNames.TextArrayArguments);

        #endregion


        /// <summary>
        /// 自定义Field非终结点类型(含自定义FieldName规则)
        /// </summary>
        public NonTerminal Field { get; } = new NonTerminal(GrammarNames.Field);

        ///// <summary>
        ///// 自定义Field非终结点类型(含自定义FieldName规则)
        ///// </summary>
        //public NonTerminal NumberField { get; } = new NonTerminal(GrammarNames.Field);


        public NonTerminal Bool { get; } = new NonTerminal(GrammarNames.Bool);
        public NonTerminal Constant { get; } = new NonTerminal(GrammarNames.Constant);
        public NonTerminal FieldNumberConstant { get; } = new NonTerminal(GrammarNames.FieldNumberConstant);
        public NonTerminal FieldNumberTextConstant { get; } = new NonTerminal(GrammarNames.FieldNumberTextConstant);
        public NonTerminal EmptyArgument { get; } = new NonTerminal(GrammarNames.EmptyArgument);
        public NonTerminal Error { get; } = new NonTerminal(GrammarNames.Error);
        public NonTerminal Formula { get; } = new NonTerminal(GrammarNames.Formula);
        public NonTerminal FieldNumberFormula { get; } = new NonTerminal(GrammarNames.FieldNumberFormula);
        public NonTerminal FieldNumberTextFormula { get; } = new NonTerminal(GrammarNames.FieldNumberTextFormula);
        public NonTerminal FunctionCall { get; } = new NonTerminal(GrammarNames.FunctionCall);
        public NonTerminal FunctionName { get; } = new NonTerminal(GrammarNames.FunctionName);
        public NonTerminal InfixOp { get; } = new NonTerminal(GrammarNames.TransientInfixOp);
        public NonTerminal Number { get; } = new NonTerminal(GrammarNames.Number);
        public NonTerminal Reference { get; } = new NonTerminal(GrammarNames.Reference);
        public NonTerminal ReferenceItem { get; } = new NonTerminal(GrammarNames.TransientReferenceItem);
        public NonTerminal ReferenceFunctionCall { get; } = new NonTerminal(GrammarNames.ReferenceFunctionCall);
        public NonTerminal RefError { get; } = new NonTerminal(GrammarNames.RefError);
        public NonTerminal RefFunctionName { get; } = new NonTerminal(GrammarNames.RefFunctionName);
        public NonTerminal Start { get; } = new NonTerminal(GrammarNames.TransientStart);
        public NonTerminal Text { get; } = new NonTerminal(GrammarNames.Text);
        public NonTerminal UDFName { get; } = new NonTerminal(GrammarNames.UDFName);
        public NonTerminal UDFunctionCall { get; } = new NonTerminal(GrammarNames.UDFunctionCall);
        public NonTerminal Union { get; } = new NonTerminal(GrammarNames.Union);

        #endregion

        public GeneralFormulaGrammar() : base(false)
        {
            #region Punctuation
            MarkPunctuation(OpenParen, CloseParen);
            MarkPunctuation(OpenSquareParen, CloseSquareParen);
            MarkPunctuation(OpenCurlyParen, CloseCurlyParen);
            #endregion

            #region Rules

            #region Base rules
            Root = Start;

            Start.Rule = Formula;
            MarkTransient(Start);

            Formula.Rule =
                Constant
                //| FunctionCall
                | UDFunctionCall
                //| Field + InfixOp + Formula
                | Formula + InfixOp + Formula
                | OpenParen + Formula + CloseParen
                ;

            FieldNumberConstant.Rule =
                Number
                | Field
                ;

            FieldNumberTextConstant.Rule =
                Number
                | Field
                | Text
                ;

            Constant.Rule =
                Number
                | Text
                | Bool
                //| Error
                | Field
                ;

            Number.Rule = NumberToken;
            Text.Rule = TextToken;
            Bool.Rule = BoolToken;
            //Error.Rule = ErrorToken;
            //RefError.Rule = RefErrorToken;
            Field.Rule = FieldNameToken;

            #endregion

            #region Functions

            FunctionCall.Rule =
                  FunctionName + Arguments + CloseParen
                | Formula
                | Formula + InfixOp + Formula;

            FunctionName.Rule = AksoFunction;

            Arguments.Rule = MakeStarRule(Arguments, comma, Argument);
            NumberArguments.Rule = MakeStarRule(NumberArguments, comma, FieldNumberArgument);

            EmptyArgument.Rule = EmptyArgumentToken;
            Argument.Rule = Formula
                            | EmptyArgument
                            | Field + InfixOp + Formula;

            FieldNumberArgument.Rule = FieldNumberConstant | Formula;//field为number类型或此类型的函数
            FieldDateTimeArgument.Rule = FieldNumberTextConstant | Formula;//field为长整型或字符串类型或此类型的函数
            DateYearArgument.Rule = FieldNumberConstant | Formula;
            DateMonthArgument.Rule = MonthNumberToken | Formula;//控制再细一点 MonthNumberToken控制在12以内 Field与 Formula返回值无法控制 直接放开不控制
            DateDayArgument.Rule = DayNumberToken | Formula;//再细一点DayNumberToken控制在31之内  Field与 Formula返回值无法控制 直接放开不控制
            DateValueArgument.Rule = FieldNumberTextConstant | Formula;//有没有什么办法可以定义一个DateToken 常量？
            DateValueTimezoneArgument.Rule = TextToken;// | Field | Formula;//TextToken 为TimeZone 如何规范？

            HourArgument.Rule = HourNumberToken | Formula;
            MinuteMonthArgument.Rule = MinuteOrSecondNumberToken | Formula;
            SecondArgument.Rule = MinuteOrSecondNumberToken | Formula;

            //FieldNumberFormula.Rule =
            //    FieldNumberConstant
            //    | UDFunctionCall
            //    | OpenParen + FieldNumberFormula + CloseParen;

            //FieldNumberTextFormula.Rule =
            //    FieldNumberTextConstant
            //    | UDFunctionCall
            //    | OpenParen + FieldNumberTextFormula + CloseParen
            //    ;
            #region ArrayArguments

            StrListArguments.Rule = OpenSquareParen + TextArrayArguments + CloseSquareParen;
            TextArrayArguments.Rule = MakePlusRule(TextArrayArguments, comma, Text);

            #endregion


            //自定义函数规则 添加53种即可，可能多余53种，因为参数有些是不必填的
            UDFunctionCall.Rule =   //UDFName + Arguments + CloseParen |//
                                    //Formula//基础数据操作
                                    //Date(year, month, day)
                                    DateFunctionToken + DateYearArgument + comma + DateMonthArgument + comma + DateDayArgument + CloseParen
                                    //DateValue(DateTime)
                                    | DateValueFunctionToken + DateValueArgument + CloseParen
                                    //DateValue(DateTime, Timezone)
                                    | DateValueFunctionToken + DateValueArgument + comma + DateValueTimezoneArgument + CloseParen
                                    //DateValue(DateTime, Timezone)
                                    | DayFunctionToken + DateValueArgument + CloseParen
                                    //Days(number)
                                    | DaysFunctionToken + FieldNumberArgument + CloseParen
                                    //Hour(dateTime)或者Hour()
                                    | HourFunctionToken + (FieldDateTimeArgument | EmptyArgument) + CloseParen
                                    //TotalHours(interval)
                                    | TotalHoursFunctionToken + FieldDateTimeArgument + CloseParen
                                    //Minute(DateTime) or Minute()
                                    | MinuteFunctionToken + (FieldDateTimeArgument | EmptyArgument) + CloseParen
                                    //Month(date), Month(DateTime)
                                    | MonthFunctionToken + FieldDateTimeArgument + CloseParen
                                    //Months(number)
                                    | MonthsFunctionToken + FieldNumberArgument + CloseParen
                                    //NetWorkdays(start_date/datetime, number_of_days, weekend_number, holiday_schedule)
                                    | NetWorkdaysFunctionToken + FieldNumberArgument + comma + FieldNumberArgument + comma + FieldNumberArgument + comma + Text + CloseParen
                                    //Now()
                                    | NowWorkdaysFunctionToken + CloseParen
                                    //Second(date), Second()
                                    | SecondFunctionToken + (FieldDateTimeArgument | EmptyArgument) + CloseParen
                                    //StartOfDay(date, timezone)
                                    | StartOfDayFunctionToken + FieldDateTimeArgument + comma + DateValueTimezoneArgument + CloseParen
                                    //Time(hour, minute, second)
                                    | TimeFunctionToken + HourArgument + comma + MinuteMonthArgument + comma + SecondArgument + CloseParen
                                    //Today(), Today(timezone)
                                    | TodayFunctionToken + (DateValueTimezoneArgument | EmptyArgument) + CloseParen
                                    //Weekday(date), Weekday(datetime)
                                    | WeekdayFunctionToken + (FieldDateTimeArgument | EmptyArgument) + CloseParen
                                    //Year(date)或者Year(DateTime)
                                    | YearFunctionToken + FieldDateTimeArgument + CloseParen
                                    //Years(number)
                                    | YearsFunctionToken + FieldNumberArgument + CloseParen
                                    //Abs(number)
                                    | AbsFunctionToken + FieldNumberArgument + CloseParen
                                    //Average(number1, number2, ...)
                                    | AverageFunctionToken + NumberArguments + CloseParen
                                    //Ceiling(number)
                                    | CeilingFunctionToken + FieldNumberArgument + CloseParen
                                    //Floor(number)
                                    | FloorFunctionToken + FieldNumberArgument + CloseParen
                                    //Max(value1, value2, …)
                                    | MaxFunctionToken + NumberArguments + CloseParen
                                    //Min(value1, value2, …)
                                    | MinFunctionToken + NumberArguments + CloseParen
                                    //Power(number, number)
                                    | PowerFunctionToken + FieldNumberArgument + comma + FieldNumberArgument + CloseParen
                                    //Round(number, number) Round(number, number_of_digits, 'significant') OR Round(number, number_of_digits, 'significant-astm')
                                    | RoundFunctionToken + FieldNumberArgument + comma + FieldNumberArgument + CloseParen
                                    //Round(number, number_of_digits, 'significant-astm')
                                    | RoundFunctionToken + FieldNumberArgument + comma + FieldNumberArgument + comma + Text + CloseParen
                                    //Sqrt(number)
                                    | SqrtFunctionToken + FieldNumberArgument + CloseParen
                                    //Sum(number1, number2, ...)
                                    | SumFunctionToken + NumberArguments + CloseParen
                                    //Value(text)
                                    | ValueFunctionToken + FieldNumberArgument + CloseParen
                                    //And(expression1, expression2, …)
                                    | AndFunctionToken + Arguments + CloseParen
                                    //Case(expression1, value1, result1, value2, result2, else_result)
                                    | CaseFunctionToken + Arguments + CloseParen
                                    // If(expression, value_if_true, value_if_false)
                                    | IfFunctionToken + Arguments + CloseParen
                                    // Includes(multi-value picklist, string) 或者Includes(multi-value picklist, single-value picklist)
                                    | IncludesFunctionToken + StrListArguments + comma + Text + CloseParen
                                    | IncludesFunctionToken + StrListArguments + comma + StrListArguments + CloseParen
                                    //IsBlank(expression)
                                    | IsBlankFunctionToken + Argument + CloseParen
                                    //IsNumber(text)
                                    | IsNumberFunctionToken + Text + CloseParen
                                    //Not(expression)
                                    | NotFunctionToken + Argument + CloseParen
                                    //(expression1) || (expression2)注意：用户可以使用 || 运算符代替 Or()
                                    | OrFunctionToken + Arguments + CloseParen
                                    //Concat(text1, text2, …) 注意：用户可以使用 & 代替 Concat()
                                    | ConcatFunctionToken + Arguments + CloseParen
                                    //Find(find_text, within_text)
                                    | FindFunctionToken + Argument + comma + Argument + CloseParen
                                    //Left(text, position)
                                    | LeftFunctionToken + Argument + comma + FieldNumberArgument + CloseParen
                                    //Length(text)
                                    | LengthFunctionToken + (Text | Field | Formula) + CloseParen
                                    //Lower(text)
                                    | LowerFunctionToken + (Text | Field | Formula) + CloseParen
                                    //Middle(text, start_position, end_position)
                                    | MiddleFunctionToken + (Text | Field | Formula) + comma + FieldNumberArgument + comma + FieldNumberArgument + CloseParen
                                    //Right(text, number)
                                    | RightFunctionToken + (Text | Field | Formula) + comma + FieldNumberArgument + CloseParen
                                    //Substitute(text, old_text, new_text)
                                    | SubstituteFunctionToken + (Text | Field | Formula) + comma + (Text | Field | Formula) + comma + (Text | Field | Formula) + CloseParen
                                    //Text(value, 'format') 或者Text(lifecycle_state/picklist_value/number)
                                    | TextFunctionToken + Argument + CloseParen
                                    | TextFunctionToken + Argument + comma + (Text | Field | Formula) + CloseParen
                                    //Trim(text)
                                    | TrimFunctionToken + (Text | Field | Formula) + CloseParen
                                    //Upper(text)
                                    | UpperFunctionToken + (Text | Field | Formula) + CloseParen
                                    //Hyperlink(href, label, target, connection)
                                    | HyperlinkFunctionToken + (Text | Field | Formula) + comma + (Text | Field | Formula) + comma + (Text | Field | Formula) + comma + (Text | Field | Formula) + CloseParen
                                    //RecordByLabel() RecordByLabel (“Cholecap”) 返回Cholecap产品的对象引用
                                    | RecordByLabelFunctionToken + (Text | Field | Formula | EmptyArgument) + CloseParen
                                    //state__v
                                    | StateFunctionToken + (Text | Field | Formula | EmptyArgument) + CloseParen
                                    // UrlEncode(text)
                                    | UrlEncodeFunctionToken + (Text | Field | Formula) + CloseParen;



            UDFName.Rule = UDFToken;

            InfixOp.Rule =
                  expop
                | mulop
                | divop
                | mod
                | plusop
                | minop
                | concatop
                | andop
                | orop
                | gtop
                | eqop
                | ltop
                | neqop
                | gteop
                | lteop;
            MarkTransient(InfixOp);

            #endregion

            #region References

            //Reference.Rule =
            //    ReferenceItem
            //    | ReferenceFunctionCall
            //    | OpenParen + Reference + PreferShiftHere() + CloseParen
            //    ;

            //ReferenceFunctionCall.Rule =
            //     Reference + intersectop + Reference
            //    | OpenParen + Union + CloseParen
            //    | RefFunctionName + Arguments + CloseParen
            //    ;

            //RefFunctionName.Rule = AksoRefFunctionToken;

            //Union.Rule = MakePlusRule(Union, comma, Reference);

            //ReferenceItem.Rule =
            //    Field
            //    //| RefError
            //    | UDFunctionCall
            //    ;
            //MarkTransient(ReferenceItem);

            #endregion

            #endregion

            #region 5-操作符优先级            
            RegisterOperators(Precedence.Comparison, Associativity.Left, eqop, ltop, gtop, lteop, gteop, neqop);
            RegisterOperators(Precedence.Concatenation, Associativity.Left, concatop);
            RegisterOperators(Precedence.Addition, Associativity.Left, plusop, minop);
            RegisterOperators(Precedence.Multiplication, Associativity.Left, mulop, divop, mod);
            RegisterOperators(Precedence.Exponentiation, Associativity.Left, expop);
            RegisterOperators(Precedence.UnaryPreFix, Associativity.Left, at);
            RegisterOperators(Precedence.Union, Associativity.Left, comma);
            RegisterOperators(Precedence.Intersection, Associativity.Left, intersectop);
            RegisterOperators(Precedence.And, Associativity.Left, andop);
            RegisterOperators(Precedence.Or, Associativity.Left, orop);
            #endregion
        }



        #region Precedence and Priority constants
        // Source: https://support.office.com/en-us/article/Calculation-operators-and-precedence-48be406d-4975-4d31-b2b8-7af9e0e2878a
        // Could also be an enum, but this way you don't need int casts
        private static class Precedence
        {
            // Don't use priority 0, Irony seems to view it as no priority set
            public const int And = 10;
            public const int Comparison = 20;
            public const int Concatenation = 30;
            public const int Addition = 40;
            public const int Multiplication = 50;
            public const int Exponentiation = 60;
            public const int UnaryPostFix = 70;
            public const int UnaryPreFix = 80;
            public const int Reference = 81;
            public const int Union = 90;
            public const int Intersection = 100;
            public const int Or = 110;
        }

        // Terminal priorities, indicates to lexer which token it should pick when multiple tokens can match
        // E.g. "A1" is both a CellToken and NamedRange, pick cell token because it has a higher priority
        // E.g. "A1Blah" Is Both a CellToken + NamedRange, NamedRange and NamedRangeCombination, pick NamedRangeCombination
        private static class TerminalPriority
        {
            // Irony Low value
            //public const int Low = -1000;

            //public const int SRColumn = -900;

            public const int Name = -800;
            public const int ReservedName = -700;

            //public const int FileName = -500;

            public const int SingleQuotedString = -100;

            // Irony Normal value, default value
            //public const int Normal = 0;
            public const int Bool = 0;

            //public const int MultipleSheetsToken = 100;

            // Irony High value
            //public const int High = 1000;

            public const int FieldToken = 1000;

            //public const int NamedRangeCombination = 1100;

            public const int UDF = 1150;

            public const int AksoFunction = 1200;
            public const int AksoRefFunction = 1200;
            //public const int FileNameNumericToken = 1200;
        }
        #endregion

        private static List<string> _aksoFunctionList => GetAksoFunctionList() ?? new List<string>();
        private static List<string> GetAksoFunctionList()
        {
            #region 可以从文件中加载 便于动态扩展自定义函数

            var assembly = typeof(GeneralFormulaGrammar).GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("Parakeet.Net.FormulaGrammars.Resources.AksoBuiltinFunctionList.txt");
            using var sr = new StreamReader(resource);
            var funcs = sr.ReadToEnd().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            #endregion

            #region 固定集合

            //var funcs = new List<string>
            //{
            //    "Date",
            //    "DateValue",
            //    "Day",
            //    "Days",
            //    "Hour",
            //    "TotalHours",
            //    "Minute",
            //    "Month",
            //    "Months",
            //    "NetWorkdays",
            //    "Now",
            //    "Second",
            //    "StartOfDay",
            //    "Time",
            //    "Today",
            //    "Weekday",
            //    "Workday",
            //    "Year",
            //    "Years",
            //    "Abs",
            //    "Average",
            //    "Ceiling",
            //    "Floor",
            //    "Max",
            //    "Min",
            //    "Power",
            //    "Round",
            //    "Sqrt",
            //    "Sum",
            //    "Value",
            //    "And",
            //    "Case",
            //    "If",
            //    "Includes",
            //    "IsBlank",
            //    "IsNumber",
            //    "Not",
            //    "Or",
            //    "Concat",
            //    "Find",
            //    "Left",
            //    "Length",
            //    "Lower",
            //    "Middle",
            //    "Right",
            //    "Substitute",
            //    "Text",
            //    "Trim",
            //    "Upper",
            //    "Hyperlink",
            //    "RecordByLabel",
            //    "State",
            //    "UrlEncode",
            //};
            #endregion

            return funcs?.ToList();
        }
    }
}
