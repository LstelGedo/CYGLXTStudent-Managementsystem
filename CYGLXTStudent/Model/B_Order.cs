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
    
    public partial class B_Order
    {
        public int orderID { get; set; }
        public Nullable<int> number { get; set; }
        public Nullable<System.DateTime> orderTime { get; set; }
        public Nullable<decimal> money { get; set; }
        public string remark { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<System.DateTime> cutOffTime { get; set; }
    }
}
