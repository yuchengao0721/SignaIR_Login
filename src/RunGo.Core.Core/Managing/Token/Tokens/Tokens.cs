using Abp.Domain.Entities;
using System;

namespace RunGo.Core.Managing
{
    /// <summary>
    /// Token令牌类
    /// </summary>
    public class Tokens:Entity<string>
    {
        public Tokens(int OverdueTime)
        {
            this.CreationTime = DateTime.Now;
            Id = Guid.NewGuid().ToString("N");
            if (OverdueTime < 0)
                this.OverdueTime = this.CreationTime.AddMinutes(30);
            else
                this.OverdueTime = this.CreationTime.AddSeconds(OverdueTime);
            this.Token = Guid.NewGuid().ToString("N");
        }
        public Tokens()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 账户名
        /// </summary>
        public virtual string AccountId { set; get; }
        /// <summary>
        /// 令牌
        /// </summary>
        public virtual string Token { set; get; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public virtual DateTime OverdueTime { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreationTime { set; get; } = DateTime.Now;

    }
}
