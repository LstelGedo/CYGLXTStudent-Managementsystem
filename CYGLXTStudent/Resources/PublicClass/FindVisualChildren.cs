using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CYGLXTStudent.Resources.PublicClass
{
    /// <summary>
    /// 从视觉树找到目标控件的所有子控件
    /// 
    /// </summary>
    public static class FindVisualChildren
    {
        /// <summary>
        /// 采用了递归的方式，在多级中去查找指定Name属性的子控件。
        /// </summary>
        /// <typeparam name="T">控件类别</typeparam>
        /// <param name="depObj">父控件</param>
        /// <returns>子控件集合</returns>
        public static List<T> FindVisualChildrens<T>(DependencyObject depObj) where T : DependencyObject
        {
            /*
             * 这段代码能够获取到所有的类型为T的子元素，并返回一个List<T>的对象，
             * 这个方法的好处就是能够沿着当前传入的DependencyObject为起点沿着视觉树一直向下查找，包括模板中找到的元素。
             */
            List<T> list = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        list.Add(t);
                    }
                    // 递归
                    List<T> childItems = FindVisualChildrens<T>(child);
                    if (childItems != null && childItems.Count() > 0)
                    {
                        foreach (var item in childItems)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

    }

    /// <summary>
    ///【WPF】查找父/子控件（元素、节点）
    /// 整理一下项目中常用的找控件功能，包括找父/子控件、找到所有同类型子控件（比如ListBox找到所有Item）。
    /// 用于查找控件的工具类：找到父控件、子控件
    /// </summary>
    public class ControlsSearchHelper
    {
        /// <summary>
        /// 1、查找父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="obj">要找的是obj的父控件</param>
        /// <param name="name">想找的父控件的Name属性</param>
        /// <returns>目标父控件</returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T t && (t.Name == name | string.IsNullOrEmpty(name)))
                {
                    return t;
                }

                // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }


        /// <summary>
        /// 2、查找子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>目标子控件</returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is T t && (t.Name == name | string.IsNullOrEmpty(name)))
                {
                    return t;
                }
                else
                {
                    // 在下一级中没有找到指定名字的子控件，就再往下一级找
                    T grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;

        }


        /// <summary>
        /// 3、获取所有同一类型的子控件
        /// </summary>
        /// <typeparam name="T">子控件的类型</typeparam>
        /// <param name="obj">要找的是obj的子控件集合</param>
        /// <param name="name">想找的子控件的Name属性</param>
        /// <returns>子控件集合</returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is T t && (t.Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add(t);
                }

                childList.AddRange(GetChildObjects<T>(child, ""));
            }

            return childList;

        }
    }
}
