using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace IOTCS.EdgeGateway.Domain.Models
{
    [Table(Name = "tb_rules")]
    public class RuleModel
    {
        /// <summary>
        /// 规则ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public string RuleId { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// 处理主题
        /// </summary>
        public string Topics { get; set; }
        /// <summary>
        /// 规则Json
        /// </summary>
        [Column(DbType="longtext")]
        public string RuleJson { get; set; }
        /// <summary>
        ///规则引擎json
        /// </summary>
        [Column(DbType = "longtext")]
        public string RuleEngineJson { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }
}
