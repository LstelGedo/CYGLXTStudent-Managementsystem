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

namespace CYGLXTStudent.ViewModel.Opens
{
    public class DiningTableViewModel:ViewModelBase
    {
        public DiningTableViewModel()
        {
            LoadedCommand = new RelayCommand(DiningTable);
            //鼠标移入
            MouseEnterCommand = new RelayCommand(GridMouseEnter);
            //鼠标移出
            MouseLeaveCommand = new RelayCommand(GridMouseLeave);
            //鼠标点击（开台、加菜/退菜/结账/会员结账、清台）
            MouseDownCommand = new RelayCommand(MouseDown);

        }
        #region [全局变量]
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        /// <summary>
        /// 定义委托（刷新餐桌）
        /// </summary>
        public delegate void RefreshDelegate();
        /// <summary>
        /// 定义事件（刷新餐桌）
        /// </summary>
        public event RefreshDelegate RefreshDiningTableEvent;
        /// <summary>
        /// 定义带参数订单ID的委托（刷新订单信息：表格数据、统计信息）
        /// </summary>
        /// <param name="text">订单ID</param>
        public delegate void PassingIDDelegate(int intOrderID);
        /// <summary>
        /// 定义带参数订单ID的事件（刷新订单信息：表格数据、统计信息）
        /// </summary>
        public event PassingIDDelegate PassingIDEvent;
        

        /// <summary>
        /// 加载层
        /// </summary>
        readonly JiaZai jiaZai = new JiaZai();
        #endregion
        #region 1、【属性】
        /// <summary>
        /// 自动属性：临时餐桌实体（用于记录选中餐桌）
        /// </summary>
        public S_DiningTable CurrentDiningTableEntity { get; set; }
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
        #region 页面属性绑定
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
        /// 字段：就餐人数
        /// </summary>
        private string number;
        /// <summary>
        /// 属性：就餐人数
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
        /// <summary>
        /// 字段：就餐金额(总金额)
        /// </summary>
        private string totalMoney;
        /// <summary>
        /// 属性：就餐金额(总金额)
        /// </summary>
        public string TotalMoney
        {
            get { return totalMoney; }
            set
            {
                if (totalMoney != value)
                {
                    totalMoney = value;
                    RaisePropertyChanged(() => TotalMoney);

                }
            }
        }
        /// <summary>
        /// 字段：就餐时长(持续时间)
        /// </summary>
        private string durationTime;
        /// <summary>
        /// 属性：就餐时长(持续时间)
        /// </summary>
        public string DurationTime
        {
            get { return durationTime; }
            set
            {
                if (durationTime != value)
                {
                    durationTime = value;
                    RaisePropertyChanged(() => DurationTime);

                }
            }
        }

        #endregion
        #endregion
        #region 2、【命令】
        /// <summary>
        /// 2.1 页面加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; private set; }
        /// <summary>
        /// 2.2 鼠标按下餐桌（开台命令）
        /// </summary>
        public RelayCommand MouseDownCommand { get; private set; }
        /// <summary>
        /// 2.3 鼠标移入餐桌（查询详情命令）
        /// </summary>
        public RelayCommand MouseEnterCommand { get; private set; }
        /// <summary>
        /// 2.4 鼠标移开餐桌（清除用餐信息命令）
        /// </summary>
        public RelayCommand MouseLeaveCommand { get; private set; }
        #endregion
        #region 【方法】
        /// <summary>
        /// 绑定餐桌信息
        /// </summary>
        private void DiningTable()
        {
            /*
            * 思路：
            *  1、根据餐桌ID查询餐桌信息
            *  2、提取单元格信息（桌子编号、状态、就餐人数/桌子容纳人数、总金额）
            *  3、（临时保存数据）把IList集合转换成DataTable数据类型
            */
            try
            {
                if (DiningTableID>0)
                {

                    //1、根据餐桌ID查询餐桌信息
                    var listDiningTable = (from tbDiningTable in myModels.S_DiningTable
                                           where tbDiningTable.diningTableID == DiningTableID//餐桌ID
                                           select tbDiningTable).ToList();
                    //2、提取单元格信息（桌子编号、状态、就餐人数/桌子容纳人数、总金额）
                    TableNumber = listDiningTable[0].tableNumber.Trim();//桌子编号
                    Status = listDiningTable[0].status.Trim(); //状态
                    //判读是否开台（是：餐桌人数=就餐人数；否：餐桌人数=默认）
                    if (listDiningTable[0].orderID > 0)
                    {
                        //订单ID
                        int intOrderID = Convert.ToInt32(listDiningTable[0].orderID);
                        //根据订单ID查询订单信息
                        var listOrder = (from tbOrder in myModels.B_Order
                                         where tbOrder.orderID == intOrderID
                                         select tbOrder).ToList();
                        //（1）餐桌人数=就餐人数
                        Number = listOrder[0].number.ToString().Trim() + "人";
                    }
                    else
                    {
                        //（2）餐桌人数=默认人数
                        Number = listDiningTable[0].number.ToString().Trim() + "人";
                    }

                    //总金额
                    decimal decTotalMoney = Convert.ToDecimal(listDiningTable[0].totalMoney);
                    //将小数值舍入到指定数量为2的小数位，并将中点值舍入到最接近的偶数。
                    decTotalMoney = Math.Round(decTotalMoney, 2);
                    if (decTotalMoney > 0)
                    {
                        TotalMoney = decTotalMoney.ToString();
                    }
                    //3、临时实体
                    CurrentDiningTableEntity = listDiningTable[0];
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 鼠标移入餐桌(查询就餐详情：菜单表格数据)
        /// </summary>
        private void GridMouseEnter()
        {
            try
            {
                //1、餐桌状态
                string Status = CurrentDiningTableEntity.status.Trim();
                //判断订单ID && 状态为"空桌"
                if (CurrentDiningTableEntity.orderID != null && Status != "空桌")
                {
                    //订单ID
                    int intOrderID = (int)CurrentDiningTableEntity.orderID;
                    //2、调用委托事件（（刷新订单信息：表格数据、统计信息））
                    PassingIDEvent(intOrderID);
                    //3、更新就餐时长
                    DurationTime = TimeOfDuration(intOrderID);
                }
                else
                {
                    //默认订单ID=0
                    int intOrderID = 0;
                    //调用委托事件（（刷新订单信息：表格数据、统计信息））
                    PassingIDEvent(intOrderID);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 计算就餐时长（持续时间）
        /// </summary>
        /// <param name="OrderID">订单ID</param>
        /// <returns></returns>
        public string TimeOfDuration(int OrderID)
        {
            try
            {
                //登录时长（持续时间）
                string strDurationTime = "";
                if (OrderID > 0)
                {
                    //根据订单ID查询订单信息
                    var listOrder = (from tbOr in myModels.B_Order
                                     where tbOr.orderID == OrderID
                                     select tbOr).ToList();
                    //就餐时间=返回序列中的最后一个元素
                    DateTime dateTime = Convert.ToDateTime(listOrder.LastOrDefault().orderTime);
                    DateTime dtNow = DateTime.Now;//当前时间
                    TimeSpan ts = dtNow - dateTime;//当前时间-就餐时间                   
                    if (ts.Days > 0)
                    {
                        //x天x小时x分钟x秒
                        strDurationTime = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
                    }
                    if (ts.Days == 0 && ts.Hours > 0)
                    {
                        //x小时x分钟x秒
                        strDurationTime = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
                    }
                    if (ts.Hours == 0 && ts.Minutes > 0)
                    {
                        //x分钟x秒
                        strDurationTime = ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
                    }
                    if (ts.Hours == 0 && ts.Minutes == 0)
                    {
                        //x秒
                        strDurationTime = ts.Seconds + "秒";
                    }
                }
                //返回登录时长
                return strDurationTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                //返回空字符串
                return string.Empty;
            }
        }
        /// <summary>
        /// 3.3 鼠标移出（清除信息）
        /// </summary>
        private void GridMouseLeave()
        {
            //清空就餐时长
            DurationTime = String.Empty;
            //默认订单ID=0
            int intOrderID = 0;
            //调用委托事件(刷新餐桌)
            PassingIDEvent(intOrderID);
        }
        /// <summary>
        /// 3.4 鼠标点击（开台、加菜/退菜/结账/会员结账、清台）
        /// </summary>
        private void MouseDown()
        {
            try
            {
                if (CurrentDiningTableEntity!=null)
                {
                    //*餐桌三种状态：
                    string strStatus = CurrentDiningTableEntity.status.Trim();
                    if (strStatus== "空桌")
                    {
                        //*1、空桌：
                        //① 实例化“开台”窗口：Order_KT
                        Order_KT kT = new Order_KT();
                        //② 设置窗口数据上下文为：OrderKTViewModel
                        OrderKTViewModel viewModel = kT.DataContext as OrderKTViewModel;
                        //③ 传递参数餐桌ID：diningTableID、餐桌人数：number
                        viewModel.DiningTableID =(int)CurrentDiningTableEntity.diningTableID;
                        viewModel.Number =Convert.ToInt32(CurrentDiningTableEntity.number);
                        //④ 打开窗口
                        kT.ShowDialog();
                        //⑤ 委托事件刷新餐桌                      
                        RefreshDiningTableEvent?.Invoke();
                    }
                    if (strStatus== "待付款")
                    {
                        //* 2、待付款：
                        //① 实例化“结账”窗口：Dining_Options
                        Dining_Options options=new Dining_Options ();
                        //② 设置窗口数据上下文为：DiningOptionsViewModel
                        DiningOptionsViewModel viewModel = options.DataContext as DiningOptionsViewModel;
                        //③ 传递参数订单ID：orderID
                        viewModel.OrderID =(int) CurrentDiningTableEntity.orderID;
                        //④ 打开窗口
                        options.ShowDialog();
                        //⑤ 委托事件刷新餐桌
                        RefreshDiningTableEvent?.Invoke();
                    }
                    if (strStatus== "已付款")
                    {
                        //* 3、已付款：提示是否清理餐桌
                        if (MessageBox.Show("是否要清理餐桌?", "系统提示：", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                        {
                            //① 根据餐桌ID查询餐桌信息，
                            S_DiningTable table = myModels.S_DiningTable.Find(CurrentDiningTableEntity.diningTableID);
                            //② 修改餐桌表：S_DiningTable（修改餐桌状态为“空桌”）
                            table.status = "空桌";
                            myModels.Entry(table).State=System.Data.Entity.EntityState.Modified;
                            if (myModels.SaveChanges()>0)
                            {                               
                                MessageBox.Show("清理餐桌成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                //③ 委托事件刷新餐桌
                                RefreshDiningTableEvent?.Invoke();
                            }
                            else
                            {
                                MessageBox.Show("清理餐桌失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                        }
                    }
                }
                else
                {
                    MessageBox.Show("请选中餐桌！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
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
