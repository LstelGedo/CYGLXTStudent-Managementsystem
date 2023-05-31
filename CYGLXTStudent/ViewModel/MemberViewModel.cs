using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CYGLXTStudent.Model;
using CYGLXTStudent.Views.Members;
using CYGLXTStudent.ViewModel.Members;
using System.Windows.Input;
using CYGLXTStudent.Model.Vo;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 会员类型
    /// </summary>
    public class MemberViewModel: ViewModelBase
    {
        public MemberViewModel()
        {
            //加载
            LoadedCommand=new RelayCommand (Loaded);
            //会员类型
            MemberInit();
            //会员卡
            VipInit();
        }
        #region 【公共部分】
        CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 加载方法
        /// </summary>
        private void Loaded()
        {
            //绑定会员类型信息
            SelectMember();
            //绑定会员卡基本信息
            SelectVip();
            //下拉框绑定
            SelectComboboxMember();
        }
        #endregion
        #region 一、【会员类型】
        #region 【1、属性】
        private string strType;
        /// <summary>
        /// 会员类型（条件筛选）
        /// </summary>
        public string StrType
        {
            get { return strType; }
            set
            {
                if (strType != value)
                {
                    strType = value;
                    RaisePropertyChanged(() => StrType);
                }

            }
        }
        
        private List<S_Member> listMember;
        /// <summary>
        /// 会员类型列表(绑定表格数据)
        /// </summary>
        public List<S_Member> ListMember
        {
            get { return listMember; }
            set
            {
                if (listMember != value)
                {
                    listMember = value;
                    RaisePropertyChanged(() => ListMember);
                }

            }
        }

        /// <summary>
        /// 选中实体（修改、删除）
        /// </summary>
        private S_Member selectMemberEntity;
        /// <summary>
        /// 会员类型列表(绑定表格数据)
        /// </summary>
        public S_Member SelectMemberEntity
        {
            get { return selectMemberEntity; }
            set
            {
                if (selectMemberEntity != value)
                {
                    selectMemberEntity = value;
                    RaisePropertyChanged(() => SelectMemberEntity);
                }

            }
        }
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.1 打开新增窗口命令
        /// </summary>
        public RelayCommand AddCommand { get; set; }
        /// <summary>
        /// 2.2 打开修改窗口命令
        /// </summary>
        public RelayCommand EditCommand { get; set; }
        /// <summary>
        /// 2.3 删除命令
        /// </summary>
        public RelayCommand DeleteCommand { get; set; }
        /// <summary>
        /// 2.4 刷新命令
        /// </summary>
        public RelayCommand RefreshCommand { get; set; }
        #endregion
        #region 【3、方法】
        private void MemberInit()
        {
            RefreshCommand = new RelayCommand(SelectMember);
            AddCommand = new RelayCommand(OpenAdd);
            EditCommand = new RelayCommand(OpenEdit);
            DeleteCommand= new RelayCommand(Delete);
        }
        /// <summary>
        /// 3.1 会员类型信息
        /// </summary>
        private void SelectMember()
        {
            try
            {
                //1、linq to sql 单表查询会员类型
                var list=from tbMember in myModel.S_Member select tbMember;
                //2、条件筛选数据
                if (!string.IsNullOrEmpty(StrType))
                {
                    list = list.Where(o => o.type.Contains(StrType));
                }
                //3、绑定数据
                ListMember = list.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 打开新增窗口
        /// </summary>
        private void OpenAdd()
        {
            try
            {
                //实例化窗口
                MemberAddOrEditWindow window = new MemberAddOrEditWindow();
                //ViewModel传值（新的实体，新增标志true,委托与事件（刷新数据））
                var viewModel=window.DataContext as MemberAddOrEditViewModel;
                viewModel.CurrentMemberEntity = new S_Member();
                viewModel.BlAdd = true;//新增
                viewModel.RefreshEvent += SelectMember;//必须在window.ShowDialog前面
                //打开窗口
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 打开修改窗口
        /// </summary>
        private void OpenEdit()
        {
            try
            {
                if (SelectMemberEntity!=null)
                {
                    //实例化窗口
                    MemberAddOrEditWindow window = new MemberAddOrEditWindow();
                    //ViewModel传值（新的实体，新增标志true,委托与事件（刷新数据））
                    var viewModel = window.DataContext as MemberAddOrEditViewModel;
                    viewModel.CurrentMemberEntity = SelectMemberEntity;//选中实体
                    viewModel.BlAdd = false;//修改
                    viewModel.RefreshEvent += SelectMember;//必须在window.ShowDialog前面
                    //打开窗口
                    window.ShowDialog();
                }
                else
                {
                    MessageBox.Show("请选择要修改的数据", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.3 删除会员类型
        /// </summary>
        private void Delete()
        {
            try
            {
                //1、删除数据不为空
                if (SelectMemberEntity!=null)
                {
                    //2、显示消息框提醒用户
                    if (MessageBox.Show("是否要删除"+SelectMemberEntity.type+"类型！", "系统提示：", MessageBoxButton.YesNo, MessageBoxImage.Error)==MessageBoxResult.Yes)
                    {
                        //3、确定没有被使用
                        int intCount = myModel.S_VIP.Where(o => o.memberID == SelectMemberEntity.memberID).ToList().Count();
                        if (intCount>0)
                        {
                            MessageBox.Show("会员类型正在使用中，无法删除！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        //4、删除
                        S_Member member = myModel.S_Member.Find(SelectMemberEntity.memberID);
                        myModel.S_Member.Remove(member);                      
                        if (myModel.SaveChanges()>0)
                        {
                            MessageBox.Show("删除成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                            //5、刷新数据
                            SelectMember();
                        }
                        else
                        {
                            MessageBox.Show("删除失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("请选择要删除的会员类型！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #endregion
        #region 二、【会员卡】
        #region 【1、属性】
        private List<VipVo> listVIPVo;
        /// <summary>
        /// 会员卡扩展实体列表
        /// </summary>
        public List<VipVo> ListVIPVo
        {
            get { return listVIPVo; }
            set
            {
                if (listVIPVo != value)
                {
                    listVIPVo = value;
                    RaisePropertyChanged(() => ListVIPVo);
                }

            }
        }
        /// <summary>
        /// 字段：选中VIP实体（绑定表格）
        /// </summary>
        private S_VIP selectVipEntity;
        /// <summary>
        /// 属性：选中VIP实体（绑定表格）
        /// </summary>
        public S_VIP SelectVipEntity
        {
            get { return selectVipEntity; }
            set
            {
                if (selectVipEntity != value)
                {
                    selectVipEntity = value;
                    RaisePropertyChanged(() => SelectVipEntity);

                }
            }
        }
        #region 条件筛选属性
        /// <summary>
        /// 字段：卡号
        /// </summary>
        private string cardNo;
        /// <summary>
        /// 属性：卡号
        /// </summary>
        public string CardNo
        {
            get { return cardNo; }
            set
            {
                if (cardNo != value)
                {
                    cardNo = value;
                    RaisePropertyChanged(() => CardNo);
                }
            }
        }
        /// <summary>
        /// 字段：名称
        /// </summary>
        private string name;
        /// <summary>
        /// 属性：名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }
        /// <summary>
        /// 字段：会员类型ID
        /// </summary>
        private int memberID;
        /// <summary>
        /// 属性：会员类型ID
        /// </summary>
        public int MemberID
        {
            get { return memberID; }
            set
            {
                if (memberID != value)
                {
                    memberID = value;
                    RaisePropertyChanged(() => MemberID);
                }
            }
        }
        /// <summary>
        /// 字段：电话
        /// </summary>
        private string phone;
        /// <summary>
        /// 属性：电话
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set
            {
                if (phone != value)
                {
                    phone = value;
                    RaisePropertyChanged(() => Phone);
                }
            }
        }
        /// <summary>
        /// 字段：积分
        /// </summary>
        private string integral;
        /// <summary>
        /// 属性：积分
        /// </summary>
        public string Integral
        {
            get { return integral; }
            set
            {
                if (integral != value)
                {
                    integral = value;
                    RaisePropertyChanged(() => Integral);
                }
            }
        }
        /// <summary>
        /// 字段：状态
        /// </summary>
        private string state;
        /// <summary>
        /// 属性：状态
        /// </summary>
        public string State
        {
            get { return state; }
            set
            {
                if (state != value)
                {
                    state = value;
                    RaisePropertyChanged(() => State);
                }
            }
        }
        #endregion
        private List<S_Member> members;
        /// <summary>
        /// 会员类型下拉框
        /// </summary>
        public List<S_Member> Members
        {
            get { return members; }
            set {
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
        /// 2.1 查询按钮刷新命令
        /// </summary>
        public ICommand VipRefreshCommand { get; set; }
        /// <summary>
        /// 2.2 重置条件命令
        /// </summary>
        public RelayCommand RemoveCommand { get; set; }
        /// <summary>
        /// 2.3 打开会员卡新增窗口命令
        /// </summary>
        public ICommand VipAddCommand { get; set; }
        /// <summary>
        /// 2.4 打开会员卡修改窗口命令
        /// </summary>
        public ICommand VipUpdateCommand { get; set; }
        /// <summary>
        ///  2.5 打开会员卡充值窗口命令
        /// </summary>
        public ICommand MemberCardCommand { get; set; }
        /// <summary>
        /// 2.6 打开会员卡退卡窗口命令
        /// </summary>
        public ICommand CancelMembershipCommand { get; set; }
        /// <summary>
        /// 2.7 打开会员卡修改密码窗口命令
        /// </summary>
        public ICommand UpdatePasswordCommand { get; set; }

        #endregion
        #region 【3、方法】
        private void VipInit()
        {
            //刷新
            VipRefreshCommand = new RelayCommand(SelectVip);
            //重置
            RemoveCommand = new RelayCommand(RemoveSelects);
            //新增 
            VipAddCommand = new RelayCommand(AddVip);
            //修改 
            VipUpdateCommand = new RelayCommand(UpdateVip);
            //充值       
            MemberCardCommand = new RelayCommand(MemberCard);
            //退卡
            CancelMembershipCommand = new RelayCommand(CancelMembership);
            //修改密码
            UpdatePasswordCommand = new RelayCommand(UpdatePassword);

        }
        /// <summary>
        /// 查询会员卡
        /// </summary>
        private void SelectVip()
        {
            try
            {
                /*
                  * 思路：
                  * 1、封装实体VIPVo，然后linq连表查询会员卡S_VIP与会员类型S_Member基本信息列表
                  * 2、判断条件不为空，条件筛选列表
                  * 3、绑定属性
                  */
                var list = (from tbVIP in myModel.S_VIP
                            join tbMember in myModel.S_Member on tbVIP.memberID equals tbMember.memberID
                            select new VipVo
                            {
                                VIPID = tbVIP.VIPID,//会员ID
                                memberID = tbVIP.memberID,//会员类型ID
                                cardNo = tbVIP.cardNo,//卡号
                                password = tbVIP.password,//密码
                                name = tbVIP.name,//姓名
                                phone = tbVIP.phone,//联系电话
                                availableBalance = tbVIP.availableBalance,//可用余额
                                openingTime = tbVIP.openingTime,//开通时间
                                Integral = tbVIP.Integral,//积分
                                type = tbMember.type,//会员类型
                                state = tbVIP.state,//状态
                                diningDiscount = tbMember.diningDiscount,//餐饮折扣
                                freshDiscount = tbMember.freshDiscount,//生鲜折扣
                                remark = tbMember.remark,//备注                                                             
                                IsVisibility = tbVIP.state,//按钮：显示与隐藏（修改、充值、修改密码、退卡）
                            });
                //2、判断条件不为空，条件筛选列表
                if (!string.IsNullOrEmpty(CardNo))
                {
                    list = list.Where(o => o.cardNo.Contains(CardNo));
                }
                //姓名          
                if (!string.IsNullOrEmpty(Name))
                {
                    list = list.Where(o => o.name.Contains(Name));
                }
                //会员类型          
                if (MemberID > 0)
                {
                    list = list.Where(o => o.memberID == MemberID);
                }
                //电话          
                if (!string.IsNullOrEmpty(Phone))
                {
                    list = list.Where(o => o.phone.Contains(Phone));
                }
                //状态          
                if (!string.IsNullOrEmpty(State))
                {
                    list = list.Where(o => o.state.Contains(State));
                }
                //绑定页面数据（表格）
                ListVIPVo = list.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
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
        /// 3.2 重置条件
        /// </summary>
        private void RemoveSelects()
        {            
            CardNo = string.Empty;//卡号
            Name = string.Empty;//名称
            MemberID = 0;//会员类型ID
            Phone = string.Empty;//电话
            State = string.Empty;//状态
        }

        /// <summary>
        /// 3.3 打开新增窗口
        /// </summary>
        private void AddVip()
        {
            /*
           * 思路：
           * 1、实例化新增窗口
           * 2、获取窗口对应的ViewModel;
           * 3、给ViewModel传递参数（new 新实体，新增标志）
           * 4、打开窗口。
           */
            try
            {
                VipAddOrEditWindow addOrEditWindow = new VipAddOrEditWindow();
                var addOrEditViewModel = addOrEditWindow.DataContext as VipAddOrEditViewModel;
                addOrEditViewModel.CurrentVIPEntity = new S_VIP();
                addOrEditViewModel.IsAdd = true;//新增标记               
                addOrEditWindow.ShowDialog();//特别提醒，打开窗口必须放在最后
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.4 修改信息
        /// </summary>
        private void UpdateVip()
        {
            /*
             * 思路：
             * 1、判断选中实体不为空
             * （1）、实例化修改窗口
             * （2）、获取窗口对应的ViewModel;
             * （3）、给ViewModel传递参数（选中实体，回填卡号，修改标志）
             * （4）、打开窗口。
             * 2、显示消息框提示选择修改数据。
             */
            try
            {
                if (SelectVipEntity != null)
                {
                    VipAddOrEditWindow addOrEditWindow = new VipAddOrEditWindow();
                    var addOrEditViewModel = addOrEditWindow.DataContext as VipAddOrEditViewModel;
                    addOrEditViewModel.CurrentVIPEntity = SelectVipEntity;//选中实体
                    addOrEditViewModel.CarNumber = SelectVipEntity.cardNo;//回填卡号
                    addOrEditViewModel.IsAdd = false;//修改标记           

                    addOrEditWindow.ShowDialog();//打开窗口  

                }
                else
                {
                    MessageBox.Show("请选择要修改的会员卡！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 3.5 打开充值窗口
        /// </summary>
        private void MemberCard()
        {
            /*
            * 思路：
            * 1、判断选中实体不为空
            * （1）、实例化充值MemberCardWindow窗口
            * （2）、获取窗口对应的ViewModel;
            * （3）、给ViewModel传递参数（会员卡ID）
            * （4）、打开窗口。
            * 2、显示消息框提示选择要充值的会员卡。
            */
            try
            {
                if (SelectVipEntity != null)
                {
                    //实例化充值窗口对象
                    MemberCardWindow addOrEditWindow = new MemberCardWindow();
                    var addOrEditViewModel = addOrEditWindow.DataContext as MemberCardViewModel;
                    addOrEditViewModel.VIPID = SelectVipEntity.VIPID; //传递参数（会员卡ID）                   
                    addOrEditWindow.ShowDialog();//打开窗口
                }
                else
                {
                    MessageBox.Show("请选择要充值的会员卡！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// 3.6 打开退费窗口
        /// </summary>
        private void CancelMembership()
        {
            /*
              * 思路：
              * 1、判断选中实体不为空
              * （1）、实例化退费CancelMembershipWindow窗口
              * （2）、获取窗口对应的ViewModel;
              * （3）、给ViewModel传递参数（会员卡ID）
              * （4）、打开窗口。
              * 2、显示消息框提示选择要退费的会员卡。
              */
            try
            {
                if (SelectVipEntity != null)
                {
                    //实例化退费窗口对象
                    CancelMembershipWindow addOrEditWindow = new CancelMembershipWindow();
                    var addOrEditViewModel = addOrEditWindow.DataContext as CancelMembershipViewModel;
                    addOrEditViewModel.VIPID = SelectVipEntity.VIPID; //传递参数（会员卡ID）                   
                    addOrEditWindow.ShowDialog();//打开窗口
                }
                else
                {
                    MessageBox.Show("请选择要退费的会员卡！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.7 打开修改密码窗口
        /// </summary>
        private void UpdatePassword()
        {
            /*
              * 思路：
              * 1、判断选中实体不为空
              * （1）、实例化修改密码UpdatePasswordWindow窗口
              * （2）、获取窗口对应的ViewModel;
              * （3）、给ViewModel传递参数（会员卡ID）
              * （4）、打开窗口。
              * 2、显示消息框提示选择要退费的会员卡。
              */
            try
            {
                if (SelectVipEntity != null)
                {
                    //实例化修改密码窗口对象
                    UpdatePasswordWindow addOrEditWindow = new UpdatePasswordWindow();
                    var viewModel = addOrEditWindow.DataContext as UpdatePasswordViewModel;
                    viewModel.VIPID = SelectVipEntity.VIPID;//传递参数（会员卡ID）                  
                    //打开窗口
                    addOrEditWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("请选择要修改密码的会员卡！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #endregion

    }
}
