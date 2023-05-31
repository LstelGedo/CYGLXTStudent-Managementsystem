using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace CYGLXTStudent.Resources.PublicClass
{
    /// <summary>
    /// C#导入Excel数据的方式（两种）
    /// </summary>
    public static class ImportToExcel
    {
        //方式一、导入数据到数据集对象，只支持Excel的标准格式，即不能合并单元格等等

        /// <summary>
        /// 导入数据到数据集中
        /// 备注:此种方法只支持excel原文件
        /// </summary>
        /// <param name="Path">文件路劲</param>
        /// <param name="exceptionMsg">异常信息</param>
        /// <returns></returns>
        public static System.Data.DataTable InputExcel(string Path, ref string exceptionMsg)
        {
            System.Data.DataTable dt = null;
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + Path + ";" + "Extended Properties=Excel 8.0;";
                using (OleDbConnection conn = new OleDbConnection(strConn))
                {
                    conn.Open();
                    System.Data.DataTable sheetDt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string[] sheet = new string[sheetDt.Rows.Count];
                    for (int i = 0; i < sheetDt.Rows.Count; i++)
                    {
                        sheet[i] = sheetDt.Rows[i]["TABLE_NAME"].ToString();
                    }
                    string strExcel = string.Format("select * from [{0}]", sheet[0]);
                    OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
                    dt = new System.Data.DataTable();
                    myCommand.Fill(dt);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                exceptionMsg = ex.Message;
            }
            return dt;
        }

        //方法二、读取Excel文件，然后根据里面的数据信息拼装

        #region 读取Excel表格中数据到DataTable中

        public static System.Data.DataTable ChangeExcelToDateTable(string _path)
        {
            //实例化表格 
            System.Data.DataTable tempdt = new System.Data.DataTable();
            tempdt.TableName = "Excel";
            //打开一个Excel应用
            Microsoft.Office.Interop.Excel.Application app = new Application();
            object obj = System.Reflection.Missing.Value;
            try
            {
                //打开工作簿（WorkBook：即Excel文件主体本身）
                Workbook _wBook = app.Workbooks.Open(_path, obj, obj, obj, obj, obj, obj, obj, obj, obj, obj, obj, obj, obj, obj);
                //获取工作表（即Excel里的子表sheet） 1表示选择第一个Sheet页
                Worksheet _wSheet = (Worksheet)_wBook.Worksheets.get_Item(1);
                //声明行列
                DataRow newRow = null;
                DataColumn newColumn = null;
                //获取工作表单元格数据
                for (int i = 2; i <= _wSheet.UsedRange.Rows.Count; i++)
                {
                    newRow = tempdt.NewRow();
                    //Excel单元格第一个从索引1开始
                    for (int j = 1; j <= _wSheet.UsedRange.Columns.Count; j++)
                    {
                        if (i == 2 && j == 1)
                        {
                            //1、表头
                            for (int k = 1; k <= _wSheet.UsedRange.Columns.Count; k++)
                            {
                                string str = (_wSheet.UsedRange[1, k] as Range).Value2.ToString();
                                newColumn = new DataColumn(str);
                                newRow.Table.Columns.Add(newColumn);
                            }
                        }
                        //2、数据
                        Range range = _wSheet.Cells[i, j] as Range;
                        if (range != null && !"".Equals(range.Text.ToString()))
                        {
                            newRow[j - 1] = range.Value2;

                        }
                    }
                    //把行数据添加给表格DataTable
                    tempdt.Rows.Add(newRow);
                }
                //清空数据，
                _wSheet = null;
                _wBook = null;
                app.Quit();
                //结束进程
                Kill(app);
                int generation = System.GC.GetGeneration(app);
                app = null;
                System.GC.Collect(generation);
                //返回数据
                return tempdt;
            }
            catch (Exception ex)
            {
                app.Quit();
                Kill(app);
                int generation = System.GC.GetGeneration(app);
                app = null;
                throw ex;
            }
        }

        #endregion

        #region 结束进程

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        private static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口
            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }

        #endregion
    }
}
