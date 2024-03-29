﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CYGLXTStudent.Resources.PublicClass
{
    /// <summary>    
    ///  DataTable表转换成List<T>列表
    /// </summary>    
    public class ModelConvertHelper<T> where T : new()
    {
        /// <summary>
        /// DataTable表转换成List<T>列表
        /// </summary>
        /// <param name="dt">DataTable表</param>
        /// <returns>List<T>列表</returns>
        public static List<T> ConvertToModel(DataTable dt)
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;  // 检查DataTable是否包含此列    

                    if (dt.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                            pi.SetValue(t, value, null);
                    }
                }
                ts.Add(t);
            }
            return ts;
        }
    }
}
