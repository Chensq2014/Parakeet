namespace Parakeet.Net.FormulaGrammars
{
    /// <summary>
    /// 常规公式语法常量
    /// 保留这些常量，而不是方法/属性，因为这样可以在switch语句中使用它们
    /// </summary>
    public static class GrammarNames
    {
        #region Non-Terminals
        public const string Argument = "Argument";
        public const string Arguments = "Arguments";
        public const string NumberArguments = "NumberArguments";
        public const string FieldNumberArgument = "FieldNumberArgument";
        public const string FieldDateTimeArgument = "FieldDateTimeArgument";
        public const string DateYearArgument = "DateYearArgument";
        public const string DateMonthArgument = "DateMonthArgument";
        public const string DateDayArgument = "DateDayArgument";
        public const string DateValueArgument = "DateValueArgument";
        public const string DateValueTimezoneArgument = "ArrayColumns";
        public const string HourArgument = "HourArgument";
        public const string MinuteMonthArgument = "MinuteMonthArgument";
        public const string SecondArgument = "SecondArgument";
        public const string StrListArguments = "StrListArguments";
        public const string TextArrayArguments = "TextArrayArguments";
        public const string ArrayColumns = "ArrayColumns";
        public const string ArrayConstant = "ArrayConstant";
        public const string ArrayFormula = "ArrayFormula";
        //public const string ArrayRows = "ArrayRows";
        public const string Bool = "Bool";
        public const string Field = "Field";
        public const string Constant = "Constant";
        public const string FieldNumberConstant = "FieldNumberConstant";
        public const string FieldNumberTextConstant = "FieldNumberTextConstant";
        public const string ConstantArray = "ConstantArray";
        //public const string DynamicDataExchange = "DynamicDataExchange";
        public const string EmptyArgument = "EmptyArgument";
        public const string Error = "Error";
        public const string AksoFunction = "AksoFunction";
        //public const string File = "File";
        public const string Formula = "Formula";
        public const string FieldNumberFormula = "FieldNumberFormula";
        public const string FieldNumberTextFormula = "FieldNumberTextFormula";
        public const string FormulaWithEq = "FormulaWithEq";
        public const string FunctionCall = "FunctionCall";
        public const string FunctionName = "FunctionName";
        //public const string HorizontalRange = "HRange";
        //public const string MultiRangeFormula = "MultiRangeFormula";
        public const string NamedRange = "NamedRange";
        public const string Number = "Number";
        public const string Prefix = "Prefix";
        //public const string QuotedFileSheet = "QuotedFileSheet";
        //public const string Range = "Range";
        public const string Reference = "Reference";
        public const string ReferenceFunctionCall = "ReferenceFunctionCall";
        public const string RefError = "RefError";
        public const string RefFunctionName = "RefFunctionName";
        public const string ReservedName = "ReservedName";
        //public const string Sheet = "Sheet";
        //public const string StructuredReference = "StructuredReference";
        //public const string StructuredReferenceElement = "StructuredReferenceElement";
        //public const string StructuredReferenceExpression = "StructuredReferenceExpression";
        //public const string StructuredReferenceTable = "StructuredReferenceTable";
        public const string Text = "Text";
        public const string UDFName = "UDFName";
        public const string UDFunctionCall = "UDFunctionCall";
        public const string Union = "Union";
        //public const string VerticalRange = "VRange";
        #endregion

        #region Transient Non-Terminals
        public const string TransientStart = "Start";
        public const string TransientInfixOp = "InfixOp";
        //public const string TransientPostfixOp = "PostfixOp";
        //public const string TransientPrefixOp = "PrefixOp";
        public const string TransientReferenceItem = "ReferenceItem";
        #endregion

        #region Terminals
        public const string TokenBool = "BoolToken";
        public const string TokenCell = "CellToken";
        public const string TokenEmptyArgument = "EmptyArgumentToken";
        public const string TokenError = "ErrorToken";
        public const string TokenAksoRefFunction = "TokenAksoRefFunction";
        //public const string TokenExcelConditionalRefFunction = "ExcelConditionalRefFunctionToken";
        //public const string TokenFilePath = "FilePathToken";
        //public const string TokenFileName = "FileNameToken";
        //public const string TokenFileNameEnclosedInBrackets = "FileNameEnclosedInBracketsToken";
        //public const string TokenFileNameNumeric = "FileNameNumericToken";
        //public const string TokenHRange = "HRangeToken";
        public const string TokenIntersect = "INTERSECT";
        //public const string TokenMultipleSheets = "MultipleSheetsToken";
        //public const string TokenMultipleSheetsQuoted = "MultipleSheetsQuotedToken";
        public const string FieldName = "FieldName";
        //public const string TokenNamedRangeCombination = "NamedRangeCombinationToken";
        public const string TokenNumber = "NumberToken";
        public const string MonthNumberToken = "MonthNumberToken";
        public const string DayNumberToken = "DayNumberToken";
        public const string TokenRefError = "RefErrorToken";
        public const string TokenReservedName = "ReservedNameToken";
        public const string TokenSingleQuotedString = "SingleQuotedString";
        //public const string TokenSheet = "SheetNameToken";
        //public const string TokenSheetQuoted = "SheetNameQuotedToken";
        //public const string TokenSRTableName = "SRTableName";
        //public const string TokenSRKeyword = "SRKeyword";
        //public const string TokenSRColumn = "SRColumn";
        //public const string TokenSREnclosedColumn = "SREnclosedColumn";
        public const string TokenText = "TextToken";
        public const string TokenUDF = "UDFToken";
        public const string TokenUnionOperator = ",";
        //public const string TokenVRange = "VRangeToken";

        #endregion
    }
}
