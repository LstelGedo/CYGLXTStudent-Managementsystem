using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.Control;
using CYGLXTStudent.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 消费/充值记录
    /// 回调用到自定义的用户控件，前提条件：要在页面上引用用户控件
    /// </summary>
    public class XpenseTrackerViewModel : ViewModelBase
    {
        public XpenseTrackerViewModel()
        {
            //页面加载
            LoadedCommand = new RelayCommand<UserControl>(LoadedSelect, uc => uc != null);
            //消费记录查询         
            ConsumptionRecordCommand = new RelayCommand<UserControl>(SelectConsumption, uc => uc != null);
            //充值记录查询           
            TopUpRecordCommand = new RelayCommand(TopUpRecord);
            // 清空消费记录条件命令
            ClearConsumptionCommand = new RelayCommand(ClearConsumption);
            // 清空充值记录条件命令
            ClearTopUpCommand = new RelayCommand(ClearTopUp);
        }
        #region 【全局变量】
        /// <summary>
        /// （全局变量）实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        /// <summary>
        /// （全局变量）消费分页控件(封装分页控件PagingControl)
        /// </summary>
        public PagingControl ConsumptionPager;
        /// <summary>
        /// （全局变量）消费表格控件
        /// </summary>
        public DataGrid ConsumptionDG;
        #endregion

        #region 1、【属性】  
        private List<VipVo> topUpVIPVos;
        /// <summary>
        ///  充值VIP列表
        /// </summary>
        public List<VipVo> TopUpVIPVos
        {
            get { return topUpVIPVos; }
            set
            {
                if (topUpVIPVos != value)
                {
                    topUpVIPVos = value;
                    RaisePropertyChanged(() => TopUpVIPVos);
                }
            }
        }
        #region 条件筛选     

        private string consumptionStart;
        /// <summary>
        /// 消费开始时间
        /// </summary>
        public string ConsumptionStart
        {
            get { return consumptionStart; }
            set
            {
                if (consumptionStart != value)
                {
                    consumptionStart = value;
                    RaisePropertyChanged(() => ConsumptionStart);
                }
            }
        }


        private string consumptionEnd;
        /// <summary>
        /// 消费结束时间
        /// </summary>
        public string ConsumptionEnd
        {
            get { return consumptionEnd; }
            set
            {
                if (consumptionEnd != value)
                {
                    consumptionEnd = value;
                    RaisePropertyChanged(() => ConsumptionEnd);
                }
            }
        }


        private string topUpStart;
        /// <summary>
        /// 充值开始时间
        /// </summary>
        public string TopUpStart
        {
            get { return topUpStart; }
            set
            {
                if (topUpStart != value)
                {
                    topUpStart = value;
                    RaisePropertyChanged(() => TopUpStart);
                }
            }
        }


        private string topUpEnd;
        /// <summary>
        /// 充值结束时间
        /// </summary>
        public string TopUpEnd
        {
            get { return topUpEnd; }
            set
            {
                if (topUpEnd != value)
                {
                    topUpEnd = value;
                    RaisePropertyChanged(() => TopUpEnd);
                }
            }
        }
        #endregion

        #endregion
        #region 2、【命令】
        /// <summary>
        /// 2.1 页面加载命令
        /// </summary>
        public RelayCommand<UserControl> LoadedCommand { set; get; }
        /// <summary>
        /// 2.2 消费记录命令
        /// </summary>
        public RelayCommand<UserControl> ConsumptionRecordCommand { set; get; }
        /// <summary>
        /// 2.3 充值记录命令
        /// </summary>
        public RelayCommand TopUpRecordCommand { set; get; }
        /// <summary>
        /// 2.4 清空消费记录条件命令
        /// </summary>
        public RelayCommand ClearConsumptionCommand { set; get; }
        /// <summary>
        /// 2.5 清空充值记录条件命令
        /// </summary>
        public RelayCommand ClearTopUpCommand { set; get; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 加载方法
        /// </summary>
        /// <param name="userControl">用户控件自身</param>
        private void LoadedSelect(UserControl userControl)
        {
            //查询消费记录
            SelectConsumption(userControl);
            //查询收入记录
            TopUpRecord();
        }
        /// <summary>
        /// 3.2 查询消费记录
        /// </summary>
        /// <param name="userControl">用户控件自身</param>
        private void SelectConsumption(UserControl userControl)
        {
            try
            {
                //调用封装好的方法，从视觉树找到目标控件的所有子控件
                List<PagingControl> pagings = FindVisualChildren.FindVisualChildrens<PagingControl>(userControl);
                //分页控件
                ConsumptionPager = pagings[0];
                List<DataGrid> grids = FindVisualChildren.FindVisualChildrens<DataGrid>(userControl);
                //表格控件
                ConsumptionDG = grids[0];

                //linq 语句查询消费记录
                var query = from tbVIPCDetails in myModels.B_VIPConsumptionDetails
                            join tbVip in myModels.S_VIP on tbVIPCDetails.VIPID equals tbVip.VIPID
                            where tbVIPCDetails.consumptionType != "充值"
                            orderby tbVIPCDetails.VIPConsumptionDetailsID descending//倒叙排序
                            select new VipVo
                            {
                                name = tbVip.name,//姓名
                                cardNo = tbVip.cardNo,//卡号
                                phone = tbVip.phone,//电话
                                money = tbVIPCDetails.money,//金额
                                integral = tbVIPCDetails.integral,//积分
                                consumptionTiming = tbVIPCDetails.consumptionTiming,//消费时间
                                consumptionType = tbVIPCDetails.consumptionType,//消费类型
                                consumptionReason = tbVIPCDetails.consumptionReason,//资金来由 
                            };
                //判断条件是否为空
                if (!string.IsNullOrEmpty(ConsumptionStart)&& !string.IsNullOrEmpty(ConsumptionEnd))
                {
                    DateTime dtStart = Convert.ToDateTime(ConsumptionStart);
                    DateTime dtEnd = Convert.ToDateTime(ConsumptionEnd).AddDays(1);
                    //时间段数据
                    query = query.Where(o => o.consumptionTiming >= dtStart && o.consumptionTiming <= dtEnd);
                }
                DataTable dt = ListToDataTable.ListToDataTablen(query.ToList());
                //调用分页控件绑定表格数据
                ConsumptionPager.ShowPages(ConsumptionDG, dt);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 3.3 条件查询充值记录
        /// </summary>
        /// <param name="uc">页面用户控件</param>
        public void TopUpRecord()
        {

            try
            {
                #region 查询数据
                var query = from tbVIPCDetails in myModels.B_VIPConsumptionDetails
                            join tbVip in myModels.S_VIP on tbVIPCDetails.VIPID equals tbVip.VIPID
                            where tbVIPCDetails.consumptionType.Trim() == "充值"
                            orderby tbVIPCDetails.VIPConsumptionDetailsID descending//倒叙排序
                            select new VipVo
                            {
                                name = tbVip.name,//姓名
                                cardNo = tbVip.cardNo,//卡号
                                phone = tbVip.phone,//电话
                                money = tbVIPCDetails.money,//金额                           
                                consumptionTiming = tbVIPCDetails.consumptionTiming,//消费时间
                                consumptionType = tbVIPCDetails.consumptionType,//消费类型
                                consumptionReason = tbVIPCDetails.consumptionReason,//资金来由             

                            };
                //判断是否选择时间段
                if (!string.IsNullOrEmpty(TopUpStart) && !string.IsNullOrEmpty(TopUpEnd))
                {
                    DateTime dtStart = Convert.ToDateTime(TopUpStart);
                    DateTime dtEnd = Convert.ToDateTime(TopUpEnd);
                    DateTime dtDay = dtEnd.AddDays(+1);//天数加1
                    query = query.Where(o => o.consumptionTiming >= dtStart && o.consumptionTiming <= dtDay);
                }
                //列表属性赋值（绑定表格）
                TopUpVIPVos = query.ToList();

                #endregion
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 清空消费记录条件
        /// </summary>
        public void ClearConsumption()
        {
            ConsumptionStart = string.Empty;
            ConsumptionEnd = string.Empty;

        }
        /// <summary>
        /// 3.4 清空充值记录条件
        /// </summary>
        public void ClearTopUp()
        {
            TopUpStart = string.Empty;
            TopUpEnd = string.Empty;
        }
        #endregion
    }
}
