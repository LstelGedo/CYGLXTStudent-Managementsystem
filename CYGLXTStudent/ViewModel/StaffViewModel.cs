using CYGLXTStudent.Model;
using CYGLXTStudent.Model.Vo;
using CYGLXTStudent.Resources.Control;
using CYGLXTStudent.Resources.PublicClass;
using CYGLXTStudent.ViewModel.Staffs;
using CYGLXTStudent.Views.Staffs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace CYGLXTStudent.ViewModel
{
    /// <summary>
    /// 员工管理
    /// </summary>
    public class StaffViewModel: ViewModelBase
    {
        public StaffViewModel()
        {
            //加载命令执行异步方法（显示第一页）
            LoadedCommand = new RelayCommand(QueryCommandFunc);
            //其他页命名执行分页查询异步方法（其它页）
            NextPageSearchCommand = new RelayCommand(NextPageSearchCommandFunc);
            //清空条件
            ClearCommand = new RelayCommand(ClearConditions);
            //打开新增窗口
            AddCommand = new RelayCommand(OpenAddWindow);
            //打开修改窗口
            EditCommand = new RelayCommand(OpenUpdateWindow);
            //删除员工信息
            DeleteCommand = new RelayCommand(DeleteStaff);
            //员工离职
            DepartureCommand = new RelayCommand(DepartureStaff);
            //打印工作证
            PrintCommand = new RelayCommand(PrintStaff);
            //导出员工信息
            ExportCommand = new RelayCommand(ExportStaff);
            //导入员工信息
            ImportCommand = new RelayCommand(ImportStaff);
        }
        #region 【0、全局变量】
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        readonly CYGLXTEntities myModel = new CYGLXTEntities();
        /// <summary>
        /// 当前用户控件
        /// </summary>
        public System.Windows.Controls.UserControl UC;
        /// <summary>
        /// 消费分页(封装分页控件PagingControl)
        /// </summary>
        public PagingControl ConsumptionPager;
        /// <summary>
        /// 员工表格
        /// </summary>
        public System.Windows.Controls.DataGrid StaffDataGrid;
        #endregion
        #region 【1、属性 】
        /// <summary>
        /// 字段：员工实体(记录选中项)
        /// </summary>
        private S_Staff selectStaffEntity;
        /// <summary>
        /// 属性：员工实体(记录选中项)
        /// </summary>
        public S_Staff SelectStaffEntity
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

        /// <summary>
        /// 字段：员工扩展列表(记录查询数据)
        /// </summary>
        private List<StaffVo> staffvos;
        /// <summary>
        /// 属性：员工扩展列表(记录查询数据)
        /// </summary>
        public List<StaffVo> StaffVos
        {
            get { return staffvos; }
            set
            {
                if (staffvos != value)
                {
                    staffvos = value;
                    RaisePropertyChanged(() => StaffVos);
                }
            }
        }

        #region 【条件查询属性】
        /// <summary>
        /// 字段：工号
        /// </summary>
        private string eMPNO;
        /// <summary>
        /// 属性：工号
        /// </summary>
        public string EMPNO
        {
            get { return eMPNO; }
            set
            {
                if (eMPNO != value)
                {
                    eMPNO = value;
                    RaisePropertyChanged(() => EMPNO);

                }
            }
        }
        /// <summary>
        /// 字段：员工姓名
        /// </summary>
        private string staffName;
        /// <summary>
        /// 属性：员工姓名
        /// </summary>
        public string StaffName
        {
            get { return staffName; }
            set
            {
                if (staffName != value)
                {
                    staffName = value;
                    RaisePropertyChanged(() => StaffName);

                }
            }
        }
        /// <summary>
        /// 字段：身份证
        /// </summary>
        private string iDNumber;
        /// <summary>
        /// 属性：身份证
        /// </summary>
        public string IDNumber
        {
            get { return iDNumber; }
            set
            {
                if (iDNumber != value)
                {
                    iDNumber = value;
                    RaisePropertyChanged(() => IDNumber);

                }
            }
        }

        /// <summary>
        /// 字段：性别
        /// </summary>
        private string sex;
        /// <summary>
        /// 属性：性别
        /// </summary>
        public string Sex
        {
            get { return sex; }
            set
            {
                if (sex != value)
                {
                    sex = value;
                    RaisePropertyChanged(() => Sex);

                }
            }
        }
        /// <summary>
        /// 字段：职务
        /// </summary>
        private string position;
        /// <summary>
        /// 属性：职务
        /// </summary>
        public string Position
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value;
                    RaisePropertyChanged(() => Position);

                }
            }
        }
        /// <summary>
        /// 字段：状态
        /// </summary>
        private string statust;
        /// <summary>
        /// 属性：状态
        /// </summary>
        public string Statust
        {
            get { return statust; }
            set
            {
                if (statust != value)
                {
                    statust = value;
                    RaisePropertyChanged(() => Statust);

                }
            }
        }
        #endregion
        #region 【分页相关属性】  
        /// <summary>
        /// 字段：总页数
        /// </summary>
        private string totalPage = string.Empty;
        /// <summary>
        /// 属性：总页数
        /// </summary>
        public string TotalPage
        {
            get { return totalPage; }
            set
            {
                totalPage = value;
                RaisePropertyChanged(() => TotalPage);
            }
        }
        /// <summary>
        /// 字段：当前页
        /// </summary>
        private string currentPage = "1";
        /// <summary>
        /// 属性：当前页
        /// </summary>
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
            }
        }
        /// <summary>
        /// 字段：每页显示的记录数
        /// </summary>
        private int pageSize = 16;//默认16条
        /// <summary>
        /// 属性：每页显示的记录数
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }
        /// <summary>
        /// 字段：页索引
        /// </summary>
        private int pageIndex;
        /// <summary>
        /// 属性：页索引
        /// </summary>
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                RaisePropertyChanged(() => PageIndex);
            }
        }
        /// <summary>
        /// 字段：总条数
        /// </summary>
        private int totalCount;
        /// <summary>
        /// 属性：总条数
        /// </summary>
        public int TotalCount
        {
            get { return totalCount; }
            set
            {
                totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }

        #endregion
        #endregion
        #region 【2、命令】  
        /// <summary>
        /// 2.1 加载命令
        /// </summary>
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 2.2 分页管理命令
        /// </summary>
        public RelayCommand NextPageSearchCommand { get; set; }
        /// <summary>
        /// 2.3 添加命令
        /// </summary>
        public ICommand AddCommand { get; set; }
        /// <summary>
        /// 2.4 打印工作证命令
        /// </summary>
        public ICommand PrintCommand { get; set; }
        /// <summary>
        /// 2.5 修改命令
        /// </summary>
        public ICommand EditCommand { get; set; }
        /// <summary>
        /// 2.6 离职命令
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        /// <summary>
        /// 2.7 导入Excel命令
        /// </summary>
        public ICommand ImportCommand { get; set; }
        /// <summary>
        /// 2.8 导出Excel命令
        /// </summary>
        public ICommand ExportCommand { get; set; }
        /// <summary>
        /// 2.9 删除命令
        /// </summary>
        public ICommand DepartureCommand { get; set; }
        /// <summary>
        /// 3.0 清空条件命令
        /// </summary>
        public ICommand ClearCommand { get; set; }

        #endregion
        #region 【3、方法】
        #region  0、【分页执行方法】
        /// <summary>
        /// 3.1 异步方法，查询第一页
        /// </summary>
        private async void QueryCommandFunc()
        {
            //await：为不返回值的方法指定返回值类型。
            //Run：将在线程池上运行的指定工作排队，并返回该工作的任务句柄。
            await Task.Run(() =>
            {
                //out输出参数(使用 out 参数有两个注意事项:①传方法到参数之前，可以不用先赋值;②在方法内部，必须要有给参数赋值的语句)。
                StaffVos = GetData(PageSize, out int totalCount);
                if (totalCount % PageSize == 0)
                {
                    //（1）总条数/每页显示记录数余数为0（代表能除尽）
                    //总页数=总条数/每页显示记录数
                    TotalPage = (totalCount / PageSize).ToString();
                }
                else
                {
                    //（2）总条数/每页显示记录数余数不为0（代表不能除尽）
                    //总页数=（总条数/每页显示记录数）+1
                    TotalPage = ((totalCount / PageSize) + 1).ToString();
                }

            });
        }

        /// <summary>
        /// 3.2 其他页查询
        /// </summary>
        private async void NextPageSearchCommandFunc()
        {
            await Task.Run(() =>
            {
                //当前页
                var pageIndex = System.Convert.ToInt32(CurrentPage);
                StaffVos = QueryData(pageIndex, PageSize);
            });
        }
        /// <summary>
        /// 3.3 初次获取数据（显示第一页）
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        private List<StaffVo> GetData(int pageSize, out int totalCount)
        {

            //调用查询方法
            var myStaff = GetAllStaffBy(EMPNO, StaffName, IDNumber, Sex, Position, Statust);
            //分页查询
            List<StaffVo> list = myStaff
                .OrderByDescending(a => a.staffID)//根据角色ID排序
                .Skip(0)//跳过前面页数的数据（索引）第一页
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合

            totalCount = myStaff.Count;//总行数     
            return list;
        }
        /// <summary>
        /// 3.4 页面跳转
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        private List<StaffVo> QueryData(int pageIndex, int pageSize)
        {
            //调用查询方法
            var myStaff = GetAllStaffBy(EMPNO, StaffName, IDNumber, Sex, Position, Statust);
            //分页查询
            List<StaffVo> list = myStaff
                .OrderByDescending(a => a.staffID)//倒叙排序
                .Skip((pageIndex - 1) * pageSize)//跳过前面页数的数据（开始索引）
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            return list;

        }
        #endregion
        #region 1、【公共方法封装】
        /// <summary>
        /// 3.1 条件查询所有员工数据
        /// </summary>
        /// <param name="EMPNO">工号</param>
        /// <param name="Name">姓名</param>
        /// <param name="IDNumber">身份证号码</param>
        /// <param name="Sex">性别</param>
        /// <param name="Position">职务</param>
        /// <param name="Statust">状态</param>
        /// <returns></returns>
        public List<StaffVo> GetAllStaffBy(string EMPNO, string Name, string IDNumber, string Sex, string Position, string Statust)
        {
            try
            {
                //查询全部员工信息
                var list = (from tbStaff in myModel.S_Staff
                            orderby tbStaff.staffID descending//倒叙排序
                            select new StaffVo
                            {
                                staffID = tbStaff.staffID,//员工ID
                                EMPNO = tbStaff.EMPNO,//工号
                                name = tbStaff.name,//姓名
                                phone = tbStaff.phone,//电话
                                IDNumber = tbStaff.IDNumber,//身份证号码
                                sex = tbStaff.sex,//性别
                                position = tbStaff.position,//职务
                                entryDate = tbStaff.entryDate,//入职日期
                                departureDate = tbStaff.departureDate,//离职日期
                                statust = tbStaff.statust,//状态
                                picture = tbStaff.picture,//图片
                                address = tbStaff.address,//住址
                                //按钮显示与隐藏
                                IsVisibility = tbStaff.statust,//状态（工作证、修改、离职）
                                DeleteVisibility = tbStaff.position,//职务(删除)
                            });
                //条件筛选
                //工号
                if (!string.IsNullOrEmpty(EMPNO))
                {
                    list = list.Where(m => m.EMPNO.Contains(EMPNO));
                }
                //姓名
                if (!string.IsNullOrEmpty(Name))
                {
                    list = list.Where(m => m.name.Contains(Name));
                }
                //身份证
                if (!string.IsNullOrEmpty(IDNumber))
                {
                    list = list.Where(m => m.IDNumber.Contains(IDNumber));
                }
                //性别
                if (!string.IsNullOrEmpty(Sex))
                {
                    list = list.Where(m => m.sex.Contains(Sex));
                }
                //职务
                if (!string.IsNullOrEmpty(Position))
                {
                    list = list.Where(m => m.position.Contains(Position));
                }
                //状态
                if (!string.IsNullOrEmpty(Statust))
                {
                    list = list.Where(m => m.statust.Contains(Statust));
                }
                return list.ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
        /// <summary>
        /// 3.6 清空条件
        /// </summary>
        private void ClearConditions()
        {
            EMPNO = string.Empty;
            StaffName= string.Empty;
            IDNumber = string.Empty;
            Sex = string.Empty;
            Position = string.Empty;
            Statust = string.Empty;
        }
        /// <summary>
        /// 3.7 打开新增员工窗口
        /// </summary>
        private void OpenAddWindow()
        {
            try
            {
                //1、实例化窗口
                StaffAddOrEditWindow window = new StaffAddOrEditWindow();
                //2、获取窗口绑定数据上下文 ViewModel;
                var viewModel=window.DataContext as StaffAddOrEditViewModel;
                //3、传递参数给到ViewModel(员工实体、新增/修改标志)；
                viewModel.CurrentStaffEntity = new S_Staff();
                viewModel.IsAdd=true;//新增
                //4、委托事件调用达到刷新表格数据；
                viewModel.RefreshEvent += QueryCommandFunc;
                //5、打开窗口
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.8 打开修改员工窗口
        /// </summary>
        private void OpenUpdateWindow()
        {
            try
            {
                if (SelectStaffEntity!=null)
                {
                    //1、实例化窗口
                    StaffAddOrEditWindow window = new StaffAddOrEditWindow();
                    //2、获取窗口绑定数据上下文 ViewModel;
                    var viewModel = window.DataContext as StaffAddOrEditViewModel;
                    //3、传递参数给到ViewModel(员工实体、新增/修改标志、图片)；
                    viewModel.CurrentStaffEntity = SelectStaffEntity;
                    viewModel.IsAdd = false;//修改
                    //图片
                    viewModel.CaseCoverImage = GetImage.Bytearraytobitmapimage(SelectStaffEntity.picture);
                    //4、委托事件调用达到刷新表格数据；
                    viewModel.RefreshEvent += QueryCommandFunc;
                    //5、打开窗口
                    window.ShowDialog();
                }
                else
                {
                    System.Windows.MessageBox.Show("请选择要修改的员工", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }      
        /// <summary>
        /// 3.9 删除员工信息
        /// </summary>
        private void DeleteStaff()
        {
            /*
            * 思路：删除员工存在两种情况：
            * 一、分配权限组
            *   ①超级管理员：不能删除；
            *   ②其他权限组：删除员工表S_Staff、删除权限组用户表R_JurisdictionGroupUser
            * 二、未分配权限组
            *    直接删除员工表S_Staff、
            */
            try
            {
                if (SelectStaffEntity!=null)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("请确认是否删除员工：" + SelectStaffEntity.name, "系统提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        //判断是否分配权限
                        int intCount = myModel.R_JurisdictionGroupUser.Where(o => o.staffID == SelectStaffEntity.staffID).Count();
                        if (intCount > 0)
                        {
                            //一、分配权限组
                            //*   ①超级管理员：不能删除；
                            using (TransactionScope transaction = new TransactionScope())
                            {
                                //*②其他权限组：删除员工表S_Staff、删除权限组用户表R_JurisdictionGroupUser
                                var deleteStaff = myModel.S_Staff.Find(SelectStaffEntity.staffID);
                                myModel.S_Staff.Remove(deleteStaff);
                                //删除权限组用户表R_JurisdictionGroupUser
                                var deleteJGU = myModel.R_JurisdictionGroupUser.Where(o => o.staffID == SelectStaffEntity.staffID).Single();
                                myModel.R_JurisdictionGroupUser.Remove(deleteJGU);
                                if (myModel.SaveChanges() > 0)
                                {
                                    //提交事务
                                    transaction.Complete();
                                    System.Windows.MessageBox.Show(deleteStaff.name + "删除成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show(deleteStaff.name + "删除失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }

                        }
                        else
                        {
                            //二、未分配权限组
                            //直接删除员工表S_Staff、  
                            var deleteStaff = myModel.S_Staff.Find(SelectStaffEntity.staffID);
                            myModel.S_Staff.Remove(deleteStaff);
                            if (myModel.SaveChanges() > 0)
                            {
                                System.Windows.MessageBox.Show(deleteStaff.name + "删除成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show(deleteStaff.name + "删除失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }   
                }
                else
                {
                    System.Windows.MessageBox.Show("请选择要删除的员工数据！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.10 员工离职
        /// </summary>
        private void DepartureStaff()
        {
            /*
            * 思路：
            * 1、查询员工是否分配账号
            * 一、分配权限组
            * ①超级管理员：不能被离职；
            * 二、未分配
            * ① 根据主键ID查询修改的员工信息
            * ② 其他员工修改:状态=离职，修改离职日期=当前时间 
            */
            try
            {
                if (SelectStaffEntity!=null)
                {
                    //1、查询员工是否分配账号
                    //判断是否分配权限
                    int intCount = myModel.R_JurisdictionGroupUser.Where(o => o.staffID == SelectStaffEntity.staffID).Count();
                    if (intCount > 0)
                    {
                        // *一、分配权限组
                        //* ①超级管理员：不能被离职；
                        if (SelectStaffEntity.position== "超级管理员")
                        {
                            System.Windows.MessageBox.Show("超级管理员：不能被离职！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }                       
                    }
                    MessageBoxResult result = System.Windows.MessageBox.Show("请确认该员工是否离职？", "系统提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        //* ① 根据主键ID查询修改的员工信息
                        var updateStaff = myModel.S_Staff.Find(SelectStaffEntity.staffID);
                        //* ② 其他员工修改: 状态 = 离职，修改离职日期 = 当前时间
                        updateStaff.statust = "离职";
                        updateStaff.departureDate = DateTime.Now;
                        myModel.Entry(updateStaff).State = System.Data.Entity.EntityState.Modified;
                        if (myModel.SaveChanges() > 0)
                        {
                            System.Windows.MessageBox.Show(updateStaff.name + "离职成功！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(updateStaff.name + "离职失败！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                    }  
                }
                else
                {
                    System.Windows.MessageBox.Show("请选择要离职的员工！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.11 打印工作证
        /// </summary>
        private void PrintStaff()
        {
            try
            {
                //1、判断是否选中实体
                if (SelectStaffEntity!=null)
                {
                    //2、实例化打印工作证的窗口
                    PrintStaffWindow print = new PrintStaffWindow();
                    //3、获取窗口绑定数据上下文ViewModel
                    PrintStaffViewModel viewModel = print.DataContext as PrintStaffViewModel;
                    //4、参数传递；
                    viewModel.CurrentStaffEntity = SelectStaffEntity;//员工实体
                    viewModel.CaseCoverImage = ByteToBitmapimage.Bytearraytobitmapimage(SelectStaffEntity.picture);//图片                   
                    //5、打开窗口
                    print.ShowDialog();
                }
                else
                {
                    System.Windows.MessageBox.Show("请选择要打印工作证的员工！", "系统提示：", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "系统提示：", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 3.12 导出Excel
        /// </summary>
        private void ExportStaff()
        {

            try
            {
                //1、调用方法：条件查询所有员工列表数据；
                var list = GetAllStaffBy(EMPNO, StaffName, IDNumber, Sex, Position, Statust);
                //2、实例化创建一个Excel运行环境:;
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                //3、创建Excel中的工作薄
                Workbook excelWB = excelApp.Workbooks.Add(Type.Missing);
                //4、创建Excel工作薄中的页sheet （1表示在子表sheet1里进行数据导出）
                Worksheet sheet = (Worksheet)excelWB.Worksheets[1];
                //5、如果数据中存在数字类型 可以让它变文本格式显示(身份证)
                sheet.Cells.NumberFormat = "@";
                //6、表头数据导出；第一行标题，第二行表头
                string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                sheet.Cells[1, 9] = "员工信息" + dtime;
                //第二行表头
                sheet.Cells[2, 1] = "序号";
                sheet.Cells[2, 2] = "工号";
                sheet.Cells[2, 3] = "姓名";
                sheet.Cells[2, 4] = "电话";
                sheet.Cells[2, 5] = "身份证号";
                sheet.Cells[2, 6] = "性别";
                sheet.Cells[2, 7] = "职务";
                sheet.Cells[2, 8] = "入职时间";
                sheet.Cells[2, 9] = "离职时间";
                sheet.Cells[2, 10] = "状态";
                sheet.Cells[2, 11] = "住址";
                //7、for 循环列表，将数据导入到工作表的单元格（从第三行开始）；
                for (int i = 0; i < list.Count; i++)
                {
                    sheet.Cells[i + 3, 1] = i + 1;//序号
                    sheet.Cells[i + 3, 2] = list[i].EMPNO.Trim();//工号
                    sheet.Cells[i + 3, 3] = list[i].name.Trim();//姓名
                    sheet.Cells[i + 3, 4] = list[i].phone.Trim();//电话
                    sheet.Cells[i + 3, 5] = list[i].IDNumber.Trim();//身份证号
                    sheet.Cells[i + 3, 6] = list[i].sex.Trim();//性别
                    sheet.Cells[i + 3, 7] = list[i].position.Trim();//职务
                    sheet.Cells[i + 3, 8] = list[i].entryDate.ToString().Trim();//入职时间
                    sheet.Cells[i + 3, 9] = list[i].departureDate.ToString().Trim();//离职时间
                    sheet.Cells[i + 3, 10] = list[i].statust.Trim();//状态
                    sheet.Cells[i + 3, 11] = list[i].address.Trim();//住址
                }
                //8、 Excel表格样式设置；
                #region Excel表格样式设置
                //第一行标题：合并单元格
                sheet.get_Range("A1", "K1").Merge(sheet.get_Range("A1", "K1").MergeCells);
                //第二行表头：设置Excel表格的列宽
                sheet.get_Range("A1", "D1").ColumnWidth = 8;//A1-C1列宽
                sheet.get_Range("D1", "H1").ColumnWidth = 15;//D1-G1列宽
                sheet.get_Range("H1", "J1").ColumnWidth = 20;//H1-I1列宽
                sheet.get_Range("K1", "K1").ColumnWidth = 40;//K1列宽
                                                             //行高
                sheet.get_Range("A1", "K1").RowHeight = 24;//第一行标题，
                sheet.get_Range("A2", "K10000").RowHeight = 20;//第二行表头、第三行之后数据行
                                                               //设置单元格的填充颜色
                sheet.get_Range("A1", "K1").Interior.ColorIndex = 35;
                //设置单元格字体颜色
                sheet.get_Range("A1", "K1").Font.Color = -16744448;//（搜索Excel颜色对照表）第一行标题
                sheet.get_Range("A2", "K2").Font.Color = -16776961;//（搜索Excel颜色对照表）第二行表头

                //得到 Range 范围， 域对象
                Range range = sheet.get_Range("A1", "K1");//第一行标题
                                                          //文字
                range.HorizontalAlignment = XlHAlign.xlHAlignCenter;// 居中
                range.Font.Size = 16;//字体大小
                range.Font.Bold = 2;//加粗
                range.Borders.LineStyle = 1;//边框

                Range range1 = sheet.get_Range("A2", "K2");//第二行表头
                                                           //文字
                range1.HorizontalAlignment = XlHAlign.xlHAlignCenter;// 居中
                range1.Font.Size = 10;//字体大小
                range1.Font.Bold = 1;//加粗
                range1.Borders.LineStyle = 1;//边框
                #endregion
                //9、提示用户选择文件夹,导出文件位置;
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description = "请选择文件保存位置"
                };
                string foldPath = string.Empty;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //选择文件保存路径
                    foldPath = dialog.SelectedPath;
                }
                //10、保存文件，提示用户。
                excelWB.SaveAs(foldPath + "\\员工信息Excel文件.xlsx");
                excelWB.Close();
                excelApp.Quit();
                System.Windows.MessageBox.Show("员工信息Excel文件导出成功！", "提示：", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                return;
            }

        }
        /// <summary>
        /// 3.13 打开导入窗口
        /// </summary>
        private void ImportStaff()
        {
            //实例导入窗口
            ImportStaffWindow import = new ImportStaffWindow();
            import.ShowDialog();
        }
        #endregion
    }
}
