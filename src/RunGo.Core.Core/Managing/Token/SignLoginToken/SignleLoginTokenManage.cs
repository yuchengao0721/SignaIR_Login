using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UtilHelp;
using RunGo.Core.Managing;
using SignaIR;
using Microsoft.AspNetCore.SignalR;

namespace RunGo.Core.Managing.Token
{
    /// <summary>
    /// <para>项目名称 ：RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>项目描述 ：保持单点登录的token</para>
    /// <para>类 名 称 ：SignToken</para>
    /// <para>类 描 述 ：</para>
    /// <para>所在的域 ：余承浩</para>
    /// <para>命名空间 ：RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>机器名称 ：余承浩 </para>
    /// <para>CLR 版本 ：4.0.30319.42000</para>
    /// <para>作    者 ：Administrator</para>
    /// <para>创建时间 ：2019/7/19 11:23:47</para>
    /// <para>更新时间 ：2019/7/19 11:23:47</para>
    /// <para>版 本 号 ：v1.0.0.0</para>
    /// <para>Copyright @ Administrator 2019. All rights reserved.</para>
    /// </summary>
    public class SignleLoginTokenManage : ISignleLoginTokenManage
    {
        ITokensManage tokensManage = new TokensManage();
        RedisHelper redisHelper;
        LoginHub loginrHub;
        private readonly static string RedisKey = LoginHub.SignaLoginToken_RedisKey;

        public SignleLoginTokenManage(IHubContext<LoginHub> context)
        {
            redisHelper = RedisHelper.GetRedisHelper();
            loginrHub = new LoginHub(context);
        }

        #region 生命周期
        #region 创建
        /// <summary>
        /// 用户状态Token创建
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public bool CreateToken(SignleLoginToken signleLoginToken)
        {
            return redisHelper.HashSet(RedisKey, signleLoginToken.AccountId, signleLoginToken.Token);
        }

        /// <summary>
        /// 创建一个30S的单点登录等待状态token(实际上是31S 预留1S的缓冲时间)
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public bool CreateWaitToken(SignleLoginToken signleLoginToken)
        {
            if (!redisHelper.KeyExists(signleLoginToken.AccountId))
                return redisHelper.StringSet(signleLoginToken.AccountId, signleLoginToken.Token, new TimeSpan(0, 0, 33));
            else
                return false;
        }
        /// <summary>
        /// 获取一个30S的单点登录等待状态token(实际上是31S 预留1S的缓冲时间)
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public string GetWaitToken(string AccountId)
        {
            if (!redisHelper.KeyExists(AccountId))
                return redisHelper.StringGet(AccountId);
            else
                return string.Empty;
        }
        #endregion

        #region 使用
        /// <summary>
        /// 验证当前 账户是否已经登录（true：已登录；false：未登录）
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public bool CheckLoginState(string AccountId)
        {
            return redisHelper.HashExists(RedisKey, AccountId);
        }

        /// <summary>
        /// 判断是否有用户在等待(true：有人等待；false：无人等待)
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public bool CheckWaitLoginState(string AccountId)
        {
            return redisHelper.KeyExists(AccountId);
        }

        #region 操作
        /// <summary>
        /// 获取当前登录用户的token
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        public string GetTokenByAccountId(string AccountId)
        {
            return redisHelper.HashGet(RedisKey, AccountId);
        }
        /// <summary>
        /// 是否允许登录
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public bool SureLogin(string code, bool sure)
        {
            var tokens = Jwt.JwtHelp.TokenDecryption(code);
            if (redisHelper.KeyExists(tokens.aud))
            {
                //加密后的JWTtoken
                var accessToken = redisHelper.StringGet(tokens.aud);
                //应答单点登陆请求消息
                loginrHub.AnswerSignaLogin(new AnswerSignaLoginDto() { firstToken = code, secondToken = accessToken, answer = sure });
                if (sure)
                {
                    //删除当前登录用户的登录标志
                    tokensManage.DelToken(tokens.jti);
                    CreateToken(new SignleLoginToken(tokens.aud, accessToken));
                }
                else
                {
                    var secondToken = Jwt.JwtHelp.TokenDecryption(accessToken);
                    //删除第二者的登录请求标志
                    tokensManage.DelToken(secondToken.jti);
                }
                redisHelper.KeyDelete(tokens.aud);
                return true;
            }
            else
                return false;
        } 
        #endregion

        #endregion

        #region 销毁
        /// <summary>
        /// 销毁当前登陆者token
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        public bool DelToken(string AccountId)
        {
            return redisHelper.HashDelete(RedisKey, AccountId);
        }
        #endregion 

        #endregion
        
    }
}
