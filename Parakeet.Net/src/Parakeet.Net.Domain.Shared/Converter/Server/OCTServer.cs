namespace Parakeet.Net.Converter.Server
{
    public class OCTServer : BaseServer
    {
        public override char[] CharArray => SystemConstant.OCTCharArray.ToCharArray();
        public override int BitType => SystemConstant.OCTType;
    }
}
