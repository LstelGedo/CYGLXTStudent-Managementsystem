using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.Control;
using CYGLXTStudent.Views.Staffs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CYGLXTStudent.ViewModel.Staffs
{
    /// <summary>
    /// 员工新增/修改
    /// </summary>
    public class StaffAddOrEditViewModel: ViewModelBase
    {
        public StaffAddOrEditViewModel()
        {
            //关闭窗口
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, wd => wd != null);
            //浏览图片
            OpenCommand = new RelayCommand(BrowsePicture);
            //清空图片
            CleanCommand = new RelayCommand(CleanPicture);
            //重置员工信息
            ResetCommand = new RelayCommand(ResetStaff);
            //保存
            SaveCommand= new RelayCommand<Window>(SaveStaff, wd => wd != null);
        }
        #region 【全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 定义委托
        /// </summary>
        public delegate void RefreshDelegate();
        /// <summary>
        /// 定义事件
        /// </summary>
        public event RefreshDelegate RefreshEvent;

        /// <summary>
        /// 实例化二进制数组列表（接收图片）
        /// </summary>
        readonly List<byte[]> lstBytes = new List<byte[]>();
        #endregion
        #region 【1、属性】
        /// <summary>
        /// 私有字段
        /// </summary>
        private S_Staff currentStaffEntity;
        /// <summary>
        /// 属性：员工实体
        /// </summary>
        public S_Staff CurrentStaffEntity
        {
            get { return currentStaffEntity; }
            set
            {
                if (currentStaffEntity != value)
                {
                    currentStaffEntity = value;
                    RaisePropertyChanged(() => CurrentStaffEntity);
                } 
            }
        }

        private bool isAdd;
        /// <summary>
        /// 新增/修改标志
        /// </summary>
        public bool IsAdd
        {
            get { return isAdd; }
            set
            {
                if (isAdd != value)
                {
                    isAdd = value;
                    RaisePropertyChanged(() => IsAdd);
                }; }
        }

        private BitmapImage caseCoverImage;
        /// <summary>
        /// 员工图片
        /// </summary>
        public BitmapImage CaseCoverImage
        {
            get { return caseCoverImage; }
            set
            {
                if (caseCoverImage != value)
                {
                    caseCoverImage = value;
                    RaisePropertyChanged(() => CaseCoverImage);

                } }
        }


        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.1 关闭窗口命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        /// <summary>
        /// 2.2 浏览图片命令
        /// </summary>
        public RelayCommand OpenCommand { get; set; }
        /// <summary>
        /// 2.3 清空图片命令
        /// </summary>
        public RelayCommand CleanCommand { get; set; }
        /// <summary>
        /// 2.4 重置命令
        /// </summary>
        public RelayCommand ResetCommand { get; set; }
        /// <summary>
        /// 2.5 保存更改命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }

        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 关闭窗口
        /// </summary>
        /// <param name="window">窗口</param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.2 浏览图片并绑定
        /// </summary>
        private void BrowsePicture()
        {
            try
            {
                //1、声明流
                Stream stream;
                //(2)打开（文件框）
                OpenFileDialog open = new OpenFileDialog
                {
                    Multiselect = false,//不允许选择多张图片
                    //筛选文件类型（提示）
                    Filter = "ALL Image Files|*.*"
                };
                //(3)显示通用对话框
                if ((bool)open.ShowDialog())
                {
                    //选定的文件(选定的文件打开只读流)
                    if ((stream = open.OpenFile()) != null)
                    {
                        //获取文件长度（用字节表示的流长度 ）                                          
                        int length = (int)stream.Length;
                        //声明数组
                        byte[] bytes = new byte[length];
                        //读取文件（字节数组，从零开始的字节偏移量，读取的字节数）
                        stream.Read(bytes, 0, length);
                        //把数组添加到列表（临时记录）
                        lstBytes.Add(bytes);
                        //绑定图片
                        BitmapImage images = new BitmapImage(new Uri(open.FileName));  
                        //属性赋值
                        CaseCoverImage = images;
                    }
                }
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 清空图片
        /// </summary>
        private void CleanPicture()
        {
            lstBytes.Clear();
            CaseCoverImage = null;
        }
        /// <summary>
        /// 3.4 重置员工信息
        /// </summary>
        private void ResetStaff()
        {
            //判断是否新增操作
            if (IsAdd)
            {
                CurrentStaffEntity = new S_Staff();
                CaseCoverImage = null;
            }
        }
        /// <summary>
        /// 3.5 保存员工信息
        /// </summary>
        /// <param name="window">窗口</param>
        private void SaveStaff(Window window)
        {
            try
            {
                //一、新增
                //1、判断必填项不能为空
                if (string.IsNullOrEmpty(CurrentStaffEntity.EMPNO))
                {
                    MessageBox.Show("请输入工号", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (string.IsNullOrEmpty(CurrentStaffEntity.name))
                {
                    MessageBox.Show("请输入员工姓名", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                string strIDCard = CurrentStaffEntity.IDNumber.Trim();
                if (string.IsNullOrEmpty(strIDCard))
                {
                    MessageBox.Show("请输入员工身份证号", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //闰年出生日期的合法性正则表达式 || 平年出生日期的合法性正则表达式 
                else if (!Regex.IsMatch(strIDCard, 
                    @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|[1-2][0-9]))[0-9]{3}[0-9Xx]$)") ||
                    !Regex.IsMatch(strIDCard, @"(^[1-9][0-9]{5}(19|20)[0-9]{2}((01|03|05|07|08|10|12)(0[1-9]|[1-2][0-9]|3[0-1])|(04|06|09|11)(0[1-9]|[1-2][0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))[0-9]{3}[0-9Xx]$)"))
                {
                    MessageBox.Show("身份证不合法", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;                   
                }
                if (string.IsNullOrEmpty(CurrentStaffEntity.position))
                {
                    MessageBox.Show("请输入员工职务", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (string.IsNullOrEmpty(CurrentStaffEntity.statust))
                {
                    MessageBox.Show("请输入员工状态", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (CurrentStaffEntity.entryDate ==null)
                {
                    MessageBox.Show("请输入员工入职日期", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //2、判断数据准确性（身份证、电话）               
                /*
                  国内手机号码的规则:前3位为网络识别号；第4-7位为地区编码；第8-11位为用户号码。
                   现有手机号段:
                   移动：134 135 136 137 138 139 147 148 150 151 152 157 158 159 172 178 182 183 184 187 188 195 198 
                   联通：130 131 132 145 146 155 156 166 171 175 176 185 186  
                   电信：133 149 153 173 174 177 180 181 189 199 
                   虚拟运营商：170
                */
                if (!string.IsNullOrEmpty(CurrentStaffEntity.phone.Trim()))
                {
                    if (!Regex.IsMatch(CurrentStaffEntity.phone.Trim(), "^1(3\\d|4[5-9]|5[0-35-9]|6[567]|7[0-8]|8\\d|9[0-35-9])\\d{8}$"))
                    {
                        MessageBox.Show("电话号码不准确", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                }

                //3、判断重复性
                int intCount = myModel.S_Staff.Where(o => o.IDNumber == strIDCard && o.staffID != CurrentStaffEntity.staffID).Count();
                if (intCount > 0)
                {
                    MessageBox.Show("添加的员工信息重复！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                //提取上传的文件
                byte[] bytepicture = new byte[lstBytes.Count];
                for (int i = 0; i < lstBytes.Count; i++)
                {
                    bytepicture = lstBytes[i];
                }

                //实例化实体并赋值
                S_Staff staff = new S_Staff
                {
                    EMPNO=CurrentStaffEntity.EMPNO,
                    password=CurrentStaffEntity.EMPNO,
                    name=CurrentStaffEntity.name,
                    phone=CurrentStaffEntity.phone,
                    IDNumber=CurrentStaffEntity.IDNumber,
                    sex=CurrentStaffEntity.sex,
                    position=CurrentStaffEntity.position,
                    entryDate=CurrentStaffEntity.entryDate,
                    statust=CurrentStaffEntity.statust,
                    address=CurrentStaffEntity.address,
                    picture = bytepicture,
                };              
                if (IsAdd)
                {
                    //4、新增保存
                    myModel.S_Staff.Add(staff);
                    if (myModel.SaveChanges() > 0)
                    {
                        //5、提示用户，调用委托事件刷新数据。
                        MessageBox.Show(CurrentStaffEntity.name+"新增成功", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information); 
                        //调用委托事件（刷新主页面表格数据）
                        RefreshEvent?.Invoke();
                        //关闭窗口
                        window.Close();
                    }
                    else
                    {
                        MessageBox.Show(CurrentStaffEntity.name + "新增失败", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    //二、修改
                    staff.staffID = CurrentStaffEntity.staffID;
                    myModel.Entry(staff).State=System.Data.Entity.EntityState.Modified;
                    if (myModel.SaveChanges() > 0)
                    {
                        //5、提示用户，调用委托事件刷新数据。
                        MessageBox.Show(CurrentStaffEntity.name+"修改成功", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        //调用委托事件（刷新主页面表格数据）
                        RefreshEvent?.Invoke();
                        //关闭窗口
                        window.Close();
                    }
                    else
                    {
                        MessageBox.Show(CurrentStaffEntity.name + "修改失败", "系统提示", MessageBoxButton.OK, MessageBoxImage.Error);
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
