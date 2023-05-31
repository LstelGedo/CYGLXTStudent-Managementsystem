using CYGLXTStudent.Model;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.Views.Menus;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CYGLXTStudent.ViewModel.Menus
{
    /// <summary>
    /// 菜单用户控件
    /// </summary>
    public class MenuUCViewModel: ViewModelBase
    {
        public MenuUCViewModel()
        {
            //加载命令（绑定菜单信息）
            LoadedCommand = new RelayCommand(SelectMenu);
            //打开修改窗口
            UpdateCommand = new RelayCommand(OpenUpdateWindow);
            //菜单下架
            ShelvesCommand= new RelayCommand(ShelvesMenu);
        }
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 临时列表(用于临时记录菜单实体)
        /// </summary>
        DataTable dtMenu;
        #region 【1、属性】
        /// <summary>
        /// 私有字段：菜单ID
        /// </summary>
        private int menuID;
        /// <summary>
        /// 属性：菜单ID
        /// </summary>
        public int MenuID
        {
            get { return menuID; }
            set
            {
                if (menuID != value)
                {
                    menuID = value;
                    RaisePropertyChanged(() => MenuID);
                } 
            }
        }

        private BitmapImage imgMenu;
        /// <summary>
        /// 图片
        /// </summary>
        public BitmapImage ImgMenu
        {
            get { return imgMenu; }
            set
            {
                if (imgMenu != value)
                {
                    imgMenu = value;
                    RaisePropertyChanged(() => ImgMenu);
                }
            }
        }

        private string dishes;
        /// <summary>
        /// 菜名
        /// </summary>
        public string Dishes
        {
            get { return dishes; }
            set { 
                if (dishes != value)
                {
                    dishes = value;
                    RaisePropertyChanged(() => Dishes);
                }
            }
        }

        private string price;
        /// <summary>
        /// 售价
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
        #region 【2、命令】
        /// <summary>
        /// 2.0 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 2.1 打开修改窗口命令
        /// </summary>
        public RelayCommand UpdateCommand { get; set; }
        /// <summary>
        /// 2.2 菜单下架命令
        /// </summary>
        public RelayCommand ShelvesCommand { get; set; }        
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 查询菜单实体数据
        /// </summary>
        private void SelectMenu()
        {
            try
            {
               //判断属性菜单ID>0
                if (MenuID>0)
                {
                    //1、根据菜单ID查询菜单实体信息
                    var listMenu = (from tbMenu in myModel.S_Menu
                                where tbMenu.menuID == MenuID
                                select tbMenu).ToList();
                    //2、使用临时列表保存菜单
                    dtMenu = ListToDataTable.ListToDataTablen(listMenu);
                    //3、基目录下提取图片+ @"MenuPicture"
                    string strBD = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture";
                    if (!Directory.Exists(strBD))
                    {
                        //判断文件是否存在，不存在，就创建
                        Directory.CreateDirectory(strBD);
                    }
                    //文件路径
                    string strPath = strBD + "\\" + listMenu[0].picture.Trim();
                    //4、属性绑定
                    //图片
                    ImgMenu = GetImage.GetBitmapImage(strPath);
                    //菜名
                    Dishes = listMenu[0].dishes.Trim();
                    Price = listMenu[0].price.ToString()+"元";
                }

            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 打开修改菜单窗口
        /// </summary>
        private void OpenUpdateWindow()
        {
            try
            {
                //1、lambda表达式根据MenuID查询当前选中的数据菜单；
                S_Menu menuEntity = myModel.S_Menu.Find(MenuID);
                //3、实例化修改窗口MenuAddOrEditWindow；
                MenuAddOrEditWindow window = new MenuAddOrEditWindow();
                //4、获取参与数据绑定时的数据上下文ViewModel
                var viewModel=window.DataContext as MenuAddOrEditViewModel;
                //2、获取图片 = 基目录 + @"MenuPicture" + "\\" + 图片名；
                string strPath = AppDomain.CurrentDomain.BaseDirectory + @"MenuPicture\\"+ menuEntity.picture.Trim();
                //5、参数传递（选中实体、图片、图片路径、下拉框、修改标记、委托刷新）
                viewModel.MenuEntity = menuEntity;//修改回填实体
                viewModel.CaseCoverImage = GetImage.GetBitmapImage(strPath);
                viewModel.OldImage = strPath;//图片路径(旧路径)
                viewModel.Cuisine = menuEntity.cuisine;
                viewModel.IsAdd = false;//修改标志
                viewModel.RefreshEvent += SelectMenu;//委托调用刷新                
                window.ShowDialog();//打开窗口
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 菜单下架
        /// </summary>
        private void ShelvesMenu()
        {
            try
            {
                //1、显示提示框，提示用户是否下架菜单；
                if (MessageBox.Show("是否下架当前菜单？", "系统提示：", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                {
                    //2、筛选要下架的菜单；
                    S_Menu menu=myModel.S_Menu.Find(MenuID);
                    //3、修改菜品有效状态为fasle，执行修改操作
                    menu.effective = false;//无效
                    myModel.Entry(menu).State=System.Data.Entity.EntityState.Modified;
                    //4、保存更改，提示，刷新数据。
                    if (myModel.SaveChanges()>0)
                    {
                        MessageBox.Show(menu.dishes+"菜单下架成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(menu.dishes + "菜单下架失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
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
