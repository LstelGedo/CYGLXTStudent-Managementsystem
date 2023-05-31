using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace CYGLXTStudent.ViewModel.Members
{
    /// <summary>
    /// VIP会员卡
    /// </summary>
    public class VipAddOrEditViewModel : ViewModelBase
    {
        public VipAddOrEditViewModel()
        {
            //直接绑定
            SelectComboboxMember();
            //自动生成卡号
            SelectionChangedCommand = new RelayCommand<ComboBox>(CreateVipNo, obo => obo != null);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, win => win != null);
            ResetCommand = new RelayCommand(ResetWindow);
            //验证电话号码
            ValidatePhoneCommand = new RelayCommand(ValidatePhone);
            //保存
            SaveCommand = new RelayCommand<Window>(SaveVip, win => win != null);
        }
        
        #region 【1、属性】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();

        /// <summary>
        /// 字段：VIP实体（用于获取主页面传递过来的选中实体）
        /// </summary>
        private S_VIP currentVIPEntity;
        /// <summary>
        /// 属性：VIP实体（用于获取主页面传递过来的选中实体）
        /// </summary>
        public S_VIP CurrentVIPEntity
        {
            get { return currentVIPEntity; }
            set
            {
                if (currentVIPEntity != value)
                {
                    currentVIPEntity = value;
                    RaisePropertyChanged(() => CurrentVIPEntity);

                }
            }
        }
        /// <summary>
        /// 字段：新增和修改的标志，默认修改
        /// </summary>
        private bool isAdd = false;
        /// <summary>
        /// 属性：新增和修改的标志，默认修改
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
                }
            }
        }
        /// <summary>
        /// 字段：卡号（自动生成）
        /// </summary>
        private string carNumber;
        /// <summary>
        /// 属性：卡号（自动生成）
        /// </summary>
        public string CarNumber
        {
            get { return carNumber; }
            set
            {
                if (carNumber != value)
                {
                    carNumber = value;
                    RaisePropertyChanged(() => CarNumber);
                }
            }
        }
        /// <summary>
        /// 字段：会员类型列表(绑定下拉框)
        /// </summary>
        private List<S_Member> members;
        /// <summary>
        /// 属性：会员类型列表(绑定下拉框)
        /// </summary>
        public List<S_Member> Members
        {
            get { return members; }
            set
            {
                if (members != value)
                {
                    members = value;
                    RaisePropertyChanged(() => Members);
                }
            }
        }

        #endregion
        #region 【2、命令】
        /// <summary>
        /// 会员类型下拉框改变命令
        /// </summary>
        public RelayCommand<ComboBox> SelectionChangedCommand { get; set; }
        /// <summary>
        /// 验证电话命令
        /// </summary>
        public RelayCommand ValidatePhoneCommand { get; set; }
        /// <summary>
        /// 验证密码命令
        /// </summary>
        public RelayCommand ValidatePasswordCommand { get; set; }
        /// <summary>
        /// 定义保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 重置命令
        /// </summary>
        public RelayCommand ResetCommand { get; set; }
        /// <summary>
        /// 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 会员类型（下拉框）
        /// </summary>
        private void SelectComboboxMember()
        {
            try
            {
                //实例化实体列表
                List<S_Member> list = new List<S_Member>();
                //添加数据
                list.Add(new S_Member { memberID = 0, type = "----请选择----" });
                //查询数据数据
                var lists = from tbMember in myModel.S_Member select tbMember;
                //数据合并
                list.AddRange(lists);
                //绑定属性
                Members = list.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 自动生成卡号
        /// </summary>
        /// <param name="box">下拉框</param>
        private void CreateVipNo(ComboBox box)
        {
            /*
             * 思路：
                1、卡号属性默认为空、卡号前缀默认为空；
                2、判断下拉框选项不为空，获取会员类型ID
                3、根据会员类型ID，查询会员类型名type；
                4、引用拼音简码方法，使用类型名充当：卡号前缀；
                5、根据会员类型ID查询卡号，判断是否查到数据
                （1）否，首次使用类型，默认卡号从 1开始；
                    ①、卡号赋值= 前缀 + 0001
                    ②、页面属性赋值。
                （2）是，存在该类型卡号，Last()获取最后一条数据的最大卡号，
                    ①、编号=截取字符串的后3位，
                    ②、新编号=最大编号 + 1 ，
                    ③、编号存在四种情况，使用switch分支语句操作，
                    ④、卡号：前缀 + 新编号
                    ⑤、页面属性赋值。
             */

            try
            {
                //1、卡号属性默认为空、卡号前缀默认为空；
                CarNumber = String.Empty;
                string strQZ = string.Empty;
                //2、判断下拉框选项不为空，获取会员类型ID
                if (box.SelectedValue!=null)
                {
                    int intMemberID =Convert.ToInt32(box.SelectedValue);
                    //3、根据会员类型ID，查询会员类型名type；
                    string strType = myModel.S_Member.Where(o => o.memberID == intMemberID).Single().type;
                    //4、引用拼音简码方法，使用类型名充当：卡号前缀；
                    strQZ = Resources.PublicClass.PublicStaticMethod.GetChineseSpell(strType);
                    //5、根据会员类型ID查询卡号，判断是否查到数据
                    var listVip = myModel.S_VIP.Where(o => o.memberID == intMemberID).ToList();
                    if (listVip.Count>0)
                    {
                        //（2）是，存在该类型卡号，Last()获取最后一条数据的最大卡号，
                        string strBig = listVip.Last().cardNo;
                        //    ①、编号 = 截取字符串的后3位，                        
                        string strNmuber = strBig.Substring(strBig.Length - 4, 4);
                        //    ②、新编号 = 最大编号 + 1 ，
                        string intNewNumber = (Convert.ToInt32(strNmuber) + 1).ToString();
                        //    ③、编号存在四种情况，使用switch分支语句操作，
                        //000 00 0 
                       
                        //    ⑤、页面属性赋值。
                        switch (intNewNumber.Length)
                        {
                            case 4:
                                CarNumber = strQZ + intNewNumber;
                                break;
                            case 3:
                                CarNumber = strQZ + "0"+ intNewNumber;
                                break;
                            case 2:
                                CarNumber = strQZ +"00"+ intNewNumber;
                                break;
                            case 1:
                                CarNumber = strQZ +"000"+ intNewNumber;
                                break;
                            default:
                                break;
                        }
                        //    ④、卡号：前缀 + 新编号
                    }
                    else
                    {
                        //（1）否，首次使用类型，默认卡号从 1开始；
                        //    ①、卡号赋值 = 前缀 + 0001
                        //    ②、页面属性赋值。
                        CarNumber = strQZ + "0001";                       
                    }
                    CurrentVIPEntity.cardNo = CarNumber;
                    CurrentVIPEntity.memberID = intMemberID;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 关闭窗口
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
        /// 重置
        /// </summary>
        private void ResetWindow()
        {
            if (IsAdd)
            {
                CurrentVIPEntity = new S_VIP();
            }
        }
        /// <summary>
        /// 验证电话号码
        /// </summary>
        private void ValidatePhone()
        {
            /*
            * 思路：
               1、判断手机号不为空；
               2、获取手机号；
               3、判断手机号长度=11，Regex.IsMatch(手机号，正则)正则验证手机号，错误则清空手机号并提示；
               4、会员卡S_VIP表验证手机号重复性，重复则显示提示框提示。            
            */

            try
            {
                //1、判断手机号不为空；
                if (!string.IsNullOrEmpty(CurrentVIPEntity.phone))
                {
                    //2、获取手机号；
                    string strPhone = CurrentVIPEntity.phone;
                    //点好号码长度
                    if (strPhone.Length == 11)
                    {
                        //3、判断手机号长度 = 11，Regex.IsMatch(手机号，正则)正则验证手机号，错误则清空手机号并提示；
                        if (!Regex.IsMatch(strPhone, @"^0?(13[0-9]|14[5-9]|15[012356789]|166|17[0-8]|18[0-9]|19[89])[0-9]{8}$"))
                        {
                            CurrentVIPEntity.phone = string.Empty;
                            MessageBox.Show("电话有误！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        //4、会员卡S_VIP表验证手机号重复性，重复则显示提示框提示。(不比较自身)
                        int intCount = myModel.S_VIP.Where(o => o.phone == strPhone&& o.VIPID!=CurrentVIPEntity.VIPID).ToList().Count();
                        if (intCount > 0)
                        {
                            CurrentVIPEntity.phone = string.Empty;
                            MessageBox.Show("电话号码重复！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="window"></param>
        private void SaveVip(Window window)
        {
            /*
            * 思路：
            * 1、多表操作开启事务
            * 2、必填项不能为空
            * 3、实例化实体对象
            * 4、判断是否重复(不比较自身)
            * 两种情况：
            * （1）、新增
            *       ①新增：会员卡表：S_VIP 
            *       ②修改：会员类型表：S_Member  （修改数量）
            * （2）、修改：会员卡表：S_VIP 
            * 5、提交事务、提示、调用委托事件、关闭窗口
            */
            try
            {
                //*1、多表操作开启事务
                using (TransactionScope scope = new TransactionScope())
                {
                    //* 2、必填项不能为空
                    //2、必填项不能为空
                    if (CurrentVIPEntity.memberID > 0 && !string.IsNullOrEmpty(CarNumber)
                        && !string.IsNullOrEmpty(CurrentVIPEntity.name) && !string.IsNullOrEmpty(CurrentVIPEntity.phone)
                        && !string.IsNullOrEmpty(CurrentVIPEntity.password))
                    {
                        //* 3、实例化实体对象                       
                        S_VIP vipEntity = new S_VIP()
                        {
                            memberID = CurrentVIPEntity.memberID,//会员类型ID
                            name = CurrentVIPEntity.name,//会员卡名称
                            cardNo = CurrentVIPEntity.cardNo,// 卡号                           
                            phone = CurrentVIPEntity.phone,//电话                           
                            password = CurrentVIPEntity.password,//密码                            
                        };
                        //4、判断是否重复(不比较自身)
                        var userTypeCount = myModel.S_VIP.Where(m => m.cardNo == CurrentVIPEntity.cardNo.Trim() && m.VIPID != CurrentVIPEntity.VIPID).ToList().Count;
                        if (userTypeCount > 0)
                        {
                            MessageBox.Show("添加重复", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                        else
                        {
                            //（1）新增
                            if (IsAdd)
                            {
                                vipEntity.availableBalance = 0;// 可用余额
                                vipEntity.Integral = 0; //积分
                                vipEntity.state = "使用中";//状态
                                vipEntity.openingTime = DateTime.Now;//开通时间
                                //①新增：会员卡表：S_VIP 
                                myModel.S_VIP.Add(vipEntity);

                                //②修改：会员类型表：S_Member  （修改数量）
                                //查询修改会员信息 by 会员类型ID
                                S_Member dbMember = myModel.S_Member.Where(o => o.memberID == CurrentVIPEntity.memberID).Single();
                                dbMember.amount += 1;//dbMember.amount=dbMember.amount+1;
                                myModel.Entry(dbMember).State = EntityState.Modified;
                            }
                            else
                            {

                                vipEntity.availableBalance = CurrentVIPEntity.availableBalance;// 可用余额
                                vipEntity.Integral = CurrentVIPEntity.Integral; //积分
                                vipEntity.state = CurrentVIPEntity.state;//状态
                                vipEntity.openingTime =CurrentVIPEntity.openingTime;//开通时间
                                //主键ID
                                vipEntity.VIPID = CurrentVIPEntity.VIPID;
                                //（2）、修改：会员卡表：S_VIP
                                myModel.Entry(vipEntity).State = EntityState.Modified;
                            }
                            if (myModel.SaveChanges()>0)
                            {
                                // 5、提交事务、提示、关闭窗口
                                scope.Complete();
                                MessageBox.Show("会员卡保存成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                //关闭窗口
                                window.Close();
                            }
                            else
                            {
                                MessageBox.Show("会员卡保存失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("带星号的是必填项，不能为空！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
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
