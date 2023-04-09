namespace Parakeet.Net.Converter.Server
{
    /// <summary>
    /// 二进制转换器
    /// </summary>
    public class HEXServer : BaseServer
    {
        public override char[] CharArray => SystemConstant.HEXCharArray.ToCharArray();
        public override int BitType => SystemConstant.HEXType;

        protected override int CharToInt(char charVal)
        {
            var bitValue = 0;
            switch (charVal)
            {
                case 'A':
                case 'a':
                    bitValue = 10;
                    break;
                case 'B':
                case 'b':
                    bitValue = 11;
                    break;
                case 'C':
                case 'c':
                    bitValue = 12;
                    break;
                case 'D':
                case 'd':
                    bitValue = 13;
                    break;
                case 'E':
                case 'e':
                    bitValue = 14;
                    break;
                case 'F':
                case 'f':
                    bitValue = 15;
                    break;
                default:
                    bitValue = int.Parse(charVal.ToString());
                    break;
            }

            return bitValue;
        }

        protected override char IntToChar(int charVal)
        {
            char quotientStr;
            switch (charVal)
            {
                case 10:
                    quotientStr = 'A';
                    break;
                case 11:
                    quotientStr = 'B';
                    break;
                case 12:
                    quotientStr = 'C';
                    break;
                case 13:
                    quotientStr = 'D';
                    break;
                case 14:
                    quotientStr = 'E';
                    break;
                case 15:
                    quotientStr = 'F';
                    break;
                default:
                    quotientStr = char.Parse(charVal.ToString());
                    break;
            }
            return quotientStr;
        }
    }
}
