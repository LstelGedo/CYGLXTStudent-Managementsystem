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
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel.Opens
{
    /// <summary>
    /// 点菜菜单(封装菜单)
    /// </summary>
    public class DMenuUCViewModel : ViewModelBase
    {
        public DMenuUCViewModel()
        {
            //页面加载
            LoadedCommand = new RelayCommand(Menu_UC);
            //点餐
            DiningCommand = new RelayCommand(OrderDishes);
        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 定义无返回值，有带参数的委托（传递菜单ID，用于刷新“左侧”菜单列表）
        /// </summary>
        /// <param name="intMenuID">菜单ID</param>
        public delegate void RefreshBindingMenusDelegate(int intMenuID);
        /// <summary>
        /// 定义无返回值，有带参数的事件（传递菜单ID，用于刷新“左侧”菜单列表）
        /// </summary>
        public event RefreshBindingMenusDelegate RefreshBindingMenusEvent;

       
        #endregion
        #region 1、【属性】
        /// <summary>
        /// 当前选中菜单实体（用于记录选中菜单）
        /// </summary>
        public S_Menu CurrentMenuEntity { get; set; }
        /// <summary>
        /// 字段：菜单ID
        /// </summary>
        private int _menuID;
        /// <summary>
        /// 属性：菜单ID
        /// </summary>
        public int MenuID
        {
            get { return _menuID; }
            set
            {
                if (_menuID != value)
                {
                    _menuID = value;
                    RaisePropertyChanged(() => MenuID);

                }
            }
        }
        #region 显示内容
        /// <summary>
        /// 字段：菜单图片
        /// </summary>
        private BitmapImage menuPicture;
        /// <summary>
        /// 属性：菜单图片
        /// </summary>
        public BitmapImage MenuPicture
        {
            get { return menuPicture; }
            set
            {
                if (menuPicture != value)
                {
                    menuPicture = value;
                    RaisePropertyChanged(() => MenuPicture);

                }
            }
        }
        /// <summary>
        /// 字段：菜单名称
        /// </summary>
        private string dishes;
        /// <summary>
        /// 属性：菜单名称
        /// </summary>
        public string Dishes
        {
            get { return dishes; }
            set
            {
                if (dishes != value)
                {
                    dishes = value;
                    RaisePropertyChanged(() => Dishes);

                }
            }
        }
        /// <summary>
        /// 字段：菜单售价
        /// </summary>
        private string price;
        /// <summary>
        /// 属性：菜单售价
        /// </summary>
        public string Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    RaisePropertyChanged(() => Price);

                }
            }
        }


        #endregion
        #endregion
        #region 2、【命令】
        //Loaded命令
        public ICommand LoadedCommand { get; private set; }
        //点餐命令
        public ICommand DiningCommand { get; private set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 菜品信息
        /// </summary>
        private void Menu_UC()
        {
            try
            {
                if (MenuID>0)
                {
                    //1、根据MenuID查询菜单信息（图片，名称和价格）           
                    var listMenu = (from tbMenu in myModel.S_Menu
                                    where tbMenu.menuID == MenuID
                                    select new MenuVo
                                    {
                                        menuID = tbMenu.menuID,//菜单ID
                                        dishes = tbMenu.dishes,//菜名
                                        price = tbMenu.price,//价格
                                        picture = tbMenu.picture,//图片
                                    }).ToList();
                    //提取文件：文件夹
                    //2、判断菜单数据不为空
                    if (listMenu != null)
                    {
                        //3、提取所需单元格（图片，名称和价格）         
                        string strPicture = listMenu[0].picture.Trim();//图片名
                        string strDishes = listMenu[0].dishes.Trim();//菜单名称           
                        string strDirectory = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture"; //基目录下文件                
                        string strPath = strDirectory + "\\" + strPicture.Trim(); //路径=目录+图片名
                        MenuPicture = GetImage.GetBitmapImage(strPath);
                        Dishes = strDishes.Trim();
                        Price = listMenu[0].price.ToString();
                        //临时记录实体
                        CurrentMenuEntity = listMenu[0];
                    }
                }              

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 3.2 点餐
        /// </summary>
        private void OrderDishes()
        {
            if (CurrentMenuEntity!=null)
            {
                //委托事件调用刷新
                RefreshBindingMenusEvent(CurrentMenuEntity.menuID);
            }
        }
        #endregion
    }
}
