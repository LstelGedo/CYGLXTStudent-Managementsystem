using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Views.Opens;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Transactions;

namespace CYGLXTStudent.ViewModel.Opens
{
    /// <summary>
    /// 结账窗口（加退菜/结账/会员结账）
    /// </summary>
    public class DiningOptionsViewModel: ViewModelBase
    {
        public DiningOptionsViewModel()
        {
            // 关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, wd => wd != null);
            // 页面加载
            LoadedCommand = new RelayCommand(SelectOrderDetails);
            // 加菜按钮
            MouseDownJCCommand = new RelayCommand<Window>(MouseDownJC);
            // 普通结账
            ButtonClickCommand = new RelayCommand<Window>(SettleAccounts, wd => wd != null);
            // 会员结账
            ButtonClickHYCommand = new RelayCommand<Window>(MemberSettle, wd => wd != null);

        }
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        #region 1、【属性】
        /// <summary>
        /// 私有字段：订单ID
        /// </summary>
        private int orderID;
        /// <summary>
        /// 属性：订单ID（窗口传值）
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
        /// 私有字段：账单金额
        /// </summary>
        private string money;
        /// <summary>
        /// 属性：账单金额
        /// </summary>
        public string Money
        {
            get { return money; }
            set
            {
                if (money != value)
                {
                    money = value;
                    RaisePropertyChanged(() => Money);

                }
            }
        }
        /// <summary>
        /// 私有字段：顾客人数
        /// </summary>
        private string amount;
        /// <summary>
        /// 属性：顾客人数
        /// </summary>
        public string Amount
        {
            get { return amount; }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    RaisePropertyChanged(() => Amount);

                }
            }
        }
        /// <summary>
        /// 私有字段：下单时间
        /// </summary>
        private string orderTime;
        /// <summary>
        /// 属性：下单时间
        /// </summary>
        public string OrderTime
        {
            get { return orderTime; }
            set
            {
                if (orderTime != value)
                {
                    orderTime = value;
                    RaisePropertyChanged(() => OrderTime);

                }
            }
        }
        /// <summary>
        /// 私有字段：菜品总数
        /// </summary>
        private string quantity;
        /// <summary>
        /// 属性：菜品总数
        /// </summary>
        public string Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    RaisePropertyChanged(() => Quantity);

                }
            }
        }
        /// <summary>
        /// 私有字段：订单明细信息（绑定表格）
        /// </summary>
        private List<OrderVo> orderDetails;
        /// <summary>
        /// 属性：订单明细信息（绑定表格）
        /// </summary>
        public List<OrderVo> OrderDetails
        {
            get { return orderDetails; }
            set
            {
                if (orderDetails != value)
                {
                    orderDetails = value;
                    RaisePropertyChanged(() => OrderDetails);

                }
            }
        }

        #endregion
        #region 2、【命令】
        /// <summary>
        /// 2.1 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        /// <summary>
        /// 2.2 页面加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 2.3 普通结账命令
        /// </summary>
        public RelayCommand<Window> ButtonClickCommand { get; set; }
        /// <summary>
        /// 2.4 会员结账命令
        /// </summary>
        public RelayCommand<Window> ButtonClickHYCommand { get; set; }
        /// <summary>
        /// 2.5 加菜按钮命令
        /// </summary>
        public RelayCommand<Window> MouseDownJCCommand { get; set; }
        #endregion
        #region 3、【方法】
        /// <summary>
        /// 3.1 窗口数据回填
        /// </summary>
        private void SelectOrderDetails()
        {
            try
            {
                //(1)连表查询订单明细信息 by 订单ID
                var listOrderDetails = (from tbOrderDetails in myModels.B_Orderdetails
                                        join tbOrder in myModels.B_Order on tbOrderDetails.orderID equals tbOrder.orderID
                                        join tbMenu in myModels.S_Menu on tbOrderDetails.menuID equals tbMenu.menuID
                                        where tbOrderDetails.orderID == OrderID//订单ID
                                        select new OrderVo
                                        {
                                            Dishes = tbMenu.dishes,//菜名
                                            MenuID = tbMenu.menuID,//菜单ID
                                            Quantity = tbOrderDetails.quantity,//菜单数量
                                            number = tbOrder.number,//就餐人数
                                            orderTime = tbOrder.orderTime,//下单时间
                                            money = tbOrder.money,// 金额
                                            remark = tbOrder.remark,//备注
                                            amount = tbOrder.amount,//菜单总数
                                            Price = tbMenu.price,//售价
                                        }).ToList();
                //(2)判断列表数据是否有数据
                if (listOrderDetails.Count >= 1)
                {
                    //(2)属性赋值
                    Money = Math.Round(Convert.ToDecimal(listOrderDetails[0].money), 2).ToString();//账单金额
                    Amount = listOrderDetails[0].number.ToString().Trim();//顾客人数
                    OrderTime = listOrderDetails[0].orderTime.ToString().Trim();//下单时间
                    Quantity = listOrderDetails[0].amount.ToString().Trim();//菜品总数
                    OrderDetails = listOrderDetails;//订单明细信息
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.1 关闭窗口
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.2 加退菜（打开窗口）
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void MouseDownJC(Window window)
        {
            /*
            * 思路：
            * 1、判断订单ID大于0
            * 2、实例化加载层 JiaZai，弹出加载层
            * 3、实例化加菜窗口 Order_JC
            * 4、设置窗口数据上下文 OrderJCViewModel
            * 5、传递参数OrderID
            * 6、打开窗口
            * 7、隐藏加载层
            * 8、关闭当前窗口
            */
            try
            {

                // 1、判断订单ID大于0
                if (OrderID > 0)
                {
                    JiaZai jiaZai = new JiaZai();
                    jiaZai.Show();
                    Order_JC jC = new Order_JC();
                    OrderJCViewModel viewModel = jC.DataContext as OrderJCViewModel;
                    viewModel.OrderID = OrderID;
                    jC.ShowDialog();
                    jiaZai.Hide();
                    window.Close();
                }
                   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 普通结账
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void SettleAccounts(Window window)
        {
            /*
            1、显示消息框：提示用户是否结账，再次确认；
            2、多表操作，开启事务
            3、操作如下：
            （1）、修改订单信息:B_Order
            （2）、修改餐桌（修改状态）：S_DiningTable
            （3）、修改账号表（修改总金额，和可用余额）：S_Account
            （4）、添加账户明细表（新增一条进账记录）：S_AmountOfDetail
            （5）、修改就餐记录（已付款）：B_EatInRecord
            （6）、保存更改，提交事务，提示，关闭窗口
             */
            try
            {
                //1、显示消息框：提示用户是否结账，再次确认；
                if (MessageBox.Show("是否要结账？", "系统提示：", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                {
                    //2、多表操作，开启事务
                    using (TransactionScope scope=new TransactionScope ())
                    {
                        //3、操作如下：
                        //（1）、修改订单信息: B_Order                      
                        B_Order pW_Order = (from tbOrder in myModels.B_Order
                                            where tbOrder.orderID == OrderID
                                            select tbOrder).Single();
                        pW_Order.cutOffTime = DateTime.Now;//修改结账时间
                        myModels.Entry(pW_Order).State = System.Data.Entity.EntityState.Modified;

                        //（2）、修改餐桌（修改状态）：S_DiningTable
                        //（2）修改餐桌（订单ID清空、修改状态）
                        S_DiningTable sYS_DiningTable = (from tbDining in myModels.S_DiningTable
                                                         where tbDining.orderID == OrderID
                                                         select tbDining).Single();
                        sYS_DiningTable.orderID = null;
                        sYS_DiningTable.status = "已付款";
                        sYS_DiningTable.totalMoney = 0;
                        myModels.Entry(sYS_DiningTable).State = System.Data.Entity.EntityState.Modified;

                        //（3）、修改账号表（修改总金额，和可用余额）：S_Account
                        S_Account dbAccount = (from tbaccount in myModels.S_Account
                                               select tbaccount).Single();
                        decimal? decTotalMoney = dbAccount.TotalMoney;//账户总金额
                        decimal? decAvail = dbAccount.Avail;//账户可用金额 
                        dbAccount.Avail = decAvail + Convert.ToDecimal(Money);//账户可用金额=账户可用金额+结账金额
                        dbAccount.TotalMoney = decTotalMoney + Convert.ToDecimal(Money);//账户总金额=账户总金额+结账金额
                        myModels.Entry(dbAccount).State = System.Data.Entity.EntityState.Modified;

                        //（4）、添加账户明细表（新增一条进账记录）：S_AmountOfDetail
                        //（4）添加账户明细表（新增一条进账记录）
                        S_AmountOfDetail amountOfDetail = new S_AmountOfDetail
                        {
                            money = Convert.ToDecimal(Money),
                            reason = "餐饮消费",
                            time = DateTime.Now,
                            remark = "餐饮收入" + Convert.ToDecimal(Money) + "元," + "该金额由餐饮收入",
                            capitalPosition = "收入"
                        };
                        myModels.S_AmountOfDetail.Add(amountOfDetail);
                        //（5）、修改就餐记录（已付款）：B_EatInRecord
                        B_EatInRecord eatInRecord = (from tbeatInRecord in myModels.B_EatInRecord
                                                     where tbeatInRecord.orderID == OrderID
                                                     select tbeatInRecord).Single();
                        eatInRecord.status = "已付款";
                        eatInRecord.endTime = DateTime.Now;//当前时间
                        myModels.Entry(eatInRecord).State = System.Data.Entity.EntityState.Modified;
                        //（6）、保存更改，提交事务，提示，关闭窗口
                        //保存更改
                        if (myModels.SaveChanges() > 0)
                        {
                            //提交事务
                            scope.Complete();
                            //提示
                            MessageBox.Show("结算成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            //关闭窗口
                            window.Close();
                        }
                        else
                        {
                            MessageBox.Show("结算失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
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
        /// 3.4 打开会员结账窗口
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void MemberSettle(Window window)
        {
            /* 
             * 1、判断订单ID大于0
             * 2、实例化会员结账窗口Members_Checkout
             * 3、设置窗口数据上下文MembersCheckoutViewModel
             * 4、传递参数OrderID
             * 5、打开窗口
             * 6、关闭当前窗口
             */
            try
            {
                // 1、判断订单ID大于0
                if (OrderID>0)
                {
                    // 2、实例化会员结账窗口Members_Checkout
                    Members_Checkout checkout = new Members_Checkout();
                    // 3、设置窗口数据上下文MembersCheckoutViewModel
                    MembersCheckoutViewModel viewModel = checkout.DataContext as MembersCheckoutViewModel;
                    // 4、传递参数OrderID
                    viewModel.OrderID = OrderID;
                    // 5、打开窗口
                    checkout.ShowDialog();
                    // 6、关闭当前窗口
                    window.Close();
                }
                else
                {
                    MessageBox.Show("无法识别结账订单，请关闭当前窗口，重新选择订单", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
