using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Transactions;

namespace CYGLXTStudent.ViewModel.Opens
{
    /// <summary>
    /// 点餐
    /// </summary>
    public class OrderKTViewModel : ViewModelBase
    {
        public OrderKTViewModel()
        {           
            //关闭窗口
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, wd => wd != null);
            //页面加载
            LoadedCommand = new RelayCommand<WrapPanel>(SelectMenu, wp => wp != null);
            //全部
            AllMenuCommand = new RelayCommand(SelectAllMenu);
            //冷菜
            ColdDishCommand = new RelayCommand(SelectColdDish);
            //热菜
            HotFoodCommand = new RelayCommand(SelectHotFood);
            //糕点
            PastryCommand = new RelayCommand(SelectPastry);
            //水果
            FruitsCommand = new RelayCommand(SelectFruits);
            //酒水饮料
            DrinksCommand = new RelayCommand(SelectDrinks);

            //菜品数量：减少
            DishesReduceCommand = new RelayCommand(DishesReduce);
            //菜品数量：增加
            DishesIncreaseCommand = new RelayCommand(DishesIncrease);
            //菜品数量：输入
            DishesCustomCommand = new RelayCommand(DishesCustom);
            //取消(移除)
            DeleteCommand = new RelayCommand(DMButton_Click_Delete);

            //就餐人数：减少
            PeopleReduceCommand = new RelayCommand(PeopleReduce);
            //就餐人数：增加
            PeopleIncreaseCommand = new RelayCommand(PeopleIncrease);
            ///就餐人数：输入人数(自定义人数)
            PeopleCustomCommand = new RelayCommand(PeopleCustom);

            //下单
            OrderCommand = new RelayCommand<Window>(PlaceTheOrder, obj => obj != null);

        }
        #region 【0、全局变量】
        /// <summary>
        /// 实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();

        /// <summary>
        /// 动态数据集合（临时集合：后期用来操作菜单）
        /// 更新集合属性:这里不要用List要用ObservableCollection达到刷新的效果
        /// </summary>
        readonly ObservableCollection<MenuVo> Menus = new ObservableCollection<MenuVo>();
        /// <summary>
        /// 菜单所属类型(冷菜、热菜等)
        /// </summary>
        private static string Cuisine = null;
        /// <summary>
        /// (是否添加菜单标记)true：表格数据添加，false：数据重复（菜单数量累加）
        /// </summary>
        private bool blAdd = true;
        /// <summary>
        /// 环绕面板（用于记录控件）
        /// </summary>
        public WrapPanel WPSimers;
        #endregion
        #region 【1、属性】 
        #region 参数传递和接收
        /// <summary>
        /// 私有字段：餐桌ID
        /// </summary>
        private int diningTableID;
        /// <summary>
        /// 属性：餐桌ID
        /// </summary>
        public int DiningTableID
        {
            get { return diningTableID; }
            set
            {
                if (diningTableID != value)
                {
                    diningTableID = value;
                    RaisePropertyChanged(() => DiningTableID);

                }
            }
        }
        /// <summary>
        /// 私有字段：餐桌人数
        /// </summary>
        private int number;
        /// <summary>
        /// 属性：餐桌人数（主页面传递）
        /// </summary>
        public int Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                    RaisePropertyChanged(() => Number);

                }
            }
        }
        #endregion
        #region 窗口绑定属性
        /// <summary>
        /// 私有字段：菜单扩展实体动态数据集合（绑定表格）
        /// </summary>
        private ObservableCollection<MenuVo> orderDetails = new ObservableCollection<MenuVo>();
        /// <summary>
        /// 属性：菜单扩展实体动态数据集合（绑定表格）
        /// </summary>
        public ObservableCollection<MenuVo> OrderDetails
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
        /// 私有字段：菜品总数
        /// </summary>
        private string dishesTotal;
        /// <summary>
        /// 属性：菜品总数
        /// </summary>
        public string DishesTotal
        {
            get { return dishesTotal; }
            set
            {
                if (dishesTotal != value)
                {
                    dishesTotal = value;
                    RaisePropertyChanged(() => DishesTotal);

                }
            }
        }
        /// <summary>
        /// 私有字段：账单金额
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
        /// 私有字段：餐桌用餐人数
        /// </summary>
        private string peopleNumber = "1";
        /// <summary>
        /// 属性：餐桌用餐人数
        /// </summary>
        public string PeopleNumber
        {
            get { return peopleNumber; }
            set
            {
                if (peopleNumber != value)
                {
                    peopleNumber = value;
                    RaisePropertyChanged(() => PeopleNumber);

                }
            }
        }
        /// <summary>
        /// 私有字段：餐桌备注（整单备注）
        /// </summary>        
        private string tableNote;
        /// <summary>
        /// 属性：餐桌备注（整单备注）
        /// </summary>    
        public string TableNote
        {
            get { return tableNote; }
            set
            {
                if (tableNote != value)
                {
                    tableNote = value;
                    RaisePropertyChanged(() => TableNote);

                }
            }
        }
        /// <summary>
        /// 私有字段：菜单扩展实体（用于记录当前选中菜单）
        /// </summary>
        private MenuVo selectOrderEntity;
        /// <summary>
        /// 属性：菜单扩展实体（用于记录当前选中菜单）
        /// </summary>
        public MenuVo SelectOrderEntity

        {
            get { return selectOrderEntity; }
            set
            {
                if (selectOrderEntity != value)
                {
                    selectOrderEntity = value;
                    RaisePropertyChanged(() => SelectOrderEntity);

                }
            }
        }
        #endregion
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.0 加载命令(批量生成菜单)
        /// </summary>
        public RelayCommand<WrapPanel> LoadedCommand { get; private set; }
        /// <summary>
        /// 2.1 关闭命令（关闭窗口）
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        /// <summary>
        /// 2.2 全部命令（查询全部菜单）
        /// </summary>
        public ICommand AllMenuCommand { get; set; }
        /// <summary>
        /// 3.3 冷菜命令（查询冷菜）
        /// </summary>
        public ICommand ColdDishCommand { get; set; }
        /// <summary>
        /// 3.4 热菜命令（查询热菜）
        /// </summary>
        public ICommand HotFoodCommand { get; set; }
        /// <summary>
        /// 3.5 糕点命令（查询糕点）
        /// </summary>
        public ICommand PastryCommand { get; set; }
        /// <summary>
        /// 3.6 水果命令（查询水果）
        /// </summary>
        public ICommand FruitsCommand { get; set; }
        /// <summary>
        /// 3.7 酒水饮料命令（查询 酒水饮料）
        /// </summary>
        public ICommand DrinksCommand { get; set; }
        /// <summary>
        /// 3.8 就餐人数：减少命令
        /// </summary>
        public ICommand PeopleReduceCommand { get; set; }
        /// <summary>
        /// 3.9 就餐人数：增加命令
        /// </summary>
        public ICommand PeopleIncreaseCommand { get; set; }
        /// <summary>
        /// 3.10 就餐人数：输入（自定义）命令
        /// </summary>
        public ICommand PeopleCustomCommand { get; set; }

        /// <summary>
        /// 3.11 菜品数量：减少命令
        /// </summary>
        public ICommand DishesReduceCommand { get; set; }
        /// <summary>
        /// 3.12 菜品数量：增加命令
        /// </summary>
        public ICommand DishesIncreaseCommand { get; set; }

        /// <summary>
        /// 3.13 菜品数量：输入命令
        /// </summary>
        public ICommand DishesCustomCommand { get; set; }

        /// <summary>
        /// 3.14 取消菜品（移除菜单）命令
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        /// <summary>
        /// 3.15 下单命令（点餐）
        /// </summary>
        public RelayCommand<Window> OrderCommand { get; set; }

        /// <summary>
        /// 3.16 整单备注（菜单备注）
        /// </summary>
        public RelayCommand RemarkCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.0 关闭窗口
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                //关闭窗口
                window.Close();
            }
        }
        /// <summary>
        /// 3.1 加载菜单
        /// </summary>
        /// <param name="panel">环绕面板</param>
        private void SelectMenu(WrapPanel panel)
        {
            try
            {
                //提取控件
                WPSimers = panel;
                if (WPSimers!=null)
                {
                    //移除所有元素
                    WPSimers.Children.Clear();
                }
                //声明局部列表
                List<MenuVo> listMenu = new List<MenuVo>();
                //生成菜单：
                if (Cuisine == null)
                {
                    //（1）查询全部有效菜单
                    listMenu = (from tbMenu in myModels.S_Menu
                                where tbMenu.effective == true
                                select new MenuVo
                                {
                                    menuID = tbMenu.menuID,//菜单ID
                                    dishes = tbMenu.dishes,//菜名
                                    price = tbMenu.price,//价格
                                    picture = tbMenu.picture,//菜单图片
                                }).ToList();
                }
                else
                {
                    //（2）查询有效菜单 by 菜单类型Cuisine
                    listMenu = (from tbMenu in myModels.S_Menu
                                where tbMenu.effective == true
                                where tbMenu.cuisine == Cuisine//类型
                                select new MenuVo
                                {
                                    menuID = tbMenu.menuID,//菜单ID
                                    dishes = tbMenu.dishes,//菜名
                                    price = tbMenu.price,//价格
                                    picture = tbMenu.picture,//菜单图片
                                }).ToList();
                }
                //3、WPSimers添加子控件DMenu_UC
                if (listMenu!=null)
                {
                    for (int i = 0; i < listMenu.Count; i++)
                    {
                        Views.Opens.DMenu_UC uC = new Views.Opens.DMenu_UC();
                        //viewModel 传参
                        DMenuUCViewModel viewModel = uC.DataContext as DMenuUCViewModel;
                        viewModel.MenuID = listMenu[i].menuID;
                        //委托事件调用（刷新左侧表格)
                        viewModel.RefreshBindingMenusEvent += BindingMenus;
                        WPSimers.Children.Add(uC);
                    }
                }
                else
                {
                    MessageBox.Show("无法生成菜单，请去菜单管理模块生成菜单！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region 3.4 菜式选择
        /// <summary>
        /// 3.4.1 查询菜单“全部”
        /// </summary>
        private void SelectAllMenu()
        {
            Cuisine = null;//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }

        /// <summary>
        /// 3.4.2 查询菜单 “冷菜”
        /// </summary>
        private void SelectColdDish()
        {
            Cuisine = "冷菜";//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }

        /// <summary>
        /// 3.4.3 查询菜单 “热菜”
        /// </summary>
        private void SelectHotFood()
        {
            Cuisine = "热菜";//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }

        /// <summary>
        /// 3.4.4 查询菜单 “糕点”
        /// </summary>
        private void SelectPastry()
        {
            Cuisine = "糕点";//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }

        /// <summary>
        /// 3.4.5 查询菜单 “水果”
        /// </summary>
        private void SelectFruits()
        {
            Cuisine = "水果";//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }
        /// <summary>
        /// 3.4.6 查询菜单 “酒水饮料”
        /// </summary>
        private void SelectDrinks()
        {
            Cuisine = "酒水饮料";//菜品所属类型
            SelectMenu(WPSimers);//更新菜单数据
        }
        #endregion
        /// <summary>
        ///  3.2 绑定菜单表格数据
        /// </summary>
        /// <param name="intMenuID">菜单ID</param>
        private void BindingMenus(int intMenuID)
        {
            try
            {
                //1、判断菜单ID是否大于0              
                if (intMenuID>0)
                {
                    //2、查询菜单扩展实体信息 by 菜单ID
                    MenuVo vo = (from tbMenu in myModels.S_Menu
                                 where tbMenu.menuID == intMenuID
                                 select new MenuVo
                                 {
                                     menuID = tbMenu.menuID,//菜单ID
                                     dishes = tbMenu.dishes,//菜名
                                     price = tbMenu.price,//售价（单价）
                                     picture = tbMenu.picture,//图片
                                     Amount = 1,//菜单总数
                                     MenuPrice = tbMenu.price,//总价

                                 }).Single();
                    //3、for循环动态数据集合属性OrderDetails，判断菜单数据是否重复：两种情况
                    for (int i = 0; i < OrderDetails.Count; i++)
                    {
                        //（1）菜单重复，数量叠加；                 
                        //① 实例化菜单扩展实体对象获取动态数据集实体
                        MenuVo menuVo = OrderDetails[i];
                        //② 声明局部变量获取菜单ID和菜单数量；
                        int intExistMenuID = menuVo.menuID;//菜单ID
                        int? intAmount = menuVo.Amount;//菜单数量
                        //③ 判断：存在菜单ID == 当前菜单IDintMenuID
                        if (intExistMenuID== intMenuID)
                        {
                            //④ 添加标记 = false(代表重复)；
                            blAdd = false;
                            //数量叠加：菜单数量 = 菜单数量 + 1；
                            menuVo.Amount = intAmount + 1;
                            //菜单总金额 = 单价 * 菜单数量
                            menuVo.MenuPrice = menuVo.price * menuVo.Amount;
                            //⑤ 更新动态数据集合（刷新左侧表格）= 临时集合 Menus
                            OrderDetails = Menus;                           
                        }
                    }
                    if (blAdd==true)
                    {
                        //（2）添加标记 == true,菜单不重复，菜单数据追加；
                        //① 临时集合追加一条菜单数据
                        Menus.Add(vo);
                        //② 更新动态数据集合（刷新左侧表格）= 临时集合
                        OrderDetails = Menus;
                    }
                    //4、添加标记：恢复默认值true
                    blAdd = true;
                    //5、调用方法，更新统计信息
                    OrderStatistics();
                }

            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        ///  3.3 订单列表下的统计（菜品总数，账单金额）
        /// </summary>
        private void OrderStatistics()
        {
            /*
            思路：
           1、初始化两个局部变量（全部菜品总数量、账单消费总金额）
           2、循环：左侧菜单列表：OrderDetails，叠加数量和总金额
           3、属性绑定
            */
            try
            {
                //初始化变量（用于记录：全部菜品总数量）
                int? intAmount = 0;
                //初始化变量（用于记录：账单消费总金额）
                decimal? decMenuPrice = 0;
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    //intAmount = intAmount + OrderDetails[i].Amount;
                    //decMenuPrice = decMenuPrice + OrderDetails[i].MenuPrice;
                    MenuVo menu = OrderDetails[i];
                    intAmount += menu.Amount;//每个菜品“数量”叠加
                    decMenuPrice += menu.MenuPrice;//每个菜品“总价”叠加
                }
                DishesTotal = intAmount.ToString();
                BillAmount = decMenuPrice.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 菜品数量：减少
        /// </summary>
        private void DishesReduce()
        {
            if (SelectOrderEntity!=null)
            {
                //获取选中项
                MenuVo menuVo = SelectOrderEntity;
                int? intAmount = menuVo.Amount;               
                if (intAmount >1)
                {
                    menuVo.Amount = intAmount - 1;
                    menuVo.MenuPrice = menuVo.price * menuVo.Amount;
                }
                //刷新订单列表下的统计（菜品总数，账单金额）
                OrderStatistics();
            }
        }
        /// <summary>
        /// 3.5 菜品数量：增加
        /// </summary>
        private void DishesIncrease()
        {
            if (SelectOrderEntity != null)
            {
                //获取选中项
                MenuVo menuVo = SelectOrderEntity;
                int? intAmount = menuVo.Amount;
                menuVo.Amount = intAmount + 1;
                menuVo.MenuPrice = menuVo.price * menuVo.Amount;
                //刷新订单列表下的统计（菜品总数，账单金额）
                OrderStatistics();
            }
        }
        /// <summary>
        /// 3.6 菜品数量：输入(在这里作用不大，因为Textbook有错误验证)
        /// </summary>
        private void DishesCustom()
        {
            //正则验证输入必须是正整数
            if (SelectOrderEntity != null)
            {
                //获取选中项
                MenuVo menuVo = SelectOrderEntity;
                //2、菜单总数量=输入菜单数
                int intAmount = Convert.ToInt32(menuVo.Amount);
                //4、是否符合正则表达式(正整数)
                bool isCorrect = Regex.IsMatch(intAmount.ToString(), @"^[1-9]\d*$");
                if (isCorrect)
                {
                    menuVo.MenuPrice = intAmount * menuVo.price;//每个菜单总金额= 价格 * 菜单总数量    
                    OrderDetails = Menus;//表格重置（绑定刷新表格数据）
                }
                else
                {
                    menuVo.Amount = 1;//默认为1
                    menuVo.MenuPrice = menuVo.Amount * menuVo.price;//每个菜单总金额= 价格 * 菜单总数量    
                    OrderDetails = Menus;//表格重置（绑定刷新表格数据）
                    MessageBox.Show("输入的值必须为正整数！", "提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                //刷新订单列表下的统计（菜品总数，账单金额）
                OrderStatistics();
            }

        }
        /// <summary>
        /// 3.7 移除菜单
        /// </summary>
        private void DMButton_Click_Delete()
        {
            try
            {
                //获取当前选中行
                if (SelectOrderEntity != null)
                {
                    //获取实体
                    MenuVo menu = SelectOrderEntity;
                    //临时动态数据集合移除数据
                    Menus.Remove(menu);
                    //② 更新动态数据集合（刷新左侧表格）= 临时集合
                    OrderDetails = Menus;
                    //刷新信息：订单列表下的统计（菜品总数，账单金额）
                    OrderStatistics();
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.8 减去人数按钮（最小1）
        /// </summary>
        private void PeopleReduce()
        {
            if (!string.IsNullOrEmpty(PeopleNumber))
            {
                //获取：现有就餐人数
                int intNumber = Convert.ToInt32(PeopleNumber);
                if (intNumber > 1)
                {
                    //就餐人数= 现有就餐人数-1
                    int EndNumber = intNumber - 1;
                    PeopleNumber = EndNumber.ToString();
                }
            }
            else
            {
                MessageBox.Show("请输入就餐人数!", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        /// <summary>
        /// 3.6 加上人数按钮（最大不超过容纳人数）
        /// </summary>
        private void PeopleIncrease()
        {
            //获取：现有就餐人数
            int intNumber = Convert.ToInt32(PeopleNumber);
            //就餐人数 <= 餐桌可容纳人数
            if ((intNumber + 1) <= Number)
            {
                //就餐人数= 现有就餐人数 + 1
                int EndNumber = intNumber + 1;
                PeopleNumber = EndNumber.ToString();
            }
        }
        /// <summary>
        ///  3.7 用餐人数输入（必须输入数字）
        /// </summary>
        private void PeopleCustom()
        {

            //1、获取就餐人数
            string strPeopleNumber = PeopleNumber.ToString();
            //2、正则判断输入人数
            if (!Regex.IsMatch(strPeopleNumber, @"^[1-9]\d*$"))
            {
                //恢复默认值1
                PeopleNumber = "1";
                MessageBox.Show("请输入数字！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            //3、获取输入人数
            int intNumber = Convert.ToInt32(strPeopleNumber);
            //4、就餐人数 <= 餐桌可容纳人数
            if (intNumber <= Number)
            {
                //获取就餐人数
                PeopleNumber = strPeopleNumber;
            }
            else
            {
                //恢复初始值
                PeopleNumber = "1";
                MessageBox.Show("就餐人数不能大于餐桌容纳人数！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        /// <summary>
        /// 点餐
        /// </summary>
        /// <param name="window">窗口</param>
        private void PlaceTheOrder(Window window)
        {
            try
            {
                // *1、多表操作开启事务
                using (TransactionScope scope= new TransactionScope ())
                {
                    //* 2、获取表格数据集合：OrderDetails 大于等于1（代表表格有数据）
                    if (OrderDetails.Count>0)
                    {
                        //* （1）新增：订单表点餐表：B_Order
                        string Remark = TableNote;//餐桌备注
                        //（1）新增点餐表：B_Order
                        B_Order dbOrder = new B_Order
                        {
                            number = Convert.ToInt32(PeopleNumber),//用餐人数
                            orderTime = DateTime.Now,//下单时间
                            money = Convert.ToDecimal(BillAmount)//账单金额
                        };
                        if (Remark != "＋整单备注")
                        {
                            dbOrder.remark = TableNote;//备注
                        }
                        else
                        {
                            dbOrder.remark = null;
                        }
                        dbOrder.amount = Convert.ToInt32(DishesTotal);//菜品总数
                        myModels.B_Order.Add(dbOrder); //添加                        
                        myModels.SaveChanges();//保存
                        //* （2）批量新增：点餐明细表：B_Orderdetails
                        //（2）批量新增，点餐明细表：B_Orderdetails
                        for (int i = 0; i < OrderDetails.Count; i++)
                        {
                            MenuVo dbMenu = OrderDetails[i] as MenuVo;
                            int intMenuID = dbMenu.menuID;//菜名ID
                            int? intQuantity = dbMenu.Amount;//点菜数量
                            //实例化点餐明细表
                            B_Orderdetails dbOrderdetails = new B_Orderdetails
                            {
                                orderID = dbOrder.orderID,//点餐ID（更新增）
                                menuID = intMenuID,//菜名ID
                                quantity = intQuantity//每个菜单总数
                            };
                            //添加
                            myModels.B_Orderdetails.Add(dbOrderdetails);
                        }
                        //* （3）修改：餐桌表：S_DiningTable（餐桌状态 =“待付款”）
                        S_DiningTable dbDiningTable = myModels.S_DiningTable.Find(DiningTableID);
                        dbDiningTable.orderID = dbOrder.orderID;
                        dbDiningTable.totalMoney = Convert.ToDecimal(BillAmount);
                        dbDiningTable.status = "待付款";
                        myModels.Entry(dbDiningTable).State = System.Data.Entity.EntityState.Modified;
                        //* （4）新增：就餐记录表：B_EatInRecord
                        //（4）新增就餐记录表：B_EatInRecord
                        B_EatInRecord dbEatInRecord = new B_EatInRecord
                        {
                            number = Convert.ToInt32(PeopleNumber),//用餐人数
                            status = "待付款",//就餐记录状态
                            totalMoney = Convert.ToDecimal(BillAmount),//账单金额
                            orderID = dbOrder.orderID,//点餐订单ID
                            startTime = DateTime.Now//就餐时间
                        };
                        myModels.B_EatInRecord.Add(dbEatInRecord);//新增
                       //* （5）保存更改，提交事务
                        if (myModels.SaveChanges() > 0)
                        {
                            scope.Complete();//事务提交
                            MessageBox.Show("下单成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            window.Close();//关闭窗口
                        }
                        else
                        {
                            MessageBox.Show("下单失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("还没下单，请选择菜品！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
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
