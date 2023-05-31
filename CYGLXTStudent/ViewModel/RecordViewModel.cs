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
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 登录记录
    /// </summary>
    public class RecordViewModel : ViewModelBase
    {
        public RecordViewModel()
        {
            //页面加载
            LoadedCommand = new RelayCommand(SelectRecord);
            //登录清空条件按钮
            CleanCommand = new RelayCommand(CleanConditions);

            #region 登录记录分页
            FirstCommand = new RelayCommand(First);//首页
            LastCommand = new RelayCommand(Last);//上一页
            NextCommand = new RelayCommand(Next);//下一页        
            EndCommand = new RelayCommand(End);//尾页
            GoCommand = new RelayCommand(Go);//跳转
            #endregion  
        }
        #region 【1、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();

        /// <summary>
        /// 临时列表：数据源
        /// </summary>
        List<StaffVo> lists = new List<StaffVo>();
        /// <summary>
        /// 临时列表：页面显示的数据
        /// </summary>
        List<StaffVo> listRecord = new List<StaffVo>();
        #region 分页全局变量
        //加载页面
        //！！！！！！！！！！！！！该分页方法要求查询出来的数据必须大于或等于1条！！！！！！！！！！！！
        /// <summary>
        /// 设置每一页要显示的条数
        /// </summary>
        public int showCount;
        /// <summary>
        /// 分页后的余数
        /// </summary>
        public int remainder;
        /// <summary>
        /// 总页数（最后一页）
        /// </summary>
        public int allPage;
        /// <summary>
        /// 首页
        /// </summary>
        public int firstPageSize = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        public int nowPageSize;
        /// <summary>
        /// 最后一条数据的索引
        /// </summary>
        public int theLastDataIndex;
        /// <summary>
        /// 最后一页
        /// </summary>
        public int thelastPageSize;
        /// <summary>
        /// 最后一页数据的开始索引
        /// </summary>
        public int thelastPageStartIndex;
        /// <summary>
        /// 所有的数据的条数
        /// </summary>
        public int count;
        /// <summary>
        /// 初始行号
        /// </summary>
        public int rowNumber;

        #endregion

        #endregion
        #region 【1、属性】      

        #region 分页属性（绑定页面）
        /// <summary>
        /// 字段：最大页
        /// </summary>
        private string maxPage;
        /// <summary>
        /// 属性：最大页
        /// </summary>
        public string MaxPage
        {
            get { return maxPage; }
            set
            {
                if (maxPage != value)
                {
                    maxPage = value;
                    RaisePropertyChanged(() => MaxPage);

                }
            }
        }
        /// <summary>
        /// 字段：总条数
        /// </summary>
        private string allCount;
        /// <summary>
        /// 属性：总条数
        /// </summary>
        public string AllCount
        {
            get { return allCount; }
            set
            {
                if (allCount != value)
                {
                    allCount = value;
                    RaisePropertyChanged(() => AllCount);

                }
            }
        }
        /// <summary>
        /// 字段：当前页
        /// </summary>
        private string currentPage;
        /// <summary>
        /// 属性：当前页
        /// </summary>
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    RaisePropertyChanged(() => CurrentPage);

                }
            }
        }
        /// <summary>
        /// 字段：跳转页
        /// </summary>
        private string jumpPage;
        /// <summary>
        /// 属性：跳转页
        /// </summary>
        public string JumpPage
        {
            get { return jumpPage; }
            set
            {
                if (jumpPage != value)
                {
                    jumpPage = value;
                    RaisePropertyChanged(() => JumpPage);

                }
            }
        }
        /// <summary>
        /// 字段：登录记录（用于绑定表格）
        /// </summary>
        private ObservableCollection<StaffVo> record;
        /// <summary>
        /// 属性：登录记录（用于绑定表格）
        /// </summary>
        public ObservableCollection<StaffVo> Record
        {
            get { return record; }
            set
            {
                if (record != value)
                {
                    record = value;
                    RaisePropertyChanged(() => Record);

                }
            }
        }
        #endregion
        #region 条件筛选属性（筛选数据）
        /// <summary>
        /// 字段：工号
        /// </summary>
        private string eMPNO;
        /// <summary>
        /// 属性：工号
        /// </summary>
        public string EMPNO
        {
            get { return eMPNO; }
            set
            {
                if (eMPNO != value)
                {
                    eMPNO = value;
                    RaisePropertyChanged(() => EMPNO);

                }
            }
        }
        /// <summary>
        /// 字段：员工名称
        /// </summary>
        private string staffName;
        /// <summary>
        /// 属性：员工名称
        /// </summary>
        public string StaffName
        {
            get { return staffName; }
            set
            {
                if (staffName != value)
                {
                    staffName = value;
                    RaisePropertyChanged(() => StaffName);

                }
            }
        }
        /// <summary>
        /// 字段：身份证
        /// </summary>
        private string iDNumber;
        /// <summary>
        /// 属性：身份证
        /// </summary>
        public string IDNumber
        {
            get { return iDNumber; }
            set
            {
                if (iDNumber != value)
                {
                    iDNumber = value;
                    RaisePropertyChanged(() => IDNumber);

                }
            }
        }

        /// <summary>
        /// 字段：性别
        /// </summary>
        private string sex;
        /// <summary>
        /// 属性：性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set
            {
                if (sex != value)
                {
                    sex = value;
                    RaisePropertyChanged(() => Sex);

                }
            }
        }
        /// <summary>
        /// 字段：职务
        /// </summary>
        private string position;
        /// <summary>
        /// 属性：职务
        /// </summary>
        public string Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    RaisePropertyChanged(() => Position);

                }
            }
        }
        /// <summary>
        /// 字段：状态
        /// </summary>
        private string statust;
        /// <summary>
        /// 属性：状态
        /// </summary>
        public string Statust
        {
            get { return statust; }
            set
            {
                if (statust != value)
                {
                    statust = value;
                    RaisePropertyChanged(() => Statust);

                }
            }
        }
        #endregion               

        #endregion
        #region 【2、命令】
        /// <summary>
        /// 加载命令
        /// </summary>
        public ICommand LoadedCommand { get; set; }
        /// <summary>
        /// 登录情况条件查询命令
        /// </summary>
        public ICommand SelectCommand { get; set; }
        /// <summary>
        /// 登录清空按钮命令
        /// </summary>
        public ICommand CleanCommand { get; set; }
        #region 分页命令
        /// <summary>
        /// 首页命令
        /// </summary>
        public ICommand FirstCommand { get; set; }
        /// <summary>
        /// 上一页命令
        /// </summary>
        public ICommand LastCommand { get; set; }
        /// <summary>
        /// 下一页命令
        /// </summary>
        public ICommand NextCommand { get; set; }
        /// <summary>
        /// 尾页命令
        /// </summary>
        public ICommand EndCommand { get; set; }
        /// <summary>
        /// 跳转命令
        /// </summary>
        public ICommand GoCommand { get; set; }
        #endregion
        #endregion
        #region 【3、方法】        
        /// <summary>
        /// 3.1 登录情况条件查询
        /// </summary>
        private void SelectRecord()
        {
            try
            {
                //1、数据列表：清空
                listRecord.Clear();
                //2、初始显示的数据条数
                showCount = 16;
                //3、临时列表：记录查询登录记录
                lists = (from tbTheLoginDetails in myModels.S_TheLoginDetails
                         join tbStaff in myModels.S_Staff on tbTheLoginDetails.staffID equals tbStaff.staffID
                         orderby tbTheLoginDetails.theLoginDetailsID descending//倒叙排序
                         select new StaffVo
                         {
                             staffID = tbStaff.staffID,//员工ID
                             EMPNO = tbStaff.EMPNO,//工号
                             name = tbStaff.name,//姓名
                             phone = tbStaff.phone,//电话
                             IDNumber = tbStaff.IDNumber,//身份证号码
                             sex = tbStaff.sex,//性别
                             position = tbStaff.position,//职务
                             statust = tbStaff.statust,//员工状态
                             entryDate = tbStaff.entryDate,//入职日期
                             LogonTime = tbTheLoginDetails.logonTime,//登录时间
                             OfflineTime = tbTheLoginDetails.offlineTime,//离线时间
                             TheLogin = tbTheLoginDetails.theLogin,//登录时长                                  
                         }).ToList();
                //条件筛选
                //工号
                if (!string.IsNullOrEmpty(EMPNO))
                {
                    lists = lists.Where(m => m.EMPNO.Contains(EMPNO)).ToList();
                }
                //姓名
                if (!string.IsNullOrEmpty(StaffName))
                {
                    lists = lists.Where(m => m.name.Contains(StaffName)).ToList();
                }
                //身份证
                if (!string.IsNullOrEmpty(IDNumber))
                {
                    lists = lists.Where(m => m.IDNumber.Contains(IDNumber)).ToList();
                }
                //性别
                if (!string.IsNullOrEmpty(Sex))
                {
                    lists = lists.Where(m => m.sex.Contains(Sex)).ToList();
                }
                //职务
                if (!string.IsNullOrEmpty(Position))
                {
                    lists = lists.Where(m => m.position.Contains(Position)).ToList();
                }
                //员工状态
                if (!string.IsNullOrEmpty(Statust))
                {
                    lists = lists.Where(m => m.statust.Contains(Statust)).ToList();
                }
                //5、获取总条数
                count = lists.Count();
                if (count > 0)
                {
                    //自定义序号
                    for (int i = 0; i < count; i++)
                    {
                        lists[i].SerialNumber = i + 1;
                    }
                    //余数
                    remainder = count % showCount;
                    //最后一条数据的索引
                    theLastDataIndex = count - 1;
                    if (count < showCount)
                    {

                        //一页都不够分
                        allPage = 1;
                        remainder = count;
                        for (int i = 0; i < count; i++)
                        {
                            listRecord.Add(lists[i]);
                        }
                    }
                    else
                    {
                        if (remainder == 0)
                        {
                            //共几页
                            allPage = count / showCount;
                        }
                        if (remainder > 0)
                        {
                            //共几页
                            allPage = (count / showCount) + 1;
                        }
                        //页面显示的数据
                        for (int i = 0; i < showCount; i++)
                        {
                            listRecord.Add(lists[i]);
                        }
                    }
                    //给变量赋值
                    nowPageSize = 1;
                    //最后一页数据的开始索引
                    thelastPageStartIndex = (allPage - 1) * showCount;
                    //给页面赋值
                    MaxPage = allPage.ToString();//最大页
                    AllCount = count.ToString();//总条数
                    CurrentPage = firstPageSize.ToString();//当前页
                    //每页显示N条数据   
                    Record = new ObservableCollection<StaffVo>(listRecord);//表格数据
                }
                else
                {
                    MaxPage = "0";//最大页
                    AllCount = "0";//总条数
                    CurrentPage = "1";//当前页
                    Record = null;//表格数据
                    MessageBox.Show("数据为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 登录清空条件
        /// </summary>
        private void CleanConditions()
        {
            EMPNO = string.Empty;//工号
            StaffName = string.Empty;//员工名称
            IDNumber = string.Empty;//身份证
            Sex = string.Empty;//性别
            Position = string.Empty;//职务
            Statust = string.Empty;//状态
        }
        #region 管理翻页按钮
        /// <summary>
        /// 3.3 首页
        /// </summary>
        private void First()
        {

            try
            {
                //1、临时列表：清空
                listRecord.Clear();
                nowPageSize = 1;//当前页=首页1
                int firstPageStartIndex = 0;//首页开始索引
                //2、总条数<每页显示条数
                if (count < showCount)
                {
                    //首页
                    SelectRecord();//查询登录记录
                    listRecord = lists;//临时记录数据
                }
                else
                {
                    //其他页
                    int firstPageEndIndex = firstPageStartIndex + showCount - 1;//首页开始索引=首页开始索引+每页显示条数-1
                    for (int i = 0; i <= firstPageEndIndex; i++)
                    {
                        //记录索引范围内数据
                        listRecord.Add(lists[i]);
                    }
                }
                //给页面赋值          
                Record = new ObservableCollection<StaffVo>(listRecord);//登录记录（绑定表格数据）
                CurrentPage = nowPageSize.ToString();//当前页
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 上一页
        /// </summary>
        private void Last()
        {
            try
            {
                //1、临时列表：清空
                listRecord.Clear();
                //2、上一页的数据的索引
                int lastPageStartIndex = 0;//上一页开始索引
                int lastPageEndIndex;//上一页结束索引
                //3、总条数<每页显示条数
                if (count < showCount)
                {
                    SelectRecord();//查询登录记录
                    listRecord = lists;//临时记录数据
                }
                else
                {
                    if (nowPageSize == 1)
                    {
                        //首页
                        lastPageStartIndex = 0;//上一页开始索引= 索引从0开始
                    }
                    if (nowPageSize > 1)
                    {
                        //其他页                        
                        lastPageStartIndex = (nowPageSize - 2) * showCount;//上一页开始索引=(当前页-2) * 每页显示条数
                        nowPageSize--;//当前页=当前页 - 1
                    }
                    lastPageEndIndex = lastPageStartIndex + showCount - 1;//上一页结束索引=上一页结束索引+每页显示条数 - 1
                    for (int i = lastPageStartIndex; i <= lastPageEndIndex; i++)
                    {
                        //记录索引范围内数据
                        listRecord.Add(lists[i]);
                    }
                }
                //给页面赋值          
                Record = new ObservableCollection<StaffVo>(listRecord);//登录记录（绑定表格数据）
                CurrentPage = nowPageSize.ToString();//当前页
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 3.5 下一页
        /// </summary>
        private void Next()
        {
            //1、临时列表：清空
            listRecord.Clear();
            //2、下一页的数据的索引
            int nextPageStartIndex = 0;//下一页开始索引
            int nextPageEndIndex = 0;//下一页结束索引
            //3、总条数<每页显示条数
            if (count < showCount)
            {

                SelectRecord(); //查询登录记录
                listRecord = lists; //临时记录数据
                nowPageSize = 1; //当前页
            }
            //4、总条数 >=每页显示条数
            else
            {
                if (nowPageSize == allPage)
                {
                    //当前页为最后一页时
                    nextPageStartIndex = (nowPageSize - 1) * showCount;//下一页开始索引
                    nextPageEndIndex = theLastDataIndex;//下一页结束索引
                    nowPageSize = allPage;//当前页
                }
                if (nowPageSize < allPage)
                {
                    //其他页
                    nextPageStartIndex = nowPageSize * showCount;//下一页开始索引
                    nowPageSize++;//当前页
                    nextPageEndIndex = (nowPageSize * showCount) - 1;//下一页结束索引
                }

                //检查下一页的数据的结束索引是否超出或等于最后一条数据的索引
                if (nextPageEndIndex > theLastDataIndex || nextPageEndIndex == theLastDataIndex)
                {
                    //最后一页
                    for (int i = nextPageStartIndex; i <= theLastDataIndex; i++)
                    {
                        //记录索引范围内数据
                        listRecord.Add(lists[i]);
                    }
                }
                else
                {
                    //其他页
                    for (int i = nextPageStartIndex; i <= nextPageEndIndex; i++)
                    {
                        //记录索引范围内数据
                        listRecord.Add(lists[i]);
                    }
                }
            }
            //给页面赋值           
            Record = new ObservableCollection<StaffVo>(listRecord);//登录记录（绑定表格数据）
            CurrentPage = nowPageSize.ToString();//当前页
        }

        /// <summary>
        /// 3.6 尾页
        /// </summary>
        private void End()
        {
            try
            {
                //临时列表：清空
                listRecord.Clear();
                nowPageSize = allPage;//当前页=尾页
                int finallyPageStartIndex = (nowPageSize - 1) * showCount;//尾页开始索引 =（当前页-1）*  每页显示条数
                if (finallyPageStartIndex > 1)
                {
                    for (int i = finallyPageStartIndex; i <= theLastDataIndex; i++)
                    {
                        //记录索引范围内数据
                        listRecord.Add(lists[i]);
                    }
                    //给页面赋值           
                    Record = new ObservableCollection<StaffVo>(listRecord);//登录记录（绑定表格数据）
                    CurrentPage = nowPageSize.ToString();//当前页
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 3.7 页面跳转
        /// </summary>
        private void Go()
        {
            try
            {
                int nowPageStartIndex;//跳转页开始索引
                int nowPageEndIndex;//跳转页结束索引
                //1、临时列表：清空
                listRecord.Clear();
                if (JumpPage == "" || JumpPage == null)
                {
                    //跳转页默认为1
                    JumpPage = "1";
                }
                string toNPageText = JumpPage;//获取跳转页
                int toNPage = nowPageSize;//当前页
                if (!string.IsNullOrEmpty(toNPageText))
                {
                    toNPageText = toNPageText.Trim();
                }
                //C#正则表达式格式@+"正则表达式"
                //是否符合正则表达式(正整数)
                bool isCorrect = Regex.IsMatch(toNPageText, @"^[1-9]\d*$");
                if (isCorrect)
                {
                    //当前页=获取跳转页
                    toNPage = Convert.ToInt32(toNPageText);

                    //（1）当前跳转页 > 总页数
                    if (toNPage > allPage)
                    {
                        //指定跳转到最大页
                        toNPage = allPage;//当前跳转页=最大页(最后一页)
                        nowPageSize = allPage;//当前页=最大页(最后一页)
                        nowPageStartIndex = (allPage - 1) * showCount;//跳转页开始索引=（最大页-1）* 每页显示条数
                        if (nowPageStartIndex > 1)
                        {
                            for (int i = nowPageStartIndex; i <= theLastDataIndex; i++)
                            {
                                //记录索引范围内数据
                                listRecord.Add(lists[i]);
                            }
                        }
                    }
                    //（2）当前跳转页 <= 总页数
                    else
                    {

                        nowPageStartIndex = (toNPage - 1) * showCount;//跳转页开始索引=（当前页-1）* 每页显示条数
                        nowPageEndIndex = toNPage * showCount - 1;//跳转页结束索引 = 当前页 * 每页显示条数 - 1
                        if (nowPageEndIndex > theLastDataIndex || nowPageEndIndex == theLastDataIndex)
                        {
                            nowPageSize = allPage;//当前页=最大页(最后一页)
                            toNPage = allPage;//当前跳转页=最大页(最后一页)
                            if (nowPageStartIndex > 1)
                            {
                                for (int i = nowPageStartIndex; i <= theLastDataIndex; i++)
                                {
                                    //记录索引范围内数据
                                    listRecord.Add(lists[i]);
                                }
                            }
                        }
                        else
                        {

                            for (int i = nowPageStartIndex; i <= nowPageEndIndex; i++)
                            {
                                //记录索引范围内数据
                                listRecord.Add(lists[i]);
                            }

                        }
                    }
                    if (listRecord.Count >= 1)
                    {
                        Record = new ObservableCollection<StaffVo>(listRecord);//登录记录（绑定表格数据）
                        CurrentPage = toNPage.ToString();//当前页
                    }
                }
                else
                {
                    JumpPage = nowPageSize.ToString();//跳转页=当前页
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
    }
}
