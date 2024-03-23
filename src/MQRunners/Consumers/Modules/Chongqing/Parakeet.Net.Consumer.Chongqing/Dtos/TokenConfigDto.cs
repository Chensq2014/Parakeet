namespace Parakeet.Net.Consumer.Chongqing.Dtos
{
    public class TokenConfigDto
    {
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string GrantType { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public string Uri { get; set; }

        public string Url => Port == 80 || Port == 443 ? $"{Host}{Uri}" : $"{Host}:{Port}{Uri}";

        /// <summary>
        /// 获取请求Host+Port  获取token只需要host:port
        /// </summary>
        public string HostPortString => Port == 80 || Port == 443 ? $"{Host}" : $"{Host}:{Port}";
    }
}
