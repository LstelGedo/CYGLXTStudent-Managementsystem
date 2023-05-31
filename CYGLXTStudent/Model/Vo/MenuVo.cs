using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYGLXTStudent.Model.Vo
{
    /// <summary>
    /// 菜单扩展实体类：继承菜单实体、接口
    ///  如何：实现属性更改通知：https://docs.microsoft.com/zh-cn/dotnet/desktop/wpf/data/how-to-implement-property-change-notification?view=netframeworkdesktop-4.8
    /// </summary>
    public class MenuVo : S_Menu, INotifyPropertyChanged
    {
        #region 实现属性更改通知
        //声明事件
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 私有字段：每个菜单总数量    
        /// </summary>
        private int? amount;
        /// <summary>
        /// 属性：每个菜单总数量    
        /// </summary>
        public int? Amount
        {
            get
            {
                return amount;
            }
            set
            {
                amount = value;
                //每当属性更新时，调用OnPropertyChanged
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
            }
        }
        /// <summary>
        /// 私有字段：每个菜单总金额= 价格 * 每个菜单总数量    
        /// </summary>
        public decimal? menuPrice;
        /// <summary>
        /// 属性：每个菜单总金额= 价格 * 每个菜单总数量    
        /// </summary>
        public decimal? MenuPrice
        {
            get
            {
                return menuPrice;
            }
            set
            {
                menuPrice = value;
                //每当属性更新时，调用OnPropertyChanged
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MenuPrice"));
            }
        }
        #endregion
        /// <summary>
        /// 描述
        /// </summary>

        public string Describe { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// 订单明细ID
        /// </summary>
        public int OrderdetailsID { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int? Number { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Money { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }
}
