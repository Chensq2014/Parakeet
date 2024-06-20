using Grpc.Core;

#if NETSTANDARD2_1

using Grpc.Net.Client;

#endif

using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Parakeet.Net.ROClient
{
    public class GrpcOption
    {
        private static GrpcOption single;
        private static readonly object FLAG = new object();

        internal string AppId { get; private set; }
        internal string AppKey { get; private set; }
        internal string AppSecret { get; private set; }
        public string Host { get; private set; }

        private GrpcOption()
        {
#if NETSTANDARD2_0
            Host = "reverse.spdyun.cn:80";
#else
            Host = "https://reverse.spdyun.cn";//https://localhost:5003
#endif
        }

        public static GrpcOption Instance
        {
            get
            {
                //判断是否实例化过
                if (single == null)
                {
                    //进入lock
                    lock (FLAG)
                    {
                        //判断是否实例化过
                        if (single == null)
                        {
                            single = new GrpcOption();
                        }
                    }
                }

                return single;
            }
        }

        public void Initialize(string appId, string appKey, string appSecret, string host = null)
        {
            this.AppId = appId;
            this.AppKey = appKey;
            this.AppSecret = appSecret;

            if (!string.IsNullOrWhiteSpace(host))
            {
                this.Host = host;
            }
            else
            {
#if DEBUG
                Host = "https://localhost:5003";
#endif
            }
        }

        /// <summary>
        /// 构建含有 Authorization token的 Channel
        /// </summary>
        /// <returns></returns>
        public ChannelBase Channel
        {
            get
            {
#if NETSTANDARD2_0
                return new Channel(Host, ChannelCredentials.Insecure, new List<ChannelOption>() { new ChannelOption("grpc.enable_http_proxy", 0) });
#else
                if (Host.StartsWith("https:"))
                {
                    var channelCredentials = ChannelCredentials.Create(new SslCredentials(), CallCredentials);
                    return GrpcChannel.ForAddress(Host, new GrpcChannelOptions
                    {
                        Credentials = channelCredentials
                    });
                }
                else
                {
                    return new Channel(Host, ChannelCredentials.Insecure, new List<ChannelOption>() { new ChannelOption("grpc.enable_http_proxy", 0) });
                }
#endif
            }
        }

        /// <summary>
        /// 获取Credentials
        /// </summary>
        /// <returns></returns>
        internal CallCredentials CallCredentials
        {
            get
            {
                return CallCredentials.FromInterceptor((context, metadata) =>
                {
                    var unixTimeStamp = DateTime.Now.ToUnixTimeTicks(10);
                    var token = GenerateToken(unixTimeStamp);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        metadata.Add("AppId", $"{AppId}");
                        metadata.Add("AppKey", $"{AppKey}");
                        metadata.Add("AppToken", $"{token}");
                        metadata.Add("TimeStamp", $"{unixTimeStamp}");
                    }
                    return Task.CompletedTask;
                });
            }
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        private string GenerateToken(long unixTimeStamp)
        {
            var plainText = $"a={AppId}&s={AppSecret}&t={unixTimeStamp}&v=9";

            using var mac = new HMACSHA256(Encoding.UTF8.GetBytes(AppKey));
            var hash = mac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            var pText = Encoding.UTF8.GetBytes(plainText);
            var all = new byte[hash.Length + pText.Length];
            Array.Copy(hash, 0, all, 0, hash.Length);
            Array.Copy(pText, 0, all, hash.Length, pText.Length);
            using var md5 = MD5.Create();
            return Convert.ToBase64String(md5.ComputeHash(all));
        }

        public CallOptions CallOptions
        {
            get
            {
#if NETSTANDARD2_0
                return CreateNewCallOptions();
#else
                if (Host.StartsWith("https:"))
                {
                    return new CallOptions();
                }

                return CreateNewCallOptions();
#endif
            }
        }

        private CallOptions CreateNewCallOptions()
        {
            var unixTimeStamp = DateTime.Now.ToUnixTimeTicks(10);
            var token = GenerateToken(unixTimeStamp);

            var metadata = new Metadata
            {
                { "AppId", $"{AppId}" },
                { "AppKey", $"{AppKey}" },
                { "AppToken", $"{token}" },
                { "TimeStamp", $"{unixTimeStamp}" }
            };

            return new CallOptions(headers: metadata);
        }
    }
}