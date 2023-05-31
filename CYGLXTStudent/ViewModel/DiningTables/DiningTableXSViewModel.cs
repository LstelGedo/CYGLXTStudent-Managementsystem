using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel.DiningTables
{
    /// <summary>
    /// 餐桌用户控件
    /// </summary>
    public class DiningTableXSViewModel : ViewModelBase
    {
        public DiningTableXSViewModel()
        {
            LoadedCommand = new RelayCommand(SelectDingTableByID);
            //删除餐桌
            DeleteCommand = new RelayCommand(DeleteDingTable);
        }
        #region 【全局变量】
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void RefreshDelegate();
        /// <summary>
        /// 定义事件
        /// </summary>
        public event RefreshDelegate RefreshDiningTable;
        #endregion


        #region 【属性】
        /// <summary>
        /// 字段：餐桌ID
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
        /// 字段：餐桌编号
        /// </summary>
        private string tableNumber;
        /// <summary>
        /// 属性：餐桌编号
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
        /// <summary>
        /// 字段：餐桌状态
        /// </summary>
        private string status;
        /// <summary>
        /// 属性：餐桌状态
        /// </summary>
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged(() => Status);

                }
            }
        }
        /// <summary>
        /// 字段：餐桌容纳人数
        /// </summary>
        private string number;
        /// <summary>
        /// 属性：餐桌容纳人数
        /// </summary>
        public string Number
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
        #region 【命令】
        /// <summary>
        /// 页面加载命令
        /// </summary>
        public ICommand LoadedCommand { get; private set; }
        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand DeleteCommand { get; private set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 根据餐桌主键ID查询绑定的餐桌信息
        /// </summary>
        private void SelectDingTableByID()
        {
            /*
             思路：
            1、判断传递过来的餐桌ID是否大于0，是则继续，否则不做操作。
            2、根据餐桌ID查询餐桌实体；
            3、属性赋值=餐桌实体的餐桌编号、餐桌状态
            4、判断订单ID不为空,餐桌人数：两种情况
               ① 餐桌被预定：绑定预订实际人数；
                    从查询餐桌实体获取预订订单ID；
                    根据订单ID获取订单的预订人数；
                    餐桌人数属性=预订人数；
               ② 空桌，默认绑定餐桌容纳人数；
              */
            try
            {
                //1、判断传递过来的餐桌ID是否大于0，是则继续，否则不做操作。
                if (DiningTableID>0)
                {
                    //2、根据餐桌ID查询餐桌实体；
                    var dingTable = (from tb in myModel.S_DiningTable
                                     where tb.diningTableID == DiningTableID
                                     select tb).Single();
                    //3、属性赋值 = 餐桌实体的餐桌编号、餐桌状态
                    TableNumber = dingTable.tableNumber;
                    Status = dingTable.status;
                    //4、判断订单ID不为空,餐桌人数：两种情况
                    if (dingTable.orderID!=null)
                    {
                        //① 餐桌被预定：绑定预订实际人数；
                        //从查询餐桌实体获取预订订单ID；
                        var orderNumber = (from tbOrder in myModel.B_Order
                                           where tbOrder.orderID == dingTable.orderID
                                           select tbOrder).Single().number;
                        //根据订单ID获取订单的预订人数；
                        int intNumber = Convert.ToInt32(orderNumber);
                        //餐桌人数属性 = 预订人数；
                        Number = intNumber.ToString();
                    }
                    else
                    {
                        //② 空桌，默认绑定餐桌容纳人数；
                        Number = dingTable.number.ToString();
                    }
                }

            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 删除餐桌
        /// </summary>
        private void DeleteDingTable()
        {
            try
            {
                if (DiningTableID>0)
                {
                    //1、根据餐桌ID 查询删除餐桌信息
                    var deleteDingTable = (from tb in myModel.S_DiningTable
                                           where tb.diningTableID == DiningTableID
                                           select tb).Single();
                    //2、判断餐桌订单ID是否为空，为空，直接删除，不为空，提示；
                    if (deleteDingTable.orderID!=null)
                    {
                        //不为空，提示；
                        MessageBox.Show("餐桌使用中，无法删除！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    else
                    {
                        //为空，直接删除
                        myModel.S_DiningTable.Remove(deleteDingTable);
                       if (myModel.SaveChanges()>0) 
                        {
                            MessageBox.Show(deleteDingTable.tableNumber+"餐桌删除成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (RefreshDiningTable!=null)
                            {
                                RefreshDiningTable();
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show(deleteDingTable.tableNumber + "餐桌删除失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
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
