using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RunGo.Jwt
{
    public static class JwtHelp
    {
        private readonly static string secret = "xiajibashezhide";
        /// <summary>
        /// 创建token
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static string CreatToken(string Token,string accountId, int Min = 30)
        {
            var now = DateTime.Now;
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            var payload = new Dictionary<string, object>()
            {
                { "iss","xxxxxxxx" },
                { "iat", (now - unixEpoch).TotalSeconds},
                { "exp",(now.AddMinutes(Min) - unixEpoch).TotalSeconds},
                { "jti" ,Token},
                { "aud", accountId}
            };
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret);
            return token;
        }
        /// <summary>
        /// Token解密
        /// </summary>
        /// <param name="JwtToken">加密后的token</param>
        /// <returns></returns>
        public static JwtEntity TokenDecryption(string JwtToken)
        {
            string json = string.Empty;
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                json = decoder.Decode(JwtToken, secret, verify: true);//token为之前生成的字符串
            }
            catch (InvalidTokenPartsException)
            {
                return null;
            }
            catch (TokenExpiredException)
            {
                return null;
            }
            catch (SignatureVerificationException)
            {
                return null;
            }
            return json == null ? default(JwtEntity) : JsonConvert.DeserializeObject<JwtEntity>(json);
        }
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static double GetNow(DateTime now)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            return (now - unixEpoch).TotalSeconds;
        }
        /// <summary>
        /// 获取当前时间一定秒数的时间戳
        /// </summary>
        /// <returns></returns>
        public static double GetNow(DateTime now,int seconds)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); // or use JwtValidator.UnixEpoch
            return (DateTime.Now.AddSeconds(seconds) - unixEpoch).TotalSeconds;
        }
        public class JwtEntity
        {
            /// <summary>
            /// jwt签发者
            /// </summary>
            public string iss { set; get; }
            /// <summary>
            /// jwt的签发时间
            /// </summary>
            public double iat { set; get; }
            /// <summary>
            /// jwt的过期时间，这个过期时间必须要大于签发时间
            /// </summary>
            public double exp { set; get; }
            /// <summary>
            /// jwt的唯一身份标识，主要用来作为一次性token,从而回避重放攻击。
            /// </summary>
            public string jti { set; get; }
            /// <summary>
            /// 定义在什么时间之前，该jwt都是不可用的.
            /// </summary>
            public DateTime? nbf { set; get; }
            /// <summary>
            /// 接收jwt的一方
            /// </summary>
            public string aud { set; get; }
        }
    }
}
