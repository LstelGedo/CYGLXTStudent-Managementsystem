using CYGLXTStudent.Model;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.ViewModel.Menus;
using CYGLXTStudent.Views.Menus;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            //加载命令
            LoadedCommand = new RelayCommand<UserControl>(SelectMenu, uc => uc != null);
            //全部数据
            AllCommand = new RelayCommand(AllMenu);
            //冷菜
            ColdDishCommand = new RelayCommand(ColdDish);
            //热菜
            HotFoodCommand = new RelayCommand(HotFood);
            //糕点
            PastryCommand=new RelayCommand(Pastry);
            //水果
            FruitCommand = new RelayCommand(Fruit);
            //酒水饮料
            DrinksCommand = new RelayCommand(Drinks);
            //打开编辑菜单窗口
            AddCommand = new RelayCommand(OpenEditWindow);
        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 接收用户控件自身；
        /// </summary>
        UserControl UC;
        /// <summary>
        /// 环绕面板（用于生成子菜单）
        /// </summary>
        WrapPanel WP;
        /// <summary>
        /// 所属类型(菜单分类)
        /// </summary>
        string Cuisine;
        #endregion       
        #region 【2、命令】
        /// <summary>
        /// 2.1 加载命令
        /// </summary>
        public RelayCommand<UserControl> LoadedCommand { get; set; }
        /// <summary>
        /// 2.2 全部命令
        /// </summary>
        public RelayCommand AllCommand { get; set; }
        /// <summary>
        /// 2.3 冷菜命令
        /// </summary>
        public RelayCommand ColdDishCommand { get; set; }
        /// <summary>
        /// 2.4 热菜命令
        /// </summary>
        public RelayCommand HotFoodCommand { get; set; }
        /// <summary>
        /// 2.5 糕点命令
        /// </summary>
        public RelayCommand PastryCommand { get; set; }
        /// <summary>
        /// 2.6 水果命令
        /// </summary>
        public RelayCommand FruitCommand { get; set; }
        /// <summary>
        /// 2.7 酒水饮料命令
        /// </summary>
        public RelayCommand DrinksCommand { get; set; }
        /// <summary>
        /// 2.8 打开新增窗口命令
        /// </summary>
        public RelayCommand AddCommand { get; set; }

        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 查询菜单信息
        /// </summary>
        /// <param name="userControl">用户控件</param>
        private void SelectMenu(UserControl userControl)
        {
            try
            {
                UC = userControl;
                //1、根据用户控件，提取环绕面板（从视觉树找到目标控件的所有子控件）；
                List<WrapPanel> wraps = FindVisualChildren.FindVisualChildrens<WrapPanel>(userControl);
                WP = wraps[0];//获取环绕面板
                //2、清空环绕面板子元素
                if (WP != null)
                {
                    //移除所有子元素
                    WP.Children.Clear();
                }
                //3、加入加载层
                //JiaZai jiaZai=new JiaZai ();
                //jiaZai.Show();// 打开加载层

                //实例化菜单实体列表
                List<S_Menu> listMenus = new List<S_Menu>();
                //3、查询菜单信息                
                if (Cuisine == null)
                {
                    //（1）查询全部菜单（有效）
                    listMenus = (from tbMenu in myModel.S_Menu
                                 orderby tbMenu.cuisine
                                 where tbMenu.effective==true
                                 select tbMenu).ToList();
                }
                else
                {
                    //（2）查询菜单（有效） by 所属类型
                    listMenus = (from tbMenu in myModel.S_Menu
                                 orderby tbMenu.cuisine
                                 where tbMenu.effective == true && tbMenu.cuisine == Cuisine
                                 select tbMenu).ToList();
                }
                //4、循环菜单列表，生成菜单
                for (int i = 0; i < listMenus.Count; i++)
                {
                    //实例化菜单用户控件
                    Menu_UC menu = new Menu_UC();
                    //菜单用户控件，获取数据上下文
                    var viewModel = menu.DataContext as MenuUCViewModel;
                    //传递参数，（菜单ID）
                    viewModel.MenuID = listMenus[i].menuID;
                    WP.Children.Add(menu);
                }
                //jiaZai.Hide();//隐藏加载层
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.1.1 用户委托调用的方法
        /// </summary>
        private void RefreshMenu()
        {
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.2 查询全部菜单
        /// </summary>
        private void AllMenu()
        {
            //所属类型(菜单分类)
            Cuisine = null;
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.3 查询冷菜
        /// </summary>
        private void ColdDish()
        {
            //所属类型(菜单分类)
            Cuisine = "冷菜";
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.4 查询热菜
        /// </summary>
        private void HotFood()
        {
            //所属类型(菜单分类)
            Cuisine = "热菜";
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.5 查询糕点
        /// </summary>
        private void Pastry()
        {
            //所属类型(菜单分类)
            Cuisine = "糕点";
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.6 查询水果
        /// </summary>
        private void Fruit()
        {
            //所属类型(菜单分类)
            Cuisine = "水果";
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.7 查询酒水饮料
        /// </summary>
        private void Drinks()
        {
            //所属类型(菜单分类)
            Cuisine = "酒水饮料";
            //查询菜单信息
            SelectMenu(UC);
        }
        /// <summary>
        /// 3.8 打开新增窗口
        /// </summary>
        private void OpenEditWindow()
        {
            try
            {
                //实例化窗口
                MenuAddOrEditWindow window=new MenuAddOrEditWindow ();               
                //设置窗口数据上下文
                var viewModel = window.DataContext as MenuAddOrEditViewModel;
                //传递参数
                viewModel.MenuEntity = new S_Menu();
                viewModel.IsAdd = true;//新增
                //调用委托事件刷新菜单
                viewModel.RefreshEvent += RefreshMenu;
                //打开窗口
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        #endregion
    }
}
