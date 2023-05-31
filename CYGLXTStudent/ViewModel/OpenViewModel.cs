using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using CYGLXTStudent.Model.Vo;
using System.Windows;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.Views.Opens;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 开台模块
    /// </summary>
    public class OpenViewModel : ViewModelBase
    {
        public OpenViewModel()
        {
            LoadedCommand = new RelayCommand<UserControl>(LoadedDindingTable, uc => uc != null);
        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        /// <summary>
        /// 用户控件自身（临时保存用户控件）
        /// </summary>
        public UserControl UC;
        /// <summary>
        /// 环绕面板：餐桌生成父控件
        /// </summary>
        public WrapPanel wpSimers;
        /// <summary>
        /// 排号按钮
        /// </summary>
        public Button btnNumeral;
        /// <summary>
        /// 一键清理按钮
        /// </summary>
        public Button btnCleanUp;
        #endregion
        #region 【1、属性】
        /// <summary>
        /// 字段：右边（订单明细表格数据）
        /// </summary>
        private List<OrderVo> orderDetails;
        /// <summary>
        /// 属性：右边（订单明细表格数据）
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
        /// <summary>
        /// 字段：空桌总数（用于实时记录空桌数量）
        /// </summary>
        private string emptyNumber;
        /// <summary>
        /// 属性：空桌总数（用于实时记录“空桌”餐桌数量）
        /// </summary>
        public string EmptyNumber
        {
            get { return emptyNumber; }
            set
            {
                if (emptyNumber != value)
                {
                    emptyNumber = value;
                    RaisePropertyChanged(() => EmptyNumber);

                }
            }
        }
        /// <summary>
        /// 字段：待付款（用于实时记录“待付款”餐桌数量）
        /// </summary>
        private string stayNumber;
        /// <summary>
        /// 属性：待付款（用于实时记录“待付款”餐桌数量）
        /// </summary>
        public string StayNumber
        {
            get { return stayNumber; }
            set
            {
                if (stayNumber != value)
                {
                    stayNumber = value;
                    RaisePropertyChanged(() => StayNumber);

                }
            }
        }
        /// <summary>
        /// 字段：已付款总数（用于实时记录“已付款”餐桌数量）
        /// </summary>
        private string paymentNumber;
        /// <summary>
        /// 属性：已付款总数（用于实时记录“已付款”餐桌数量）
        /// </summary>
        public string PaymentNumber
        {
            get { return paymentNumber; }
            set
            {
                if (paymentNumber != value)
                {
                    paymentNumber = value;
                    RaisePropertyChanged(() => PaymentNumber);

                }
            }
        }
        /// <summary>
        /// 字段：表格上的标题
        /// </summary>
        private string titleGuide;
        /// <summary>
        /// 属性：表格上的标题
        /// </summary>
        public string TitleGuide
        {
            get { return titleGuide; }
            set
            {
                if (titleGuide != value)
                {
                    titleGuide = value;
                    RaisePropertyChanged(() => TitleGuide);

                }
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
            get { return billAmount; }
            set
            {
                if (billAmount != value)
                {
                    billAmount = value;
                    RaisePropertyChanged(() => BillAmount);

                }
            }
        }
        /// <summary>
        /// 字段：菜品总数
        /// </summary>
        private string dishesAmount;
        /// <summary>
        /// 属性：菜品总数
        /// </summary>
        public string DishesAmount
        {
            get { return dishesAmount; }
            set
            {
                if (dishesAmount != value)
                {
                    dishesAmount = value;
                    RaisePropertyChanged(() => DishesAmount);

                }
            }
        }
        /// <summary>
        /// 字段：下单时间
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
        #endregion
        #region【2、命令】
        /// <summary>
        /// 2.1 页面加载命令
        /// </summary>
        public RelayCommand<UserControl> LoadedCommand { get; private set; }
        /// <summary>
        /// 2.2 一键清理命令
        /// </summary>
        public RelayCommand BtnInsertCommand { get; private set; }

        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 加载餐桌
        /// </summary>
        /// <param name="userControl">用户控件</param>
        private void LoadedDindingTable(UserControl userControl)
        {
            /*
            * 思路：
            * 1、获取控件；
            *  （1）获取当前用户控件，调用封装的方法获取子控件；
            *  （2）子控件：操作按钮（排序、一键清理）；
            *  （3）子控件：用于嵌套生成餐桌的WrapPanel；
            *  （4）WrapPanel移除所有子元素；
            * 2、查询餐桌信息
            *  （1）for循环生成餐桌（餐桌是封装的用户控件DiningTable_UC）
            * 3、查询不同状态的餐桌列表数据           
            *  （1）查询“空桌”状态的的餐桌列表数据
            *  （2）查询“待付款”状态的的餐桌列表数据
            *  （3）查询“已付款”状态的餐桌列表数据
            * 4、根据餐桌的状态控制“一键清理”按钮和“排序”按钮的透明和不透明    
            *  （1）“空桌”状态的的餐桌列表数据>0,“排号”按钮不透明；否则，“排号”按钮透明；
            *  （2）“已付款”状态的餐桌列表数据>0,“一键清理”按钮不透明；否则，“一键清理”按钮透明；
            * 5、绑定底部统计数据
            *  （1）“空桌”总桌数=“空桌”状态的的餐桌列表数据的总数
            *  （2）“待付款”总桌数=“待付款”状态的的餐桌列表数据的总数
            *  （3）“已付款”总桌数=“已付款”状态的餐桌列表数据的总数
            */
            try
            {
                UC = userControl;
                //1、从视觉树找到目标控件的所有子控件
                //（1）获取WrapPanel集合
                List<WrapPanel> wraps = FindVisualChildren.FindVisualChildrens<WrapPanel>(userControl);
                //环绕面板：餐桌生成父控件
                wpSimers = wraps[0];
                //（2）获取按钮集合
                List<Button> buttons = FindVisualChildren.FindVisualChildrens<Button>(userControl);
                //按钮：排号
                btnNumeral = buttons[0];
                //按钮：一键清理
                btnCleanUp = buttons[1];
                if (wpSimers != null)
                {
                    //移除所有子元素
                    wpSimers.Children.Clear();
                }
                //2、查询餐桌信息
                var listDiningTable = (from tbDiningTable in myModels.S_DiningTable
                                       orderby tbDiningTable.tableNumber
                                       select tbDiningTable).ToList();
                //（1）for循环生成餐桌（餐桌是封装的用户控件DiningTable_UC）
                for (int i = 0; i < listDiningTable.Count; i++)
                {
                    //实例化用户控件
                    DiningTable_UC diningTable_UC = new DiningTable_UC();
                    //把餐桌添加到wpSimers环绕面板
                    wpSimers.Children.Add(diningTable_UC);

                    Opens.DiningTableViewModel dining = diningTable_UC.DataContext as Opens.DiningTableViewModel;
                    //参数传递
                    dining.DiningTableID = listDiningTable[i].diningTableID;//餐桌ID
                    //委托事件：刷新餐桌
                    dining.RefreshDiningTableEvent += RefreshDiningTable;
                    //定义带参数订单ID的委托（刷新订单信息：表格数据、统计信息）
                    dining.PassingIDEvent += ShowOrderdetails;

                }
                //3、查询不同状态的餐桌列表数据
                //*  （1）查询“空桌”状态的的餐桌列表数据
                var listKZ = (from tbDiningTable in myModels.S_DiningTable
                              orderby tbDiningTable.tableNumber
                              where tbDiningTable.status == "空桌"
                              select tbDiningTable).ToList();
                //*  （2）查询“待付款”状态的的餐桌列表数据
                var listDFK = (from tbDiningTable in myModels.S_DiningTable
                               orderby tbDiningTable.tableNumber
                               where tbDiningTable.status == "待付款"
                               select tbDiningTable).ToList();
                //*  （3）查询“已付款”状态的餐桌列表数据
                var listYFK = (from tbDiningTable in myModels.S_DiningTable
                               orderby tbDiningTable.tableNumber
                               where tbDiningTable.status == "已付款"
                               select tbDiningTable).ToList();

                EmptyNumber = listKZ.Count() + "台";//空桌总数
                StayNumber = listDFK.Count() + "台";//待付款总数
                PaymentNumber = listYFK.Count() + "台";//已付款总数

                //* 4、根据餐桌的状态控制“一键清理”按钮和“排序”按钮的透明和不透明    
                //*  （1）“空桌”状态的的餐桌列表数据>0,“排号”按钮不透明；否则，“排号”按钮透明；
                if (listKZ.Count > 0)
                {
                    //“排号”按钮不透明；
                    btnNumeral.Opacity = 1;

                }
                else
                {
                    //否则，“排号”按钮透明；
                    btnNumeral.Opacity = 0;
                }
                //*  （2）“已付款”状态的餐桌列表数据>0,“一键清理”按钮不透明；否则，“一键清理”按钮透明；
                if (listYFK.Count > 0)
                {
                    //“一键清理”按钮不透明；
                    btnCleanUp.Opacity = 1;
                }
                else
                {
                    //否则，“一键清理”按钮透明；
                    btnCleanUp.Opacity = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 刷新餐桌(委托事件调用)
        /// </summary>
        private void RefreshDiningTable()
        {
            LoadedDindingTable(UC);
        }
        /// <summary>
        /// 3.3 刷新订单信息：表格数据、统计信息
        /// </summary>
        /// <param name="intOrderID">订单ID</param>
        private void ShowOrderdetails(int intOrderID)
        {
            try
            {
                //根据订单ID，查询订单明细
                if (intOrderID>0)
                {
                    //根据订单ID，查询订单明细
                    var listOrderDetails = (from tbOrderDetails in myModels.B_Orderdetails
                                            join tbOrder in myModels.B_Order on tbOrderDetails.orderID equals tbOrder.orderID
                                            join tbMenu in myModels.S_Menu on tbOrderDetails.menuID equals tbMenu.menuID
                                            join tbDingTable in myModels.S_DiningTable on tbOrder.orderID equals tbDingTable.orderID
                                            where tbOrderDetails.orderID == intOrderID
                                            select new OrderVo
                                            {
                                                tableNumber = tbDingTable.tableNumber,//桌子编号
                                                number = tbOrder.number,//就餐人数
                                                orderTime = tbOrder.orderTime,//下单时间
                                                money = tbOrder.money,//支付金额
                                                remark = tbOrder.remark,//备注
                                                amount = tbOrder.amount,//订单数量
                                                Price = tbMenu.price,//菜单价格
                                                Dishes = tbMenu.dishes,//菜名
                                                MenuID = tbMenu.menuID,//菜单ID
                                                Quantity = tbOrderDetails.quantity,//总数量
                                            }).ToList();
                    //2、属性绑定（页面数据绑定）
                    if (listOrderDetails.Count>0)
                    {
                        //绑定表格下面基本数据
                        decimal decMoney = Convert.ToDecimal(listOrderDetails[0].money);
                        decMoney = Math.Round(decMoney, 1);
                        TitleGuide = "餐桌" + listOrderDetails[0].tableNumber.ToString().Trim() + "菜单";//标题                       
                        BillAmount = decMoney.ToString().Trim();//账单金额
                        DishesAmount = listOrderDetails[0].amount.ToString().Trim();//菜品总数
                        OrderTime = listOrderDetails[0].orderTime.ToString().Trim();//下单时间
                        OrderDetails = listOrderDetails;//（右边）订单明细表              

                    }
                    else
                    {
                        TitleGuide = "菜单列表";//标题
                                            //没有数据显示为空
                        BillAmount = string.Empty;//账单金额
                        DishesAmount = string.Empty;//菜品总数
                        OrderTime = string.Empty;//下单时间
                        OrderDetails = new List<OrderVo>(); //（右边）订单明细表 
                    }
                }
                else
                {
                    TitleGuide = "菜单列表";//标题
                    //没有数据显示为空
                    BillAmount = string.Empty;//账单金额
                    DishesAmount = string.Empty;//菜品总数
                    OrderTime = string.Empty;//下单时间
                    OrderDetails = new List<OrderVo>(); //（右边）订单明细表   
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
    }
}
