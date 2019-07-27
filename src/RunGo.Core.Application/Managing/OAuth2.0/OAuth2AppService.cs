using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RunGo.Core.Managing.Token;
using SignaIR;
using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UtilHelp;

namespace RunGo.Core.Managing
{
    //[EnableCors("CorsOAuth")]
    public class OAuth2AppService: IApplicationService
    {
        private readonly ITokensManage _tokensManage = new TokensManage();
        private readonly ISignleLoginTokenManage _signleLoginTokenManage;
        private IHubContext<LoginHub> _context;
        LoginHub loginrHub;
        public OAuth2AppService(IHubContext<LoginHub> context)
        {
            _context = context;
            loginrHub = new LoginHub(_context);
            _signleLoginTokenManage = new SignleLoginTokenManage(_context);
        }

        #region 单点登录简易登录测试方案
        /// <summary>
        /// 这是一个单点登录的测试接口
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [DontWrapResult]
        public Task<dynamic> LoginTest(string account, string password)
        {
            dynamic result = new ExpandoObject();

            #region 验证过程
            var access_token = Jwt.JwtHelp.CreatToken(_tokensManage.CreateToken(new Tokens(-1) { AccountId = account }), account);
            result.access_token = access_token;
            #region 单点登录逻辑
            //已经有人登录
            if (_signleLoginTokenManage.CheckLoginState(account))
            //if (false)
            {
                //没人在等(那就加入等待队列)
                if (!_signleLoginTokenManage.CheckWaitLoginState(account))
                {
                    result.LoginWaitState = true;
                    _signleLoginTokenManage.CreateWaitToken(new SignleLoginToken(account, access_token));
                    //通知当前登录用户确认消息
                    try
                    {
                        loginrHub.SignaLoginMsg(access_token, _signleLoginTokenManage.GetTokenByAccountId(account), account);
                    }
                    catch (Exception e)
                    {

                    }
                }
                //有人在等（直接Out）
                else
                {
                    result.LoginWaitState = false;
                    result.Msg = "当前账号正在排队使用中，请稍后再试";
                }
            }
            else
            {
                result.LoginWaitState = false;
                _signleLoginTokenManage.CreateToken(new SignleLoginToken(account, access_token));
            }

            #endregion
            return Task.FromResult<dynamic>(result);
            #endregion
        }
        /// <summary>
        /// 单点登录测试接口（应答）2
        /// </summary>
        /// <param name="code"></param>
        /// <param name="sureLogin"></param>
        /// <returns></returns>
        [HttpPost]
        [DontWrapResult]
        public Task<dynamic> AnswerLoginRequestTest(string code, bool sureLogin)
        {
            dynamic result = new ExpandoObject();

            #region 验证过程
            result.SureDo = _signleLoginTokenManage.SureLogin(code, sureLogin);
            return Task.FromResult<dynamic>(result);
            #endregion

        }

        /// <summary>
        /// 提供给第三方进行token验证（并返回当前操作用户ID）
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost]
        [DontWrapResult]
        public Task<CheckResultDto> CheckTokenTest(string Token)
        {
            var checkResult = new CheckResultDto();
            var nowDate = DateTime.Now;
            var now = Jwt.JwtHelp.GetNow(nowDate);
            #region 身份验证
            var token = Jwt.JwtHelp.TokenDecryption(Token);
            if (token != null)
            {
                if (token.iat > token.exp || token.exp < now)
                {
                    checkResult.StatusCode = CheckState.Failure;
                    DelToken(token?.aud, token?.jti);
                    return Task.FromResult(checkResult);
                }
                //和redis里的token进行验证
                var tokens = _tokensManage.GetToken(token.jti);
                if (tokens == null)
                {
                    checkResult.StatusCode = CheckState.Failure;
                    return Task.FromResult(checkResult);
                }
                else
                {
                    if (tokens.OverdueTime < nowDate)
                    {
                        checkResult.StatusCode = CheckState.Failure;
                        DelToken(token?.aud, token?.jti);
                        return Task.FromResult(checkResult);
                    }
                    else
                    {
                        if (_signleLoginTokenManage.GetTokenByAccountId(tokens.AccountId).Equals(Token))
                        {
                            checkResult.accountId = tokens.AccountId;
                            checkResult.StatusCode = CheckState.Success;
                            _tokensManage.RefToken(tokens);
                        }
                        else
                        {
                            checkResult.StatusCode = CheckState.Failure;
                            return Task.FromResult(checkResult);
                        }
                    }
                }
            }
            else
                checkResult.StatusCode = CheckState.Failure;
            #endregion

            return Task.FromResult(checkResult);
        }
        #endregion

        #region 拓展方法
        /// <summary>
        /// 用户退出，统一销毁Tokens（登录状态以及基本信息值）、SignleTokens（单点登录状态和保持Token）、WaitSignaIRClientId（推送状态保持Token）
        /// </summary>
        /// <param name="accountId">当前账户ID</param>
        /// <param name="token">当前使用Token</param>
        private void DelToken(string accountId,string token) {
            try
            {
                _tokensManage.DelToken(token);
                loginrHub.DelToken(token);

            }
            catch (Exception e)
            { }
        }
        #endregion
    }
}
