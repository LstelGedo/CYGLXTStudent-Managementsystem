using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 账户信息
    /// </summary>
    public class AccountViewModel: ViewModelBase
    {
        public AccountViewModel()
        {
            //加载
            LoadedCommand = new RelayCommand<UserControl>(Loaded, uc => uc != null);
            //搜索
            SelectIncomeCommand = new RelayCommand(SelectIncomeRecord);
            //清空条件
            CleanIncomeCommand = new RelayCommand(CleanIncome);
            #region 【收入分页】
            IncomeFirstCommand = new RelayCommand(IncomeFirst);//首页
            IncomeLastCommand = new RelayCommand(IncomeLast);//上一页
            IncomeNextCommand = new RelayCommand(IncomeNext);//下一页        
            IncomeEndCommand = new RelayCommand(IncomeEnd);//尾页
            IncomeGoCommand = new RelayCommand(IncomeGo);//跳转
            #endregion           
            //支出情况条件查询
            SelectSpendingCommand = new RelayCommand(SelectSpendingRecord);
            //支出清空条件按钮
            CleanSpendingCommand = new RelayCommand(CleanSpendingConditions);
            #region 【支出分页】
            SpendingFirstCommand = new RelayCommand(SpendingFirst);//首页
            SpendingLastCommand = new RelayCommand(SpendingLast);//上一页
            SpendingNextCommand = new RelayCommand(SpendingNext); //下一页
            SpendingEndCommand = new RelayCommand(SpendingEnd);//尾页
            SpendingGoCommand = new RelayCommand(SpendingGo);//跳转
            #endregion
        }
        #region 一、【总金额、冻结金额、可用金额】
        #region 【1、属性】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModels = new CYGLXTEntities();
        /// <summary>
        /// 临时列表：数据源（收入）
        /// </summary>
        List<AccountVo> listIncomes = new List<AccountVo>();
        /// <summary>
        /// 临时列表：页面显示的数据（收入）
        /// </summary>
        List<AccountVo> listIncomeRecord = new List<AccountVo>();
        /// <summary>
        /// 临时列表：数据源（支出）
        /// </summary>
        List<AccountVo> listSpendings = new List<AccountVo>();
        /// <summary>
        /// 临时列表：页面显示的数据（支出）
        /// </summary>
        List<AccountVo> listSpendingRecord = new List<AccountVo>();


        /// <summary>
        /// 字段：总金额（用于绑定页面）
        /// </summary> 
        private string totalMoney;
        /// <summary>
        /// 属性：总金额（用于绑定页面）
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
        /// 字段：可用金额（用于绑定页面）
        /// </summary>
        private string avail;
        /// <summary>
        /// 属性：可用金额（用于绑定页面）
        /// </summary>
        public string Avail
        {
            get { return avail; }
            set
            {
                if (avail != value)
                {
                    avail = value;
                    RaisePropertyChanged(() => Avail);

                }
            }
        }
        /// <summary>
        /// 字段：冻结金额（用于绑定页面）
        /// </summary>
        private string accountFrozen;
        /// <summary>
        /// 属性：冻结金额（用于绑定页面）
        /// </summary>
        public string AccountFrozen
        {
            get { return accountFrozen; }
            set
            {
                if (accountFrozen != value)
                {
                    accountFrozen = value;
                    RaisePropertyChanged(() => AccountFrozen);

                }
            }
        }
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand<UserControl> LoadedCommand { get; set; }
        #endregion
        #region 【3、方法】
        private void Loaded(UserControl userControl)
        {
            //账户信息绑定
            SelectAccount();
            //收入记录：表格绑定
            SelectIncomeRecord();
            //支出记录：表格绑定
            SelectSpendingRecord();

        }
        /// <summary>
        ///  3.1 查询账号金额（绑定头部：总金额、可用金额、冻结金额）
        /// </summary>
        private void SelectAccount()
        {
            /*
             思路：
            1、linq查询账号基本信息列表（账号ID、总金额、可用金额、冻结金额）
            2、判断列表总数>0
             (1)提取总金额，Math.Round(金额,取小数点位数)四舍五入取两位小数点
                ①判断总金额>10000，则显示为万元单位，否则直接显示
             (2)提取可用金额，Math.Round(金额,取小数点位数)四舍五入取两位小数点
                ①判断可用金额>10000，则显示为万元，否则直接显示
             (3)提取冻结金额，Math.Round(金额,取小数点位数)四舍五入取两位小数点
                ①判断冻结金额>10000，则显示为万元，否则直接显示
             */
            try
            {
                //查询账号信息
                var listAccount = (from tbAccount in myModels.S_Account
                                   select new AccountVo
                                   {
                                       accountID = tbAccount.accountID,//账号ID
                                       TotalMoney = tbAccount.TotalMoney,//总金额
                                       Avail = tbAccount.Avail,//可用金额
                                       AccountFrozen = tbAccount.AccountFrozen,//冻结金额
                                   }).ToList();
                if (listAccount.Count()>0)
                {
                    //(1)提取总金额，Math.Round(金额, 取小数点位数)四舍五入取两位小数点
                    decimal decTotalMoney = (decimal)listAccount[0].TotalMoney;
                    decimal intTotalMoney = Math.Round(decTotalMoney, 0); 
                    //①判断总金额 > 10000，则显示为万元单位，否则直接显示
                    if (intTotalMoney>10000)
                    {
                        intTotalMoney /= 10000;//intTotalMoney=intTotalMoney/10000
                        TotalMoney = intTotalMoney.ToString()+"万元";
                    }
                    else
                    {
                        TotalMoney = intTotalMoney.ToString()+"元";
                    }
                    //（2）可用金额
                    decimal decAvails = Convert.ToDecimal(listAccount[0].Avail);
                    //取整数
                    decimal decAvail = Math.Round(decAvails, 0);//四舍五入（0位小数点）
                    if (decAvail > 10000)
                    {
                        decAvail /= 10000;//decAvail=decAvail/10000
                        Avail = decAvail.ToString() + "万元";
                    }
                    else
                    {
                        Avail = decAvail.ToString() + "元";
                    }
                    //（3）冻结金额
                    decimal decFrozen = Convert.ToDecimal(listAccount[0].AccountFrozen);
                    //取整数
                    decimal decAccountFrozen = Math.Round(decFrozen, 0);//四舍五入（0位小数点）

                    if (decAccountFrozen > 10000)
                    {
                        decAccountFrozen /= 10000;//decAccountFrozen=decAccountFrozen/10000
                        AccountFrozen = decAccountFrozen.ToString() + "万元";
                    }
                    else
                    {
                        AccountFrozen = decAccountFrozen.ToString() + "元";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
        #endregion
        #region 二、【收入记录】
        #region 【1、属性】
        //页面绑定（日期、表格+分页、局部变量操作）
        #region 条件筛选属性（绑定页面）
        /// <summary>
        /// 字段：收入时间（开始）
        /// </summary>
        private string incomeTimeStart;
        /// <summary>
        /// 属性：收入时间（开始）
        /// </summary>
        public string IncomeTimeStart
        {
            get { return incomeTimeStart; }
            set
            {
                if (incomeTimeStart != value)
                {
                    incomeTimeStart = value;
                    RaisePropertyChanged(() => IncomeTimeStart);

                }
            }
        }
        /// <summary>
        /// 字段：收入时间（结束）
        /// </summary>
        private string incomeTimeEnd;
        /// <summary>
        /// 属性：收入时间（结束）
        /// </summary>
        public string IncomeTimeEnd
        {
            get { return incomeTimeEnd; }
            set
            {
                if (incomeTimeEnd != value)
                {
                    incomeTimeEnd = value;
                    RaisePropertyChanged(() => IncomeTimeEnd);

                }
            }
        }
        #endregion
        #region 分页属性（绑定页面）
        /// <summary>
        /// 字段：最大页（收入）
        /// </summary>
        private string incomeMaxPage;
        /// <summary>
        /// 属性：最大页（收入）
        /// </summary>
        public string IncomeMaxPage
        {
            get { return incomeMaxPage; }
            set
            {
                if (incomeMaxPage != value)
                {
                    incomeMaxPage = value;
                    RaisePropertyChanged(() => IncomeMaxPage);

                }
            }
        }
        /// <summary>
        /// 字段：总条数（收入）
        /// </summary>
        private string incomeAllCount;
        /// <summary>
        /// 属性：总条数（收入）
        /// </summary>
        public string IncomeAllCount
        {
            get { return incomeAllCount; }
            set
            {
                if (incomeAllCount != value)
                {
                    incomeAllCount = value;
                    RaisePropertyChanged(() => IncomeAllCount);

                }
            }
        }
        /// <summary>
        /// 字段：当前页（收入）
        /// </summary>
        private string incomeCurrentPage;
        /// <summary>
        /// 属性：当前页（收入）
        /// </summary>
        public string IncomeCurrentPage
        {
            get { return incomeCurrentPage; }
            set
            {
                if (incomeCurrentPage != value)
                {
                    incomeCurrentPage = value;
                    RaisePropertyChanged(() => IncomeCurrentPage);

                }
            }
        }
        /// <summary>
        /// 字段：跳转页（收入记录）
        /// </summary>
        private string incomeJumpPage;
        /// <summary>
        /// 属性：跳转页（收入记录）
        /// </summary>
        public string IncomeJumpPage
        {
            get { return incomeJumpPage; }
            set
            {
                if (incomeJumpPage != value)
                {
                    incomeJumpPage = value;
                    RaisePropertyChanged(() => IncomeJumpPage);

                }
            }
        }
        /// <summary>
        /// 字段：收入记录（用于绑定表格）
        /// </summary>
        private ObservableCollection<AccountVo> incomeRecord;
        /// <summary>
        /// 属性：收入记录（用于绑定表格）
        /// </summary>
        public ObservableCollection<AccountVo> IncomeRecord
        {
            get { return incomeRecord; }
            set
            {
                if (incomeRecord != value)
                {
                    incomeRecord = value;
                    RaisePropertyChanged(() => IncomeRecord);

                }
            }
        }
        #endregion
        #region 分页参数 收入(分页操作使用局部变量)
        //加载页面
        //！！！！！！！！！！！！！该分页方法要求查询出来的数据必须大于或等于1条！！！！！！！！！！！！
        /// <summary>
        /// (全局变量)设置每一页要显示的条数（收入）
        /// </summary>
        public int showCount;
        /// <summary>
        ///  (全局变量)分页后的余数（收入）
        /// </summary>
        public int remainder;
        /// <summary>
        ///  (全局变量)总页数（最后一页）（收入）
        /// </summary>
        public int allPage;
        /// <summary>
        ///  (全局变量)首页（收入）
        /// </summary>
        public int firstPageSize = 1;
        /// <summary>
        ///  (全局变量)当前页（收入）
        /// </summary>
        public int nowPageSize;
        /// <summary>
        ///  (全局变量)最后一条数据的索引（收入）
        /// </summary>
        public int theLastDataIndex;
        /// <summary>
        ///  (全局变量)最后一页（收入）
        /// </summary>
        public int thelastPageSize;
        /// <summary>
        ///  (全局变量)最后一页数据的开始索引（收入）
        /// </summary>
        public int thelastPageStartIndex;
        /// <summary>
        ///  (全局变量)所有的数据的条数（收入）
        /// </summary>
        public int count;
        /// <summary>
        ///  (全局变量)初始行号（收入）
        /// </summary>
        public int rowNumber;

        #endregion
        #endregion
        #region 【2、命令】
        #region 收入分页命令
        /// <summary>
        /// 首页（收入）命令
        /// </summary>
        public ICommand IncomeFirstCommand { get; set; }
        /// <summary>
        /// 上一页（收入）命令
        /// </summary>
        public ICommand IncomeLastCommand { get; set; }
        /// <summary>
        /// 下一页（收入）命令
        /// </summary>
        public ICommand IncomeNextCommand { get; set; }
        /// <summary>
        /// 尾页（收入）命令
        /// </summary>
        public ICommand IncomeEndCommand { get; set; }
        /// <summary>
        /// 跳转（收入）命令
        /// </summary>
        public ICommand IncomeGoCommand { get; set; }
        #endregion
        /// <summary>
        /// 收入情况条件查询
        /// </summary>
        public ICommand SelectIncomeCommand { get; set; }
        /// <summary>
        /// 收入清空按钮
        /// </summary>
        public ICommand CleanIncomeCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 查询账户金额收入记录（绑定收入表格）
        /// </summary>
        private void SelectIncomeRecord()
        {

            //已知条件：总页数Count、页面默认显示条数showCount = 13、、
            //最大索引theLastDataIndex = Count - 1
            //要求实现功能、绑定页数数据：
            //存在两种情况：Count比较showCount
            //1、Count <= showCount刚好一页显示完成；
            //一种：直接绑定
            //2、Count > showCount超出一页（多页）
            //1页 13 % 13 = 0 1
            //26 % 13 = 0 2
            //14 % 13 = 1  1 + 1 = 2

            /*
           1、清空页面显示数据临时列表listIncomeRecord；
           2、设置收入每一页要显示的条数showCount（默认13）
           3、linq条件查询收入数据并赋值给临时列表listIncomes,日期条件筛选
           4、count总数=临时列表总数；
           5、判断总数>0，是：
             （1）、for循环列表，自定义序号
             （2）、获取分页后余数remainder=总数%显示的条数showCount
             （3）、最后一条数据的索引=count总数-1
             （4）、判断：count总数<设置每一页要显示的条数showCount
                  ①是：allPage总页数=1，remainder分页后的余数=count总数
                  ②否，存在两种情况
                   一是：余数remainder==0,总页数=（总数/每页显示条数）；
                   二是：余数>0,总页数=（总数/每页显示条数）+1；
                   最后：for循环count总数,列表添加数据listIncomeRecord.（listIncomes[i]）
             （5）、分页属性赋值：
                  nowPageSize当前页=1，
                  最后一页数据的开始索引thelastPageStartIndex=（allPage总页数-1）*显示的条数showCount
                  最大页IncomeMaxPage=总页数allPage
                  总条数IncomeAllCount=count总数；
                  当前页IncomeCurrentPage=firstPageSize首页
                  表格数据绑定IncomeRecord。
           6、否则，显示消息框，提示“数据为空”。
            */
            try
            {
                //1、清空页面显示数据临时列表listIncomeRecord；
                listIncomeRecord.Clear();
                //2、设置收入每一页要显示的条数showCount（默认13）
                showCount = 13;
                //3、linq条件查询收入数据并赋值给临时列表listIncomes,日期条件筛选
                listIncomes = (from tbAmountOfDetai in myModels.S_AmountOfDetail
                               where tbAmountOfDetai.capitalPosition == "收入"
                               select new AccountVo
                               {
                                   AmountOfDetailID = tbAmountOfDetai.amountOfDetailID,//账号明细ID
                                   Money = tbAmountOfDetai.money,//金额
                                   Reason = tbAmountOfDetai.reason,//资金来由
                                   Time = tbAmountOfDetai.time,//时间
                                   Remark = tbAmountOfDetai.remark,//备注
                                   CapitalPosition = tbAmountOfDetai.capitalPosition,//资金状况
                               }).ToList();

                //日期条件筛选

                if (!string.IsNullOrEmpty(IncomeTimeStart) && !string.IsNullOrEmpty(IncomeTimeEnd))
                {
                    DateTime dtStart = Convert.ToDateTime(IncomeTimeStart);//开始时间
                    DateTime dtEnd = Convert.ToDateTime(IncomeTimeEnd);//结束时间
                    DateTime dtDay = dtEnd.AddDays(+1);//天数加1
                    //筛选时间段内数据
                    listIncomes = listIncomes.Where(h => h.Time >= dtStart).ToList();
                    listIncomes = listIncomes.Where(n => n.Time <= dtDay).ToList();
                }
                //4、count总数 = 临时列表总数；
                count = listIncomes.Count();
                //5、判断总数 > 0，是：
                if (listIncomes.Count>0)
                {
                    //（1）、for循环列表，自定义序号
                    for (int i = 0; i < count; i++)
                    {
                        //自定义序号
                        listIncomes[i].SerialNumber = i + 1;
                    }
                    //（2）、获取分页后余数remainder = 总数 % 显示的条数showCount
                    remainder = count % showCount;
                    //（3）、最后一条数据的索引 = count总数 - 1
                    theLastDataIndex = count - 1;
                    //（4）、判断：count总数 < 设置每一页要显示的条数showCount
                    if (count<= showCount)
                    {
                        //①是：allPage总页数 = 1，remainder分页后的余数 = count总数
                        allPage = 1;
                        remainder = count;
                        for (int i = 0; i < count; i++)
                        {
                            listIncomeRecord.Add(listIncomes[i]);
                        }
                    }
                    else
                    {
                        //②否，存在两种情况
                        //余数为零
                        if (remainder == 0)
                        {
                            //共几页
                            allPage = count / showCount;
                        }
                        if (remainder > 0)
                        {
                            //二是：余数 > 0,总页数 =（总数 / 每页显示条数）+1；
                            //共几页
                            allPage = (count / showCount) + 1;
                        }

                        //最后：for循环count总数,列表添加数据listIncomeRecord.（listIncomes[i]）
                        //页面显示的数据
                        for (int i = 0; i < showCount; i++)
                        {
                            listIncomeRecord.Add(listIncomes[i]);
                        } 
                        
                    }
                    //（5）、分页属性赋值：
                    nowPageSize = 1;
                    //nowPageSize当前页 = 1，
                    //最后一页数据的开始索引thelastPageStartIndex =（allPage总页数 - 1）*显示的条数showCount
                    thelastPageStartIndex = (allPage - 1) * showCount;
                    //最大页IncomeMaxPage = 总页数allPage
                    IncomeMaxPage = allPage.ToString();//最大页
                    IncomeAllCount = count.ToString();//总条数
                    IncomeCurrentPage = firstPageSize.ToString();//当前页
                    //每页显示N条数据   
                    IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);//表格数据（收入记录）
                }
                else
                {
                    IncomeMaxPage = "0";//最大页
                    IncomeAllCount = "0";//总条数
                    IncomeCurrentPage = "1";//当前页
                    IncomeRecord = null;//表格数据（收入记录）
                    MessageBox.Show("数据为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 清空条件
        /// </summary>
        private void CleanIncome()
        {
            IncomeTimeStart = String.Empty;
            IncomeTimeEnd = String.Empty;
        }
        /// <summary>
        /// 3.3 首页
        /// </summary>
        private void IncomeFirst()
        {
            /*
              首页：
              1、listIncomeRecord临时列表：移除所有元素

              2、初始化当前页nowPageSize=1，声明局部变量：首页开始索引firstPageStartIndex=0；
              3、判断所有条数count<每页显示条数showCount
              （1）是：调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord=listIncomes；
              （2）否：
                      ①声明变量：首页结束索引 firstPageEndIndex= 首页开始索引firstPageStartIndex+ 每页显示条数showCount-1；
                      ②for循环首页结束索引，添加每页显示数据列表listIncomeRecord.Add(listIncomes[i]);
              4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
              5、当前页赋值IncomeCurrentPage=当前页nowPageSize。
          */
            try
            {
                //1、listIncomeRecord临时列表：移除所有元素
                listIncomeRecord.Clear();
                //2、初始化当前页nowPageSize = 1，声明局部变量：首页开始索引firstPageStartIndex = 0；
                nowPageSize = 1;
                int firstPageStartIndex = 0;
                //3、判断所有条数count < 每页显示条数showCount
                if (count<showCount)
                {
                    //总页数一页
                    //（1）是：调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord = listIncomes；
                    SelectIncomeRecord();
                    listIncomeRecord = listIncomes;
                }
                else
                { //（2）否：
                  //①声明变量：首页结束索引 firstPageEndIndex = 首页开始索引firstPageStartIndex + 每页显示条数showCount - 1；
                    int firstPageEndIndex = firstPageStartIndex + showCount - 1;
                    //②for循环首页结束索引，添加每页显示数据列表listIncomeRecord.Add(listIncomes[i]);
                    for (int i = 0; i <= firstPageEndIndex; i++)
                    {
                        listIncomeRecord.Add(listIncomes[i]);
                    }
                }
                //4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
                IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);
                //5、当前页赋值IncomeCurrentPage = 当前页nowPageSize。
                IncomeCurrentPage = nowPageSize.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 上一页
        /// </summary>
        private void IncomeLast()
        {
            /*
            上一页：
               1、临时列表listIncomeRecord：清空；
               2、声明两个局部变量：上一页的数据的索引lastPageStartIndex=0，上一页结束索引lastPageEndIndex；
               3、判断:总条数count < 每页显示条数showCount
               （1）是:调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord=listIncomes；
               （2）否：
                ①判断：当前页nowPageSize==1；上一页开始索引lastPageStartIndex= 索引从0开始;
                ②判断：当前页nowPageSize>1；上一页开始索引lastPageStartIndex=(当前页nowPageSize-2) * 每页显示条数showCount; 当前页nowPageSize--;
                ③上一页结束索引lastPageEndIndex=上一页结束索引lastPageStartIndex+每页显示条数showCount - 1
                ④for循环首页结束索引，添加每页显示数据列表listIncomeRecord.Add(listIncomes[i]);                      
               4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
               5、当前页赋值IncomeCurrentPage=当前页nowPageSize。
            */
            try
            {
                //1、临时列表listIncomeRecord：清空；
                listIncomeRecord.Clear();
                //2、声明两个局部变量：上一页的数据的索引lastPageStartIndex = 0，上一页结束索引lastPageEndIndex；
                int lastPageStartIndex = 0;
                int lastPageEndIndex;
                //3、判断: 总条数count < 每页显示条数showCount
                if (count< showCount)
                {
                    //（1）是: 调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord = listIncomes；
                    SelectIncomeRecord();
                    listIncomeRecord = listIncomes;
                }

                //（2）否：
                else
                {
                    //①判断：当前页nowPageSize == 1；上一页开始索引lastPageStartIndex = 索引从0开始;
                    if (nowPageSize == 1)
                    {
                        lastPageStartIndex = 0;
                    }
                    //②判断：当前页nowPageSize > 1；上一页开始索引lastPageStartIndex = (当前页nowPageSize - 2) * 每页显示条数showCount; 当前页nowPageSize--;
                    if (nowPageSize > 1)
                    {
                        lastPageStartIndex = (nowPageSize - 2) * showCount;
                        nowPageSize--;
                    }
                    //③上一页结束索引lastPageEndIndex = 上一页结束索引lastPageStartIndex + 每页显示条数showCount - 1
                    lastPageEndIndex = lastPageStartIndex + showCount - 1;
                    //④for循环首页结束索引，添加每页显示数据列表listIncomeRecord.Add(listIncomes[i]);
                    for (int i = lastPageStartIndex; i <= lastPageEndIndex; i++)
                    {
                        //添加索引范围内数据
                        listIncomeRecord.Add(listIncomes[i]);
                    }
                }
                //4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
                IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);
                //5、当前页赋值IncomeCurrentPage = 当前页nowPageSize。
                IncomeCurrentPage = nowPageSize.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.5下一页
        /// </summary>
        private void IncomeNext()
        {
            /*
           下一页：
              1、临时列表listIncomeRecord：清空；
              2、声明两个局部变量：下一页开始索引nextPageStartIndex=0，下一页结束索引nextPageEndIndex=0；
              3、判断:总条数count < 每页显示条数showCount
              （1）是:调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord=listIncomes；当前页nowPageSize=1
              （2）否：
                    ①判断：当前页nowPageSize==总页数allPage；（代表尾页）
                       下一页开始索引nextPageStartIndex=（当前页nowPageSize-1）*每页显示条数showCount
                       下一页结束索引nextPageEndIndex=最后一条数据的索引theLastDataIndex
                       当前页=总页数；
                    ②判断：  当前页 < 总页数；（代表其它页）
                       下一页开始索引=当前页*每页显示条数
                       当前页++；
                       下一页结束索引=当前页*每页显示条数-1
                    ③检查下一页的数据的结束索引是否超出或等于最后一条数据的索引
                        是：（1）最后一页,for循环theLastDataIndex，添加列表数据listIncomeRecord；
                        否：（2）其他页，for循环nextPageEndIndex，添加列表数据listIncomeRecord。 
              4、绑定收入记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
              5、当前页赋值IncomeCurrentPage=当前页nowPageSize。
           */
            try
            {
                //1、临时列表listIncomeRecord：清空；
                listIncomeRecord.Clear();
                //2、声明两个局部变量：下一页开始索引nextPageStartIndex = 0，下一页结束索引nextPageEndIndex = 0；
                int nextPageStartIndex=0, nextPageEndIndex = 0;
                //3、判断: 总条数count < 每页显示条数showCount
                if (count< showCount)
                {
                    //（1）是: 调用查询方法SelectIncomeRecord(),直接绑定表格数据listIncomeRecord = listIncomes；当前页nowPageSize = 1
                    SelectIncomeRecord();
                    listIncomeRecord = listIncomes;
                    nowPageSize = 1;
                }
                //（2）否：
                else
                {
                    //①判断：当前页nowPageSize == 总页数allPage；（代表尾页）
                    if (nowPageSize == allPage)
                    {
                        //下一页开始索引nextPageStartIndex =（当前页nowPageSize - 1）*每页显示条数showCount
                        nextPageStartIndex = (nowPageSize - 1) * showCount;
                        //下一页结束索引nextPageEndIndex = 最后一条数据的索引theLastDataIndex
                        nextPageEndIndex = theLastDataIndex;
                        //当前页 = 总页数；
                        nowPageSize = allPage;
                    }
                    //②判断：  当前页 < 总页数；（代表其它页）
                    if (nowPageSize< allPage)
                    {
                        //下一页开始索引 = 当前页 * 每页显示条数
                        nextPageStartIndex = nowPageSize * showCount;
                        //当前页++；
                        nowPageSize++;
                        //下一页结束索引 = 当前页 * 每页显示条数 - 1
                        nextPageEndIndex = nowPageSize * showCount - 1;
                    }
                    //③检查下一页的数据的结束索引是否超出或等于最后一条数据的索引
                    if (nextPageEndIndex> theLastDataIndex|| nextPageEndIndex== theLastDataIndex)
                    {
                        //是：（1）最后一页,for循环theLastDataIndex，添加列表数据listIncomeRecord；
                        for (int i = nextPageStartIndex; i <= theLastDataIndex; i++)
                        {
                            listIncomeRecord.Add(listIncomes[i]);
                        }
                    }
                    else
                    {
                        //否：（2）其他页，for循环nextPageEndIndex，添加列表数据listIncomeRecord。 
                        for (int i = nextPageStartIndex; i <= nextPageEndIndex; i++)
                        {
                            listIncomeRecord.Add(listIncomes[i]);
                        }
                    }
                   
                    
                }
                //4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
                IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);
                //5、当前页赋值IncomeCurrentPage = 当前页nowPageSize。
                IncomeCurrentPage = nowPageSize.ToString();
            }
            catch (Exception)
            {
                return;
            }
        } 
        /// <summary>
        /// 3.6 尾页
        /// </summary>
        private void IncomeEnd()
        {
            /*
            尾页：
               1、临时列表清空；listIncomeRecord
               2、当前页=尾页：nowPageSize = allPage;
               3、声明局部变量尾页开始索引finallyPageStartIndex =（当前页nowPageSize-1）*  每页显示条数showCount；
               4、判断尾页开始索引finallyPageStartIndex>1,
                  ①是，则for循环theLastDataIndex生成列表listIncomeRecord数据；
                  ②页面数据赋值（收入记录（绑定表格数据）、当前页）。

            */
            try
            {
                //1、临时列表清空；listIncomeRecord
                listIncomeRecord.Clear();
                //2、当前页 = 尾页：nowPageSize = allPage;
                nowPageSize = allPage;
                //3、声明局部变量尾页开始索引finallyPageStartIndex =（当前页nowPageSize - 1）*每页显示条数showCount；
                int finallyPageStartIndex = (nowPageSize - 1) * showCount;
                //4、判断尾页开始索引finallyPageStartIndex > 1,
                if (finallyPageStartIndex > 1)
                {
                    //①是，则for循环theLastDataIndex生成列表listIncomeRecord数据；
                    for (int i = finallyPageStartIndex; i < theLastDataIndex; i++)
                    {
                        listIncomeRecord.Add(listIncomes[i]);
                    }
                    //②页面数据赋值（收入记录（绑定表格数据）、当前页）。
                    //4、绑定支出记录表格数据 IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord)
                    IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);
                    //5、当前页赋值IncomeCurrentPage = 当前页nowPageSize。
                    IncomeCurrentPage = nowPageSize.ToString();
                }

            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.7 跳转
        /// </summary>
        private void IncomeGo()
        {
            /*
            跳转页：
           1、声明两个局部变量：跳转页开始索引、跳转页结束索引：nowPageStartIndex、nowPageEndIndex；
           2、临时列表：清空：listIncomeRecord
           3、判断页面输入跳转页IncomeJumpPage是否为空，是，则跳转页默认为1
           4、声明两个局部变量：strint toNPageText=获取跳转页IncomeJumpPage、int toNPage=当前页nowPageSize
           5、判断获取跳转页是否为空，非空，直接获取toNPageText=toNPageText.Trim();
           6、使用正则判断输入跳转页是否符合正则表达式(正整数)；

             符合：当前页toNPage=获取跳转页toNPageText；
                  （1）判断当前跳转页 toNPage> 总页数allPage；是：指定跳转到最大页
                    当前跳转页toNPage=最大页(最后一页)allPage；
                    当前页nowPageSize=最大页(最后一页)allPage；
                    跳转页开始索引nowPageStartIndex=（最大页allPage-1）* 每页显示条数showCount
                    判断:跳转页开始索引nowPageStartIndex>1,
                         是，for循环theLastDataIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                 （2）否：（当前跳转页 <= 总页数）
                    跳转页开始索引nowPageStartIndex=（当前页toNPage-1）* 每页显示条数showCount；
                    跳转页结束索引nowPageEndIndex = 当前页 toNPage* 每页显示条数showCount - 1；
                    判断跳转页结束索引nowPageEndIndex是否大于或者等于最后一条数据索引theLastDataIndex；
                    ①是：当前页nowPageSize=最大页(最后一页)allPage；
                          当前跳转页toNPage=最大页(最后一页)allPage；
                          判断：跳转页开始索引>1,for循环theLastDataIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                    ②否：for循环nowPageEndIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                 （3）判断：页面显示数据总数listIncomeRecord.Count>=1
                            页面数据绑定：收入记录（绑定表格数据）、当前页                    

            不符合：跳转页IncomeJumpPage=当前页nowPageSize;
                        显示消息框提醒用户。
            */
            try
            {
                //1、声明两个局部变量：跳转页开始索引、跳转页结束索引：nowPageStartIndex、nowPageEndIndex；
                int nowPageStartIndex;
                int nowPageEndIndex;
                //2、临时列表：清空：listIncomeRecord
                listIncomeRecord.Clear();
                //3、判断页面输入跳转页IncomeJumpPage是否为空，是，则跳转页默认为1
                if (string.IsNullOrEmpty(IncomeJumpPage))
                {
                    IncomeJumpPage ="1";                    
                }
                //4、声明两个局部变量：strint toNPageText = 获取跳转页IncomeJumpPage、int toNPage = 当前页nowPageSize
                string toNPageText = IncomeJumpPage;
                int toNPage = nowPageSize;
                //5、判断获取跳转页是否为空，非空，直接获取toNPageText = toNPageText.Trim();
                if (!string.IsNullOrEmpty(IncomeJumpPage))
                {
                    toNPageText = toNPageText.Trim();
                }
                //6、使用正则判断输入跳转页是否符合正则表达式(正整数)；
                bool blRegex= Regex.IsMatch(toNPageText, @"^[1-9]\d*$");
                if (blRegex)
                {
                    //符合：当前页toNPage = 获取跳转页toNPageText；
                    toNPage =Convert.ToInt32(toNPageText);
                    //（1）判断当前跳转页 toNPage> 总页数allPage；是：指定跳转到最大页
                    if (toNPage> allPage)
                    {
                        //指定跳转到最大页
                        //当前跳转页toNPage = 最大页(最后一页)allPage；
                        toNPage = allPage;
                        //当前页nowPageSize = 最大页(最后一页)allPage；
                        nowPageSize = allPage;
                        //跳转页开始索引nowPageStartIndex =（最大页allPage - 1）*每页显示条数showCount
                        nowPageStartIndex = (allPage - 1) * showCount;
                        //判断: 跳转页开始索引nowPageStartIndex > 1,
                        if (nowPageStartIndex > 1)
                        {
                            //是，for循环theLastDataIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                            for (int i = nowPageStartIndex; i <= theLastDataIndex; i++)
                            {
                                listIncomeRecord.Add(listIncomes[i]);
                            }
                        }
                        IncomeJumpPage = allPage.ToString();
                    }
                    else
                    {
                        //（2）否：（当前跳转页 <= 总页数）
                        //跳转页开始索引nowPageStartIndex =（当前页toNPage - 1）*每页显示条数showCount；
                        nowPageStartIndex = (toNPage - 1) * showCount;
                        //跳转页结束索引nowPageEndIndex = 当前页 toNPage* 每页显示条数showCount -1；
                        nowPageEndIndex = toNPage * showCount - 1;
                        //判断跳转页结束索引nowPageEndIndex是否大于或者等于最后一条数据索引theLastDataIndex；
                        if (nowPageEndIndex> theLastDataIndex|| nowPageEndIndex== theLastDataIndex)
                        {
                            //①是：当前页nowPageSize = 最大页(最后一页)allPage；
                            nowPageSize = allPage;
                            //        当前跳转页toNPage = 最大页(最后一页)allPage；
                            toNPage = allPage;
                            // 判断：跳转页开始索引 > 1,for循环theLastDataIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                            if (nowPageStartIndex>1)
                            {
                                //for循环theLastDataIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                                for (int i = nowPageStartIndex; i <= theLastDataIndex; i++)
                                {
                                    listIncomeRecord.Add(listIncomes[i]);
                                }
                            }
                        }
                        else
                        {
                            //②否：for循环nowPageEndIndex，添加记录索引范围内数据 listIncomeRecord.Add(listIncomes[i]);
                            for (int i = nowPageStartIndex; i <= nowPageEndIndex; i++)
                            {
                                listIncomeRecord.Add(listIncomes[i]);
                            }
                           
                        }                     

                    }
                    //（3）判断：页面显示数据总数listIncomeRecord.Count >= 1
                    if (listIncomeRecord.Count >= 1)
                    {
                        //页面数据绑定：收入记录（绑定表格数据）、当前页
                        IncomeRecord = new ObservableCollection<AccountVo>(listIncomeRecord);
                        //5、当前页赋值IncomeCurrentPage = 当前页toNPage。
                        IncomeCurrentPage = toNPage.ToString();
                    }
                }
                else
                {
                    //不符合：跳转页IncomeJumpPage = 当前页nowPageSize;
                    //显示消息框提醒用户。
                    IncomeJumpPage = nowPageSize.ToString();
                    MessageBox.Show("输入的值必须为正整数！", "提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
        #endregion
        #region 三、【支出记录】
        #region 【1、属性】        
        #region 条件筛选属性（绑定页面）
        /// <summary>
        /// 字段：支出时间（开始）
        /// </summary>
        private string spendingTimeStart;
        /// <summary>
        /// 属性：支出时间（开始）
        /// </summary>
        public string SpendingTimeStart
        {
            get { return spendingTimeStart; }
            set
            {
                if (spendingTimeStart != value)
                {
                    spendingTimeStart = value;
                    RaisePropertyChanged(() => SpendingTimeStart);

                }
            }
        }
        /// <summary>
        /// 字段：支出时间（结束）
        /// </summary>
        private string spendingTimeEnd;
        /// <summary>
        /// 属性：支出时间（结束）
        /// </summary>
        public string SpendingTimeEnd
        {
            get { return spendingTimeEnd; }
            set
            {
                if (spendingTimeEnd != value)
                {
                    spendingTimeEnd = value;
                    RaisePropertyChanged(() => SpendingTimeEnd);

                }
            }
        }
        #endregion
        #region 分页属性（绑定页面）
        /// <summary>
        /// 字段：最大页（支出）
        /// </summary>
        private string spendingMaxPage;
        /// <summary>
        /// 属性：最大页（支出）
        /// </summary>
        public string SpendingMaxPage
        {
            get { return spendingMaxPage; }
            set
            {
                if (spendingMaxPage != value)
                {
                    spendingMaxPage = value;
                    RaisePropertyChanged(() => SpendingMaxPage);

                }
            }
        }
        /// <summary>
        /// 字段：总条数（支出）
        /// </summary>
        private string spendingAllCount;
        /// <summary>
        /// 属性：总条数（支出）
        /// </summary>
        public string SpendingAllCount
        {
            get { return spendingAllCount; }
            set
            {
                if (spendingAllCount != value)
                {
                    spendingAllCount = value;
                    RaisePropertyChanged(() => SpendingAllCount);

                }
            }
        }
        /// <summary>
        /// 字段：当前页（支出）
        /// </summary>
        private string spendingCurrentPage;
        /// <summary>
        /// 属性：当前页（支出）
        /// </summary>
        public string SpendingCurrentPage
        {
            get { return spendingCurrentPage; }
            set
            {
                if (spendingCurrentPage != value)
                {
                    spendingCurrentPage = value;
                    RaisePropertyChanged(() => SpendingCurrentPage);

                }
            }
        }
        /// <summary>
        /// 字段：跳转页数（支出记录）
        /// </summary>
        private string spendingJumpPage;
        /// <summary>
        /// 属性：跳转页数（支出记录）
        /// </summary>
        public string SpendingJumpPage
        {
            get { return spendingJumpPage; }
            set
            {
                if (spendingJumpPage != value)
                {
                    spendingJumpPage = value;
                    RaisePropertyChanged(() => SpendingJumpPage);

                }
            }
        }
        /// <summary>
        /// 字段：支出记录（用于绑定表格）
        /// </summary>
        private ObservableCollection<AccountVo> spendingRecord;
        /// <summary>
        /// 属性：支出记录（用于绑定表格）
        /// </summary>
        public ObservableCollection<AccountVo> SpendingRecord
        {
            get { return spendingRecord; }
            set
            {
                if (spendingRecord != value)
                {
                    spendingRecord = value;
                    RaisePropertyChanged(() => SpendingRecord);

                }
            }
        }
        #endregion
        #region （全局变量）分页参数 支出        
        //加载页面
        //！！！！！！！！！！！！！该分页方法要求查询出来的数据必须大于或等于1条！！！！！！！！！！！！
        /// <summary>
        /// (全局变量)设置每一页要显示的条数（支出）
        /// </summary>
        public int showCount1;
        /// <summary>
        /// (全局变量)分页后的余数
        /// </summary>
        public int remainder1;
        /// <summary>
        /// (全局变量)总页数（最后一页）
        /// </summary>
        public int allPage1;
        /// <summary>
        /// (全局变量)首页
        /// </summary>
        public int firstPageSize1 = 1;
        /// <summary>
        /// (全局变量)当前页
        /// </summary>
        public int nowPageSize1;
        /// <summary>
        /// (全局变量)最后一条数据的索引
        /// </summary>
        public int theLastDataIndex1;
        /// <summary>
        /// (全局变量)最后一页
        /// </summary>
        public int thelastPageSize1;
        /// <summary>
        /// (全局变量)最后一页数据的开始索引
        /// </summary>
        public int thelastPageStartIndex1;
        /// <summary>
        /// (全局变量)所有的数据的条数
        /// </summary>
        public int count1;
        /// <summary>
        /// (全局变量)初始行号
        /// </summary>
        public int rowNumber1;


        #endregion
        #endregion
        #region 【2、命令】
        #region 支出分页命令
        /// <summary>
        /// 首页（支出）命令
        /// </summary>
        public ICommand SpendingFirstCommand { get; set; }
        /// <summary>
        /// 上一页（支出）命令
        /// </summary>
        public ICommand SpendingLastCommand { get; set; }
        /// <summary>
        /// 下一页（支出）命令
        /// </summary>
        public ICommand SpendingNextCommand { get; set; }
        /// <summary>
        /// 尾页（支出）命令
        /// </summary>
        public ICommand SpendingEndCommand { get; set; }
        /// <summary>
        /// 跳转（支出）命令
        /// </summary>
        public ICommand SpendingGoCommand { get; set; }
        #endregion
        /// <summary>
        /// 支出情况条件查询
        /// </summary>
        public ICommand SelectSpendingCommand { get; set; }

        /// <summary>
        /// 支出清空按钮
        /// </summary>
        public ICommand CleanSpendingCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 查询账户金额支出记录（绑定支出表格）
        /// </summary>
        private void SelectSpendingRecord()
        {
            /*
            1、清空页面显示数据临时列表listSpendingRecord
            2、设置支出每一页要显示的条数showCount1（默认13）
            3、linq条件查询支出数据并赋值给临时列表listSpendings，
            4、时间条件筛选数据
            5、count1总数=临时列表总数；
            6、判断总数>0，是：
              （1）、for循环列表，自定义序号
              （2）、获取分页后余数remainder1=总数%显示的条数showCount1
              （3）、最后一条数据的索引=count1总数-1
              （4）、判断：count1总数<设置每一页要显示的条数showCount
                   ①是：allPage1总页数=1，remainder1分页后的余数=count1总数
                   ②否，存在两种情况
                    一是：余数remainder1==0,总页数=（总数/每页显示条数）；
                    二是：余数>0,总页数=（总数/每页显示条数）+1；
                    最后：for循环count1总数,列表添加数据listSpendingRecord.(listSpendings[i])
              （5）、分页属性赋值：
                   nowPageSize1当前页=1，
                   最后一页数据的开始索引thelastPageStartIndex=（allPage1总页数-1）*显示的条数showCount1
                   最大页SpendingMaxPage=总页数allPage1
                   总条数SpendingAllCount=count1总数；
                   当前页SpendingCurrentPage=firstPageSize1首页
                   表格数据绑定SpendingRecord。
            7、否则，显示消息框，提示“数据为空”。
             */
            try
            {
                //1、临时列表数据：移除
                listSpendingRecord.Clear();
                //2、初始显示的数据条数
                showCount1 = 13;
                //3、记录查询支出记录
                listSpendings = (from tbAmountOfDetai in myModels.S_AmountOfDetail
                                 where tbAmountOfDetai.capitalPosition == "支出"
                                 select new AccountVo
                                 {
                                     AmountOfDetailID = tbAmountOfDetai.amountOfDetailID,//账号明细ID
                                     Money = tbAmountOfDetai.money,//金额
                                     Reason = tbAmountOfDetai.reason,//资金来由
                                     Time = tbAmountOfDetai.time,//时间
                                     Remark = tbAmountOfDetai.remark,//备注
                                     CapitalPosition = tbAmountOfDetai.capitalPosition,//资金状况
                                 }).ToList();
                //4、条件筛选数据
                if (!string.IsNullOrEmpty(SpendingTimeStart) && !string.IsNullOrEmpty(SpendingTimeEnd))
                {
                    DateTime dtStart = Convert.ToDateTime(SpendingTimeStart);//开始时间
                    DateTime dtEnd = Convert.ToDateTime(SpendingTimeEnd);//结束时间
                    DateTime dtDay = dtEnd.AddDays(+1);//结束时间天数
                    //获取时间段内数据
                    listSpendings = listSpendings.Where(h => h.Time >= dtStart).ToList();
                    listSpendings = listSpendings.Where(n => n.Time <= dtDay).ToList();

                }
                //5、获取总条数
                count1 = listSpendings.Count();
                if (count1 > 0)
                {
                    //自定义序号
                    for (int i = 0; i < count1; i++)
                    {
                        listSpendings[i].SerialNumber = i + 1;
                    }
                    //余数
                    remainder1 = count1 % showCount1;
                    //最后一条数据的索引
                    theLastDataIndex1 = count1 - 1;
                    if (count1 < showCount1)
                    {
                        //一页都不够分
                        allPage1 = 1;
                        remainder1 = count1;
                        for (int i = 0; i < count1; i++)
                        {
                            listSpendingRecord.Add(listSpendings[i]);
                        }
                    }
                    else
                    {

                        if (remainder1 == 0)
                        {
                            //共几页
                            allPage1 = count1 / showCount1;
                        }
                        if (remainder1 > 0)
                        {
                            //共几页
                            allPage1 = (count1 / showCount1) + 1;
                        }
                        //页面显示的数据
                        for (int i = 0; i < showCount1; i++)
                        {
                            listSpendingRecord.Add(listSpendings[i]);
                        }
                    }
                    //给变量赋值
                    nowPageSize1 = 1;
                    //最后一页数据的开始索引
                    thelastPageStartIndex1 = (allPage1 - 1) * showCount1;
                    //给页面赋值
                    SpendingMaxPage = allPage1.ToString();//最大页
                    SpendingAllCount = count1.ToString();//总条数
                    SpendingCurrentPage = firstPageSize1.ToString();//当前页
                    //每页显示N条数据
                    SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);//支出记录（表格数据）
                }
                else
                {
                    SpendingMaxPage = "0";//最大页
                    SpendingAllCount = "0";//总条数
                    SpendingCurrentPage = "1";//当前页
                    SpendingRecord = null;//支出记录（表格数据）
                    MessageBox.Show("数据为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 支出清空条件
        /// </summary>
        private void CleanSpendingConditions()
        {
            SpendingTimeStart = string.Empty;//开始时间
            SpendingTimeEnd = string.Empty;//结束时间
        }

        #region 管理翻页按钮 支出       

        /// <summary>
        /// 3.3 首页（支出）
        /// </summary>
        private void SpendingFirst()
        {
            /*
            首页：
            1、listSpendingRecord临时列表：移除所有元素
            2、初始化当前页nowPageSize1=1，声明局部变量：首页开始索引firstPageStartIndex=0；
            3、判断所有条数count1<每页显示条数showCount1
            （1）是：调用查询方法SelectSpendingRecord(),直接绑定表格数据listSpendingRecord=listSpendings；
            （2）否：
                    ①声明变量：首页结束索引 firstPageEndIndex= 首页开始索引firstPageStartIndex+ 每页显示条数showCount1-1；
                    ②for循环首页结束索引，添加每页显示数据列表listSpendingRecord.Add(listSpendings[i]);
            4、绑定支出记录表格数据 SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);
            5、当前页赋值SpendingCurrentPage=当前页nowPageSize1。
            */
            try
            {
                //1、临时列表：移除
                listSpendingRecord.Clear();
                nowPageSize1 = 1;//当前页=1
                int firstPageStartIndex = 0;//首页开始索引=0
                if (count1 < showCount1)
                {
                    SelectSpendingRecord();//查询支出记录（表格数据）
                    listSpendingRecord = listSpendings;//记录支出记录
                }
                else
                {
                    int firstPageEndIndex = firstPageStartIndex + showCount1 - 1; //首页结束索引 = 首页开始索引+ 每页显示条数-1
                    for (int i = 0; i <= firstPageEndIndex; i++)
                    {
                        //记录索引范围内数据
                        listSpendingRecord.Add(listSpendings[i]);
                    }
                }
                SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);//支出记录
                SpendingCurrentPage = nowPageSize1.ToString();//当前页
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 上一页（支出）
        /// </summary>
        private void SpendingLast()
        {
            /*
             上一页

             */
            try
            {
                //临时列表：清除
                listSpendingRecord.Clear();
                int lastPageStartIndex = 0;
                int lastPageEndIndex;
                if (count1 < showCount1)
                {
                    SelectSpendingRecord();
                    listSpendingRecord = listSpendings;
                }
                else
                {
                    if (nowPageSize1 == 1)
                    {
                        lastPageStartIndex = 0;
                    }
                    if (nowPageSize1 > 1)
                    {
                        lastPageStartIndex = (nowPageSize1 - 2) * showCount1;
                        nowPageSize1--;
                    }
                    lastPageEndIndex = lastPageStartIndex + showCount1 - 1;
                    for (int i = lastPageStartIndex; i <= lastPageEndIndex; i++)
                    {
                        listSpendingRecord.Add(listSpendings[i]);
                    }
                }
                //给页面赋值

                SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);

                SpendingCurrentPage = nowPageSize1.ToString();
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 3.5 下一页（支出）
        /// </summary>
        private void SpendingNext()
        {
            try
            {
                listSpendingRecord.Clear();
                //下一页的数据的开始索引
                int nextPageStartIndex = 0;
                int nextPageEndIndex = 0;
                if (count1 < showCount1)
                {
                    SelectSpendingRecord();
                    listSpendingRecord = listSpendings;
                    nowPageSize1 = 1;
                }
                else
                {
                    if (nowPageSize1 == allPage1)
                    {
                        //当前页为最后一页时
                        nextPageStartIndex = (nowPageSize1 - 1) * showCount1;
                        nextPageEndIndex = theLastDataIndex1;
                        nowPageSize1 = allPage1;
                    }
                    if (nowPageSize1 < allPage1)
                    {
                        nextPageStartIndex = nowPageSize1 * showCount1;
                        nowPageSize1++;
                        nextPageEndIndex = (nowPageSize1 * showCount1) - 1;
                    }

                    //检查下一页的数据的结束索引是否超出或等于最后一条数据的索引
                    if (nextPageEndIndex > theLastDataIndex1 || nextPageEndIndex == theLastDataIndex1)
                    {
                        for (int i = nextPageStartIndex; i <= theLastDataIndex1; i++)
                        {
                            listSpendingRecord.Add(listSpendings[i]);
                        }
                    }
                    else
                    {
                        for (int i = nextPageStartIndex; i <= nextPageEndIndex; i++)
                        {
                            listSpendingRecord.Add(listSpendings[i]);
                        }
                    }
                }
                //给页面赋值
                SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);
                SpendingCurrentPage = nowPageSize1.ToString();
            }
            catch (Exception)
            {
                return;
            }

        }
        /// <summary>
        /// 3.6 尾页（支出）
        /// </summary>
        private void SpendingEnd()
        {
            try
            {
                listSpendingRecord.Clear();
                nowPageSize1 = allPage1;
                int finallyPageStartIndex = (nowPageSize1 - 1) * showCount1;
                if (finallyPageStartIndex > 1)
                {
                    for (int i = finallyPageStartIndex; i <= theLastDataIndex1; i++)
                    {
                        listSpendingRecord.Add(listSpendings[i]);
                    }
                    SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);
                    SpendingCurrentPage = nowPageSize1.ToString();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 3.7 页面跳转（支出）
        /// </summary>
        private void SpendingGo()
        {
            try
            {
                int nowPageStartIndex;
                int nowPageEndIndex;
                listSpendingRecord.Clear();
                if (SpendingJumpPage == "" || SpendingJumpPage == null)
                {
                    SpendingJumpPage = "1";
                }
                string toNPageText = SpendingJumpPage;
                int toNPage = nowPageSize1;
                if (!string.IsNullOrEmpty(toNPageText))
                {
                    toNPageText = toNPageText.Trim();
                }
                //C#正则表达式格式@+"正则表达式"
                //是否符合正则表达式(正整数)
                bool isCorrect = Regex.IsMatch(toNPageText, @"^[1-9]\d*$");
                if (isCorrect)
                {
                    toNPage = Convert.ToInt32(toNPageText);
                    if (toNPage > allPage1)
                    {
                        toNPage = allPage1;
                        nowPageSize1 = allPage1;
                        nowPageStartIndex = (allPage1 - 1) * showCount1;
                        if (nowPageStartIndex > 1)
                        {
                            for (int i = nowPageStartIndex; i <= theLastDataIndex1; i++)
                            {
                                listSpendingRecord.Add(listSpendings[i]);
                            }
                        }
                    }
                    else
                    {
                        nowPageStartIndex = (toNPage - 1) * showCount1;
                        nowPageEndIndex = toNPage * showCount1 - 1;
                        if (nowPageEndIndex > theLastDataIndex1 || nowPageEndIndex == theLastDataIndex1)
                        {
                            nowPageSize1 = allPage1;
                            toNPage = allPage1;
                            if (nowPageStartIndex > 1)
                            {
                                for (int i = nowPageStartIndex; i <= theLastDataIndex1; i++)
                                {
                                    listSpendingRecord.Add(listSpendings[i]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = nowPageStartIndex; i <= nowPageEndIndex; i++)
                            {
                                listSpendingRecord.Add(listSpendings[i]);
                            }
                        }
                    }
                    if (listSpendingRecord.Count >= 1)
                    {
                        SpendingRecord = new ObservableCollection<AccountVo>(listSpendingRecord);
                        SpendingCurrentPage = toNPage.ToString();
                    }
                }
                else
                {
                    SpendingJumpPage = nowPageSize1.ToString();
                    System.Windows.MessageBox.Show("输入的值必须为正整数！", "提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion
        #endregion
        #endregion
    }
}
