using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CYGLXTStudent.Model.Vo;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 登录ViewMoel
    /// </summary>
    public class LoginViewModel: ViewModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LoginViewModel()
        {
            //命令执行方法
            CalcelCommand = new RelayCommand(CloseApplication);
            //验证登录
            LoginCommand = new RelayCommand<Window>(CheckLogin, win => win != null);
            //加载（自动登录）
            LoadedCommand =new RelayCommand<Window> (Loaded, win => win != null);
        }
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        Model.CYGLXTEntities myModel = new Model.CYGLXTEntities();
        #region 【1、属性】
        /// <summary>
        /// 字段：账号
        /// </summary>
        private string userName;
        /// <summary>
        /// 属性：账号
        /// 属性其实就是外界访问私有字段的入口，属性本身不保存任何数据，在对属性赋值和读取的时候其实就是操作的对应私有字段;
        /// 属性本质其实就是一个方法，通过get和set方法来操作对应的字段.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set 
            {
                if (userName != value)
                {
                    userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        private string password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }
        /// <summary>
        /// 自动属性：记住账号
        /// </summary>
        public bool IsRemember { get; set; }
        /// <summary>
        /// 自动登录
        /// </summary>
        public bool IsLogin { get; set; }
        #endregion
        #region 【2、命令】
        /// <summary>
        ///2.1 自动属性：取消命令
        /// </summary>
        public RelayCommand CalcelCommand { get; set; }
        /// <summary>
        /// 2.2 验证登录命令：传递参数Window类型
        /// </summary>
        public RelayCommand<Window> LoginCommand { get; set; }
        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand<Window> LoadedCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 取消
        /// </summary>
        private void CloseApplication()
        {
            if (MessageBox.Show("是否关闭应用程序","系统提示：",MessageBoxButton.OKCancel,MessageBoxImage.Question)==MessageBoxResult.OK)
            {
                //关闭应用程序
                Application.Current.Shutdown();
            }
        }
        /// <summary>
        /// 3.2 验证登录
        /// </summary>
        /// <param name="window">登录窗口</param>
        private void CheckLogin(Window window)
        {
            /*
            * 思路：
            * 1、判断账号密码不为空；
            * 2、用账号作为条件查询员工基本信息，能查到数据则账号正确。
            * 3、用密码和数据库查到的密码进行比较，相等则正确。
            * 4、判断查到的用户状态是否为【离职】，离职则提醒，在职则继续下一步。
            * 5、新增员工登录记录S_TheLoginDetails。
            * 6、记住秘密、自动登录
            *   （1）、AppDomain获取基目录,Debug路径下创建一个文件RememberAccount.txt保存账号和密码；
            *   （2）记住秘密为true，
            *        ①File.Exists判断文件是否存在，不存在File.Create创建文件；
            *        ②实例化实体对象RememberAccount并赋值；
            *        ③调用封装类SerializerBinary序列化对象保存文件。
            *  7、实例化Main窗口并打开，传递参数(登录页面传递员工ID给主页面)，关闭Login窗口。
            */
            try
            {
                //1、判断账号密码不为空；
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (!string.IsNullOrEmpty(Password))
                    {
                        //2、用账号作为条件查询员工基本信息，能查到数据则账号正确。
                        var listStaff = myModel.S_Staff.Where(o => o.EMPNO == UserName).ToList();
                        if (listStaff.Count==1)
                        {
                            //3、用密码和数据库查到的密码进行比较，相等则正确。
                            if (listStaff[0].password == Password)
                            {
                                //4、判断查到的用户状态是否为【离职】，离职则提醒，在职则继续下一步。
                                if (listStaff[0].statust != "离职")
                                {
                                    //5、新增员工登录记录S_TheLoginDetails。
                                    S_TheLoginDetails theLoginDetails = new S_TheLoginDetails
                                    {
                                        staffID = listStaff[0].staffID,
                                        logonTime = DateTime.Now,
                                    };
                                    myModel.S_TheLoginDetails.Add(theLoginDetails);
                                    myModel.SaveChanges();
                                    //6、记住秘密、自动登录
                                    // （1）、AppDomain获取基目录,Debug路径下创建一个文件RememberAccount.txt保存账号和密码；
                                    string strPath = AppDomain.CurrentDomain.BaseDirectory + "RememberAccount.txt";
                                    //* （2）记住秘密为true，
                                    if (IsRemember==true)
                                    {                                        
                                        if (!File.Exists(strPath))
                                        {
                                            //*        ①File.Exists判断文件是否存在，不存在File.Create创建文件；
                                            File.Create(strPath).Close();
                                        }
                                    }
                                    // ②实例化实体对象RememberAccount并赋值；
                                    RememberAccount remember = new RememberAccount
                                    {
                                        MyUserName=UserName,
                                        MyPassword=Password,
                                        MyIsRemember=IsRemember,
                                        MyIsLogin=IsLogin,
                                    };
                                    //*        ③调用封装类SerializerBinary序列化对象保存文件。
                                    BinaryFormatter binary=new BinaryFormatter();
                                    using (FileStream fileStream=new FileStream (strPath,FileMode.OpenOrCreate))
                                    {
                                        binary.Serialize(fileStream, remember);
                                    }
                                    //*7、实例化Main窗口并打开，传递参数(登录页面传递员工ID给主页面)，关闭Login窗口。
                                    MainWindow main = new MainWindow();                                    
                                    MainViewModel mainView = main.DataContext as MainViewModel;
                                    //登录页面传递员工ID给主页面ViewModel
                                    mainView.StaffID=listStaff[0].staffID;
                                    main.Show();
                                    window.Close();

                                }
                                else
                                {
                                    MessageBox.Show("该员工已经离职，无权访问系统！", "系统系统：", MessageBoxButton.OK, MessageBoxImage.Stop);
                                }
                            }
                            else
                            {
                                MessageBox.Show("密码有误", "系统系统：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                        else
                        {
                            MessageBox.Show("账号有误", "系统系统：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请输入密码", "系统系统：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
                else
                {
                    MessageBox.Show("请输入账号", "系统系统：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统系统：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 加载（实现自动登录）
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void Loaded(Window window)
        {
            /*
            * 思路：
            * 1、AppDomain获取基目录,Debug路径，记住密码文件RememberAccount.txt
            * 2、声明RememberAccount对象接收=调用封装类SerializerBinary的反序列化文件对象方法
            * 3、对象不为空，获取对象信息：记住账号、自动登录;
            * 4、判断记住账号 IsRemember== true；则执行回填账号、密码
            * 5、自动登录IsLogin==true，调用验证登录方法，实现登录。
            */
            try
            {
                //*1、AppDomain获取基目录,Debug路径，记住密码文件RememberAccount.txt
                string strPath = AppDomain.CurrentDomain.BaseDirectory + "RememberAccount.txt";
                //* 2、声明RememberAccount对象接收 = 调用封装类SerializerBinary的反序列化文件对象方法
                BinaryFormatter formatter = new BinaryFormatter();
                //接受反序列化的实体对象
                RememberAccount remember = new RememberAccount();
                using (FileStream fileStream=new FileStream (strPath,FileMode.Open))
                {
                    remember= formatter.Deserialize(fileStream) as RememberAccount;
                }
                //* 3、对象不为空，获取对象信息：记住账号、自动登录;
                if (remember!=null)
                {
                    //记住账号、自动登录;
                    IsRemember = remember.MyIsRemember;
                    IsLogin = remember.MyIsLogin;
                    //*4、判断记住账号 IsRemember== true；则执行回填账号、密码
                    if (IsRemember==true)
                    {
                        UserName = remember.MyUserName;
                        Password = remember.MyPassword;
                    }
                    //* 5、自动登录IsLogin == true，调用验证登录方法，实现登录。
                    if (IsLogin == true)
                    {
                        //验证登录方法调用
                        CheckLogin(window);
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
