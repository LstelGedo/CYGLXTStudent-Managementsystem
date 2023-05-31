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
using CYGLXTStudent.ViewModel.Opens;
using CYGLXTStudent.Views.Opens;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Transactions;

namespace CYGLXTStudent.ViewModel.Opens
{
    public class OrderJCViewModel : ViewModelBase
    {
        public OrderJCViewModel()
        {
            //关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, obj => obj != null);
            //查询数据
            LoadedCommand = new RelayCommand<WrapPanel>(SelectMenun, obj => obj != null);

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
            //取消
            DeleteCommand = new RelayCommand(RemoveDishes);
            //确认修改
            UpdateOrderCommand = new RelayCommand<Window>(UpdateOrder, obj => obj != null);

        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        /// <summary>
        /// 临时保持控件WrapPanel（用于后期添加菜单）
        /// </summary>
        public WrapPanel WP;
        /// <summary>
        /// 当前窗口
        /// </summary>
        public Window WD;
        /// <summary>
        /// 所属类型（全部、冷菜、热菜、糕点、酒水饮料）)
        /// </summary>
        private string Cuisine;
        /// <summary>
        /// 表格追加菜单数据的标记：（默认追加true）
        /// </summary>
        private bool blAdd = true;
        /// <summary>
        /// 第一次生成菜单否的标记 （默认生成true）        
        /// </summary>
        private bool blOne = true;
        /// <summary>
        /// 动态数据集合（用于记录菜单数据MenuVo）
        /// </summary>
        readonly ObservableCollection<MenuVo> menuVos = new ObservableCollection<MenuVo>();

        #endregion
        #region 1、【属性】
        /// <summary>
        /// 字段：订单ID（接收页面传过来的值）
        /// </summary>
        private int orderID;
        /// <summary>
        /// 属性：订单ID（接收页面传过来的值）
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

        #region 页面数值绑定属性
        /// <summary>
        /// 字段：菜单扩展实体动态数据集合（用于绑定表格）
        /// </summary>
        private ObservableCollection<MenuVo> orderDetails = new ObservableCollection<MenuVo>();
        /// <summary>
        /// 属性：菜单扩展实体动态数据集合（用于绑定表格）
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
        /// 字段：菜单扩展实体（选中实体）
        /// </summary>
        private MenuVo selectOrderEntity;
        /// <summary>
        /// 属性：菜单扩展实体（选中实体）
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
        #endregion

        #endregion
        #region 2、【命令】
        // 关闭命令
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        // 页面加载命令
        public RelayCommand<WrapPanel> LoadedCommand { get; private set; }
        //全部
        public ICommand AllMenuCommand { get; set; }
        //冷菜
        public ICommand ColdDishCommand { get; set; }
        //热菜
        public ICommand HotFoodCommand { get; set; }
        //糕点
        public ICommand PastryCommand { get; set; }
        //水果
        public ICommand FruitsCommand { get; set; }
        //酒水饮料
        public ICommand DrinksCommand { get; set; }

        //减少菜品数量
        public ICommand DishesReduceCommand { get; set; }
        //增加菜品数量
        public ICommand DishesIncreaseCommand { get; set; }

        //输入菜品数量
        public ICommand DishesCustomCommand { get; set; }

        //取消菜品
        public ICommand DeleteCommand { get; set; }

        //确认修改下单
        public RelayCommand<Window> UpdateOrderCommand { get; set; }

        #region  菜式选择
        public RelayCommand<Window> BtnAllClickCommand { get; private set; }
        public RelayCommand<Window> BtnLCClickCommand { get; private set; }
        public RelayCommand<Window> BtnRCClickCommand { get; private set; }
        public RelayCommand<Window> BtnGDClickCommand { get; private set; }
        public RelayCommand<Window> BtnSGClickCommand { get; private set; }
        public RelayCommand<Window> BtnJSClickCommand { get; private set; }
        #endregion
        #endregion
        #region 3、【方法】
        /// <summary>
        /// 3.1 关闭窗口
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.2 加载数据（订单明细信息回填、菜单列表生成（纯代码））
        /// </summary>
        /// <param name="wrapPanel"></param>
        private  void SelectMenun(WrapPanel wrapPanel)
        {
           /*
           * 思路：
           * 一、获取嵌套控件（环绕面板），判断是否是有数据，有移除所有元素；
           * 二、回填订单信息（左侧表格和表格下面的统计信息）
           * 三、批量生成菜单（代码生成）
           * 1、实例化菜单扩展实体列表：List<MenuVo> 
           * 2、菜单类型：Cuisine（全部、热菜、冷菜、糕点、饮料等）
           * （1）Cuisine==null，查询全部有效菜单
           * （2）查询有效菜单 by 菜单类型 Cuisine(热菜、冷菜、糕点、酒水饮料等)
           * 3、判断List<MenuVo> 菜单数据不为空
           * 4、循环菜单列表（批量生成菜单）           
           * 5、调用查看菜单列表方法（绑定左侧菜单明细表）         
           */
            try
            {
                // 一、获取嵌套控件（环绕面板），判断是否是有数据，有移除所有元素；
                WP = wrapPanel;
                if (WP!=null)
                {
                    WP.Children.Clear();
                }
                //二、回填数据 (封装成一个方法)
                SelectOrderDetails();
                //三、菜单列表生成（纯代码）
                //1、linq语句提取有效菜单信息，
                List<MenuVo> menuVos = new List<MenuVo>();
                //（1）全部数据
                if (Cuisine==null)
                {
                    menuVos = (from tbMenu in myModels.S_Menu
                               where tbMenu.effective == true
                               select new MenuVo
                               {
                                   menuID = tbMenu.menuID,//菜单ID
                                   dishes = tbMenu.dishes,//菜名
                                   price = tbMenu.price,//售价
                                   picture = tbMenu.picture,//图片(文件夹)

                               }).ToList();
                }
                //（2）筛选数据
                else
                {
                    menuVos = (from tbMenu in myModels.S_Menu
                               where tbMenu.effective == true && tbMenu.cuisine== Cuisine
                               select new MenuVo
                               {
                                   menuID = tbMenu.menuID,//菜单ID
                                   dishes = tbMenu.dishes,//菜名
                                   price = tbMenu.price,//售价
                                   picture = tbMenu.picture,//图片(文件夹)

                               }).ToList();
                }

                //2、批量生成菜单
                if (menuVos.Count>0)
                {

                    for (int i = 0; i < menuVos.Count; i++)
                    {
                        //1、提取已有数据(菜名、售价、图片)
                        string strDishes = menuVos[i].dishes.Trim();
                        string strPrice = menuVos[i].price.ToString();
                        //文件夹提取图片
                        string strPicture = menuVos[i].picture.ToString();
                        //图片路径
                        string strPath = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture" + "\\"+ strPicture;
                        #region 代码生成菜单
                        Border border = new Border
                        {
                            Background = Brushes.White,//背景色
                            BorderBrush = Brushes.SkyBlue,//边框颜色
                            BorderThickness = new Thickness(1, 1, 1, 1),//边框粗度
                            CornerRadius = new CornerRadius(5),//圆角
                            Margin = new Thickness(5)
                        };

                        //（2）Grid
                        Grid grdShow = new Grid
                        {
                            Width = 130,
                            Height = 150
                        };
                        //Grid 分成 3 行
                        grdShow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });//行高占2*
                        grdShow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//行高占1*
                        grdShow.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//行高占1*

                        //1、图片
                        Grid gridPicture = new Grid();
                        Image imgMenu = new Image
                        {
                            Margin = new Thickness(5),
                        };
                        //判断图片文件是否存在
                        if (System.IO.File.Exists(strPath))
                        {
                            //调用方法GetBitmapImage
                            imgMenu.Source = GetImage.GetBitmapImage(strPath);
                        }
                        else
                        {
                            imgMenu.Source = null;
                        }
                        //图片内容添加到gridPicture
                        gridPicture.Children.Add(imgMenu);

                        //2、文字内容
                        Grid gridTxt = new Grid();
                        //gridtxt 分成 2 行 5 列
                        gridTxt.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//行高占1*
                        gridTxt.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });//行高占1*              
                        gridTxt.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });//列宽度为固定值 5
                        gridTxt.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });//列宽占1*
                        gridTxt.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });
                        gridTxt.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });//列宽占3*
                        gridTxt.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });


                        //（1）、菜名（文本标题）                   
                        TextBlock txt_Dishes = new TextBlock
                        {
                            Text = "菜名:",
                            FontSize = 13,
                            Foreground = Brushes.Gray,//字体颜色
                            VerticalAlignment = VerticalAlignment.Center//垂直居中
                        };
                        //单元格（第一行，第二列）菜名（文本标题）      
                        txt_Dishes.SetValue(Grid.RowProperty, 0);
                        txt_Dishes.SetValue(Grid.ColumnProperty, 1);

                        //菜名 值
                        TextBlock txtDishes = new TextBlock
                        {
                            Text = strDishes,
                            FontSize = 13,
                            Foreground = Brushes.Red,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        //单元格（第一行，第四列）菜名 值
                        txtDishes.SetValue(Grid.RowProperty, 0);
                        txtDishes.SetValue(Grid.ColumnProperty, 3);


                        //（2）、价格（文本标题）        
                        TextBlock txt_Price = new TextBlock
                        {
                            Text = "售价:",
                            FontSize = 13,
                            Foreground = Brushes.Gray,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        //单元格（第二行，第二列）价格（文本标题）      
                        txt_Price.SetValue(Grid.RowProperty, 1);
                        txt_Price.SetValue(Grid.ColumnProperty, 1);

                        //价格 值                        
                        TextBlock txtPrice = new TextBlock
                        {
                            Text = "￥" + Math.Round(Convert.ToDecimal(strPrice), 2),
                            FontSize = 13,
                            Foreground = Brushes.Red,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        //单元格（第二行，第四列）价格 值
                        txtPrice.SetValue(Grid.RowProperty, 1);
                        txtPrice.SetValue(Grid.ColumnProperty, 3);

                        //内容添加到 gridTxt
                        gridTxt.Children.Add(txt_Dishes);
                        gridTxt.Children.Add(txtDishes);
                        gridTxt.Children.Add(txt_Price);
                        gridTxt.Children.Add(txtPrice);


                        //3、按钮
                        Grid gridBtn = new Grid();
                        //gridbtn 分成3列
                        gridBtn.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });//列宽度为 5
                        gridBtn.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                        gridBtn.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
                        //实例化按钮                 
                        Button DiningBtn = new Button
                        {
                            Content = "点餐",
                            Tag = menuVos[i].menuID//菜单ID
                        };
                        //引用资源样式（红色填充按钮）
                        DiningBtn.SetValue(Button.StyleProperty, Application.Current.Resources["BtnRedFillStyle"]);
                        //定义RoutedEventHandler 委托事件
                        DiningBtn.Click += new RoutedEventHandler(UpdateMenun);
                        DiningBtn.SetValue(Grid.ColumnProperty, 1);//附加属性Grid.Column：第2列           
                        gridBtn.Children.Add(DiningBtn);


                        gridPicture.SetValue(Grid.RowProperty, 0);//附加属性Grid.Row 第1行（图片）
                        gridTxt.SetValue(Grid.RowProperty, 1);//附加属性Grid.Row 第2行（文字内容）
                        gridBtn.SetValue(Grid.RowProperty, 2);//附加属性Grid.Row 第3行（按钮）

                        //内容添加到 grdShow
                        grdShow.Children.Add(gridPicture);//1、图片
                        grdShow.Children.Add(gridTxt);//2、文字内容
                        grdShow.Children.Add(gridBtn);//3、按钮


                        border.Child = grdShow;//边框（Grid）
                        WP.Children.Add(border);//WraPannel(border)
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 绑定菜单表格数据（表格）
        /// </summary>
        private void SelectOrderDetails()
        {
            /*
           * 思路：
           * 1、查询菜单明细信息 by 订单ID:OrderID
           * 2、 blOne == true 作为第一次添加数据标志 （解决点击菜单按钮的时候出现多次添加数据）   
           * 3、循环菜单明细列表，把菜单数据添加到：动态数据集合menuVos
           * 4、更新菜单表格数据（刷新左侧表格）
           * 5、更新订单统计信息（菜品总数，账单金额）              
           */
            try
            {

                //1、查询菜单明细信息 by 订单ID:OrderID
                List<MenuVo> orderVos = (from tbOrderdetails in myModels.B_Orderdetails
                                          join tbOrder in myModels.B_Order on tbOrderdetails.orderID equals tbOrder.orderID
                                          join tbMenu in myModels.S_Menu on tbOrderdetails.menuID equals tbMenu.menuID
                                         where tbOrder.orderID == OrderID //菜单ID
                                         select new MenuVo
                                          {
                                              dishes = tbMenu.dishes,//菜名
                                              menuID = tbMenu.menuID,//菜单ID
                                              OrderID = tbOrder.orderID,//订单ID
                                              OrderdetailsID = tbOrderdetails.orderDetailsID,//订单明细ID
                                              Amount = tbOrderdetails.quantity,//菜单数量(单条)
                                              Number = tbOrder.number,//就餐人数
                                              Money = tbOrder.money,//金额
                                              Remark = tbOrder.remark,//备注
                                              price = tbMenu.price,//图片
                                              MenuPrice = tbMenu.price,//价格
                                          }).ToList();

                //2、 blOne == true 作为第一次添加数据标志 （解决点击菜单按钮的时候出现多次添加数据）              
                if (orderVos.Count() > 0 && blOne == true)
                {
                    //3、循环菜单明细列表，把菜单数据添加到：动态数据集合menuVos
                    for (int i = 0; i < orderVos.Count; i++)
                    {
                        //把菜单数据添加到：动态数据集合menuVos
                        menuVos.Add(orderVos[i]);
                    }
                    //4、更新菜单表格数据（刷新左侧表格）
                    OrderDetails = menuVos;
                }
                //5、更新订单统计信息（菜品总数，账单金额）
                OrderStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.4 订单列表下的统计（菜品总数，账单金额）
        /// </summary>
        private void OrderStatistics()
        {
            //初始化变量（用于记录：全部菜品总数量）
            int? intAmount = 0;
            //初始化变量（用于记录：账单消费总金额）
            decimal? decMenuPrice = 0;
            //循环：左侧菜单列表：OrderDetails
            for (int i = 0; i < OrderDetails.Count; i++)
            {
                MenuVo menu = OrderDetails[i] as MenuVo;
                intAmount += menu.Amount;//累加总数
                decMenuPrice += menu.MenuPrice;//累加金额
            }
            //全部菜品总数量
            DishesTotal = intAmount.ToString();
            //账单消费总金额
            BillAmount = decMenuPrice.ToString();
        }
        #region 3.6 菜式选择
        /// <summary>
        /// 3.6.1 查询菜单“全部”
        /// </summary>
        private void SelectAllMenu()
        {
            Cuisine = null;//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }

        /// <summary>
        /// 3.6.2 查询菜单 “冷菜”
        /// </summary>
        private void SelectColdDish()
        {
            Cuisine = "冷菜";//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }
        /// <summary>
        /// 3.6.3 查询菜单 “热菜”
        /// </summary>       
        private void SelectHotFood()
        {
            Cuisine = "热菜";//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }

        /// <summary>
        /// 3.6.4 查询菜单 “糕点”
        /// </summary>
        private void SelectPastry()
        {
            Cuisine = "糕点";//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }

        /// <summary>
        /// 3.6.5 查询菜单 “水果”
        /// </summary>
        private void SelectFruits()
        {
            Cuisine = "水果";//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }
        /// <summary>
        /// 3.6.6 查询菜单 “酒水饮料”
        /// </summary>
        private void SelectDrinks()
        {
            Cuisine = "酒水饮料";//菜品所属类型
            blOne = false;//不追加
            SelectMenun(WP);//更新菜单数据
        }
        #endregion
        /// <summary>
        /// 3.5 点餐方法
        /// </summary>
        /// <param name="sender">事件处理程序所附加到的对象。</param>
        /// <param name="e">事件数据。</param>
        private void UpdateMenun(object sender, RoutedEventArgs e)
        {
            try
            {
                //1、提取菜单信息（菜单ID）
                Button button = (Button)sender;
                int intMenuID = Convert.ToInt32(button.Tag);

                //2、查询菜单信息 by intMenuID;
                //（2）根据ID查询菜单信息
                MenuVo menuVo = (from tbMenu in myModels.S_Menu
                                 where tbMenu.menuID == intMenuID
                                 select new MenuVo
                                 {
                                     menuID = tbMenu.menuID,//菜单ID
                                     dishes = tbMenu.dishes,//菜名
                                     price = tbMenu.price,//售价
                                     picture = tbMenu.picture,//图片
                                     Amount = 1,//当前菜品数量
                                     MenuPrice = tbMenu.price,//当前菜品总额
                                 }).Single();
                //循环列表
                for (int i = 0; i < OrderDetails.Count; i++)
                {
                    MenuVo menu = OrderDetails[i];
                    int intExistID = menu.menuID;
                    int? intAmount= menu.Amount;                  
                        //2、判断点击菜单ID和列表比较，
                    if (intMenuID == intExistID)
                    {
                        // （1）存在、则数量增加；
                        blAdd=false;
                        menu.Amount = intAmount + 1;
                        menu.MenuPrice = menu.Amount * menu.price;
                        OrderDetails = null;
                        OrderDetails = menuVos;
                    }
                }
                if (blAdd==true)
                {
                    //（2）否则、数据追加。
                    menuVos.Add(menuVo);
                    OrderDetails = menuVos;
                }
                //初始化
                blAdd = true;
                //订单列表下的统计（菜品总数，账单金额）
                OrderStatistics();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #region 3.7 表格操作[+ - x]
        /// <summary>
        /// 菜品数量：减少
        /// </summary>
        private void DishesReduce()
        {
            try
            {
                if (SelectOrderEntity!=null)
                {
                    //获取当前选中菜单
                    MenuVo menu = SelectOrderEntity;
                    //每个菜单总数量
                    int intAmount = Convert.ToInt32(menu.Amount);
                    //最小不能小于1
                    if (intAmount > 1)
                    {
                        menu.Amount = intAmount - 1;//菜单总数量=菜单总数量 -1            
                        menu.MenuPrice = menu.Amount * menu.price;//每个菜单总金额= 价格 * 菜单总数量    
                        OrderDetails = menuVos;//表格重置（绑定刷新表格数据）
                    }
                    //刷新信息：订单列表下的统计（菜品总数，账单金额）
                    OrderStatistics();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        /// <summary>
        /// 菜品数量：增加
        /// </summary>
        private void DishesIncrease()
        {
            try
            {
                if (SelectOrderEntity!=null)
                {
                    //获取当前选中菜单
                    MenuVo menu = SelectOrderEntity;
                    int intAmount = Convert.ToInt32(menu.Amount);
                    menu.Amount = intAmount + 1;//菜单总数量=菜单总数量+1         
                    menu.MenuPrice = menu.price * menu.Amount;//每个菜单总金额= 价格 * 菜单总数量    
                    OrderDetails = menuVos;//表格重置（绑定刷新表格数据）
                    //刷新信息：订单列表下的统计（菜品总数，账单金额）
                    OrderStatistics();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        /// <summary>
        /// 菜品数量：菜数输入
        /// </summary>
        private void DishesCustom()
        {
            try
            {
                if (SelectOrderEntity!=null)
                {
                    //获取当前选中菜单
                    MenuVo menu = SelectOrderEntity;
                    //菜单数据不为空
                    if (menu != null)
                    {
                        //菜单总数量=输入菜单数
                        int intAmount = Convert.ToInt32(menu.Amount);
                        //是否符合正则表达式(正整数)
                        bool isCorrect = Regex.IsMatch(intAmount.ToString(), @"^[1-9]\d*$");
                        if (isCorrect)
                        {
                            //每个菜单总金额= 价格 * 菜单总数量    
                            menu.MenuPrice = intAmount * menu.price;
                            //表格重置（绑定刷新表格数据）
                            OrderDetails = menuVos;
                        }
                        else
                        {
                            menu.Amount = 1;//默认为1
                            menu.MenuPrice = menu.Amount * menu.price;//每个菜单总金额= 价格 * 菜单总数量    
                            OrderDetails = menuVos;//表格重置（绑定刷新表格数据）
                            MessageBox.Show("输入的值必须为正整数！", "提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    //刷新信息：订单列表下的统计（菜品总数，账单金额）
                    OrderStatistics();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        /// <summary>
        /// 菜品:移除
        /// </summary>
        private void RemoveDishes()
        {
            try
            {
                if (SelectOrderEntity!=null)
                {
                    //获取当前选中行
                    MenuVo menu = SelectOrderEntity;
                    //移除数据
                    menuVos.Remove(menuVos.FirstOrDefault(u => u.menuID == menu.menuID));
                    //表格重置（绑定刷新表格数据）    
                    OrderDetails = menuVos;
                    //刷新信息：订单列表下的统计（菜品总数，账单金额）
                    OrderStatistics();
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        #endregion
        /// <summary>
        /// 3.8 确认修改（加退菜）
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void UpdateOrder(Window window)
        {
            /*
           * 思路：
           * 1、多表操作开启事务
           * 2、获取表格数据集合：OrderDetails 大于等于1（代表表格有数据）
           * 3、查询现有数据B_Orderdetails：批量移除
           * 4、修改：点餐表：B_Order
           * 5、修改：餐桌表：S_DiningTable
           * 6、循环OrderDetails，批量新增：点餐明细表：B_Orderdetails
           * 7、保存更改，提交事务，提示，关闭窗口
           */
            using (TransactionScope scope=new TransactionScope ())
            {
                try
                {
                    //2、获取表格数据集合：（代表表格有数据）
                    if (OrderDetails.Count>0)
                    {                      
                        //3、查询现有数据B_Orderdetails：批量移除
                        var listOrderdetails = (from tbOrderdetails in myModels.B_Orderdetails
                                                where tbOrderdetails.orderID == OrderID
                                                select tbOrderdetails).ToList();
                        //循环列表数据
                        for (int i = 0; i < listOrderdetails.Count(); i++)
                        {
                            //移除
                            myModels.B_Orderdetails.Remove(listOrderdetails[i]);
                            //保存更改
                            myModels.SaveChanges();
                        }

                        //4、修改：点餐表：B_Order
                        B_Order dbOrder = (from tbOrder in myModels.B_Order
                                           where tbOrder.orderID == OrderID
                                           select tbOrder).Single();
                        dbOrder.money = Convert.ToDecimal(BillAmount);//账单金额
                        dbOrder.amount = Convert.ToInt32(DishesTotal);//菜数
                        myModels.Entry(dbOrder).State = System.Data.Entity.EntityState.Modified;

                        //5、修改：餐桌表：S_DiningTable
                        S_DiningTable dbDiningTable = (from tbDiningTable in myModels.S_DiningTable
                                                       where tbDiningTable.orderID == OrderID
                                                       select tbDiningTable).Single();
                        dbDiningTable.totalMoney = Convert.ToDecimal(BillAmount);//账单金额
                        myModels.Entry(dbDiningTable).State = System.Data.Entity.EntityState.Modified;


                        //6、循环列表直接新增数据
                        for (int i = 0; i < OrderDetails.Count; i++)
                        {
                            MenuVo dbMenu = OrderDetails[i];
                            //（3）批量新增：点餐明细表：B_Orderdetails
                            B_Orderdetails dbOrderdetails = new B_Orderdetails
                            {
                                orderID = OrderID,//点餐ID
                                menuID = dbMenu.menuID,//菜名ID
                                quantity = dbMenu.Amount//数量
                            };
                            //添加
                            myModels.B_Orderdetails.Add(dbOrderdetails);
                        }
                        //7、保存更改，提交事务，提示，关闭窗口
                        if (myModels.SaveChanges() > 0)
                        {
                            //事务提交
                            scope.Complete();
                            //提示
                            MessageBox.Show("改单成功！", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Information);
                            //关闭窗口
                            window.Close();
                        }
                        else
                        {
                            MessageBox.Show("改单失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }


                    }
                    else
                    {
                        MessageBox.Show("请点菜！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }
        #endregion
    }
}
