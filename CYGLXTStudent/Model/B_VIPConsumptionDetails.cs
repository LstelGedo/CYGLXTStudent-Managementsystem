//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CYGLXTStudent.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class B_VIPConsumptionDetails
    {
        public int VIPConsumptionDetailsID { get; set; }
        public Nullable<System.DateTime> consumptionTiming { get; set; }
        public Nullable<decimal> money { get; set; }
        public Nullable<decimal> integral { get; set; }
        public Nullable<int> VIPID { get; set; }
        public string consumptionReason { get; set; }
        public string consumptionType { get; set; }
        public Nullable<int> orderID { get; set; }
    }
}
