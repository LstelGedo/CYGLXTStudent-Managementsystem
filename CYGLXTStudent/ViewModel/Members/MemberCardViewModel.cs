using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CYGLXTStudent.Model.Vo;
using System.Text.RegularExpressions;
using System.Transactions;

namespace CYGLXTStudent.ViewModel.Members
{
    /// <summary>
    /// 会员卡充值
    /// </summary>
    public  class MemberCardViewModel:ViewModelBase
    {
        public MemberCardViewModel()
        {
            //加载
            LoadedCommand = new RelayCommand(WindowLoaded);
            //验证改变
            KeyUpCommand = new RelayCommand(TxtPhoneKeyUp);
            //关闭窗口
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, obj => obj != null);
            //充值
            SaveCommand=new RelayCommand<Window> (ButtonClick, obj => obj != null);
        }
        CYGLXTEntities myModels = new CYGLXTEntities();
        #region 1、【属性】

        /// <summary>
        /// 临时列表（用于记录会员卡数据）
        /// </summary>
        List<VipVo> VIP = new List<VipVo>();
        /// <summary>
        /// 字段：会员卡ID（接收主页面选中数据，传递过来的会员卡ID）
        /// </summary>
        private int vIPID;
        /// <summary>
        /// 属性：会员卡ID（接收主页面选中数据，传递过来的会员卡ID）
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
        /// 字段：会员卡扩展实体（用户获取选中会员卡信息）
        /// </summary>
        private VipVo selectVIPVoEntity;
        /// <summary>
        /// 属性：会员卡扩展实体（用户获取选中会员卡信息）
        /// </summary>
        public VipVo SelectVIPVoEntity
        {
            get { return selectVIPVoEntity; }
            set
            {
                if (selectVIPVoEntity != value)
                {
                    selectVIPVoEntity = value;
                    RaisePropertyChanged(() => SelectVIPVoEntity);

                }
            }
        }

        #endregion
        #region 2、【命令】
        /// <summary>
        /// Loaded命令
        /// </summary>
        public ICommand LoadedCommand { get; set; }
        /// <summary>
        /// KeyUp命令
        /// </summary>
        public ICommand KeyUpCommand { get; set; }
        /// <summary>
        /// 定义保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 窗口加载命令触发（会员充值页面数据回填）
        /// </summary>
        private void WindowLoaded()
        {
            try
            {
                //根据会员卡ID查询会员信息
                VIP = (from tbVip in myModels.S_VIP
                       where tbVip.VIPID == VIPID
                       select new VipVo
                       {
                           VIPID = tbVip.VIPID,//会员ID
                           cardNo = tbVip.cardNo,//卡号
                           name = tbVip.name,//姓名                       
                           availableBalance = tbVip.availableBalance,//可用余额
                       }).ToList();
                //获取第一条数据
                SelectVIPVoEntity = VIP[0];
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 3.2 关闭按钮
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
        /// 3.3  输入充值金额（更新可用余额、更新需支付金额）
        /// </summary>
        private void TxtPhoneKeyUp()
        {
            /*
            * 思路：
            * ①获取充值金额，判断不为空；
            * ②正则判断：充值金额是否为数值，否，则清零并return；
            * ③判断：可用余额不为0，获取可用余额；
            * ④充值后可用余额=可用余额+充值金额；
            * ⑤需支付金额=充值金额。
            */
            try
            {
                decimal decRemain = 0;//余额
                decimal decTopUp = 0;//充值金额

                // * ①获取充值金额，判断不为空；
                if (!string.IsNullOrEmpty(SelectVIPVoEntity.TxtMoney))
                {
                    string strMoney = SelectVIPVoEntity.TxtMoney;
                    //②正则判断：充值金额是否为数值，否，则清零；
                    if (!Regex.IsMatch(strMoney, @"^([1-9]\d{0,9}|0)([.]?|(\.\d{1,2})?)$"))
                    {
                        //清空金额
                        SelectVIPVoEntity.TxtMoney = "0";
                        return;
                    }
                    //* ③判断：可用余额不为0，获取可用余额；
                    if (SelectVIPVoEntity.availableBalance>0)
                    {
                        decRemain =Convert.ToDecimal(SelectVIPVoEntity.availableBalance);
                    }
                    decTopUp = Convert.ToDecimal(strMoney);
                    //* ④充值后可用余额 = 可用余额 + 充值金额；
                    SelectVIPVoEntity.TxtAfterMoney = (decRemain + decTopUp).ToString();
                    //* ⑤需支付金额 = 充值金额。
                    SelectVIPVoEntity.TxtCopy = decTopUp.ToString()+"元";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.4 确认支付
        /// </summary>
        private void ButtonClick(Window window)
        {
            /*
             思路：获取充值金额，金额不为空：
               1、多表操作：开启事务；
               2、修改会员表S_VIP：修改状态为使用中；
               3、新增会员消费明细表B_VIPConsumptionDetails：充值记录；
               4、处理账户明细：
                  （1）账号表S_Account：两种情况：
                      ① 账号数据不存在：新增账号；
                      ② 账号数据存在：修改账号数据；
                  （2）新增账号明细表S_AmountOfDetail。
             */
            if (!string.IsNullOrEmpty(SelectVIPVoEntity.TxtMoney))
            {
                try
                {
                    //1、多表操作：开启事务；
                    using (TransactionScope scope = new TransactionScope())
                    {
                        //2、修改会员表S_VIP：修改状态为使用中；
                        #region 1、修改会员表
                        //根据会员卡ID获取会员数据
                        S_VIP VIP = (from tbVIp in myModels.S_VIP
                                     where tbVIp.VIPID == VIPID
                                     select tbVIp).Single();
                        VIP.availableBalance = Convert.ToDecimal(SelectVIPVoEntity.TxtAfterMoney.Trim());//可用余额
                        VIP.state = "使用中";//会员卡状态
                        //修改
                        myModels.Entry(VIP).State = System.Data.Entity.EntityState.Modified;
                        #endregion
                        //3、新增会员消费明细表B_VIPConsumptionDetails：充值记录；
                        B_VIPConsumptionDetails B_VIPConsumptionDetails = new B_VIPConsumptionDetails
                        {
                            consumptionTiming = DateTime.Now,//消费时间
                            money = Convert.ToDecimal(SelectVIPVoEntity.TxtMoney),//金额
                            VIPID = VIPID,//会员卡
                            consumptionType = "充值",//消费类型
                            consumptionReason = "会员充值" + Convert.ToDecimal(SelectVIPVoEntity.TxtAfterMoney.Trim()) + "元"//资金来由
                        };
                        myModels.B_VIPConsumptionDetails.Add(B_VIPConsumptionDetails);
                        //4、处理账户明细：
                        //（1）账号表S_Account：两种情况：                      
                        var Account = (from tbAccount in myModels.S_Account
                                       select tbAccount).ToList();
                        //（1）、账户表
                        //判断账号数据是否存在
                        if (Account.Count <= 0)
                        {
                            //①不存在：新增账号数据
                            S_Account dbAccount = new S_Account
                            {
                                TotalMoney = Convert.ToDecimal(SelectVIPVoEntity.TxtAfterMoney.Trim()),//总金额=充值金额
                                Avail = 0,//可用金额=0
                                AccountFrozen = Convert.ToDecimal(SelectVIPVoEntity.TxtAfterMoney.Trim())//冻结金额=充值金额
                            };
                            //新增
                            myModels.S_Account.Add(dbAccount);
                        }
                        else
                        {                           
                            //②存在：修改账号数据(注意：充值金额存放到冻结金额里面)                           
                            S_Account dbAccount = (from tbaccount in myModels.S_Account
                                                   select tbaccount).Single();
                            //临时记录数据（账号冻结金额 、账户总金额）
                            decimal? decAccountFrozen = dbAccount.AccountFrozen;//账号冻结金额 
                            decimal? decTotalMoney = dbAccount.TotalMoney;//账户总金额 
                            dbAccount.AccountFrozen = decAccountFrozen + Convert.ToDecimal(SelectVIPVoEntity.TxtMoney);//账户冻结金额=账户冻结金额+充值金额
                            dbAccount.TotalMoney = decTotalMoney + Convert.ToDecimal(SelectVIPVoEntity.TxtMoney);//账户总金额=账户总金额 +充值金额
                            //修改
                            myModels.Entry(dbAccount).State = System.Data.Entity.EntityState.Modified;
                        }
                        //（2）新增账号明细表
                        S_AmountOfDetail amountOfDetail = new S_AmountOfDetail
                        {
                            money = Convert.ToDecimal(SelectVIPVoEntity.TxtMoney),//金额=充值金额
                            reason = "会员充值",//资金来由
                            time = DateTime.Now,//时间
                            remark = "会员充值" + Convert.ToDecimal(SelectVIPVoEntity.TxtMoney) + "元,冻结金额增加" + Convert.ToDecimal(SelectVIPVoEntity.TxtMoney) + "元，总金额增加" + Convert.ToDecimal(SelectVIPVoEntity.TxtMoney) + "元）",//备注
                            capitalPosition = "收入"//资金状况
                        };
                        //新增
                        myModels.S_AmountOfDetail.Add(amountOfDetail);
                        
                        //保存
                        if (myModels.SaveChanges() > 0)
                        {
                            scope.Complete();//提交事务
                            MessageBox.Show("充值成功！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                            window.Close();//关闭窗口
                        }
                        else
                        {
                            MessageBox.Show("充值失败！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                }   
            }
            else
            {
                MessageBox.Show("请输入充值金额", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
            }

        }
        #endregion
    }
}
