using System;
using System.ComponentModel;
using System.Windows;

namespace CYGLXTStudent.Model.Vo
{
    /// <summary>
    /// 会员卡扩展实体类
    /// </summary>
    public class VipVo:S_VIP, INotifyPropertyChanged
    {
        /// <summary>
        /// 会员类型   
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 餐饮折扣
        /// </summary>
        public Nullable<decimal> diningDiscount { get; set; }
        /// <summary>
        /// 生鲜折扣
        /// </summary>
        public Nullable<decimal> freshDiscount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        #region 按钮显示与隐藏（修改、充值、修改密码、退卡）
        /// <summary>
        /// 字段：按钮：显示与隐藏（修改、充值、修改密码、退卡）
        /// </summary>
        private Visibility isVisibility;
        /// <summary>
        /// 属性：按钮：显示与隐藏（修改、充值、修改密码、退卡）
        /// </summary>
        public string IsVisibility
        {
            get
            {
                return isVisibility.ToString();
            }
            set
            {
                try
                {
                    if (value!=null)
                    {
                        if (value.ToString() == "使用中")
                        {
                            //显示
                            isVisibility = Visibility.Visible;
                        }
                        else if (value.ToString() == "已退卡")
                        {
                            //隐藏
                            isVisibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        //显示
                        isVisibility = Visibility.Visible;
                    }
                }
                catch (Exception)
                {
                    //显示
                    isVisibility = Visibility.Visible;
                }
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsVisibility"));//对UpdateVisibility进行监听
                }
            }
        }

        #endregion

         #region 充值

        /// <summary>
        /// 字段：充值金额
        /// </summary>
        private string txtMoney;
        /// <summary>
        /// 属性：充值金额
        /// </summary>
        public string TxtMoney
        {
            get
            {
                return txtMoney;
            }
            set
            {
                txtMoney = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtMoney"));//对txtMoney进行监听
                }
            }
        }
        /// <summary>
        /// 字段：充值后可用余额
        /// </summary>
        private string txtAfterMoney;
        /// <summary>
        /// 属性：充值后可用余额
        /// </summary>
        public string TxtAfterMoney
        {
            get
            {
                return txtAfterMoney;
            }
            set
            {
                txtAfterMoney = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtAfterMoney"));//对txtAfterMoney进行监听
                }
            }
        }
        /// <summary>
        /// 字段：需支付金额
        /// </summary>
        private string txtCopy;
        /// <summary>
        /// 属性：需支付金额
        /// </summary>
        public string TxtCopy
        {
            get
            {
                return txtCopy;
            }
            set
            {
                txtCopy = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtCopy"));//对txtCopy进行监听
                }
            }
        }

        #endregion

        #region 修改密码           

        /// <summary>
        /// 字段：旧密码
        /// </summary>
        private string txtOldPassword;
        /// <summary>
        /// 属性：旧密码
        /// </summary>
        public string TxtOldPassword
        {
            get
            {
                return txtOldPassword;
            }
            set
            {
                txtOldPassword = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtOldPassword"));//对txtAfterMoney进行监听
                }
            }
        }
        /// <summary>
        /// 字段：新密码
        /// </summary>
        private string txtNewPassword;
        /// <summary>
        /// 属性：新密码
        /// </summary>
        public string TxtNewPassword
        {
            get
            {
                return txtNewPassword;
            }
            set
            {
                txtNewPassword = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtNewPassword"));//对txtAfterMoney进行监听
                }
            }
        }
        /// <summary>
        /// 字段：确认密码
        /// </summary>
        private string txtConfirmPassword;
        /// <summary>
        /// 属性：确认密码
        /// </summary>
        public string TxtConfirmPassword
        {
            get
            {
                return txtConfirmPassword;
            }
            set
            {
                txtConfirmPassword = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TxtConfirmPassword"));//对txtAfterMoney进行监听
                }
            }
        }

        #endregion

        #region 消费记录
        /// <summary>
        /// 消费时间
        /// </summary>
        public Nullable<System.DateTime> consumptionTiming { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public Nullable<decimal> money { get; set; }
        /// <summary>
        /// 资金来由
        /// </summary>
        public string consumptionReason { get; set; }
        /// <summary>
        /// 消费类型
        /// </summary>
        public string consumptionType { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public Nullable<decimal> integral { get; set; }
        
        #endregion

    }
}
