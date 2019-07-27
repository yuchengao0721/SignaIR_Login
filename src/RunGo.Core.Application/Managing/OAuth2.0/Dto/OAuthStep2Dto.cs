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
    public class OAuthStep2Dto
    {
        /// <summary>
        /// code
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 固定为authorization_code
        /// </summary>
        public string grant_type { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string client_secret { get; set; }
        /// <summary>
        /// 注册码
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// 状态，传什么返回什么
        /// </summary>
        public string state { get; set; }
    }

}
