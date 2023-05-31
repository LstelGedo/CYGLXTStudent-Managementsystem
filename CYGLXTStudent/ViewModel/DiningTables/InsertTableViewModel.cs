using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CYGLXTStudent.ViewModel.DiningTables
{
    /// <summary>
    /// 新增餐桌
    /// </summary>
    public class InsertTableViewModel : ViewModelBase
    {
        public InsertTableViewModel()
        {
            //餐桌编号（自动生成）
            SelectionChangedCommand = new RelayCommand<ComboBox>(CreateTableNumber, cb => cb != null);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, wd => wd != null);
            //保存
            SaveCommand = new RelayCommand<Window>(SaveDingTable, wd => wd != null);
        }
        #region 【全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 定义委托（用于刷新主页面数据）
        /// </summary>
        public delegate void RefreshDelegate();
        /// <summary>
        /// 定义事件（用于刷新主页面数据）
        /// </summary>
        public event RefreshDelegate RefreshEvent;
        #endregion
        #region 【属性】
        /// <summary>
        /// 字段：餐桌实体
        /// </summary>
        private S_DiningTable currentDiningTableEntity;
        /// <summary>
        /// 属性：餐桌实体
        /// </summary>
        public S_DiningTable CurrentDiningTableEntity
        {
            get { return currentDiningTableEntity; }
            set
            {
                if (currentDiningTableEntity != value)
                {
                    currentDiningTableEntity = value;
                    RaisePropertyChanged(() => CurrentDiningTableEntity);

                }
            }
        }
        /// <summary>
        /// 字段：餐桌编号（自动生成）
        /// </summary>
        public string tableNumber;
        /// <summary>
        /// 属性：餐桌编号（自动生成）
        /// </summary>
        public string TableNumber
        {
            get { return tableNumber; }
            set
            {
                if (tableNumber != value)
                {
                    tableNumber = value;
                    RaisePropertyChanged(() => TableNumber);

                }
            }
        }

        #endregion
        #region 【命令】
        /// <summary>
        /// 2.1 下拉框改变命令
        /// </summary>
        public RelayCommand<ComboBox> SelectionChangedCommand { get; private set; }
        /// <summary>
        /// 2.2 定义保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 2.3 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 选择归属(餐桌分类)，自动生成餐桌编号
        /// 自动生成餐桌编号
        /// </summary>
        /// <param name="box"></param>
        private void CreateTableNumber(ComboBox box)
        {
            //编号=前缀+编号（(1)、没有，从001开始;(2)、最大编号+1）
            try
            {
                //1、获取餐桌位置（用来决定餐桌编号生成）
                string strAffiliation = Convert.ToString(box.SelectedValue);
                string strTableNumber = "";//餐桌编号
                string strPrefix = "";//餐桌编号：前缀
                if (strAffiliation!="")
                {
                    if (strAffiliation.Contains("一楼大厅"))
                    {
                        strAffiliation = "A1";
                        strPrefix = "A";//前缀
                    }
                    else if (strAffiliation.Contains("二楼大厅"))
                    {
                        strAffiliation = "A2";
                        strPrefix = "A";//前缀
                    }
                    else if (strAffiliation.Contains("包厢"))
                    {
                        strAffiliation = "B1";
                        strPrefix = "B";//前缀
                    }
                    //2、根据选泽下拉框值获取当前类型的最大编号
                    var list = (from tb in myModel.S_DiningTable
                                where tb.affiliation == strAffiliation
                                select tb).ToList();
                    if (list.Count <= 0)
                    {
                        // (1)、没有，从001开始
                        strTableNumber = strAffiliation + "001";//前缀 + 编号
                        CurrentDiningTableEntity.affiliation = strAffiliation;//归属（餐桌分类）
                        CurrentDiningTableEntity.tableNumber = strTableNumber;// 桌子编号
                        TableNumber = strTableNumber; //桌子编号赋值
                    }
                    else
                    {
                        //（2）餐桌编号：最大编号 + 1(
                        //三种情况：
                        //①、最大编号个位数="00"+编号；
                        //②、最大编号十位数="0"+编号；
                        //①、最大编号百位数=编号；

                        //)
                        string strBigTableNumber = (list.Last()).tableNumber;
                        string strNumber = strBigTableNumber.Substring(1);//截取编号
                        decimal decTableNumber = 0;
                        if (strBigTableNumber != "")
                        {
                            decTableNumber = Convert.ToDecimal(strNumber) + 1;//最大编号 + 1
                        }
                        CurrentDiningTableEntity.affiliation = strAffiliation;//归属（餐桌分类）
                        CurrentDiningTableEntity.tableNumber = strPrefix + decTableNumber;// 桌子编号=前缀+编号
                        //桌子编号赋值
                        TableNumber = strPrefix + decTableNumber;
                        
                    }

                }



            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 关闭窗口
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
        /// 3.3 保存新增
        /// </summary>
        /// <param name="win"></param>
        private void SaveDingTable(Window win)
        {
            try
            {
                //1、判断必填项不为空
                if (string.IsNullOrEmpty(CurrentDiningTableEntity.affiliation))
                {
                    MessageBox.Show("请选择餐桌归属！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (CurrentDiningTableEntity.number==null)
                {
                    MessageBox.Show("请输入人数！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //2、实例化实体并赋值；
                if (string.IsNullOrEmpty(CurrentDiningTableEntity.tableNumber))
                {
                    MessageBox.Show("餐桌编号为空！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //3、执行新增操作；
                S_DiningTable table = new S_DiningTable
                {
                    tableNumber= CurrentDiningTableEntity.tableNumber,
                    number= CurrentDiningTableEntity.number,
                    status= "空桌",
                    affiliation= CurrentDiningTableEntity.affiliation,                    
                };
                myModel.S_DiningTable.Add(table);
                //4、保存成功，提示，调用委托事件刷新。
                if (myModel.SaveChanges()>0)
                {
                    MessageBox.Show(CurrentDiningTableEntity.tableNumber+"餐桌新增成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (RefreshEvent!=null)
                    {
                        RefreshEvent();
                        win.Close();
                    }
                }
                else
                {
                    MessageBox.Show(CurrentDiningTableEntity.tableNumber + "餐桌新增失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
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
