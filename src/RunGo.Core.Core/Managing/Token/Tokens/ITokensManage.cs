using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGo.Core.Managing
{
    public interface ITokensManage
    {
        /// <summary>
        /// 创建token并返回token值
        /// </summary>
        /// <param name="tokens">令牌</param>
        /// <returns></returns>
        string CreateToken(Tokens tokens);
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        Tokens GetToken(string token);
        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        bool RefToken(Tokens tokens);
        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        bool DelToken(string token);
    }
}
