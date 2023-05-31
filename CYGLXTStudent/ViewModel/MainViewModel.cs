using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CYGLXTStudent.Resources.PublicClass;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using CYGLXTStudent.Views.Staffs;
using CYGLXTStudent.Resources.Control;
using CYGLXTStudent.Views;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 主窗口
    /// </summary>
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel()
        {
            //加载命令执行加载方法
            LoadedCommand = new RelayCommand<TabControl>(Loaded, tab => tab != null);
            //个人信息查询
            PersonalDetailsCommand = new RelayCommand(PersonalDetails);
            //最小化
            MinimizedCommand=new RelayCommand<Window> (MinimizedWindow, win => win != null);
            //最大化/还原
            MaximizedOrNormalCommand= new RelayCommand<Window>(MaximizedOrNormal, win => win != null);
            //关闭应用程序
            CloseApplicationCommand=new RelayCommand(CloseApplication);
            //会员管理
            Init();
        }
        #region 全局变量
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 全局选显卡
        /// </summary>
        public static TabControl TC;
        /// <summary>
        /// 描述矩形宽度、高度以及位置
        /// </summary>
        Rect rect;
        /// <summary>
        /// 标志：默认还原false
        /// </summary>
        bool blMax = false;
        #endregion
        #region 一【窗口最小化、最大化、还原、关闭】
        #region 【1、属性】

        #endregion
        #region 【2、命令】
        /// <summary>
        /// 窗口最小化命令
        /// </summary>
        public RelayCommand<Window> MinimizedCommand { get; set; }
        /// <summary>
        /// 最大化/还原命令
        /// </summary>
        public RelayCommand<Window> MaximizedOrNormalCommand { get; set; }
        /// <summary>
        /// 退出应用程序命令
        /// </summary>

        public RelayCommand CloseApplicationCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="window">主窗口</param>
        private void MinimizedWindow(Window window)
        {
            window.WindowState=WindowState.Minimized;
        }
        /// <summary>
        /// 最大化/还原窗口
        /// </summary>
        /// <param name="window">主窗口</param>
        private void MaximizedOrNormal(Window window)
        {
            /*
            * 思路：
            * 1、先声明两个全局变量，类型为Rect和bool（默认为false）的变量分别记录
            * 2、两种情况
            *（1）bool的变量=true,则还原窗口（获取Rect的Left、Top、Width、Height，同时更改bool的变量=false）
            *（2）bool的变量=false，窗口最大化
            *①new Rect(),保存下当前窗口的Left、Top、Width、Height；
            *②设置窗口最大化（SystemParameters.WorkArea）获取工作区大小。
            *③同时更改bool的变量=true     
            * 
            */
            try
            {
                //*2、两种情况
                //*（1）bool的变量 = true,则还原窗口（获取Rect的Left、Top、Width、Height，同时更改bool的变量 = false）
                if (blMax)
                {
                    //还原
                    window.Width = rect.Width;
                    window.Height = rect.Height;
                    window.Top=rect.Top;
                    window.Left=rect.Left;                   
                    blMax = false;
                }
                else
                {
                    //*（2）bool的变量 = false，窗口最大化
                    //*①new Rect(),保存下当前窗口的Left、Top、Width、Height；
                    rect = new Rect(window.Left,window.Top,window.Width,window.Height);
                    //*②设置窗口最大化（SystemParameters.WorkArea）获取工作区大小。
                    window.Top = 0;
                    window.Left = 0;
                    window.Width = SystemParameters.WorkArea.Width;
                    window.Height=SystemParameters.WorkArea.Height;
                    //*③同时更改bool的变量 = true
                    blMax = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 关闭应用程序
        /// </summary>
        private void CloseApplication()
        {
            /*
            * 思路：
            * 1、对话框确认退出系统设置；
            * 2、根据员工ID查找修改登录日志表列表
            * 3、Last()函数取出最后一条数据；
            * 4、计算登录时长
            * （1）、获取当前时间、最后一条数据的登录时间；
            * （2）、获取当期时间-登录时间，等到时间间隔TimeSpan；
            * （3）、登录时长计算四种情况：分别获取时间间隔的天Days、小时Hours、分钟Minutes、秒Seconds；
            *           ①天为单位,条件Days>0
            *           ②小时为单位，条件Days==0 && Hours>0
            *           ③分钟为单位，条件Days==0 && Hours==0 && Minutes>0
            *           ④秒为单位 ，条件Days==0 && Hours==0 && Minutes==0 && Seconds>0
            * 3、修改登录日志信息表：S_TheLoginDetails（离线时间和登录时长）
            * 4、保存更改数据成功，则关闭应用程序。
            */
            try
            {
                //*1、对话框确认退出系统设置；
                if (MessageBox.Show("是否要退出应用程序？", "系统提示：", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
                {
                    //*2、根据员工ID查找修改登录日志表列表
                    var list = myModel.S_TheLoginDetails.Where(o => o.staffID == StaffID).ToList();
                    //* 3、Last()函数取出最后一条数据；
                    S_TheLoginDetails s_TheLogin = list.Last();
                    //*4、计算登录时长
                    //* （1）、获取当前时间、最后一条数据的登录时间；
                    DateTime dtNow = DateTime.Now;
                    DateTime dtLoginTime = Convert.ToDateTime(s_TheLogin.logonTime);
                    //* （2）、获取当期时间 - 登录时间，等到时间间隔TimeSpan；
                    TimeSpan timeSpan= dtNow - dtLoginTime;
                    string strTimes = "";
                    //* （3）、登录时长计算四种情况：分别获取时间间隔的天Days、小时Hours、分钟Minutes、秒Seconds；
                    //*①天为单位,条件Days > 0
                    if (timeSpan.Days>0)
                    {
                        strTimes= timeSpan.Days+"天"+ timeSpan.Hours+ "小时" +timeSpan.Minutes + "分钟" + timeSpan.Seconds + "秒";
                    }
                    //*②小时为单位，条件Days == 0 && Hours > 0
                    if (timeSpan.Days == 0 && timeSpan.Hours>0)
                    {
                        strTimes =  timeSpan.Hours + "小时" + timeSpan.Minutes + "分钟" + timeSpan.Seconds + "秒";
                    }
                    //*③分钟为单位，条件Days == 0 && Hours == 0 && Minutes > 0
                    if (timeSpan.Days == 0 && timeSpan.Hours == 0&& timeSpan.Minutes>0)
                    {
                        strTimes =  timeSpan.Minutes + "分钟" + timeSpan.Seconds + "秒";
                    }
                    //*④秒为单位 ，条件Days == 0 && Hours == 0 && Minutes == 0 && Seconds > 0
                    if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes == 0&& timeSpan.Seconds>0)
                    {
                        strTimes = timeSpan.Seconds + "秒";
                    }
                    //* 3、修改登录日志信息表：S_TheLoginDetails（离线时间和登录时长）
                    s_TheLogin.offlineTime = DateTime.Now;
                    s_TheLogin.theLogin = strTimes;
                    myModel.Entry(s_TheLogin).State=System.Data.Entity.EntityState.Modified;
                    //*4、保存更改数据成功，则关闭应用程序。
                    if (myModel.SaveChanges()>0)
                    {
                        Application.Current.Shutdown();
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
        #region 二【员工基本信息绑定】
        #region 【1、属性】
        private int staffID;
        /// <summary>
        /// 员工ID
        /// </summary>
        public int StaffID
        {
            get { return staffID; }
            set {
                if (staffID != value)
                {
                    staffID = value;
                    RaisePropertyChanged(() => StaffID);
                }
            }
        }

        private S_Staff staffEntity;
        /// <summary>
        /// 员工实体
        /// </summary>
        public S_Staff StaffEntity
        {
            get { return staffEntity; }
            set {
                if (staffEntity != value)
                {
                    staffEntity = value;
                    RaisePropertyChanged(() => StaffEntity);
                }
            }
        }

        private BitmapImage imgPicture;
        /// <summary>
        /// 员工图片
        /// </summary>
        public BitmapImage ImgPicture
        {
            get { return imgPicture; }
            set {
                if (imgPicture != value)
                {
                    imgPicture = value;
                    RaisePropertyChanged(() => ImgPicture);

                }
            }
        }

        #endregion
        #region 【2、命令】
        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand<TabControl> LoadedCommand { get; set; }
        /// <summary>
        /// 个人信息查询
        /// </summary>
        public RelayCommand PersonalDetailsCommand { get; set; }
        #endregion
        #region 【1、方法】
        private void Loaded(TabControl tabControl)
        {
            //查询员工基本信息
            SelectStaff();
            //获取选项卡
            TC = tabControl;
            //首页           
            HomePage(tabControl);
        }
        /// <summary>
        /// 查询员工基本信息
        /// </summary>
        private void SelectStaff()
        {
            /*
           * 思路
              1、根据登录人员工ID查询员工信息；   

              2、调用封装【byte[] 转换为BitmapImage】的方法绑定员工图片
           */
            try
            {
                //1、根据登录人员工ID查询员工信息；
                StaffEntity = myModel.S_Staff.Find(StaffID);
                //2、调用封装【byte[] 转换为BitmapImage】的方法绑定员工图片
                ImgPicture = ByteToBitmapimage.Bytearraytobitmapimage(StaffEntity.picture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// 个人信息查询
        /// </summary>
        private void PersonalDetails()
        {
            try
            {
                //个人信息窗口
                StaffInformation staff = new StaffInformation();
                var InformationViewModel = staff.DataContext as Staffs.StaffInformationViewModel;
                InformationViewModel.StaffID = StaffID;//员工ID
                staff.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #endregion
        #region 三【页面嵌套】
        
        #region 【2、命令】
        /// <summary>
        /// 首页命令
        /// </summary>
        public RelayCommand<TabControl> HomePageCommand { get; set; }
        /// <summary>
        /// 打开员工页面命令
        /// </summary>
        public RelayCommand<TabControl> StaffPageCommand { get; set; }
        /// <summary>
        /// 打开账户管理页面命令
        /// </summary>
        public RelayCommand<TabControl> AccountPageCommand { get; set; }
        /// <summary>
        /// 打开开台页面命令
        /// </summary>
        public RelayCommand<TabControl> OpenPageCommand { get; set; }
        /// <summary>
        /// 打开菜单页面命令
        /// </summary>
        public RelayCommand<TabControl> MenuPageCommand { get; set; }
        /// <summary>
        /// 打开餐桌页面命令
        /// </summary>
        public RelayCommand<TabControl> DiningPageCommand { get; set; }

        /// <summary>
        /// 打开记录页面命令
        /// </summary>
        public RelayCommand<TabControl> RecordPageCommand { get; set; }
        /// <summary>
        /// 打开会员管理页面命令
        /// </summary>
        public RelayCommand<TabControl> MemberPageCommand { get; set; }
        /// <summary>
        /// 打开会员消费记录页面命令
        /// </summary>
        public RelayCommand<TabControl> TrackerPageCommand { get; set; }
        /// <summary>
        /// 打开消息记录页面命令
        /// </summary>
        public RelayCommand<TabControl> InformationCommand { get; set; }

        /// <summary>
        /// 打开权限管理页面命令
        /// </summary>
        public RelayCommand<TabControl> PermissionCommand { get; set; }
        /// <summary>
        /// 打开权限组用户页面命令
        /// </summary>
        public RelayCommand<TabControl> PermissionUserCommand { get; set; }

        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.0用户控件嵌套
        /// </summary>
        private void Init()
        {
            //首页
            HomePageCommand = new RelayCommand<TabControl>(HomePage, obj => obj != null);
            //员工
            StaffPageCommand = new RelayCommand<TabControl>(StaffPage, obj => obj != null);
            //账户
            AccountPageCommand = new RelayCommand<TabControl>(AccountPage, obj => obj != null);
            //开台
            OpenPageCommand = new RelayCommand<TabControl>(OpenPage, obj => obj != null);
            //菜单
            MenuPageCommand = new RelayCommand<TabControl>(MenuPage, obj => obj != null);
            //餐桌
            DiningPageCommand = new RelayCommand<TabControl>(DiningPage, obj => obj != null);
            //记录
            RecordPageCommand = new RelayCommand<TabControl>(RecordPage, obj => obj != null);
            //会员
            MemberPageCommand = new RelayCommand<TabControl>(MemberPage, obj => obj != null);
            //会员消费记录
            TrackerPageCommand = new RelayCommand<TabControl>(TrackerPage, obj => obj != null);
            //消息
            InformationCommand = new RelayCommand<TabControl>(Information, obj => obj != null);
            //权限管理
            PermissionCommand = new RelayCommand<TabControl>(Permission, obj => obj != null);
            //权限用户
            PermissionUserCommand = new RelayCommand<TabControl>(PermissionUser, obj => obj != null);
        }

        /// <summary>
        /// 选项卡嵌套选项
        /// </summary>
        /// <param name="strName">选项标头</param>
        /// <param name="userControl">用户控件</param>
        public static void AddItem(string strName,UserControl userControl)
        {
            /*
            * 思路：
            * 1、声明局部bool类型的变量blRepeat，默认赋值false（不重复）；
            * 2、选项卡存在两种情况
            * （1）重复点击
            * 判断当前选项卡的Items.Count总数是否大于0
            * ①for循环选项卡，判断子选项的名称Name属性值==当前点击选项名称，相等选中子选项并赋值bool类型的变量=true,break打破循环；
            * ②判断bool类型的变量 == false，创建子选项（代码和【首次点击】一样）
            * 
            * （2）首次点击
            * ①实例化自定义带有关闭按钮的子选项UCTabItemWithClose
            * ②子选项对象的属性赋值（Name名称、Header标头名称、Content用户控件、Margin外边距、Height标头高度28）
            * ③TabControl选项卡添加子选项
            * ④选中子选项
            * 
            */
            try
            {
                //*1、声明局部bool类型的变量blRepeat，默认赋值false（不重复）；
                bool blRepeat = false;
                //*2、选项卡存在两种情况
                //* 选显卡有内容
                if (TC.Items.Count>0)
                {
                    //（1）重复点击
                    //* 判断当前选项卡的Items.Count总数是否大于0
                    for (int i = 0; i < TC.Items.Count; i++)
                    {
                        //* ①for循环选项卡，判断子选项的名称Name属性值 == 当前点击选项名称，相等选中子选项并赋值bool类型的变量 = true,break打破循环；
                        if (((UCTabItemWithClose)TC.Items[i]).Name== strName)
                        {
                            TC.SelectedItem = (UCTabItemWithClose)TC.Items[i];
                            blRepeat = true;
                            break;
                        }
                    }
                    if (blRepeat==false)
                    {
                        //* ②判断bool类型的变量 == false，创建子选项（代码和【首次点击】一样）
                        //* ①实例化自定义带有关闭按钮的子选项UCTabItemWithClose
                        UCTabItemWithClose itemWithClose = new UCTabItemWithClose
                        {
                            //* ②子选项对象的属性赋值（Name名称、Header标头名称、Content用户控件、Margin外边距、Height标头高度28）
                            Name = strName,
                            Header = string.Format(strName),
                            Content = userControl,
                            Margin = new Thickness(0, 0, 1, 0),
                            Height = 28,
                        };
                        //* ③TabControl选项卡添加子选项
                        TC.Items.Add(itemWithClose);
                        //* ④选中子选项
                        TC.SelectedItem = itemWithClose;
                    }

                }
                //* （2）首次点击
                else
                {
                    //* ①实例化自定义带有关闭按钮的子选项UCTabItemWithClose
                    UCTabItemWithClose itemWithClose = new UCTabItemWithClose
                    {
                        //* ②子选项对象的属性赋值（Name名称、Header标头名称、Content用户控件、Margin外边距、Height标头高度28）
                        Name = strName,
                        Header = string.Format(strName),
                        Content = userControl,
                        Margin = new Thickness(0, 0, 1, 0),
                        Height = 28,                       
                    };
                    //* ③TabControl选项卡添加子选项
                    TC.Items.Add(itemWithClose);
                    //* ④选中子选项
                    TC.SelectedItem = itemWithClose;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #region 【页面嵌套】
        /// <summary>
        /// 1、首页
        /// </summary>
        /// <param name="obj"></param>
        private void HomePage(TabControl tc)
        {
            TC = tc;
            //实例化首页页面
            HomePage myHome = new HomePage();
            AddItem("首页", myHome);
            //绑定登录人员信息
            SelectStaff();
            //消息提醒
            SelectInformationYK();
        }
        /// <summary>
        /// 2、员工管理
        /// </summary>
        /// <param name="obj"></param>
        private void StaffPage(TabControl tc)
        {
            TC = tc;
            //实例化员工管理页面         
            Staff myStaff = new Staff();
            //嵌套页面
            AddItem("员工管理", myStaff);
        }
        /// <summary>
        /// 3、账户管理
        /// </summary>
        /// <param name="obj"></param>
        private void AccountPage(TabControl tc)
        {
            TC = tc;
            Account myAccount = new Account();
            AddItem("账户管理", myAccount);
        }
        /// <summary>
        /// 4、开台
        /// </summary>
        /// <param name="obj"></param>
        private void OpenPage(TabControl tc)
        {
            TC = tc;
            Open myOpen = new Open();
            AddItem("开台", myOpen);
        }
        /// <summary>
        /// 5、菜单管理
        /// </summary>
        /// <param name="obj"></param>
        private void MenuPage(TabControl tc)
        {
            TC = tc;
            Views.Menu myMenu = new Views.Menu();
            AddItem("菜单管理", myMenu);
        }
        /// <summary>
        /// 6、餐桌管理
        /// </summary>
        /// <param name="obj"></param>
        private void DiningPage(TabControl tc)
        {
            TC = tc;
            DiningTable myDining = new DiningTable();
            AddItem("餐桌管理", myDining);
        }
        /// <summary>
        /// 7、登录记录
        /// </summary>
        /// <param name="obj"></param>
        private void RecordPage(TabControl tc)
        {
            TC = tc;
            Record myRecord = new Record();
            AddItem("登录记录", myRecord);
        }
        /// <summary>
        /// 8、会员管理
        /// </summary>
        /// <param name="obj"></param>
        private void MemberPage(TabControl tc)
        {
            TC = tc;
            Member myMember = new Member();
            AddItem("会员管理", myMember);
        }
        /// <summary>
        /// 9、会员消费记录
        /// </summary>
        /// <param name="obj"></param>
        private void TrackerPage(TabControl tc)
        {
            TC = tc;
            XpenseTracker myTracker = new XpenseTracker();
            AddItem("消费记录", myTracker);
        }
        /// <summary>
        /// 10、消息
        /// </summary>
        /// <param name="obj"></param>
        private void Information(TabControl tc)
        {
            TC = tc;
            Information myTracker = new Information();           
            AddItem("消息", myTracker);
        }
        /// <summary>
        /// 11、权限管理
        /// </summary>
        /// <param name="obj"></param>
        private void Permission(TabControl tc)
        {
            TC = tc;
            Views.PermissionUsers.Permission myPermission = new Views.PermissionUsers.Permission();
            AddItem("权限管理", myPermission);

        }
        /// <summary>
        /// 12、权限用户
        /// </summary>
        /// <param name="obj"></param>
        private void PermissionUser(TabControl tc)
        {
            TC = tc;
            PermissionUser myPermissionUser = new PermissionUser();
            AddItem("权限用户", myPermissionUser);
        }

        #endregion

        #endregion
        #endregion
        #region 四【信息提醒】
        #region 【1、属性】
        private int newsShow;
        /// <summary>
        /// 消息框透明度
        /// </summary>
        public int NewsShow
        {
            get { return newsShow; }
            set
            {
                if (newsShow != value)
                {
                    newsShow = value;
                    RaisePropertyChanged(() => NewsShow);

                }; }
        }

        private string news;
        /// <summary>
        /// 数值绑定：消息条数
        /// </summary>
        public string News
        {
            get { return news; }
            set
            {
                if (news != value)
                {
                    news = value;
                    RaisePropertyChanged(() => News);

                };
            }
        }
        #endregion
        #region 【2、命令】

        #endregion
        #region 【1、方法】
        private void SelectInformationYK()
        {
            /*
           * 思路：
           * 1、条件为（待查看、登录人登录、发送状态=true）信息的筛选信息数据；
           * 2、判断数据总条数>0
           * (1)是，显示信息图标及条数，同时弹出提示框是否要跳转到消息页，“是”则跳转。
           * (2)否，隐藏信息图标。
           */           
            var listInformation = (from tbInformation in myModel.S_Information
                                   join tbStaffReceive in myModel.S_Staff on tbInformation.staffReceiveID equals tbStaffReceive.staffID//接收人
                                   where tbInformation.status == "待查看" && tbInformation.staffReceiveID == StaffID && tbInformation.staffIDNview == true
                                   select tbInformation).ToList();
            if (listInformation.Count > 0)
            {
                NewsShow = 1;//显示
                News = listInformation.Count.ToString();//获取数量
                if (listInformation.Count > 99)
                {
                    News = "99+";
                }
                MessageBoxResult dr = MessageBox.Show("你有未查看信息,是否立即查看", "系统提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dr == MessageBoxResult.OK)
                {
                    //跳到消息页
                    Information myTracker = new Information();
                    //嵌套选项
                    AddItem("消息", myTracker);
                }
            }
            else
            {
                NewsShow = 0;//隐藏
            }
        }
        #endregion
        #endregion
    }
}
