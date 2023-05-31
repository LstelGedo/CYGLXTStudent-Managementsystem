using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel.Members
{
    /// <summary>
    /// 会员卡退卡
    /// </summary>
    public class CancelMembershipViewModel : ViewModelBase
    {
        public CancelMembershipViewModel()
        {
            LoadedCommand = new RelayCommand(LoadedVip);
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow,win=>win!=null);
            SaveCommand = new RelayCommand<Window> (CancelMembership, win => win != null);
        }
        #region 【1、属性】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModels = new CYGLXTEntities();
        #region 页面参数传递
        /// <summary>
        ///字段： VIPID(接收主页面传递过来的值)
        /// </summary>
        private int vIPID;
        /// <summary>
        ///属性： VIPID(接收主页面传递过来的值)
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
        /// 字段：VIPVo实体（保存当前选中实体）
        /// </summary>
        private VipVo currentVIPVoEntity;
        /// <summary>
        /// 属性：VIPVo实体（保存当前选中实体）
        /// </summary>
        public VipVo CurrentVIPVoEntity
        {
            get { return currentVIPVoEntity; }
            set
            {
                if (currentVIPVoEntity != value)
                {
                    currentVIPVoEntity = value;
                    //RaisePropertyChanged的作用是当数据源改变的时候，会触发PropertyChanged事件达到通知UI更改的目的（ViewModel => View）。
                    RaisePropertyChanged(() => CurrentVIPVoEntity);

                }
            }
        }
        #endregion

        #endregion
        #region 【2、命令】
        /// <summary>
        ///2.1  Loaded命令
        /// </summary>
        public ICommand LoadedCommand { get; set; }
        /// <summary>
        ///2.2  定义保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        ///2.3  关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="window"></param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 加载会员卡信息（回填）
        /// </summary>
        private void LoadedVip()
        {
            try
            {
                //1、根据主键ID筛选回填会员卡信息
                var dbVipVo = (from tb in myModels.S_VIP
                               where tb.VIPID == VIPID
                               select new VipVo
                               {
                                   VIPID = tb.VIPID,//会员卡ID
                                   cardNo =tb.cardNo,//卡号
                                   name=tb.name,
                                   availableBalance=tb.availableBalance,

                               }).Single();
                //2、给实体复制（回填页面数据）
                CurrentVIPVoEntity = dbVipVo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 会员退卡
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void CancelMembership(Window window)
        {
            /*
               * 思路：
               1、 判断“输入密码”不为空，
               2、查询会员信息S_VIP by VIPID;
               3、提取会员信息数据（可用余额 decAvailableBalance、密码 strPassword）;
               4、判断比较“输入密码”与“会员信息密码”是否一致；
               5、判断会员信息“可用余额”，存在两种情况
               一、可用余额 > 0
               多表操作：开启事务
                     ① 修改账号表S_Account：总金额与冻结金额更新；
                     ② 新增会员退费记录S_AmountOfDetail；
                     ③ 修改会卡表S_VIP：Vip状态为已退卡、余额积分清零；
                     ④ 新增会员消费明细表B_VIPConsumptionDetails：退费明细记录。
                二、可用余额 <= 0
                     ① 修改会卡表S_VIP：Vip状态为已退卡、余额积分清零
               */
            try
            {
                //1、 判断“输入密码”不为空，
                if (!string.IsNullOrEmpty(CurrentVIPVoEntity.password))
                {
                    //2、查询会员信息S_VIP by VIPID;
                    var dbVip = (from tb in myModels.S_VIP
                                 where tb.VIPID == VIPID
                                 select tb).Single();
                    //3、提取会员信息数据（可用余额 decAvailableBalance、密码 strPassword）;
                    decimal? decAvailableBalance = dbVip.availableBalance;
                    string strPassword = dbVip.password.Trim();
                    //4、判断比较“输入密码”与“会员信息密码”是否一致；
                    if (strPassword == CurrentVIPVoEntity.password)
                    {
                        //5、判断会员信息“可用余额”，存在两种情况
                        //一、可用余额 > 0
                        if (decAvailableBalance>0)
                        {
                            //多表操作：开启事务
                            using (TransactionScope scope=new TransactionScope ())
                            {
                                //    ① 修改账号表S_Account：总金额与冻结金额更新；
                                S_Account s_Account = myModels.S_Account.Single();
                                decimal? decAccountFrozen = s_Account.AccountFrozen;//账号现有的冻结金额
                                decimal? decTotalMoney = s_Account.TotalMoney;//账号现有的总金额
                                s_Account.AccountFrozen = decAccountFrozen - decAvailableBalance;//更新冻结金额=账号现有的冻结金额-退卡金额
                                s_Account.TotalMoney = decTotalMoney - decAvailableBalance;//更新总金额=账号现有的总金额-退卡金额
                                myModels.Entry(s_Account).State = System.Data.Entity.EntityState.Modified;

                                //    ② 新增会员退费记录S_AmountOfDetail；
                                S_AmountOfDetail s_AmountOfDetail = new S_AmountOfDetail
                                {
                                    money= decAvailableBalance,//金额
                                    reason= "会员退费", //资金来由
                                    time=DateTime.Now,// 时间
                                    remark = "会员退费" + decAvailableBalance + "元，冻结金额减少" + decAvailableBalance + "元，总金额减少" + decAvailableBalance + "元）", //备注
                                    capitalPosition= "支出",// 资金状况
                                };
                                myModels.S_AmountOfDetail.Add(s_AmountOfDetail);

                                //    ③ 修改会卡表S_VIP：Vip状态为已退卡、余额积分清零；
                                S_VIP  s_VIP = myModels.S_VIP.Find(VIPID);
                                s_VIP.state = "已退卡";//状态
                                s_VIP.availableBalance = 0;//可用余额
                                s_VIP.Integral = 0;//积分清零
                                myModels.Entry(s_VIP).State = System.Data.Entity.EntityState.Modified;

                                //    ④ 新增会员消费明细表B_VIPConsumptionDetails：退费明细记录。
                                B_VIPConsumptionDetails b_VIPConsumptionDetails = new B_VIPConsumptionDetails
                                {
                                    consumptionTiming = DateTime.Now,//消费时间
                                    money = decAvailableBalance,//金额=可用余额
                                    VIPID = VIPID,//会员卡ID
                                    consumptionReason = "会员退费" + decAvailableBalance + "元",//资金来由
                                    consumptionType = "退费",//消费类型                                 
                                    //积分字段和订单ID字段不做操作
                                };
                                myModels.B_VIPConsumptionDetails.Add(b_VIPConsumptionDetails);
                                if (myModels.SaveChanges()>0)
                                {
                                    scope.Complete();
                                    MessageBox.Show("会员退卡成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                    window.Close();
                                }
                                else
                                {
                                    MessageBox.Show("会员退卡失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                        else
                        {
                            //二、可用余额 <= 0
                            //    ① 修改会卡表S_VIP：Vip状态为已退卡、余额积分清零
                            S_VIP s_VIP = myModels.S_VIP.Find(VIPID);
                            s_VIP.state = "已退卡";//状态
                            s_VIP.availableBalance = 0;//可用余额
                            s_VIP.Integral = 0;//积分清零
                            myModels.Entry(s_VIP).State = System.Data.Entity.EntityState.Modified;
                            if (myModels.SaveChanges() > 0)
                            {                             
                                MessageBox.Show("会员退卡成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                window.Close();
                            }
                            else
                            {
                                MessageBox.Show("会员退卡失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    else
                    {                        
                        MessageBox.Show("用户输入密码错误！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                }
                else
                {
                    MessageBox.Show("请用户输入密码确认退卡！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
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
