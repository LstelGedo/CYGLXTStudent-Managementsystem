using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace CYGLXTStudent.Resources.PublicClass
{
    /// <summary>
    /// List集合和DataTable相互转换
    /// </summary>
    public class ListToDataTable
    {
        /// <summary>
        ///  1、把IList集合转换成DataTable数据类型
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTablen(IList list)
        {
            System.Data.DataTable result = new System.Data.DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    //获取类型
                    Type colType = pi.PropertyType;
                    //当类型为Nullable<>时
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        /// <summary>
        /// 2、datatable转换为List<T>集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> convertToList<T>(DataTable dt) where T : new()
        {
            //定义集合
            List<T> ts = new List<T>();
            //获得此模型的类型
            Type type = typeof(T);
            //定义一个临时的变量
            string tempName = "";
            //遍历datatable中所有数据行
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                //获得此模型的公共属性
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历所有属性
                foreach (PropertyInfo pi in propertys)
                {
                    //将此属性赋值给临时变量
                    tempName = pi.Name;
                    //检查datatable是否包含此列
                    if (dt.Columns.Contains(tempName))
                    {
                        //判断此属性是否有setter，这个啥意思呢，就是我们的实体层的{get;set;}如果我们的实体有了set方法，就说明可以赋值！
                        if (!pi.CanWrite) continue;
                        {
                            //取值  
                            object value = dr[tempName];
                            if (value != DBNull.Value)
                                pi.SetValue(t, value, null);
                        }
                    }
                }
                //对象添加到泛型集合中
                ts.Add(t);
            }
            return ts;
        }
    }
}
