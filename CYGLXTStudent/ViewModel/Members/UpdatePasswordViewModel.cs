using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace CYGLXTStudent.ViewModel.Members
{
    /// <summary>
    /// 修改密码
    /// </summary>
    public class UpdatePasswordViewModel: ViewModelBase
    {
        public UpdatePasswordViewModel()
        {
            //加载命令
            LoadedCommand = new RelayCommand(SelectVipPassword);
            //关闭窗口
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow,win=>win!=null);
            //旧密码KeyUp命令
            OldKeyUpCommand = new RelayCommand(OldPassword);
            //新密码KeyUp命令
            NewKeyUpCommand = new RelayCommand(NewPassword);
            //确认密码KeyUp命令
            ConfirmKeyUpCommand = new RelayCommand(ConfirmPassword);
            //保存密码
            SaveCommand = new RelayCommand<Window>(SavePassword, win => win != null);
        }
        #region 1、【属性】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModels = new CYGLXTEntities();
        #region 页面参数传递
        /// <summary>
        /// 字段：会员卡ID(接收主页面传递过来的主键ID)
        /// </summary>
        private int vIPID;
        /// <summary>
        /// 属性：会员卡ID(接收主页面传递过来的主键ID)
        /// </summary>
        public int VIPID
        {
            get { return vIPID; }
            set
            {
                if (vIPID != value)
                {
                    vIPID = value;
                    RaisePropertyChanged(() => VIPID);

                }
            }
        }
        /// <summary>
        /// 字段：会员卡扩展实体（用于获取选中实体）
        /// </summary>
        private VipVo currentVIPVoEntity;
        /// <summary>
        /// 属性：会员卡扩展实体（用于获取选中实体）
        /// </summary>
        public VipVo CurrentVIPVoEntity
        {
            get { return currentVIPVoEntity; }
            set
            {
                if (currentVIPVoEntity != value)
                {
                    currentVIPVoEntity = value;
                    RaisePropertyChanged(() => CurrentVIPVoEntity);

                }
            }
        }
        #endregion
        #endregion
        #region 2、【命令】
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 定义保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        /// <summary>
        /// 旧密码KeyUp命令
        /// </summary>
        public RelayCommand OldKeyUpCommand { get; set; }
        /// <summary>
        /// 新密码KeyUp命令
        /// </summary>
        public RelayCommand NewKeyUpCommand { get; set; }
        /// <summary>
        /// 确认密码KeyUp命令
        /// </summary>
        public RelayCommand ConfirmKeyUpCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 页面加载（回填旧密码）
        /// </summary>
        private void SelectVipPassword()
        {
            try
            {
                //提取回填数据
                var dbVipVo = (from tb in myModels.S_VIP
                               where tb.VIPID == VIPID
                               select new VipVo
                               {
                                   VIPID = tb.VIPID,//会员卡ID
                                   password=tb.password,//密码
                               }).Single();
                //属性赋值
                CurrentVIPVoEntity = dbVipVo;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.1 关闭窗口
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        /// <summary>
        /// 3.2 旧密码验证
        /// </summary>
        private void OldPassword()
        {
            /*
            思路：
           1、判断新密码不为空；
           2、获取密码
           3、正则表达式判断密码准确性（密码由小于等于6位数字组成）
            */

            /*
               * 密码校验常用正则表达式

              (1) 长度至少为8，至少含有一个字母和一个数字
               "^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$"

              (2)长度至少为8，至少含有一个字母和一个数字和一个特殊字符
              "^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$"

              (3)长度至少为8，且至少有一个数字 并同时包含大小写字母
              "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$"

              (4)长度至少为8,包含大小写字母、数字和特殊字符
              "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"

              (5)长度8到10，, 包含大小写数字和特殊字符
              "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,10}$"

            */
            try
            {
                //1、判断新密码不为空；
                if (!string.IsNullOrEmpty(CurrentVIPVoEntity.password))
                {
                    string strOldPassword = CurrentVIPVoEntity.password;
                    if (!Regex.IsMatch(strOldPassword,@"^[0-9][0-9]*$"))
                    {
                        CurrentVIPVoEntity.password = string.Empty;
                        MessageBox.Show("密码由小于等于6位数字组成！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;                       
                    }
                    //判断密码是否准确
                    string strPassword = myModels.S_VIP.Find(VIPID).password;
                    if (strPassword != strOldPassword)
                    {
                        MessageBox.Show("密码错误！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }               
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.3 新密码验证
        /// </summary>
        private void NewPassword()
        {
            try
            {
                //1、判断新密码不为空；
                if (!string.IsNullOrEmpty(CurrentVIPVoEntity.TxtNewPassword))
                {
                    string strNewPassword = CurrentVIPVoEntity.TxtNewPassword;
                    if (!Regex.IsMatch(strNewPassword, @"^[0-9][0-9]*$"))
                    {
                        CurrentVIPVoEntity.TxtNewPassword = string.Empty;
                        MessageBox.Show("新密码由小于等于6位数字组成！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.4 确认密码验证
        /// </summary>
        private void ConfirmPassword()
        {
            try
            {
                //1、判断新密码不为空；
                if (!string.IsNullOrEmpty(CurrentVIPVoEntity.TxtConfirmPassword))
                {
                    string strConfirmPassword = CurrentVIPVoEntity.TxtConfirmPassword;
                    if (!Regex.IsMatch(strConfirmPassword, @"^[0-9][0-9]*$"))
                    {
                        CurrentVIPVoEntity.TxtConfirmPassword = string.Empty;
                        MessageBox.Show("确认密码由小于等于6位数字组成！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 修改秘密
        /// </summary>
        /// <param name="window">窗口</param>
        private void SavePassword(Window window)
        {
            /*
                思路：
                   1、必填项不能为空(带星号)；
                   2、获取输入值（旧密码、新密码、确认密码）；
                   3、linq语句或者lambda表达式，根据会员卡ID查询会员密码 ；          
                   4、判断输入旧密码==查询会员密码；
                   4、判断确认密码==新密码；
                   5、修改会员卡表S_VIP（密码==新密码）；
                */
            try
            {
                //1、必填项不能为空(带星号)；
                if (!string.IsNullOrEmpty(CurrentVIPVoEntity.TxtNewPassword) && !string.IsNullOrEmpty(CurrentVIPVoEntity.TxtConfirmPassword))
                {
                    //2、获取输入值（旧密码、新密码、确认密码）；
                    //3、linq语句或者lambda表达式，根据会员卡ID查询会员密码 ；
                    S_VIP s_VIP = myModels.S_VIP.Find(VIPID);
                    //4、判断输入旧密码 == 查询会员密码；
                    if (s_VIP.password==CurrentVIPVoEntity.password)
                    {

                        //4、判断确认密码 == 新密码；
                        if (CurrentVIPVoEntity.TxtNewPassword== CurrentVIPVoEntity.TxtConfirmPassword)
                        {
                            //5、修改会员卡表S_VIP（密码 == 新密码）；
                            s_VIP.password = CurrentVIPVoEntity.TxtNewPassword;
                            myModels.Entry(s_VIP).State=System.Data.Entity.EntityState.Modified;
                            if (myModels.SaveChanges()>0)
                            {
                                MessageBox.Show("密码修改成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                window.Close();
                            }
                            else
                            {
                                MessageBox.Show("密码修改失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                        else
                        {
                            MessageBox.Show("新密码和确认密码不匹配！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                       
                    }
                    else
                    {
                        MessageBox.Show("旧密码错误！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                  
                }
                else
                {
                    MessageBox.Show("带红色星号的是必填项不能为空！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);

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
