using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;

namespace CYGLXTStudent.ViewModel.Opens
{
    /// <summary>
    /// 会员结账
    /// </summary>
    public class MembersCheckoutViewModel: ViewModelBase
    {
        public MembersCheckoutViewModel()
        {
            //关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, obj => obj != null);
            //回填金额
            LoadedCommand = new RelayCommand(SelectOrder);
            //回填会员信息
            TxtPhoneKeyUpCommand = new RelayCommand(SelectMember);
            //会员支付
            MembersPayCommand= new RelayCommand<Window>(MembersPay, obj => obj != null);
        }
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        #region 1、【属性】
        /// <summary>
        /// 字段：订单ID（主页面传递）
        /// </summary>
        private int orderID;
        /// <summary>
        /// 属性：订单ID（主页面传递）
        /// </summary>
        public int OrderID
        {
            get { return orderID; }
            set
            {
                if (orderID != value)
                {
                    orderID = value;
                    RaisePropertyChanged(() => OrderID);
                }
            }
        }
        /// <summary>
        /// 字段：会员支付扩展实体：（记录会员信息）
        /// </summary>
        private MembersPayVo membersPayVo = new MembersPayVo();
        /// <summary>
        /// 属性：会员支付扩展实体：（记录会员信息）
        /// </summary>
        public MembersPayVo CurrentMembersPayVo
        {
            get { return membersPayVo; }
            set
            {
                if (membersPayVo != value)
                {
                    membersPayVo = value;
                    RaisePropertyChanged(() => CurrentMembersPayVo);
                }
            }
        }
        #endregion
        #region 2、【命令】
        /// <summary>
        /// 2.1 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 2.2 通过手机号查询信息命令
        /// </summary>
        public RelayCommand TxtPhoneKeyUpCommand { get; set; }
        /// <summary>
        ///  2.3 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        /// <summary>
        ///  2.4 确认支付命令
        /// </summary>
        public RelayCommand<Window> MembersPayCommand { get; set; }
        #endregion
        #region 3【方法】
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.2 回填金额
        /// </summary>
        private void SelectOrder()
        {
            var list = myModels.B_Order.Find(OrderID);
            CurrentMembersPayVo.BillAmount = Math.Round(Convert.ToDecimal(list.money), 2).ToString("0.00");
                
        }
        /// <summary>
        /// 3.3 根据输入手机号，查询会员基本信息
        /// </summary>
        private void SelectMember()
        {
            try
            {
                /*
                 * 思路：
                 * 1、判断获取账单金额是否为空，是，提示。
                 * 2、获取电话号码验证；
                 * 3、查询会员信息 by 手机号 && 状态=="使用中"（数据回填）
                 * （1）获取可用余额，
                 * （2）获取可用积分，
                 * 一、积分兑换金额 >= 账单金额
                 * （1）需要支付金额
                 * （2）本次可得积分
                 * （3）折扣后需支付
                 * （4）本次需支付
                 * 二、积分兑换金额 < 账单金额
                 * （1）获取折扣：当前折扣= 餐饮折扣 * 0.1
                 * （2）需要支付金额= 账单金额 - 积分可兑金额
                 * （3）需要支付金额= 账单金额 - 积分可兑金额
                 * （4）判断：折扣后需支付=需要支付金额 * 折扣 > 可用余额
                 * （5）折扣后需支付 = 需要支付金额 * 折扣
                 * （6）本次支付后可得积分= （需要支付金额 * 折扣）*0.02
                 * （7）本次需支付=（需要支付金额 * 折扣）
                 */

                //1、获取账单金额               
                if (string.IsNullOrEmpty(CurrentMembersPayVo.BillAmount.Trim()))
                {
                    MessageBox.Show("账单金额为空，不能操作！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //2、获取电话号码验证
                string Phone = CurrentMembersPayVo.MobilePhone.Trim();
                //判断号码长度（11位）
                if (Phone.Length == 11)
                {
                    //正则验证号码
                    if (!Regex.IsMatch(Phone, @"^0?(13[0-9]|14[5-9]|15[012356789]|166|17[0-8]|18[0-9]|19[589])[0-9]{8}$"))
                    {
                        //恢复初始值
                        CurrentMembersPayVo.MobilePhone = "";
                        MessageBox.Show("请输入正确的号码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    else
                    {
                        //3、查询会员信息 by 手机号 && 状态 == "使用中"（数据回填）
                        var listVIPVo = (from tbVip in myModels.S_VIP
                                         join tbMember in myModels.S_Member on tbVip.memberID equals tbMember.memberID
                                         where tbVip.phone == Phone && tbVip.state == "使用中"//状态
                                         select new VipVo
                                         {
                                             availableBalance = tbVip.availableBalance,//可用余额
                                             Integral = tbVip.Integral,//可用积分
                                             diningDiscount = tbMember.diningDiscount,//餐饮折扣
                                         }).ToList();
                        //会员列表总数大于0
                        if (listVIPVo.Count > 0)
                        {
                            //（1）获取可用余额，
                            decimal decAvailableBalance = Convert.ToDecimal(listVIPVo[0].availableBalance);
                            decAvailableBalance = Math.Round(decAvailableBalance, 2);//可用余额:将小数值舍入到指定数量的小数位，并将中点值舍入到最接近的偶数。


                            //（2）获取可用积分，
                            decimal decIntegral = Convert.ToDecimal(listVIPVo[0].Integral);
                            //（4）获取积分兑换金额 = 可用积分
                            decimal decIntegralAmount = decIntegral;
                            double douIntegralAmount = Convert.ToDouble(decIntegralAmount);

                            CurrentMembersPayVo.AvailableBalance = decAvailableBalance.ToString(); //可用余额
                            CurrentMembersPayVo.Integral = decIntegral.ToString(); //可用积分                           
                            CurrentMembersPayVo.IntegralAmount = decIntegralAmount.ToString(); //积分兑换金额 = 可用积分

                            //一、积分兑换金额 >= 账单金额
                            if (douIntegralAmount >= Convert.ToDouble(CurrentMembersPayVo.BillAmount))
                            {

                                CurrentMembersPayVo.MoneyNeedPay = "0";//需要支付金额
                                CurrentMembersPayVo.TheIntegral = "0";//本次可得积分
                                CurrentMembersPayVo.DiscountPay = "0";//折扣后需支付
                                CurrentMembersPayVo.CopyDiscountPay = "本次需支付" + 0 + "元";//本次需支付
                            }
                            //二、积分兑换金额 < 账单金额
                            else
                            {
                                //当前折扣= 餐饮折扣 * 0.1
                                decimal decCurrentDiscount = Convert.ToDecimal(listVIPVo[0].diningDiscount) * Convert.ToDecimal(0.1);
                                //需要支付金额= 账单金额 - 积分可兑金额
                                decimal decPaymentAmount = Convert.ToDecimal(CurrentMembersPayVo.BillAmount) - decIntegralAmount;
                                //需要支付金额= 账单金额 - 积分可兑金额
                                CurrentMembersPayVo.MoneyNeedPay = decPaymentAmount.ToString();

                                //折扣后需支付=需要支付金额 * 折扣 > 可用余额
                                if (decPaymentAmount * decCurrentDiscount > decAvailableBalance)
                                {
                                    MessageBox.Show("该会员可用余额不足，请及时充值！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                                //折扣后需支付 = 需要支付金额 * 折扣
                                CurrentMembersPayVo.DiscountPay = (decPaymentAmount * decCurrentDiscount).ToString("0.00");
                                //本次支付后可得积分= （需要支付金额 * 折扣）*0.02
                                CurrentMembersPayVo.TheIntegral = (decPaymentAmount * decCurrentDiscount * Convert.ToDecimal(0.02)).ToString("0.00");
                                //本次需支付=（需要支付金额 * 折扣）
                                CurrentMembersPayVo.CopyDiscountPay = "本次需支付" + (decPaymentAmount * decCurrentDiscount).ToString("0.00") + "元";
                            }
                        }
                        else
                        {
                            MessageBox.Show("找不到此号码对应的会员，请检查号码是否有误！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.4 会员支付
        /// </summary>
        private void MembersPay(Window window)
        {
            /*
             * 思路：
             * 1、判断“电话”与“密码”不能为空；
             * 2、多表操作开启事务；
             * 3、获取页面数据；
             * 4、判断：可用余额 > 折扣后需支付
             *   （1）查询会员扩展实体VIPVo信息 by“电话”
             *   （2）判断：输入密码和会员密码是否匹配
             *   （3）查询会员卡S_VIP信息 by “电话”
             *     两种情况：
             *     一、积分兑换金额 >= 账单金额（消耗积分） 
             *     （1）修改会员表：S_VIP（只是消耗积分，积分减少）
             *     （2）新增会员消费记录：B_VIPConsumptionDetails（积分支付）  
             *     
             *     二、积分兑换金额 < 账单金额（积分减少、可用余额减少）
             *     （1）修改会员表：S_VIP（积分减少、可用余额减少）
             *     （2）新增会员消费记录：B_VIPConsumptionDetails（积分支付 + 会员扣费）
             *     （3）修改账号表：S_Account (可用资金增加，冻结资金解除)
                         * 思路：
                         * 前面：VIP充值的时候，金额为冻结金额；
                         * 这里： VIP结账：把冻结金额较少，可用金额增加                         
            *     （4）新增账号明细表记录：S_AmountOfDetail 
             * 5、修改订单表：B_Order（结单时间） 
             * 6、修改餐桌表：S_DiningTable（订单ID为空，状态为“已付款”,消费金额清零）             * 
             * 7、修改餐桌消费记录表：B_EatInRecord ( 结账状态=“已付款”、 结束时间)
             * 8、保存更改，提交事务，提示，关闭窗口
             */


            try
            {
                //1、判断“电话”与“密码”不能为空；
                //判断电话
                if (string.IsNullOrEmpty(CurrentMembersPayVo.MobilePhone.Trim()))
                {
                    MessageBox.Show("请输入会员卡号码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //判断密码
                if (string.IsNullOrEmpty(CurrentMembersPayVo.Password.Trim()))
                {
                    MessageBox.Show("请输入会员密码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //2、多表操作开启事务；
                using (TransactionScope scope = new TransactionScope())
                {
                    //3、获取页面数据；          
                    string strPhone = CurrentMembersPayVo.MobilePhone.Trim();//会员电话
                    string strPassword = CurrentMembersPayVo.Password.Trim();//密码
                    decimal decAvailableBalance = Convert.ToDecimal(CurrentMembersPayVo.AvailableBalance.Trim());//可用余额
                    decimal decIntegral = Convert.ToDecimal(CurrentMembersPayVo.Integral.Trim());//可用积分
                    decimal decIntegralAmount = Convert.ToDecimal(CurrentMembersPayVo.IntegralAmount.Trim());//积分可兑金额
                    decimal decBillAmount = Convert.ToDecimal(CurrentMembersPayVo.BillAmount.Trim());//账单金额
                    decimal decTheIntegral = Convert.ToDecimal(CurrentMembersPayVo.TheIntegral.Trim());//本次可得积分
                    decimal decDiscountPay = Convert.ToDecimal(CurrentMembersPayVo.DiscountPay.Trim());//折扣后需支付
                    //判断：手机号长度为11 && 密码不为空
                    if (CurrentMembersPayVo.MobilePhone.Length == 11 && strPassword != string.Empty)
                    {
                        //4、可用余额 > 折扣后需支付
                        if (decAvailableBalance > decDiscountPay)
                        {
                            //（1）查询会员扩展实体VIPVo信息 by“电话”
                            var dbVipVo = (from tbVip in myModels.S_VIP
                                           join tbMember in myModels.S_Member on tbVip.memberID equals tbMember.memberID
                                           where tbVip.phone == strPhone
                                           select new VipVo
                                           {
                                               availableBalance = tbVip.availableBalance,//可用余额
                                               Integral = tbVip.Integral,//积分
                                               diningDiscount = tbMember.diningDiscount,//餐饮折扣
                                               password = tbVip.password,//密码
                                               VIPID = tbVip.VIPID,//会员卡ID
                                           }).Single();
                            //（2）判断：输入密码和会员密码是否匹配
                            if (dbVipVo.password.Trim() == strPassword)
                            {
                                //（3）查询会员卡S_VIP信息 by “电话”
                                S_VIP dbVIP = (from tbVip in myModels.S_VIP
                                               where tbVip.phone == strPhone
                                               select tbVip).SingleOrDefault();

                                //一、积分兑换金额 >= 账单金额
                                if (decIntegralAmount >= decBillAmount)
                                {
                                    //（1）修改会员表：S_VIP（只是消耗积分，积分减少）
                                    dbVIP.Integral = decIntegralAmount - decBillAmount;//积分=积分可兑金额-账单金额
                                    myModels.Entry(dbVIP).State = System.Data.Entity.EntityState.Modified;

                                    //（2）新增会员消费记录：B_VIPConsumptionDetails（积分支付）
                                    B_VIPConsumptionDetails dbVIPConsumptionDetails = new B_VIPConsumptionDetails
                                    {
                                        consumptionTiming = DateTime.Now,//消费时间
                                        money = 0,//金额
                                        integral = decBillAmount,//积分
                                        VIPID = dbVipVo.VIPID,//会员卡ID
                                        consumptionReason = "会员消费积分" + decBillAmount,//资金来由
                                        consumptionType = "消费",//消费类型
                                        orderID = OrderID//订单ID
                                    };
                                    myModels.B_VIPConsumptionDetails.Add(dbVIPConsumptionDetails);
                                }
                                //二、积分兑换金额 < 账单金额
                                else
                                {
                                    //（1）修改会员表：S_VIP（积分减少、可用余额减少）
                                    dbVIP.availableBalance -= decDiscountPay;  //可用余额=可用余额-折扣后需支付金额                                 
                                    dbVIP.Integral = decTheIntegral;  //积分= 本次可得积分
                                    myModels.Entry(dbVIP).State = System.Data.Entity.EntityState.Modified;

                                    //（2）新增会员消费记录：B_VIPConsumptionDetails（积分支付 + 会员扣费）
                                    B_VIPConsumptionDetails dbVIPConsumptionDetails = new B_VIPConsumptionDetails
                                    {
                                        consumptionTiming = DateTime.Now,//消费时间
                                        money = decDiscountPay,//金额
                                        integral = decIntegralAmount,//积分
                                        VIPID = dbVipVo.VIPID,//会员卡ID
                                        consumptionReason = "会员消费积分" + decIntegralAmount + "，会员消费" + decDiscountPay,//资金来由
                                        consumptionType = "消费",//消费类型
                                        orderID = OrderID//订单ID
                                    };
                                    myModels.B_VIPConsumptionDetails.Add(dbVIPConsumptionDetails);

                                    //（3）修改账号表：S_Account(账号冻结金额减少，账号可用金额增加，账号总金额不变)
                                    S_Account dbAccount = (from tbAccount in myModels.S_Account
                                                           select tbAccount).Single();
                                    dbAccount.AccountFrozen -= decDiscountPay;//账号冻结金额 = 账号冻结金额 - 账单支付金额（折扣后需支付）
                                    dbAccount.Avail += decDiscountPay;//账号可用金额=账号可用金额+账单支付金额（折扣后需支付）
                                    myModels.Entry(dbAccount).State = System.Data.Entity.EntityState.Modified;

                                    //（4）新增账号明细表记录：S_AmountOfDetail 
                                    S_AmountOfDetail amountOfDetail = new S_AmountOfDetail
                                    {
                                        money = decDiscountPay,
                                        reason = "会员消费",
                                        time = DateTime.Now,
                                        remark = "会员消费" + decDiscountPay
                                        + "元，可用资金增加" + decDiscountPay + "元，冻结资金解除" + decDiscountPay + "元",
                                        capitalPosition = "收入"
                                    };
                                    myModels.S_AmountOfDetail.Add(amountOfDetail);
                                }
                                //5、修改订单表：B_Order（结单时间）
                                B_Order dbOrder = (from tbOrder in myModels.B_Order
                                                   where tbOrder.orderID == OrderID
                                                   select tbOrder).Single();
                                dbOrder.cutOffTime = DateTime.Now;//结单时间
                                myModels.Entry(dbOrder).State = System.Data.Entity.EntityState.Modified;


                                //6、修改餐桌表：S_DiningTable（订单ID为空，状态为“已付款”,消费金额清零）
                                S_DiningTable dbDiningTable = (from tbDiningTable in myModels.S_DiningTable
                                                               where tbDiningTable.orderID == OrderID
                                                               select tbDiningTable).Single();
                                dbDiningTable.orderID = null;//订单ID
                                dbDiningTable.status = "已付款";//餐桌状态
                                dbDiningTable.totalMoney = 0;//消费金额
                                myModels.Entry(dbDiningTable).State = System.Data.Entity.EntityState.Modified;

                                //7、修改餐桌消费记录表：B_EatInRecord(结账状态 =“已付款”、 结束时间)
                                B_EatInRecord dbEatInRecord = myModels.B_EatInRecord.Where(o => o.orderID == OrderID).Single();
                                dbEatInRecord.status = "已付款";
                                dbEatInRecord.endTime = DateTime.Now;
                                myModels.Entry(dbEatInRecord).State = System.Data.Entity.EntityState.Modified;

                                //8、保存更改，提交事务，提示，关闭窗口
                                if (myModels.SaveChanges() > 0)
                                {
                                    scope.Complete();//提交事务
                                    MessageBox.Show("会员消费成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                    window.Close();//关闭窗口                                 
                                }
                                else
                                {
                                    MessageBox.Show("会员消费失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                                }

                            }
                            else
                            {
                                MessageBox.Show("请输入正确的密码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                        else
                        {
                            MessageBox.Show("该会员可用余额不足，请及时充值！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请输入正确账号密码！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        #endregion

    }
}
