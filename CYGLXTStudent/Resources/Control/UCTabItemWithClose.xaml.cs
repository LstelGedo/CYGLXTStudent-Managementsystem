﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace CYGLXTStudent.Resources.Control
{
    /// <summary>
    /// UCTabItemWithClose.xaml 的交互逻辑
    /// </summary>
    public partial class UCTabItemWithClose : TabItem
    {
        public UCTabItemWithClose()
        {
            InitializeComponent();
        }
        #region 成员变量
        /// <summary>
        /// 父级TabControl
        /// </summary>
        private TabControl m_Parent;
        /// <summary>
        /// 约定的宽度
        /// </summary>
        private double m_ConventionWidth = 100;
        #endregion
        //页面加载事件
        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            //找到父级TabControl
            m_Parent = FindParentTabControl(this);
            if (m_Parent != null)
                Load();

        }
        //关闭按钮（关闭本身）
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            if (m_Parent == null)
                return;
            //移除自身
            m_Parent.Items.Remove(this);
            //移除事件
            m_Parent.SizeChanged -= m_Parent_SizeChanged;

            //调整剩余项大小
            //保持约定宽度item的临界个数
            int criticalCount = (int)((m_Parent.ActualWidth - 5) / m_ConventionWidth);
            //平均宽度
            double perWidth = (m_Parent.ActualWidth - 5) / m_Parent.Items.Count;
            foreach (UCTabItemWithClose item in m_Parent.Items)
            {
                if (m_Parent.Items.Count <= criticalCount)
                {
                    item.Width = m_ConventionWidth;
                }
                else
                {
                    item.Width = perWidth;
                }
            }
        }

        #region 父级TabControl尺寸发生变化
        /// <summary>
        /// 父级TabControl尺寸发生变化
        /// </summary>
        private void m_Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                //调整自身大小
                //保持约定宽度item的临界个数
                int criticalCount = (int)((m_Parent.ActualWidth - 5) / m_ConventionWidth);
                if (m_Parent.Items.Count <= criticalCount)
                {
                    //小于等于临界个数 等于约定宽度
                    this.Width = m_ConventionWidth;
                }
                else
                {
                    //大于临界个数 等于平均宽度
                    double perWidth = (m_Parent.ActualWidth - 5) / m_Parent.Items.Count;
                    this.Width = perWidth;
                }
            }
            catch (System.Exception)
            {
                return;
            }
        }
        #endregion
        #region 方法
        #region Load
        /// <summary>
        /// Load
        /// </summary>
        private void Load()
        {
            //约定的宽度
            double.TryParse(m_Parent.Tag.ToString(), out m_ConventionWidth);
            //注册父级TabControl尺寸发生变化事件
            m_Parent.SizeChanged += m_Parent_SizeChanged;

            //自适应
            //保持约定宽度item的临界个数
            int criticalCount = (int)((m_Parent.ActualWidth - 5) / m_ConventionWidth);
            if (m_Parent.Items.Count <= criticalCount)
            {
                //小于等于临界个数 等于约定宽度
                this.Width = m_ConventionWidth;
            }
            else
            {
                //大于临界个数 每项都设成平均宽度
                double perWidth = (m_Parent.ActualWidth - 5) / m_Parent.Items.Count;
                foreach (UCTabItemWithClose item in m_Parent.Items)
                {
                    item.Width = perWidth;
                }
                this.Width = perWidth;
            }
        }
        #endregion
        #region 递归找父级TabControl
        /// <summary>
        /// 递归找父级TabControl
        /// </summary>
        /// <param name="reference">依赖对象</param>
        /// <returns>TabControl</returns>
        private TabControl FindParentTabControl(DependencyObject reference)
        {
            DependencyObject dObj = VisualTreeHelper.GetParent(reference);
            if (dObj == null)
                return null;
            if (dObj.GetType() == typeof(TabControl))
                return dObj as TabControl;
            else
                return FindParentTabControl(dObj);
        }
        #endregion
        #endregion

    }
}
