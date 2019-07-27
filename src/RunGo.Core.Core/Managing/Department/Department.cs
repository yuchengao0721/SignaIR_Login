/*-----------------------------------------------
// Copyright (C) 2019 南京戎光软件科技有限公司  版权所有。
// 文件名称：     Department
// 功能描述：    组织机构实体
// 创建标识：    
// 修改标识：    
// 修改描述:     
-----------------------------------------------*/
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace RunGo.Core.Managing
{
    /// <summary>
	///  Department
	/// </summary>
    public partial class Department : FullAuditedEntity<string>, IMayBeHaveTenant<string>
    {
        public  Department()
        {
            Id= Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string DepartmentName { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string CompanyId { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentId { get; set; }
        
        
        /// <summary>
        /// 
        /// </summary>
        public virtual int? Sort { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public virtual string TenantId { get; set; }

        /// <summary>
        ///  类型:1 单位 2 部门 3 岗位
        /// </summary>
        public virtual int? Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Leader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ChargeLeader { get; set; }

    }
}