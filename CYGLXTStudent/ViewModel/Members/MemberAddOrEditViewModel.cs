using CYGLXTStudent.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Windows;

namespace CYGLXTStudent.ViewModel.Members
{
    /// <summary>
    /// 会员类型新增/修改
    /// </summary>
    public class MemberAddOrEditViewModel:ViewModelBase
    {
        public MemberAddOrEditViewModel()
        {
            //关闭
            CloseWindowCommand = new RelayCommand<Window>(CloseWindow, win => win != null);
            //重置
            ResetCommand = new RelayCommand(Reset);
            //保存
            SaveCommand=new RelayCommand<Window>(SaveMember, win => win != null);
        }

        #region 【1、属性】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        CYGLXTEntities myModel = new CYGLXTEntities();
        //定义委托
        public delegate void RefreshDelegate();
        //定义事件
        public event RefreshDelegate RefreshEvent;

        private bool blAdd;
        /// <summary>
        /// 新增和修改标志
        /// </summary>
        public bool BlAdd
        {
            get { return blAdd; }
            set
            {
                if (blAdd != value)
                {
                    blAdd = value;
                    RaisePropertyChanged(() => BlAdd);
                }

            }
        }

        private S_Member currentMemberEntity;
        /// <summary>
        /// 会员类型实体
        /// </summary>
        public S_Member CurrentMemberEntity
        {
            get { return currentMemberEntity; }
            set
            {
                if (currentMemberEntity != value)
                {
                    currentMemberEntity = value;
                    RaisePropertyChanged(() => CurrentMemberEntity);
                }

            }
        }



        #endregion
        #region 【2、命令】
        /// <summary>
        /// 关闭窗口命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; set; }
        /// <summary>
        /// 重置命令
        /// </summary>
        public RelayCommand ResetCommand { get; set; }
        /// <summary>
        /// 保存命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 重置窗口
        /// </summary>
        private void Reset()
        {
            if (BlAdd)
            {
                //新的实例
                CurrentMemberEntity = new S_Member();
            }
        }
        /// <summary>
        /// 3.3 保存（新增/修改）
        /// </summary>
        /// <param name="window">当前窗口</param>
        private void SaveMember(Window window)
        {
            /*
              保存分两种情况：
                 一、新增
                   ①必填项不能为空；
                   ②实例实体对象并获取值；
                   ③判断是否重复(拿会员类型名比较)
                   ④新增保存。
                 二、修改
                   ①必填项不能为空；
                   ②实例实体对象并获取值；
                   ③判断是否重复(拿会员类型名比较，不比较自身)
                   ④获取主键，修改保存。

             上面简化：               
                   ①必填项不能为空；
                   ②实例实体对象并获取值；
                   ③判断是否重复(拿会员类型名比较，不比较自身)
                 一、新增
                       ④新增保存。                 
                 二、修改
                       ④获取主键，修改保存。
            */
            try
            {
                //①必填项不能为空；
                if (!string.IsNullOrEmpty(CurrentMemberEntity.type)
                    && CurrentMemberEntity.diningDiscount>0 
                    && CurrentMemberEntity.freshDiscount>0)
                {
                    //  ②实例实体对象并获取值；
                    S_Member saveMember = new S_Member
                    {
                        type= CurrentMemberEntity.type,
                        diningDiscount=CurrentMemberEntity.freshDiscount,
                        freshDiscount=CurrentMemberEntity.freshDiscount,
                        remark=currentMemberEntity.remark,                      
                    };
                    //  ③判断是否重复(拿会员类型名比较，不比较自身)
                    int intCount = myModel.S_Member.Where(o => o.type == CurrentMemberEntity.type.Trim()
                    && o.memberID != CurrentMemberEntity.memberID).ToList().Count();
                    if (intCount>0)
                    {
                        MessageBox.Show("你输入的类型已经存在！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    //一、新增
                    if (BlAdd)
                    {
                        saveMember.amount = 0;//默认0
                        //      ④新增保存。
                        myModel.S_Member.Add(saveMember);
                        //保存更改
                        if (myModel.SaveChanges()>0)
                        {
                            MessageBox.Show(CurrentMemberEntity.type+"新增会员类型成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (RefreshEvent!=null)
                            {
                                //调用委托事件
                                RefreshEvent();
                                window.Close();
                            }                           
                        }
                        else
                        {
                            MessageBox.Show(CurrentMemberEntity.type + "新增会员类型失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }

                    //二、修改
                    else
                    {
                        saveMember.amount = currentMemberEntity.amount;
                        //      ④获取主键，修改保存。
                        saveMember.memberID = currentMemberEntity.memberID;
                        myModel.Entry(saveMember).State=System.Data.Entity.EntityState.Modified;
                        //保存更改
                        if (myModel.SaveChanges() > 0)
                        {
                            MessageBox.Show(CurrentMemberEntity.type + "修改会员类型成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            if (RefreshEvent != null)
                            {
                                //调用委托事件
                                RefreshEvent();
                                window.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show(CurrentMemberEntity.type + "修改会员类型失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("带星号的是必填项，不能为空！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
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
