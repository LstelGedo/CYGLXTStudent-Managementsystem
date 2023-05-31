using System;
using System.ComponentModel;
using System.Windows;

namespace CYGLXTStudent.Model.Vo
{
    /// <summary>
    /// 自定义实体类：继承员工实体
    /// </summary>
    public class StaffVo : S_Staff, INotifyPropertyChanged
    {
        /// <summary>
        /// 选中否（员工导入，批量选择数据要用）
        /// </summary>
        public bool Chked { get; set; }

        /// <summary>
        /// 进入时间
        /// </summary>
        public string EnterTime { get; set; }
        /// <summary>
        /// 登录时长
        /// </summary>
        public string TheLogin { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LogonTime { get; set; }
        /// <summary>
        /// 离线时间
        /// </summary>
        public DateTime? OfflineTime { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; }


        #region 按钮显示与隐藏
        //实现接口
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 私有字段：按钮：显示与隐藏（工作证、修改、离职）
        /// </summary>
        private Visibility isVisibility;
        /// <summary>
        /// 属性：按钮：显示与隐藏（工作证、修改、离职）
        /// </summary>
        public string IsVisibility
        {
            get
            {
                return isVisibility.ToString();
            }
            set
            {
                try
                {
                    if (value.ToString() == "离职")
                    {
                        //隐藏
                        isVisibility = Visibility.Hidden;
                    }
                    else if (value.ToString() == "在职")
                    {
                        //显示
                        isVisibility = Visibility.Visible;
                    }
                }
                catch (Exception)
                {
                    //显示
                    isVisibility = Visibility.Visible;
                }
                //简化委托调用
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibility"));//对UpdateVisibility进行监听
            }
        }


        /// <summary>
        /// 字段：按钮：显示与隐藏（删除）
        /// </summary>
        private Visibility deleteVisibility;
        /// <summary>
        /// 属性：按钮：显示与隐藏（删除）
        /// </summary>
        public string DeleteVisibility
        {
            get
            {
                return deleteVisibility.ToString();
            }
            set
            {
                try
                {
                    if (value.ToString() == "超级管理员")
                    {
                        //隐藏
                        deleteVisibility = Visibility.Hidden;
                    }
                    else
                    {
                        //显示
                        deleteVisibility = Visibility.Visible;
                    }
                }
                catch (Exception)
                {
                    //显示
                    deleteVisibility = Visibility.Visible;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DeleteVisibility"));//对UpdateVisibility进行监听
            }
        }
        #endregion

        #region 个人信息
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 字段：入职日期
        /// </summary>
        private string strEntryDate;
        /// <summary>
        /// 属性：入职日期
        /// </summary>
        public string StrEntryDate
        {
            get
            {
                return strEntryDate;
            }
            set
            {
                try
                {
                    strEntryDate = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    strEntryDate = value;
                }
            }
        }
        #endregion
    }
}
