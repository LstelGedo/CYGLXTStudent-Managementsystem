using System;

namespace CYGLXTStudent.Model.Vo
{
    /// <summary>
    /// 账户扩展实体
    /// </summary>
    public  class AccountVo: S_Account
    {
        /// <summary>
        /// 账号明细ID
        /// </summary>
        public int? AmountOfDetailID { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public int? SerialNumber { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Money { get; set; }
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? Time { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 资金状况
        /// </summary>
        public string CapitalPosition { get; set; }
    }
}
