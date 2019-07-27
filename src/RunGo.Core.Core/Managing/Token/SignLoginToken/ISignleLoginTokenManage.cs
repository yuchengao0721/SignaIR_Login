using System;
using System.Collections.Generic;
using System.Text;

namespace RunGo.Core.Managing.Token
{
    /// <summary>
    /// <para>?Ŀ?????RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>?Ŀ?? ??</para>
    /// <para>??? ????ISignleLoginTokenManage</para>
    /// <para>??? ? ??</para>
    /// <para>???????余承浩</para>
    /// <para>???ռ???RunGo.Core.Managing.Token.SignLoginToken</para>
    /// <para>????????余承浩 </para>
    /// <para>CLR ?汾 ??4.0.30319.42000</para>
    /// <para>?    ? ??Administrator</para>
    /// <para>????ʱ????2019/7/19 11:25:08</para>
    /// <para>???ʱ????2019/7/19 11:25:08</para>
    /// <para>???? ????v1.0.0.0</para>
    /// <para>Copyright @ Administrator 2019. All rights reserved.</para>
    /// </summary>
    public interface ISignleLoginTokenManage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        string GetTokenByAccountId(string AccountId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        bool DelToken(string AccountId);
        /// <summary>
        /// 判断是否有用户在等待(true：有人等待；false：无人等待)
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        bool CheckWaitLoginState(string AccountId);
        /// <summary>
        /// 验证当前 账户是否已经登录（true：已登录；false：未登录）
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        bool CheckLoginState(string AccountId);
        /// <summary>
        ///  创建单点登录的记录信息
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        bool CreateToken(SignleLoginToken signleLoginToken);
        /// <summary>
        /// 创建一个30S的单点登录等待状态token(实际上是40S 预留10S的缓冲时间)
        /// </summary>
        /// <param name="signleLoginToken"></param>
        /// <returns></returns>
        bool CreateWaitToken(SignleLoginToken signleLoginToken);
        /// <summary>
        /// 获取一个30S的单点登录等待状态token(实际上是40S 预留10S的缓冲时间)
        /// </summary>
        /// <param name="AccountId"></param>
        /// <returns></returns>
        string GetWaitToken(string AccountId);
        /// <summary>
        /// 进行确认操作
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="sure"></param>
        /// <returns></returns>
        bool SureLogin(string token, bool sure);
    }
}
