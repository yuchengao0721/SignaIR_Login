using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RunGo.Core.Managing
{
    /// <summary>
    /// 
    /// </summary>
    public class CheckResultDto
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        public string accountId { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        public CheckState StatusCode { get; set; }
    }
    /// <summary>
    /// 授权验证的状态码
    /// </summary>
    public enum CheckState {
        /// <summary>
        /// 验证成功
        /// </summary>
        Success = 200,
        /// <summary>
        /// 验证失败
        /// </summary>
        Failure = 401,
        /// <summary>
        /// 被异地登录
        /// </summary>
        NoLogin = 417
    }
    public class OAuthLoginInDto
    {
        /// <summary>
        /// 结果
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Location { get; set; }
    }
    /// <summary>
    /// 单点登录的被授权状态
    /// </summary>
    public enum WaitLoginState
    {
        /// <summary>
        /// 验证成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 验证失败
        /// </summary>
        Failure = -1,
        /// <summary>
        /// 被异地登录
        /// </summary>
        Wait = 0
    }
}
