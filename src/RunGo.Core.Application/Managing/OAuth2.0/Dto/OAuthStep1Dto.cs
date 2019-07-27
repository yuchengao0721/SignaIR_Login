using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGo.Core.Managing
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthStep1Dto
    {
        /// <summary>
        /// 账户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string passWord { get; set; }
        /// <summary>
        /// 固定为code
        /// </summary>
        public string response_type { get; set; }
        /// <summary>
        /// 授权码
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 回调url
        /// </summary>
        public string redirect_uri { get; set; }
        /// <summary>
        /// 状态，传什么返回什么
        /// </summary>
        public string state { get; set; }
    }
}
