using System.ComponentModel;

namespace CYGLXTStudent.Model.Vo
{
    public class MembersPayVo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 字段：手机号
        /// </summary>
        private string mobilePhone;
        /// <summary>
        /// 属性：手机号
        /// </summary>
        public string MobilePhone
        {
            get
            {
                return mobilePhone;
            }
            set
            {
                mobilePhone = value;
                //对MobilePhone进行监听
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MobilePhone"));
            }
        }
        /// <summary>
        /// 字段：密码
        /// </summary>
        private string password;
        /// <summary>
        /// 属性：密码
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                //对Password进行监听
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
            }
        }

        /// <summary>
        /// 字段：可用余额
        /// </summary>
        private string availableBalance;
        /// <summary>
        /// 属性：可用余额
        /// </summary>
        public string AvailableBalance
        {
            get
            {
                return availableBalance;
            }
            set
            {
                availableBalance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AvailableBalance"));
            }
        }

        /// <summary>
        /// 字段：可用积分
        /// </summary>
        private string integral;
        /// <summary>
        /// 属性：可用积分
        /// </summary>
        public string Integral
        {
            get
            {
                return integral;
            }
            set
            {
                integral = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Integral"));
            }
        }

        /// <summary>
        /// 字段：积分可兑金额
        /// </summary>
        private string integralAmount;
        /// <summary>
        /// 属性：积分可兑金额
        /// </summary>
        public string IntegralAmount
        {
            get
            {
                return integralAmount;
            }
            set
            {
                integralAmount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IntegralAmount"));
            }
        }

        /// <summary>
        /// 字段：账单金额
        /// </summary>
        private string billAmount;
        /// <summary>
        /// 属性：账单金额
        /// </summary>
        public string BillAmount
        {
            get
            {
                return billAmount;
            }
            set
            {
                billAmount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BillAmount"));
            }
        }

        /// <summary>
        /// 字段：需要支付金额
        /// </summary>
        private string moneyNeedPay;
        /// <summary>
        /// 属性：需要支付金额
        /// </summary>
        public string MoneyNeedPay
        {
            get
            {
                return moneyNeedPay;
            }
            set
            {
                moneyNeedPay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MoneyNeedPay"));
            }
        }

        /// <summary>
        /// 字段：本次可得积分
        /// </summary>
        private string theIntegral;
        /// <summary>
        /// 属性：本次可得积分
        /// </summary>
        public string TheIntegral
        {
            get
            {
                return theIntegral;
            }
            set
            {
                theIntegral = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TheIntegral"));
            }
        }

        /// <summary>
        /// 字段：折扣后需支付
        /// </summary>
        private string discountPay;
        /// <summary>
        /// 属性：折扣后需支付
        /// </summary>
        public string DiscountPay
        {
            get
            {
                return discountPay;
            }
            set
            {
                discountPay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DiscountPay"));
            }
        }

        /// <summary>
        /// 字段：本次需支付
        /// </summary>
        private string copyDiscountPay;
        /// <summary>
        /// 属性：本次需支付
        /// </summary>
        public string CopyDiscountPay
        {
            get
            {
                return copyDiscountPay;
            }
            set
            {
                copyDiscountPay = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CopyDiscountPay"));//对txtCopy进行监听
            }
        }

    }
}
