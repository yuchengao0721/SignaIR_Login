using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace RunGo.Core.Managing
{
    public class FullAuditedExpandEntity<T>:FullAuditedEntity<T>
    {
        new public virtual int IsDeleted {
            set
            {
                if (base.IsDeleted)
                    IsDeleted = 1;
                else
                    IsDeleted = 0;
            }
            get {
                return IsDeleted;
            }
        }
    }
}
