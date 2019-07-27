using Abp.Domain.Repositories;
using Abp.Domain.Services;
using SignaIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilHelp;

namespace RunGo.Core.Managing.Token
{
    /// <summary>
    /// 登录的状态保持者（可用于单点登录）
    /// </summary>
    public class TokensManage:ITokensManage
    {
        RedisHelper redisHelper;
        private readonly static string RedisKey = LoginHub.Tokens_RedisKey;

        public TokensManage()
        {
            redisHelper = RedisHelper.GetRedisHelper();
        }
        #region 登录状态Token生命周期

        #region 创建
        /// <summary>
        /// 登陆成功后创建（可用于查询登录状态，权限信息等）
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public virtual string CreateToken(Tokens tokens)
        {
            redisHelper.HashSet(RedisKey, tokens.Token, tokens);
            return tokens.Token;
        }
        #endregion

        #region 使用
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Tokens GetToken(string tokens)
        {
            var result = redisHelper.HashGet<Tokens>(RedisKey, tokens);
            return result;
        }
        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public bool RefToken(Tokens tokens)
        {
            tokens.OverdueTime = tokens.OverdueTime.AddMinutes(30);
            return redisHelper.HashSet(RedisKey, tokens.Token, tokens);
        }
        #endregion

        #region 销毁

        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool DelToken(string token)
        {
            var result = redisHelper.HashDelete(RedisKey, token);
            return result;
        }
        #endregion

        #endregion


    }
}
