using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Common.Extensions;
using static System.Security.Cryptography.RSACryptoServiceProvider;

namespace ConsoleApp.Extends
{
    public class RSAUtils
    {
        //private static readonly ILogger logger = Log.Logger;//LoggerFactory.getLogger(typeof(RSAUtils));

        /// <summary>
        /// 加密算法RSA
        /// </summary>
        public const string KEY_ALGORITHM = "RSA";

        public const string CIPHER_ALGORITHM = "RSA/ECB/PKCS1Padding";
        /// <summary>
        /// 签名算法
        /// </summary>
        public const string SIGNATURE_ALGORITHM = "SHA256WithRSA";

        /// <summary>
        /// 获取公钥的key
        /// </summary>
        private const string PUBLIC_KEY = "RSAPublicKey";

        /// <summary>
        /// 获取私钥的key
        /// </summary>
        private const string PRIVATE_KEY = "RSAPrivateKey";

        /// <summary>
        /// RSA最大加密明文大小
        /// </summary>
        private const int MAX_ENCRYPT_BLOCK = 117;

        /// <summary>
        /// RSA最大解密密文大小
        /// </summary>
        private const int MAX_DECRYPT_BLOCK = 128;

        ///// <summary>
        ///// <para>
        ///// 生成密钥对(公钥和私钥)
        ///// </para>
        ///// 
        ///// @return </summary>
        ///// <exception cref="Exception"> </exception>
        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        ////ORIGINAL LINE: public static java.util.Map<String, Object> genKeyPair() throws Exception
        //public static IDictionary<string, object> genKeyPair()
        //{
        //    var rsa = RSA.Create(KEY_ALGORITHM);
        //    keyPairGen.initialize(1024);
        //    KeyPair keyPair = keyPairGen.generateKeyPair();
        //    RSAPublicKey publicKey = (RSAPublicKey)keyPair.Public;
        //    RSAPrivateKey privateKey = (RSAPrivateKey)keyPair.Private;
        //    IDictionary<string, object> keyMap = new Dictionary<string, object>(2);
        //    keyMap[PUBLIC_KEY] = publicKey;
        //    keyMap[PRIVATE_KEY] = privateKey;
        //    return keyMap;
        //}

        ///// <summary>
        ///// <para>
        ///// 用私钥对信息生成数字签名
        ///// </para>
        ///// </summary>
        ///// <param name="data">       已加密数据 </param>
        ///// <param name="privateKey"> 私钥(BASE64编码)
        ///// @return </param>
        ///// <exception cref="Exception"> </exception>
        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        ////ORIGINAL LINE: public static String sign(byte[] data, String privateKey) throws Exception
        //public static string sign(sbyte[] data, string privateKey)
        //{
        //    byte[] keyBytes = Base64Util.decode(privateKey);
        //    PKCS8EncodedKeySpec pkcs8KeySpec = new PKCS8EncodedKeySpec(keyBytes);
        //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
        //    PrivateKey privateK = keyFactory.generatePrivate(pkcs8KeySpec);
        //    Signature signature = Signature.getInstance(SIGNATURE_ALGORITHM);
        //    signature.initSign(privateK);
        //    signature.update(data);
        //    return Base64Util.encode(signature.sign());
        //}

        ///// <summary>
        ///// <para>
        ///// 校验数字签名
        ///// </para>
        ///// </summary>
        ///// <param name="data">      已加密数据 </param>
        ///// <param name="publicKey"> 公钥(BASE64编码) </param>
        ///// <param name="sign">      数字签名
        ///// @return </param>
        ///// <exception cref="Exception"> </exception>
        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        ////ORIGINAL LINE: public static boolean verify(byte[] data, String publicKey, String sign) throws Exception
        //public static bool verify(sbyte[] data, string publicKey, string sign)
        //{
        //    sbyte[] keyBytes = Base64Util.decode(publicKey);
        //    X509EncodedKeySpec keySpec = new X509EncodedKeySpec(keyBytes);
        //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
        //    PublicKey publicK = keyFactory.generatePublic(keySpec);
        //    Signature signature = Signature.getInstance(SIGNATURE_ALGORITHM);
        //    signature.initVerify(publicK);
        //    signature.update(data);
        //    return signature.verify(Base64Util.decode(sign));
        //}

        ///// <summary>
        ///// <P>
        ///// 私钥解密
        ///// </p>
        ///// </summary>
        ///// <param name="encryptedData"> 已加密数据 </param>
        ///// <param name="privateKey">    私钥(BASE64编码)
        ///// @return </param>
        ///// <exception cref="Exception"> </exception>
        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        ////ORIGINAL LINE: public static byte[] decryptByPrivateKey(byte[] encryptedData, String privateKey) throws Exception
        //public static sbyte[] decryptByPrivateKey(sbyte[] encryptedData, string privateKey)
        //{
        //    sbyte[] keyBytes = Base64Util.decode(privateKey);
        //    PKCS8EncodedKeySpec pkcs8KeySpec = new PKCS8EncodedKeySpec(keyBytes);
        //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
        //    Key privateK = keyFactory.generatePrivate(pkcs8KeySpec);
        //    Cipher cipher = Cipher.getInstance(CIPHER_ALGORITHM);
        //    cipher.init(Cipher.DECRYPT_MODE, privateK);
        //    int inputLen = encryptedData.Length;
        //    MemoryStream @out = new MemoryStream();
        //    int offSet = 0;
        //    sbyte[] cache;
        //    int i = 0;
        //    // 对数据分段解密
        //    while (inputLen - offSet > 0)
        //    {
        //        if (inputLen - offSet > MAX_DECRYPT_BLOCK)
        //        {
        //            cache = cipher.doFinal(encryptedData, offSet, MAX_DECRYPT_BLOCK);
        //        }
        //        else
        //        {
        //            cache = cipher.doFinal(encryptedData, offSet, inputLen - offSet);
        //        }
        //        @out.Write(cache, 0, cache.Length);
        //        i++;
        //        offSet = i * MAX_DECRYPT_BLOCK;
        //    }
        //    sbyte[] decryptedData = @out.toByteArray();
        //    @out.Close();
        //    return decryptedData;
        //}

        ///// <summary>
        ///// <para>
        ///// 公钥解密
        ///// </para>
        ///// </summary>
        ///// <param name="encryptedData"> 已加密数据 </param>
        ///// <param name="publicKey">     公钥(BASE64编码)
        ///// @return </param>
        ///// <exception cref="Exception"> </exception>
        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        ////ORIGINAL LINE: public static byte[] decryptByPublicKey(byte[] encryptedData, String publicKey) throws Exception
        //public static sbyte[] decryptByPublicKey(sbyte[] encryptedData, string publicKey)
        //{
        //    sbyte[] keyBytes = Base64Util.decode(publicKey);
        //    X509EncodedKeySpec x509KeySpec = new X509EncodedKeySpec(keyBytes);
        //    KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
        //    Key publicK = keyFactory.generatePublic(x509KeySpec);
        //    Cipher cipher = Cipher.getInstance(CIPHER_ALGORITHM);
        //    cipher.init(Cipher.DECRYPT_MODE, publicK);
        //    int inputLen = encryptedData.Length;
        //    MemoryStream @out = new MemoryStream();
        //    int offSet = 0;
        //    sbyte[] cache;
        //    int i = 0;
        //    // 对数据分段解密
        //    while (inputLen - offSet > 0)
        //    {
        //        if (inputLen - offSet > MAX_DECRYPT_BLOCK)
        //        {
        //            cache = cipher.doFinal(encryptedData, offSet, MAX_DECRYPT_BLOCK);
        //        }
        //        else
        //        {
        //            cache = cipher.doFinal(encryptedData, offSet, inputLen - offSet);
        //        }
        //        @out.Write(cache, 0, cache.Length);
        //        i++;
        //        offSet = i * MAX_DECRYPT_BLOCK;
        //    }
        //    sbyte[] decryptedData = @out.toByteArray();
        //    @out.Close();
        //    return decryptedData;
        //}

        /// <summary>
        /// 公匙加密 加密rsa
        /// </summary>
        /// <param name="str"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string EncryptRsa(string str, string publicKey)
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportRSAPublicKey(Encoding.UTF8.GetBytes(publicKey), out int bytesRead); //公钥
            byte[] sample = rsa.Encrypt(Encoding.UTF8.GetBytes(str), false);
            string pass = Convert.ToBase64String(sample);
            return pass;
        }

        /// <summary>
        /// <para>
        /// 公钥加密
        /// </para>
        /// </summary>
        /// <param name="data">      源数据 </param>
        /// <param name="publicKey"> 公钥(BASE64编码)
        /// @return </param>
        /// <exception cref="Exception"> </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public static byte[] encryptByPublicKey(byte[] data, String publicKey) throws Exception
        public static byte[] EncryptByPublicKey(byte[] data, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);

            //RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //rsa.FromXmlString(encryptKey);
            //UnicodeEncoding ByteConverter = new UnicodeEncoding();
            //byte[] dataToEncrypt = ByteConverter.GetBytes(content);
            //byte[] resultBytes = rsa.Encrypt(dataToEncrypt, false);
            //return Convert.ToBase64String(resultBytes);

            byte[] keyBytes = Base64Util.Decode(publicKey);
            //X509EncodedKeySpec x509KeySpec = new X509EncodedKeySpec(keyBytes);
            //KeyFactory keyFactory = KeyFactory.getInstance(KEY_ALGORITHM);
            //Key publicK = keyFactory.generatePublic(x509KeySpec);
            // 对数据加密
            //Cipher cipher = Cipher.getInstance(CIPHER_ALGORITHM);
            //cipher.init(Cipher.ENCRYPT_MODE, publicK);
            //int inputLen = data.Length;
            MemoryStream memoryStream = new MemoryStream();
            //int offSet = 0;
            //sbyte[] cache;
            //int i = 0;
            //// 对数据分段加密
            //while (inputLen - offSet > 0)
            //{
            //    if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
            //    {
            //        cache = cipher.doFinal(data, offSet, MAX_ENCRYPT_BLOCK);
            //    }
            //    else
            //    {
            //        cache = cipher.doFinal(data, offSet, inputLen - offSet);
            //    }
            //    memoryStream.Write(cache, 0, cache.Length);
            //    i++;
            //    offSet = i * MAX_ENCRYPT_BLOCK;
            //}
            var encryptedData = memoryStream.ToArray();
            memoryStream.Close();
            return encryptedData;
        }


        #region Sign

        /// <summary>
        /// 私钥解密后获取签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Sign(string data, string privateKey)
        {
            var rsa = DecodeRSAPrivateKey(Convert.FromBase64String(privateKey));
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = rsa.SignData(dataBytes, "SHA1");//SHA256WithRSA
            return Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// 从文件中获取私钥 解密数据 然后再获取签名
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKeyPem"></param>
        /// <returns></returns>
        public static string RSASign(string data, string privateKeyPem)
        {
            RSACryptoServiceProvider rsaCsp = LoadCertificateFile(privateKeyPem);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA1");
            return Convert.ToBase64String(signatureBytes);
        }


        /// <summary>
        /// 从文件读取私钥
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static RSACryptoServiceProvider LoadCertificateFile(string filename)
        {
            using (var fs = File.OpenRead(filename))
            {
                byte[] data = new byte[fs.Length];
                byte[] res = null;
                fs.Read(data, 0, data.Length);
                if (data[0] != 0x30)
                {
                    res = GetPem("RSA PRIVATE KEY", data);
                }
                try
                {
                    RSACryptoServiceProvider rsa = DecodeRSAPrivateKey(res);
                    return rsa;
                }
                catch (Exception ex)
                {
                }
                return null;
            }
        }
        private static byte[] GetPem(string type, byte[] data)
        {
            string pem = Encoding.UTF8.GetString(data);
            string header = $"-----BEGIN {type}-----\\n";
            string footer = $"-----END {type}-----";
            int start = pem.IndexOf(header) + header.Length;
            int end = pem.IndexOf(footer, start);
            string base64 = pem.Substring(start, (end - start));
            return Convert.FromBase64String(base64);
        }
        
        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="privkey">私钥(二进制数组)</param>
        /// <returns></returns>
        private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            try
            {
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();    //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102) //version number
                    return null;
                bt = binr.ReadByte();
                if (bt != 0x00)
                    return null;


                //------ all private key components are Integer sequences ----
                elems = GetIntegerSize(binr);
                MODULUS = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                E = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                D = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                P = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                Q = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DP = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                DQ = binr.ReadBytes(elems);

                elems = GetIntegerSize(binr);
                IQ = binr.ReadBytes(elems);


                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                CspParameters CspParameters = new CspParameters();
                CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, CspParameters);
                RSAParameters RSAparams = new RSAParameters();
                RSAparams.Modulus = MODULUS;
                RSAparams.Exponent = E;
                RSAparams.D = D;
                RSAparams.P = P;
                RSAparams.Q = Q;
                RSAparams.DP = DP;
                RSAparams.DQ = DQ;
                RSAparams.InverseQ = IQ;
                RSA.ImportParameters(RSAparams);
                return RSA;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                binr.Close();
            }
        }

        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)        //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();    // data size in next byte
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();    // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;        // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {    //remove high order zeros in data
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);        //last ReadByte wasn't a removed zero, so back up a byte
            return count;
        }

        #endregion


        public static void TestApi()
        {
            var json = TextJsonConvert.SerializeObject(new
            {
                unitCode = "915001060891260711",
                projectId = "1270995441467150337",
                projectName = "重庆筑智建科技(重庆)科技有限公司",
                remark = "重庆筑智建科技(重庆)科技有限公司"
            });
            var publicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDPSBIzE4YExjUIuNl0Wn7sWIFTAv9/NEIC8OfB4AO2rFZnR+i+2g6LN8NLzamNd4KUCLvr/4kSpDHSPfCNTBia5kMNLqjd51SYuZH4m6pzfbnFDZvrcpGhuoUIYm6YXC5Fxh4zb074Ko32YjSk/eexbA03FFk4rv9qxR8kvG/bHQIDAQAB";

            //var privateKey = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBAM9IEjMThgTGNQi42XRafuxYgVMC/380QgLw58HgA7asVmdH6L7aDos3w0vNqY13gpQIu+v/iRKkMdI98I1MGJrmQw0uqN3nVJi5kfibqnN9ucUNm+tykaG6hQhibphcLkXGHjNvTvgqjfZiNKT957FsDTcUWTiu/2rFHyS8b9sdAgMBAAECgYAim4feWz4fAfDM2gAEey+Bym0dLPz3ggQbdZlcN0incTKj38+uPb375H2I5HeQlQmKKcMmbe223Q8saQcGsFPTC+6A5qznfvBoAPwXNQTy1mqr66JfWPa5EDdIDUrznadO6WKcwLVLDZ3x896wQsJnZd4pII0NB4WhogQHvz+OCQJBAO/5Wj4BNWfyt27VPep3hvgjblK88sjv0grxzpCtwlc3daQirUni8w8hX7mLK8vATiNV1HwCfxq3DS+BhzPgB28CQQDdH8ztzh4LDy0cCVLVmX7nER0TKTUiNsAJCJ8Ec9d0rqVxKoAIRPQ9mxwpR1jgHJM9Wy5nt7ApHPZShgmj56AzAkAuccgxb/JyJ9uwq7zMAE4zAEh94uvqT+ALFjmwbrDKSIWQOtXnEvGP4Bmyw2i6ioGU/SuexKzs8riulRchxx4pAkEAum3TQzOiVMSozZh2xCuzuHDPSJZXe88ZPQSNvR1Fq9SLG8wvQcmQ+lfJ+Gt03Q56fSJhD4To+uC2NIFZo1znzQJAD9gTOvcXrGrJqQJldOlbLOFo+AI8aqxFnY/Qvsba/QbdRlFcuWdYnAHxVv4Rt8NJNr+x4tNjclcgCgtKMaNfgg==";
            try
            {
                var base64PublicData = Base64Util.Encode(RSAUtils.EncryptByPublicKey(json.SerializeToUtf8Bytes(), publicKey));
                Console.WriteLine($"base64PublicData=" + base64PublicData);
                //var rawData = RSAUtils.DecryptByPrivateKey(Base64Util.Decode(base64PublicData), privateKey);
                //var cc = new String(rawData, "utf-8");
                //System.out.println("cc=" + cc);
                //String sign = RSAUtils.sign(rawData, privateKey);
                //boolean v = verify(rawData, publicKey, sign);
                //System.out.println("base64PublicData=" + base64PublicData);
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
        }

    }
}
