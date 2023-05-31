using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;


namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 首页（数据统计）
    /// </summary>
    public class HomePageViewModel : ViewModelBase
    {
        public HomePageViewModel()
        {
            //加载         
            LoadedCommand = new RelayCommand(Loaded);
        }
        #region 0、【全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        #region 饼状图使用到
        /// <summary>
        /// 营业金额
        /// </summary>
        double CZ = 0;
        /// <summary>
        /// 会员消费
        /// </summary>
        double HYSL = 0;
        /// <summary>
        /// 会员充值
        /// </summary>
        double YYE = 0;
        #endregion 
        #endregion
        #region 1、【属性】        

        #region 页面属性绑定
        /// <summary>
        /// 字段：今日开台
        /// </summary>
        private string todayOpen;
        /// <summary>
        /// 属性：今日开台
        /// </summary>
        public string TodayOpen
        {
            get { return todayOpen; }
            set
            {
                if (todayOpen != value)
                {
                    todayOpen = value;
                    RaisePropertyChanged(() => TodayOpen);
                }
            }
        }
        /// <summary>
        /// 字段：已完成
        /// </summary>
        private string hasBeenCompleted;
        /// <summary>
        /// 属性：已完成
        /// </summary>
        public string HasBeenCompleted
        {
            get { return hasBeenCompleted; }
            set
            {
                if (hasBeenCompleted != value)
                {
                    hasBeenCompleted = value;
                    RaisePropertyChanged(() => HasBeenCompleted);
                }
            }
        }
        /// <summary>
        /// 字段：待关台
        /// </summary>
        private string stayOffTaiwan;
        /// <summary>
        /// 属性：待关台
        /// </summary>
        public string StayOffTaiwan
        {
            get { return stayOffTaiwan; }
            set
            {
                if (stayOffTaiwan != value)
                {
                    stayOffTaiwan = value;
                    RaisePropertyChanged(() => StayOffTaiwan);
                }
            }
        }
        /// <summary>
        /// 字段：客户人数
        /// </summary>
        private string customerNumber;
        /// <summary>
        /// 属性：客户人数
        /// </summary>
        public string CustomerNumber
        {
            get { return customerNumber; }
            set
            {
                if (customerNumber != value)
                {
                    customerNumber = value;
                    RaisePropertyChanged(() => CustomerNumber);
                }
            }
        }
        /// <summary>
        /// 字段：会员消费
        /// </summary>
        private string memberConsumption;
        /// <summary>
        /// 属性：会员消费
        /// </summary>
        public string MemberConsumption
        {
            get { return memberConsumption; }
            set
            {
                if (memberConsumption != value)
                {
                    memberConsumption = value;
                    RaisePropertyChanged(() => MemberConsumption);
                }
            }
        }
        /// <summary>
        /// 字段：今日开卡
        /// </summary>
        private string openCardToday;
        /// <summary>
        /// 属性：今日开卡
        /// </summary>
        public string OpenCardToday
        {
            get { return openCardToday; }
            set
            {
                if (openCardToday != value)
                {
                    openCardToday = value;
                    RaisePropertyChanged(() => OpenCardToday);
                }
            }
        }
        /// <summary>
        /// 字段：今日充值
        /// </summary>
        private string topUpToday;
        /// <summary>
        /// 属性：今日充值
        /// </summary>
        public string TopUpToday
        {
            get { return topUpToday; }
            set
            {
                if (topUpToday != value)
                {
                    topUpToday = value;
                    RaisePropertyChanged(() => TopUpToday);
                }
            }
        }
        /// <summary>
        /// 字段：营业收入
        /// </summary>
        private string operatingIncome;
        /// <summary>
        /// 属性：营业收入
        /// </summary>
        public string OperatingIncome
        {
            get { return operatingIncome; }
            set
            {
                if (operatingIncome != value)
                {
                    operatingIncome = value;
                    RaisePropertyChanged(() => OperatingIncome);
                }
            }
        }

        /// <summary>
        /// 字段：营业金额
        /// </summary>
        private ChartValues<double> turnoverToday;
        /// <summary>
        /// 属性：营业金额
        /// </summary>
        public ChartValues<double> TurnoverToday
        {
            get { return turnoverToday; }
            set
            {
                if (turnoverToday != value)
                {
                    turnoverToday = value;
                    RaisePropertyChanged(() => TurnoverToday);
                }
            }
        }
        /// <summary>
        /// 字段：会员消费统计
        /// </summary>
        private ChartValues<double> memberStatistics;
        /// <summary>
        /// 属性：会员消费统计
        /// </summary>
        public ChartValues<double> MemberStatistics
        {
            get { return memberStatistics; }
            set
            {
                if (memberStatistics != value)
                {
                    memberStatistics = value;
                    RaisePropertyChanged(() => MemberStatistics);
                }
            }
        }
        /// <summary>
        /// 字段：会员充值统计
        /// </summary>
        private ChartValues<double> topUpStatistics;
        /// <summary>
        /// 属性：会员充值统计
        /// </summary>
        public ChartValues<double> TopUpStatistics
        {
            get { return topUpStatistics; }
            set
            {
                if (topUpStatistics != value)
                {
                    topUpStatistics = value;
                    RaisePropertyChanged(() => TopUpStatistics);
                }
            }
        }

        #endregion
        #endregion
        #region 2、【命令】
        /// <summary>
        /// Loaded加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }

        #endregion
        #region 3、【方法】
        /// <summary>
        /// 3.1 方法引用
        /// </summary>
        private void Loaded()
        {

            //查询今日充值
            SelectVIPCZ();
            //查询今日会员消费
            SelectVIPXF();
            //查询今日营业额
            SelectAccountJRXF();

            //查询今日开台、客户人数
            SelectEatInRecord_YCES();
            //查询今日待关台
            SelectEatInRecord_DGT();
            //查询今日开台已完成
            SelectEatInRecord_YWC();
            //查询今日会员办卡
            SelectVIPJRBK();
            //饼状图
            GetPieSeriesData();
            //柱状图
            GetColunmSeriesData();
            //折线图
            GetLineSeriesData();


        }
        /// <summary>
        /// 3.2 查询今日充值
        /// </summary>
        private void SelectVIPCZ()
        {
            try
            {
                //1、获取当前日期
                DateTime dateTime = DateTime.Today;
                //2、查询充值信息
                var listTopUp = (from tbVIPCZ in myModels.B_VIPConsumptionDetails
                                 where tbVIPCZ.consumptionType == "充值"
                                 select new VipVo
                                 {
                                     money = tbVIPCZ.money,
                                     consumptionTiming = tbVIPCZ.consumptionTiming,
                                 }).ToList();
                //3、条件筛选当前数据
                listTopUp = listTopUp.Where(h => h.consumptionTiming >= dateTime).ToList();
                decimal decMoney = 0;//记录金额累加
                //4、循环表格(每天数据金额累加)
                for (int i = 0; i < listTopUp.Count; i++)
                {
                    decimal mome = Convert.ToDecimal(listTopUp[i].money);
                    decMoney += mome;//decMoney=decMoney+mome;
                }
                //四舍五入去两位小数
                decimal decRechargeMoney = Math.Round(decMoney, 2);
                //图表
                YYE = Convert.ToDouble(decRechargeMoney);
                //两种情况：
                if (decRechargeMoney >= 10000)
                {
                    //万元：获取金额/10000
                    //decRechargeMoney = decRechargeMoney / 10000;//原始写法
                    decRechargeMoney /= 10000;//使用复合分配写法

                    TopUpToday = decRechargeMoney.ToString() + "万元";
                }
                else
                {
                    //元
                    TopUpToday = decRechargeMoney.ToString() + "元";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 3.3 查询今日会员消费
        /// </summary>
        public void SelectVIPXF()
        {
            //获取当前日期
            DateTime dateTime = DateTime.Today;
            //查询消费记录
            var listConsumption = (from tbVIPCZ in myModels.B_VIPConsumptionDetails
                                   where tbVIPCZ.consumptionType == "消费"
                                   select new VipVo
                                   {
                                       money = tbVIPCZ.money,
                                       consumptionTiming = tbVIPCZ.consumptionTiming,
                                   }).ToList();
            //条件筛选：查询今日会员消费
            listConsumption = listConsumption.Where(h => h.consumptionTiming >= dateTime).ToList();
            decimal decMoney = 0;
            for (int i = 0; i < listConsumption.Count; i++)
            {
                decimal mome = Convert.ToDecimal((listConsumption[i].money));
                decMoney += mome;

            }
            //四色五入，保留2位小数
            decimal decMoneyRound = Math.Round(decMoney, 2);
            //图表使用
            HYSL = Convert.ToDouble(decMoneyRound);
            if (decMoneyRound >= 10000)
            {

                decMoneyRound /= 10000;//decMoneyRound = decMoneyRound / 10000;
                MemberConsumption = decMoneyRound.ToString() + "万元";
            }
            else
            {
                MemberConsumption = decMoneyRound.ToString() + "元";
            }
        }

        /// <summary>
        /// 3.4 查询今日营业额
        /// </summary>
        public void SelectAccountJRXF()
        {
            //当前日期
            DateTime dateTime = DateTime.Today;
            //查询收入记录
            var listAccount = (from tbAccount in myModels.S_AmountOfDetail
                           where tbAccount.reason == "会员消费" || tbAccount.reason == "餐饮消费"
                           where tbAccount.capitalPosition == "收入"
                           select new AccountVo
                           {
                               Money = tbAccount.money,
                               Time = tbAccount.time,
                           }).ToList();
            //条件筛选：获取当前日期的输入记录
            listAccount = listAccount.Where(h => h.Time >= dateTime).ToList();
           
            decimal decMoney = 0;
            for (int i = 0; i < listAccount.Count; i++)
            {
                decimal mome = Convert.ToDecimal((listAccount[i].Money));
                decMoney += mome;
            }
            decimal decMoneyRound = Math.Round(decMoney, 2);
            //图表
            CZ = Convert.ToDouble(decMoneyRound);
            if (decMoneyRound >= 10000)
            {
                decMoneyRound /= 10000; //decMoneyRound = decMoneyRound / 10000;
               OperatingIncome = decMoneyRound.ToString() + "万元";
               
            }
            else
            {
               OperatingIncome = decMoneyRound.ToString() + "元";                
            }
        }
        /// <summary>
        /// 查询今日开台、用餐人数（客户人数）
        /// </summary>
        public void SelectEatInRecord_YCES()
        {
            //餐桌消费记录表
            var listEatInRecord = (from tbEatInRecord in myModels.B_EatInRecord
                                   select new
                                   {
                                       tbEatInRecord.number,//每单的就餐人数
                                       tbEatInRecord.startTime,//就餐时间
                                   }).ToList();
            //获取当前日期
            DateTime dateTime = DateTime.Today;
            //条件筛选数据
            listEatInRecord = listEatInRecord.Where(h => h.startTime >= dateTime).ToList();
            //初始化变量
            int Number = 0;
            //循环赋值
            for (int i = 0; i < listEatInRecord.Count; i++)
            {
                //获取就餐人数
                int number = Convert.ToInt32((listEatInRecord[i].number));
                //累加
                Number += number;
            }
            CustomerNumber = Number.ToString() + "位";//客户人数
            TodayOpen = listEatInRecord.Count().ToString() + "台";//开台
        }

        /// <summary>
        /// 查询今日待关台
        /// </summary>
        public void SelectEatInRecord_DGT()
        {

            var EatInRecord = (from tbEatInRecord in myModels.B_EatInRecord
                               where tbEatInRecord.status == "待付款"
                               select new
                               {
                                   tbEatInRecord.number,
                                   tbEatInRecord.startTime,
                               }).ToList();

            DateTime dateTime = DateTime.Today;
            EatInRecord = EatInRecord.Where(h => h.startTime >= dateTime).ToList();
            StayOffTaiwan = EatInRecord.Count().ToString() + "台";
        }
        /// <summary>
        /// 查询今日开台已完成
        /// </summary>
        public void SelectEatInRecord_YWC()
        {
            //查询全部已付款信息
            var EatInRecord = (from tbEatInRecord in myModels.B_EatInRecord
                               where tbEatInRecord.status == "已付款"
                               select new
                               {
                                   tbEatInRecord.number,
                                   tbEatInRecord.startTime,
                               }).ToList();
            //获取当前日期
            DateTime dateTime = DateTime.Today;
            //条件筛选数据
            EatInRecord = EatInRecord.Where(h => h.startTime >= dateTime).ToList();
            HasBeenCompleted = EatInRecord.Count().ToString() + "台";
        }

        /// <summary>
        /// 查询今日会员办卡 
        /// </summary>
        public void SelectVIPJRBK()
        {
            //获取当前日期
            DateTime dateTime = DateTime.Today;
            //查询全部会员信息
            var listVIP = (from tbVip in myModels.S_VIP
                           select new VipVo
                           {
                               openingTime = tbVip.openingTime,
                           }).ToList();
            //条件筛选：获取会员办卡信息
            listVIP = listVIP.Where(h => h.openingTime >= dateTime).ToList();
            OpenCardToday = listVIP.Count().ToString() + "位";
        }
        #endregion

        #region 折线图（每年收入统计）
        /// <summary>
        /// 字段：折线图集合
        /// </summary>
        SeriesCollection lineSeriesCollection = new SeriesCollection();
        /// <summary>
        /// 属性：折线图集合
        /// </summary>
        public SeriesCollection LineSeriesCollection
        {
            get
            {
                return lineSeriesCollection;
            }

            set
            {
                lineSeriesCollection = value;
            }
        }
        /// <summary>
        /// 字段：折线图X坐标
        /// </summary>
        List<string> lineXLabels = new List<string>();
        /// <summary>
        /// 属性：折线图X坐标
        /// </summary>
        public List<string> LineXLabels
        {
            get
            {
                return lineXLabels;
            }

            set
            {
                lineXLabels = value;
            }
        }



        /// <summary>
        /// 折线图(每年收入统计)
        /// </summary>
        void GetLineSeriesData()
        {
            //清除数据
            lineSeriesCollection.Clear();
            //1、按全部年月统计数据（图表值）
            var list = (from b in myModels.S_AmountOfDetail
                        where b.capitalPosition == "收入"
                        group b by new { b.time.Value.Year } into t//年                                                                                
                        select new
                        {
                            CurDate = t.Key.Year + "年",//年份             
                            TotalMoney = t.Sum(p => p.money),//收入金额                          
                        }).ToList();
            //（标题）
            List<string> strx = new List<string>();
            List<double> stry = new List<double>();
            //循环数据
            for (int i = 0; i < list.Count(); i++)
            {
                //添加列表数据
                strx.Add(list[i].CurDate.Trim());
                stry.Add(Convert.ToDouble(list[i].TotalMoney));
            }
            /*
             * list<string>{2020,2021,2022 }
               list<double>{10100,12000,121212}
             */

            //4、绑定数据
            LineSeries lineseries = new LineSeries
            {
                DataLabels = true,//线条显示数值。
                Title = "每年收入统计",//设置标题
                Values = new ChartValues<double>(stry),//设置图表值。
                Stroke = System.Windows.Media.Brushes.LightSalmon,//线条的颜色
                Fill = System.Windows.Media.Brushes.PapayaWhip//线条下方颜色          
            };//设置Series的类型为Line类型，该类型提供了一些折线图的实现
            for (int i = 0; i < strx.Count; i++)
            {
                LineXLabels.Add(strx[i]);//折线图X坐标               
            }
            LineSeriesCollection.Add(lineseries);
        }

        #endregion
        #region 饼状图（今年月份收入）
        /// <summary>
        /// 字段： 饼图图集合
        /// </summary>
        SeriesCollection pieSeriesCollection = new SeriesCollection();
        /// <summary>
        /// 属性：饼图图集合
        /// </summary>
        public SeriesCollection PieSeriesCollection
        {
            get
            {
                return pieSeriesCollection;
            }

            set
            {
                pieSeriesCollection = value;
            }
        }
        /// <summary>
        /// 1、饼状图(数据)
        /// </summary>
        void GetPieSeriesData()
        {
            pieSeriesCollection.Clear();//清空数据
            //标题
            List<string> titles = new List<string> { "营业金额", "会员消费", "会员充值" };
            //数据
            List<double> pieValues = new List<double> { CZ, HYSL, YYE };
            //ChartValues<double> chartvalue = new ChartValues<double>();
            //使用丢弃_
            _ = new ChartValues<double>();

            for (int i = 0; i < titles.Count; i++)
            {
                //chartvalue = new ChartValues<double>();
                //chartvalue.Add(pieValues[i]);
                //简化集合初始化
                ChartValues<double> chartvalue = new ChartValues<double>
                {
                    pieValues[i]
                };

                PieSeries series = new PieSeries
                {
                    DataLabels = true,
                    Title = titles[i],
                    Values = chartvalue
                };
                PieSeriesCollection.Add(series);
            }
        }


        #endregion
        #region 柱状图（今日收入）
        /// <summary>
        /// 字段：柱状图集合
        /// </summary>
        SeriesCollection colunmSeriesCollection = new SeriesCollection();
        /// <summary>
        /// 属性：柱状图集合
        /// </summary>
        public SeriesCollection ColunmSeriesCollection
        {
            get
            {
                return colunmSeriesCollection;
            }

            set
            {
                colunmSeriesCollection = value;
            }
        }
        /// <summary>
        /// 字段：柱状图X坐标
        /// </summary>
        List<string> columnXLabels = new List<string>();
        /// <summary>
        /// 属性：柱状图X坐标
        /// </summary>
        public List<string> ColumnXLabels
        {
            get
            {
                return columnXLabels;
            }

            set
            {
                columnXLabels = value;
            }
        }
        /// <summary>
        /// 柱状图(数据)
        /// </summary>
        void GetColunmSeriesData()
        {
            //清除数据
            colunmSeriesCollection.Clear();
            //按年月统计
            var list = (from b in myModels.S_AmountOfDetail
                        where b.time.Value.Year == DateTime.Now.Year//今年
                        group b by new { b.time.Value.Year, b.time.Value.Month } into t//月份分组                        
                        select new
                        {
                            CurDate = t.Key.Year + "/" + t.Key.Month,//年/月
                            TotalMoney = t.Sum(p => p.money),//收入金额
                        }).ToList();

            //实例化列表
            List<string> strx = new List<string>();//X轴数据
            List<double> stry = new List<double>();//Y轴数据
            //循环数据
            for (int i = 0; i < list.Count(); i++)
            {
                //添加列表数据
                strx.Add(list[i].CurDate.Trim());
                stry.Add(Convert.ToDouble(list[i].TotalMoney));
            }

            ColumnSeries mycolunmseries = new ColumnSeries
            {
                DataLabels = true,
                Title = "今年每月账户收入统计",
                Values = new ChartValues<double>(stry)
            };
            for (int i = 0; i < strx.Count; i++)
            {
                ColumnXLabels.Add(strx[i]);
            }
            ColunmSeriesCollection.Add(mycolunmseries);

        }
        #endregion        
    }
}
