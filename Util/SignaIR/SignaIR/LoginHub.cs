using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilHelp;

namespace SignaIR
{
    /// <summary>
    /// <para>项目名称 ：RunGo.Core.SignaIR</para>
    /// <para>项目描述 ：</para>
    /// <para>类 名 称 ：LoginAnswerHub</para>
    /// <para>类 描 述 ：</para>
    /// <para>所在的域 ：余承浩</para>
    /// <para>命名空间 ：RunGo.Core.SignaIR</para>
    /// <para>机器名称 ：余承浩 </para>
    /// <para>CLR 版本 ：4.0.30319.42000</para>
    /// <para>作    者 ：Administrator</para>
    /// <para>创建时间 ：2019/7/22 16:19:17</para>
    /// <para>更新时间 ：2019/7/22 16:19:17</para>
    /// <para>版 本 号 ：v1.0.0.0</para>
    /// <para>Copyright @ Administrator 2019. All rights reserved.</para>
    /// </summary>
    public class LoginHub:Hub
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly string RedisKey = "SignaIRClientId";
        public static readonly string Tokens_RedisKey = "Tokens";
        public static readonly string SignaLoginToken_RedisKey = "SignaLoginToken";
        RedisHelper redisHelper;
        IHubContext<LoginHub> _context;
        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public LoginHub(IHubContext<LoginHub> context)
        {
            redisHelper = RedisHelper.GetRedisHelper();
            _context = context;
        }
        #endregion

        #region SignaIR操作

        #region 创建Token
        /// <summary>
        /// 用户登录之后构建一个长连接（用于监听登录状态）
        /// </summary>
        /// <param name="waitSignaLoginDto"></param>
        /// <returns></returns>
        public void LoginSuccessOrWait(string token)
        {
            #region token验证
            var tokenObj = RunGo.Jwt.JwtHelp.TokenDecryption(token);
            var nowDate = DateTime.Now;
            var now = RunGo.Jwt.JwtHelp.GetNow(nowDate);
            var WaitSignaIRid = Context.ConnectionId;
            if (tokenObj != null)
            {
                if (tokenObj?.iat < tokenObj?.exp && tokenObj?.exp > now)
                {
                    var tokenObjRedis = redisHelper.HashGet<dynamic>(Tokens_RedisKey, tokenObj?.jti);
                    if (tokenObjRedis != null && tokenObjRedis?.OverdueTime > DateTime.Now)
                        Clients.Client(WaitSignaIRid).SendAsync("GetMsg", new SignaLoginMsgDto(MsgType.Custom, redisHelper.HashSet(RedisKey, token, WaitSignaIRid).ToString()).ToJson());
                    else
                        Clients.Client(WaitSignaIRid).SendAsync("GetMsg", new SignaLoginMsgDto(MsgType.Failure, "Token过期").ToJson());
                }
                else
                    Clients.Client(WaitSignaIRid).SendAsync("GetMsg", new SignaLoginMsgDto(MsgType.Failure, "Token过期").ToJson());
            }
            else
                Clients.Client(WaitSignaIRid).SendAsync("GetMsg", new SignaLoginMsgDto(MsgType.Failure, "请传递合理的Token").ToJson());
            #endregion

        }
        #endregion

        #region 消息通知
        /// <summary>
        /// 通知该用户 异地登录的信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void SignaLoginMsg(string sToken,string JWTtoken,string AccountId)
        {
            try
            {
                var WaitSignaIRid = redisHelper.HashGet(RedisKey, JWTtoken);
                //Context = UserList[WaitSignaIRid];
                _context.Clients.Client(WaitSignaIRid.ToString()).SendAsync("GetMsg",new SignaLoginMsgDto(MsgType.LoginSignle, "此账号在别处登录，您有30秒的时间来选择是否允许").ToJson());
            }
            catch (Exception e)
            {

            }
            //沉睡30秒后服务端主动确认状态
            finally
            {
                Task.Run(()=> {
                    Thread.Sleep(30*1000);
                    //判断30S后等待登录的信息是否被确认
                    if (redisHelper.KeyExists(AccountId))
                    {
                        var SecondToken = redisHelper.StringGet(AccountId);
                        if (SecondToken.Equals(sToken))
                        {
                            var tokenobj = RunGo.Jwt.JwtHelp.TokenDecryption(JWTtoken);
                            ////判断是否是30秒之前签发的登录请求
                            //if (RunGo.Jwt.JwtHelp.GetNow(DateTime.Now, -30) > tokenobj.iat)
                            //判断当前登陆者Token是否被销毁
                            if (redisHelper.HashExists(Tokens_RedisKey, tokenobj?.jti))
                            {
                                //判断当前登陆者Token是否仍为第一者
                                if (redisHelper.HashGet(SignaLoginToken_RedisKey, AccountId).ToString().Equals(JWTtoken))
                                {
                                    redisHelper.KeyDelete(AccountId);
                                    redisHelper.HashDelete(Tokens_RedisKey, tokenobj?.jti);
                                    redisHelper.HashSet(SignaLoginToken_RedisKey, AccountId, SecondToken);
                                    AnswerSignaLogin(new AnswerSignaLoginDto() { firstToken = JWTtoken, secondToken = SecondToken, answer = true });
                                }
                            }
                            else
                            {
                                redisHelper.KeyDelete(AccountId);
                                redisHelper.HashSet(SignaLoginToken_RedisKey, AccountId, SecondToken);
                                redisHelper.HashDelete(Tokens_RedisKey, tokenobj?.jti);
                                AnswerSignaLogin(new AnswerSignaLoginDto() { firstToken = JWTtoken, secondToken = SecondToken, answer = true });
                            }
                        }
                    }
                });
            }
        }
        #endregion

        #region 消息应答
        /// <summary>
        /// 获取对方的应答
        /// </summary>
        /// <param name="answerSignaLoginDto"></param>
        /// <returns></returns>
        public void AnswerSignaLogin(AnswerSignaLoginDto answerSignaLoginDto)
        {
            try
            {
                var clientid = redisHelper.HashGet(RedisKey, answerSignaLoginDto.secondToken).ToString();
                var clientid2 = redisHelper.HashGet(RedisKey, answerSignaLoginDto.firstToken).ToString();
                //应答登录的登录请求
                _context.Clients.Client(clientid).SendAsync("GetMsg", new SignaLoginMsgDto(MsgType.Success, answerSignaLoginDto.answer.ToString()).ToJson());
                //响应应答者消息
                _context.Clients.Client(clientid2).SendAsync("GetMsg", new SignaLoginMsgDto(answerSignaLoginDto.answer?MsgType.LoginOut:MsgType.Custom, "您的登录应答为：" + answerSignaLoginDto.answer.ToString()).ToJson());
            }
            catch (Exception e) {

            }
        }
        #endregion

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        #endregion

        #region Redis的操作
        /// <summary>
        /// 删除登录的SignaIR链接
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> DelToken(string token) {
            return await redisHelper.HashDeleteAsync(RedisKey,token);
        }
        #endregion
    }
    #region 登录通知类
    /// <summary>
    /// 登录通知类
    /// </summary>
    public class SignaLoginMsgDto {
        public SignaLoginMsgDto(MsgType stateCode, string msg)
        {
            StateCode = stateCode;
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        /// <summary>
        /// 返回的code
        /// </summary>
        public MsgType StateCode { set; get; }
        /// <summary>
        /// 响应的信息
        /// </summary>
        public string Msg { set; get; }
    }
    #endregion
    #region 登录应答类
    /// <summary>
    /// 登录应答类
    /// </summary>
    public class AnswerSignaLoginDto
    {
        /// <summary>
        /// 第一者的token
        /// </summary>
        public string firstToken { set; get; }
        /// <summary>
        /// 第二者的token
        /// </summary>
        public string secondToken { set; get; }

        /// <summary>
        /// 应答是否允许登录
        /// </summary>
        public bool answer { set; get; }
    }
    #endregion
    #region 登录等待类
    /// <summary>
    /// 登录等待类
    /// </summary>
    public class WaitSignaLoginDto {
        /// <summary>
        /// 当前机器Id
        /// </summary>
        public string clientId { set; get; }
        /// <summary>
        /// 等待登录的token
        /// </summary>
        public string token { set; get; }
    }
    #endregion
    #region 登录消息状态码
    public enum MsgType {
        /// <summary>
        /// 普通的通知消息
        /// </summary>
        Custom = 100,
        /// <summary>
        /// 异地登录提醒消息
        /// </summary>
        LoginSignle = 417,
        /// <summary>
        /// 退出登陆推送
        /// </summary>
        LoginOut = 416,
        /// <summary>
        /// 成功接收反馈
        /// </summary>
        Success = 200,
        /// <summary>
        /// 消息处理失败或者异常
        /// </summary>
        Failure = 400,
    }
    #endregion
}
