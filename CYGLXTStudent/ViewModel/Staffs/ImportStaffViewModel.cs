using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Windows;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel.Staffs
{
    /// <summary>
    /// 员工信息导入
    /// </summary>
    public class ImportStaffViewModel : ViewModelBase
    {
        public ImportStaffViewModel()
        {
            //下载模板
            DownloadCommand = new RelayCommand(DownloadExcel);
            //关闭窗口
            CloseWindowCommand= new RelayCommand<Window>(CloseWindow,wd=>wd!=null);
            //选择Excel
            ChooseCommand = new RelayCommand(ChooseExcel);
            //单个移除
            DeleteCommand = new RelayCommand(DeteTeStaff);
            //（全选/反选）
            ChecksCommand = new RelayCommand(ChecksALL);
            //批量移除
            RemoveCommand = new RelayCommand(RemoveAll);
            //批量导入
            SaveCommand=new RelayCommand<Window>(SaveStaffs, wd => wd != null);            
        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 员工临时表保存导入数据（后面用来操作移除、全选/反选、批量移除）
        /// </summary>
        DataTable dtImportStaff;
        #endregion
        #region 【1、属性】
        /// <summary>
        /// 字段：员工扩展实体列表（用来记录导入数据）  
        /// </summary>  
        private List<StaffVo> staffVos;
        /// <summary>
        /// 属性：员工扩展实体列表（用来记录导入数据）  
        /// </summary>  
        public List<StaffVo> StaffVos

        {
            get { return staffVos; }
            set
            {
                if (staffVos != value)
                {
                    staffVos = value;
                    RaisePropertyChanged(() => StaffVos);
                }
            }
        }
        /// <summary>
        /// 字段：员工扩展实体(用来记录选中数据)
        /// </summary>
        private StaffVo selectStaffEntity;
        /// <summary>
        /// 属性：员工扩展实体(用来记录选中数据)
        /// </summary>
        public StaffVo SelectStaffEntity

        {
            get { return selectStaffEntity; }
            set
            {
                if (selectStaffEntity != value)
                {
                    selectStaffEntity = value;
                    RaisePropertyChanged(() => SelectStaffEntity);
                }
            }
        }
        #endregion
        #region 【2、命令】
        /// <summary>
        /// 2.1 下载模板命令
        /// </summary>
        public ICommand DownloadCommand { get; set; }
        /// <summary>
        /// 2.2 选择Excel命令
        /// </summary>
        public ICommand ChooseCommand { get; set; }
        /// <summary>
        /// 2.3 全选/反选命令
        /// </summary>
        public ICommand ChecksCommand { get; set; }
        /// <summary>
        /// 2.4 批量移除命令
        /// </summary>
        public ICommand RemoveCommand { get; set; }
        /// <summary>
        /// 2.5 保存数据命令
        /// </summary>
        public RelayCommand<Window> SaveCommand { get; set; }
        /// <summary>
        /// 2.6 删除数据命令
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// 2.7 关闭命令
        /// </summary>
        public RelayCommand<Window> CloseWindowCommand { get; private set; }
        /// <summary>
        /// 2.8 选中行改变命令（勾选/不勾选）
        /// </summary>
        public RelayCommand SelectionChangedCommand { get; set; }
        #endregion
        #region 【3、方法】
        /// <summary>
        /// 3.1 下载模板
        /// </summary>
        private void DownloadExcel()
        {
            try
            {
                //1、获取启动应用程序的可执行文件的路径（Excel文件路径）
                string strStartupPath = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.LastIndexOf(@"\"));
                //获取指定路径下的文件目录
                string strDirectoryName = Path.GetDirectoryName(strStartupPath) + @"\Resources\Excel\员工信息.xls";
                //2、用户选择目录
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.ShowDialog();
                //判断获取用户选定的路径
                if (fbd.SelectedPath!=string.Empty)
                {
                    //3、将现有的文件复制到新的路径上，允许覆盖同名的文件。
                    File.Copy(strDirectoryName, fbd.SelectedPath + "\\员工信息.xls", true);
                    MessageBox.Show("员工信息模板下载成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.2 关闭窗口
        /// </summary>
        /// <param name="window">窗口</param>
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        /// <summary>
        /// 3.3 选择Excel
        /// </summary>
        private void ChooseExcel()
        {
            try
            {
                //1、选中文件
                OpenFileDialog openFileDialog = new OpenFileDialog
                {

                    Title = "选择Excel文件",
                    Filter = "Excel文件|*.xls|所有文件|*.*",
                    FileName = string.Empty,//文件名
                    FilterIndex = 1,//索引
                    Multiselect = false,//不允许多选
                    RestoreDirectory = true,
                    DefaultExt = "xls"//筛选后缀
                };
                if (openFileDialog.ShowDialog()==false)
                {
                    return;
                }
                //文件对话框选定的文件的完整路径
                string strPath=openFileDialog.FileName;
                //2、读取Excel表格中数据到DataTable中
                dtImportStaff = ImportToExcel.ChangeExcelToDateTable(strPath);
                if (dtImportStaff.Rows.Count>0)
                {
                    //对应列名:二：调用方法时，必须加 ref 关键字。 
                    DicChange(ref dtImportStaff);
                    //表格数据赋值
                    //绑定页面数据=把DataTable转换为IList<UserInfo>  
                    StaffVos = ModelConvertHelper<StaffVo>.ConvertToModel(dtImportStaff);
                    MessageBox.Show("导入成功!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("表格无数据！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                if (ex.GetType().FullName == "Aspose.Cells.CellsException")
                {
                    MessageBox.Show("不支持该文件格式！", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else
                {
                    MessageBox.Show("数据异常!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }
        /// <summary>
        /// 3.3 根据导入的文件生成对应的列表
        /// </summary>
        /// <param name="dataTable"></param>
        private void DicChange(ref DataTable dataTable)
        {
            ///  ref （全拼：reference）:强制要求参数按引用传值。
            ///  使用 ref 关键字 有两个注意事项，如下
            ///  一：传入参数之前，必须给参数赋值。
            ///  二：调用方法时，必须加 ref 关键字。 

            /*
            1、DataTable是引用类型，在传入方法后，传入的是对象的引用，对这个对象的修改是会返回的。
            2、当使用ref时，传入的对象的地址，当这个对象修改时，是会将地址返回的，就是说在方法里面不管对传入的对象修改什么,修改都会返回。
            */
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "工号", "EMPNO" },
                { "姓名", "name" },
                { "电话", "phone" },
                { "身份证号", "IDNumber" },
                { "性别", "sex" },
                { "职务", "position" },
                { "状态", "statust" },
                { "住址", "address" },
            };
            //1、根据中文列名，匹配英文列名
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                //根据dataTable列名匹配dic列名
                if (dic.ContainsKey(dataTable.Columns[i].ColumnName))
                {
                    dataTable.Columns[i].ColumnName= dic[dataTable.Columns[i].ColumnName].ToString();
                }
            }
            //2、添加多一列（选中否，后期用来批量删除数据）
            dataTable.Columns.Add("Chked", typeof(bool));
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                dataTable.Rows[i]["Chked"] = false;
            }
            //选中否，默认不选中false
        }
        /// <summary>
        /// 3.4 单个移除
        /// </summary>
        private void DeteTeStaff()
        {
            try
            {
                //1、判断选择项是否为空
                if (SelectStaffEntity==null)
                {
                    MessageBox.Show("请选择要移除的员工！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //2、for 循环移除数据，判断提取选中数据dtImportStaff，StaffVos更新
                for (int i = 0; i < dtImportStaff.Rows.Count; i++)
                {
                    if (dtImportStaff.Rows[i]["EMPNO"].ToString().Trim()== SelectStaffEntity.EMPNO)
                    {
                        dtImportStaff.Rows.RemoveAt(i);
                    }
                }
                //把DataTable转换成List<>
                StaffVos = ModelConvertHelper<StaffVo>.ConvertToModel(dtImportStaff);
                MessageBox.Show("移除成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        /// <summary>
        /// 3.5 全选/反选
        /// </summary>
        private  void ChecksALL()
        {
            /*
             * 思路：
             * 1、 判断临时表dtImportStaff 与 员工扩展实体列表StaffVos 不为空；
             * 2、 循环员工扩展实体列表StaffVos，判断单元格chked状态：
             *   ① chked状态被勾选，临时表dtImportStaff单元格chked：不勾选；
             *   ② chked状态不被勾选，临时表dtImportStaff单元格chked：勾选；
             * 3、刷新表格：重新绑定员工扩展实体列表StaffVos。
             */
            try
            {
                if (dtImportStaff!=null && StaffVos!=null)
                {
                    for (int i = 0; i < StaffVos.Count; i++)
                    {
                        if (staffVos[i].Chked==true)
                        {
                            dtImportStaff.Rows[i]["Chked"] = false;
                        }
                        else if(staffVos[i].Chked == false)
                        {
                            dtImportStaff.Rows[i]["Chked"] = true;
                        }
                    }
                    StaffVos=ModelConvertHelper<StaffVo>.ConvertToModel(dtImportStaff);
                }
                else
                {
                    MessageBox.Show("表格数据为空，请导入Excel数据！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.6 批量移除
        /// </summary>
        private void RemoveAll()
        {
            /*
            * 思路：
            * 1、 判断临时表dtImportStaff 与 员工扩展实体列表StaffVos 不为空；
            * 2、 循环临时列表dtImportStaff，判断单元格chked状态：
            *   ① chked状态被勾选，临时表dtImportStaff：移除对应行；
            * 3、刷新表格：重新绑定员工扩展实体列表StaffVos。
            */
            try
            {
                //1、 判断临时表dtImportStaff 与 员工扩展实体列表StaffVos 不为空；
                if (dtImportStaff==null&& StaffVos==null)
                {
                    MessageBox.Show("表格数据为空，请导入Excel数据！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //循环临时列表
                for (int i = dtImportStaff.Rows.Count-1; i >= 0; i--)
                {
                    if (Convert.ToBoolean(dtImportStaff.Rows[i]["Chked"])== true)
                    {
                        //标记移除
                        dtImportStaff.Rows[i].Delete();
                    }
                }
                //移除确认
                dtImportStaff.AcceptChanges();
                //更新实体列表
                StaffVos = ModelConvertHelper<StaffVo>.ConvertToModel(dtImportStaff);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.7 保存数据（把数据新增到数据库）
        /// </summary>
        private void SaveStaffs(Window window)
        {
            //1、判断数据准确的;（基本数据不为空、重复性、正则表达式）
            //2、获取数据实体接收数据
            //3、批量添加数据
            try
            {
                //1、 判断临时表dtImportStaff 与 员工扩展实体列表StaffVos 不为空；
                if (dtImportStaff == null && StaffVos == null)
                {
                    MessageBox.Show("表格数据为空，请导入Excel数据！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //提取现有的数据，linq查询数据表员工--用于判断重复
                List<S_Staff> allEmployees = (from tb in myModel.S_Staff
                                              select tb).ToList();
                //定义存放新增数据列表
                List<S_Staff> saveEmployees = new List<S_Staff>();
                for (int i = 0; i < StaffVos.Count; i++)
                {
                    try
                    {
                        //实例化实体：添加每一条数据
                        S_Staff s_Staff = new S_Staff();

                        #region （基本数据不为空、重复性、正则表达式）
                        //1-工号(基本数据不为空、重复性)
                        int oldCount = allEmployees.Count(o => o.EMPNO == StaffVos[i].EMPNO);
                        if (oldCount>0)
                        {
                            MessageBox.Show("第"+(i+1)+"条数据的工号"+ StaffVos[i].EMPNO + "重复！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        s_Staff.EMPNO = StaffVos[i].EMPNO;


                        //2-姓名
                        if (string.IsNullOrEmpty(StaffVos[i].name))
                        {
                            MessageBox.Show("第" + (i + 1) + "条数据的姓名 未填写！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        s_Staff.name = StaffVos[i].name;
                        //3-电话                    
                        if (string.IsNullOrEmpty(StaffVos[i].phone) ||
                            !Regex.IsMatch(StaffVos[i].phone, "^1(3\\d|4[5-9]|5[0-35-9]|6[567]|7[0-8]|8\\d|9[0-35-9])\\d{8}$"))
                        {
                            MessageBox.Show("第" + (i + 1) + "条数据的 电话 不正确，请检查!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        s_Staff.phone = StaffVos[i].phone;

                        //4-身份证号                    
                        if (string.IsNullOrEmpty(StaffVos[i].IDNumber) || !CheckIDCard18.CheckIDCard(StaffVos[i].IDNumber.Trim()))
                        {
                            MessageBox.Show("第" + (i + 1) + "条数据的 身份证号 不正确，请检查!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        s_Staff.IDNumber = StaffVos[i].IDNumber;

                        //5-性别                    
                        if (string.IsNullOrEmpty(StaffVos[i].sex) || !Regex.IsMatch(StaffVos[i].sex, "^男$|^女$"))
                        {
                            MessageBox.Show("第" + (i + 1) + "条数据的 性别 不正确，请检查!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        s_Staff.sex = StaffVos[i].sex;

                        //6-z职务     
                        s_Staff.position = StaffVos[i].position;

                        //7-入职日期 
                        s_Staff.entryDate = DateTime.Now;

                        //8-状态 
                        s_Staff.statust = StaffVos[i].statust;

                        //9-地址  
                        s_Staff.address = StaffVos[i].address;
                        #endregion
                        //添加到要保存的列表saveEmployees
                        saveEmployees.Add(s_Staff);
                        //添加到用于查重的列表allEmployees
                        allEmployees.Add(s_Staff);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                //批量添加数据
                #region 批量添加数据
                using (TransactionScope transaction=new TransactionScope())
                {
                    try
                    {
                        myModel.S_Staff.AddRange(saveEmployees);
                        myModel.SaveChanges();
                        transaction.Complete();
                        MessageBox.Show("数据导入成功，成功导入" + saveEmployees.Count() + "条用户数据!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        window.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        MessageBox.Show("数据导入保存失败!", "系统提示", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                   
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
