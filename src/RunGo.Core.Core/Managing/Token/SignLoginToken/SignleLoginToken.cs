using System;
using System.Collections.Generic;
using System.Text;

namespace RunGo.Core.Managing.Token
{
    /// <summary>
    /// <para>项目名称 ：RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>项目描述 ：</para>
    /// <para>类 名 称 ：SignLoginToken</para>
    /// <para>类 描 述 ：验证单点登录信息</para>
    /// <para>所在的域 ：余承浩</para>
    /// <para>命名空间 ：RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>机器名称 ：余承浩 </para>
    /// <para>CLR 版本 ：4.0.30319.42000</para>
    /// <para>作    者 ：Administrator</para>
    /// <para>创建时间 ：2019/7/19 11:22:06</para>
    /// <para>更新时间 ：2019/7/19 11:22:06</para>
    /// <para>版 本 号 ：v1.0.0.0</para>
    /// <para>Copyright @ Administrator 2019. All rights reserved.</para>
    /// </summary>
    public class SignleLoginToken
    {
        public SignleLoginToken(string accountId, string token)
        {
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        /// <summary>
        /// 账户名
        /// </summary>
        public virtual string AccountId { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateDate { set; get; } = DateTime.Now;
        /// <summary>
        /// 令牌
        /// </summary>
        public virtual string Token { set; get; }
    }
}
