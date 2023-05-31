using CYGLXTStudent.Model;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.ViewModel.DiningTables;
using CYGLXTStudent.Views.DiningTables;
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
    /// 餐桌管理
    /// </summary>
    public class DiningTableViewModel:ViewModelBase
    {
        public DiningTableViewModel()
        {
            //加载
            LoadedCommand = new RelayCommand<UserControl>(SelectDingTable, uc => uc != null);
            //打开新增窗口
            AddCommand = new RelayCommand(OpenAddWindow);
            //全部
            AllCommand = new RelayCommand(AllDingTable);
            //一楼大厅
            FirstCommand = new RelayCommand(FirstDingTable);
            //二楼大厅
            SecondCommand = new RelayCommand(SecondDingTable);
            //包厢
            BoxCommand = new RelayCommand(BoxDingTable);
        }
        #region 【1、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        //声明三个全局变量：分别是用户控件、环绕面板、归属(餐桌位置)；
        /// <summary>
        /// 用户控件
        /// </summary>
        UserControl UC;
        /// <summary>
        /// 环绕面板(后期添加子控件【餐桌】)
        /// </summary>
        WrapPanel WP;
        /// <summary>
        /// 归属(餐桌位置)
        /// </summary>
        string Affiliation;
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 页面加载命令
        /// </summary>
        public RelayCommand<UserControl> LoadedCommand { get; set; }
        /// <summary>
        /// 添加餐桌命令
        /// </summary>
        public RelayCommand AddCommand { get; set; }
        /// <summary>
        /// 全部命令
        /// </summary>
        public RelayCommand AllCommand { get; set; }
        /// <summary>
        /// 一楼大厅命令
        /// </summary>
        public RelayCommand FirstCommand { get; set; }
        /// <summary>
        /// 二楼大厅命令
        /// </summary>
        public RelayCommand SecondCommand { get; set; }
        /// <summary>
        /// 包厢命令
        /// </summary>
        public RelayCommand BoxCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 批量生成餐桌
        /// </summary>
        /// <param name="userControl"></param>
        private void SelectDingTable(UserControl userControl)
        {

            /*
             思路：
            1、声明三个全局变量：分别是用户控件、环绕面板、归属(餐桌位置)；
            2、调用封装的获取子控件的方法获取环绕面板WrapPanel（用于生成餐桌）；
            3、判断环绕面板是否为空null,不为空，则移除所有元素；
            4、实例化餐桌列表S_DiningTable对象
            5、判断筛选条件属性是否为空，
            （1）是，按照桌号排序,查询全部数据
            （2）否，按照桌号排序,查询数据 by 根据归属 Affiliation
            6、判断餐桌数据，数据不为空，
                循环餐桌列表，实例化调用封装餐桌用户控件DiningTable_XS_UC，
                给环绕面板WrapPanel添加子元素，达到批量生成餐桌效果
                给餐桌用户控件DiningTable_XS_UC对应ViewModel传递参数餐桌ID=DiningTableID
                委托事件调用方法：刷新页面数据            
             */
            try
            {
                UC = userControl;//获取用户控件
                //1、声明三个全局变量：分别是用户控件、环绕面板、归属(餐桌位置)；
                //2、调用封装的获取子控件的方法获取环绕面板WrapPanel（用于生成餐桌）；
                List<WrapPanel> wraps = FindVisualChildren.FindVisualChildrens<WrapPanel>(userControl);
                WP = wraps[0];
                //3、判断环绕面板是否为空null,不为空，则移除所有元素；
                if (WP!=null)
                {
                    //则移除所有元素；
                    WP.Children.Clear();
                }
                //4、实例化餐桌列表S_DiningTable对象
                //
                List<S_DiningTable> list = new List<S_DiningTable>();
                //5、判断筛选条件属性是否为空（位置标志），
                if (Affiliation==null)
                {
                    //（1）是，按照桌号排序,查询全部数据
                    list = (from tb in myModel.S_DiningTable
                            orderby tb.tableNumber
                            select tb).ToList();
                }
                else
                {
                    //（2）否，条件筛选,按照桌号排序,查询数据 by 根据归属 Affiliation
                    list = (from tb in myModel.S_DiningTable
                            orderby tb.tableNumber
                            where tb.affiliation == Affiliation
                            select tb).ToList();
                }
                //6、判断餐桌数据，数据不为空，
                if (list.Count>0)
                {
                    //循环餐桌列表，实例化调用封装餐桌用户控件DiningTable_XS_UC，
                    for (int i = 0; i < list.Count; i++)
                    {
                        //实例化用户控件
                        DiningTable_XS_UC xs = new DiningTable_XS_UC();
                        //给环绕面板WrapPanel添加子元素，达到批量生成餐桌效果
                        WP.Children.Add(xs);
                        //给餐桌用户控件DiningTable_XS_UC对应ViewModel传递参数餐桌ID = DiningTableID
                        var viewModel=xs.DataContext as DiningTableXSViewModel;
                        viewModel.DiningTableID = list[i].diningTableID;
                        //委托事件调用方法：刷新页面数据
                        viewModel.RefreshDiningTable += Item_DiningTable;
                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.1 刷新餐桌状态(被委托事件调用)
        /// </summary>
        public void Item_DiningTable()
        {
            //调用查询餐桌方法
            SelectDingTable(UC);
        }
        /// <summary>
        /// 3.2 查询全部餐桌
        /// </summary>
        private void AllDingTable()
        {
            Affiliation = null;
            SelectDingTable(UC);
        }
        /// <summary>
        /// 3.3 查询一楼大厅餐桌
        /// </summary>
        private void FirstDingTable()
        {
            Affiliation = "A1";
            SelectDingTable(UC);
        }
        /// <summary>
        /// 3.4 查询二楼大厅餐桌
        /// </summary>
        private void SecondDingTable()
        {
            Affiliation = "A2";
            SelectDingTable(UC);
        }
        /// <summary>
        /// 3.5 查询包厢餐桌
        /// </summary>
        private void BoxDingTable()
        {
            Affiliation = "B1";
            SelectDingTable(UC);
        }
        /// <summary>
        /// 3.6 打开新增窗口
        /// </summary>
        private void OpenAddWindow()
        {
            try
            {
                //新增餐桌窗口
                Insert_DiningTable insert = new Insert_DiningTable();
                var viewModel=insert.DataContext as InsertTableViewModel;
                viewModel.CurrentDiningTableEntity = new S_DiningTable();
                viewModel.RefreshEvent += Item_DiningTable;//委托事件刷新数据
                insert.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
