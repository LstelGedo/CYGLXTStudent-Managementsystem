using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CYGLXTStudent.Resources.Control
{
    /// <summary>
    /// PagingControl.xaml 的交互逻辑
    /// </summary>
    public partial class PagingControl : UserControl
    {
        public PagingControl()
        {
            InitializeComponent();
            prePageBtn.IsEnabled = false;
        }
        private DataTable _dt = new DataTable();
        /// <summary>
        /// 每页显示多少条
        /// </summary>
        private int pageNum = 10;
        /// <summary>
        /// 当前是第几页
        /// </summary>
        private int pIndex = 1;
        ///对象        
        private DataGrid grdList;
        /// <summary>
        /// 最大页数
        /// </summary>
        private int MaxIndex = 1;
        /// <summary>
        /// 一共多少条
        /// </summary>
        private int allNum = 0;

        #region 初始化数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="grd"></param>
        /// <param name="dtt"></param>
        /// <param name="Num"></param>
        public void ShowPages(DataGrid grd, DataTable dt)
        {
            _dt = dt.Clone();
            grdList = grd;
            pIndex = int.Parse(pageNoTbk.Text != "" ? pageNoTbk.Text : "1");
            foreach (DataRow r in dt.Rows)
                _dt.ImportRow(r);

            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 自定义页显示条数
        /// </summary>
        /// <param name="grd"></param>
        /// <param name="dt"></param>
        /// <param name="bol">条数</param>
        public void ShowPagesT(DataGrid grd, DataTable dt, int bol)
        {
            _dt = dt.Clone();
            grdList = grd;
            pageNum = bol;
            pIndex = int.Parse(pageNoTbk.Text != "" ? pageNoTbk.Text : "1");
            foreach (DataRow r in dt.Rows)
                _dt.ImportRow(r);

            SetMaxIndex();
            ReadDataTable();
        }

        #endregion

        #region 画数据
        /// <summary>
        /// 画数据
        /// </summary>
        private void ReadDataTable()
        {
            try
            {
                pageNoTbk.Text = pIndex.ToString(); //当前第几页
                pageCountTbk.Text = MaxIndex.ToString();//总共页数
                DataTable tmpTable = new DataTable();
                tmpTable = _dt.Clone();
                int first = pageNum * (pIndex - 1);
                first = (first > 0) ? first : 0;
                //如何总数量大于每页显示数量
                if (_dt.Rows.Count >= pageNum * pIndex)
                {
                    for (int i = first; i < pageNum * pIndex; i++)
                        tmpTable.ImportRow(_dt.Rows[i]);
                }
                else
                {
                    for (int i = first; i < _dt.Rows.Count; i++)
                        tmpTable.ImportRow(_dt.Rows[i]);
                }
                grdList.ItemsSource = tmpTable.DefaultView;
                currentCountTbk.Text = grdList.Items.Count.ToString();//当前页面条数
                totalCountTbk.Text = _dt.Rows.Count.ToString();//总共条数
                tmpTable.Dispose();
            }
            catch
            {
                MessageBox.Show("错误", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            finally
            {
                DisplayPagingInfo();
            }

        }

        #endregion

        #region 每页显示等数据
        /// <summary>
        /// 每页显示等数据
        /// </summary>
        private void DisplayPagingInfo()
        {
            if (MaxIndex == 1)
            {
                firstPageBtn.IsEnabled = false;
                prePageBtn.IsEnabled = false;
                nextPageBtn.IsEnabled = false;
                lastPageBtn.IsEnabled = false;
            }
            else
            {
                if (pIndex == 1)
                {
                    firstPageBtn.IsEnabled = false;
                    prePageBtn.IsEnabled = false;
                    nextPageBtn.IsEnabled = true;
                    lastPageBtn.IsEnabled = true;
                }
                else if (pIndex == MaxIndex)
                {
                    firstPageBtn.IsEnabled = true;
                    prePageBtn.IsEnabled = true;
                    nextPageBtn.IsEnabled = false;
                    lastPageBtn.IsEnabled = false;
                }
                else
                {
                    firstPageBtn.IsEnabled = true;
                    prePageBtn.IsEnabled = true;
                    nextPageBtn.IsEnabled = true;
                    nextPageBtn.IsEnabled = true;
                }
            }
        }
        #endregion


        #region 设置最多大页面
        /// <summary>
        /// 设置最多大页面
        /// </summary>
        private void SetMaxIndex()
        {
            //多少页
            int Pages = _dt.Rows.Count / pageNum;
            if (_dt.Rows.Count != (Pages * pageNum))
            {
                if (_dt.Rows.Count < (Pages * pageNum))
                    Pages--;
                else
                    Pages++;
            }
            MaxIndex = Pages;
            allNum = _dt.Rows.Count;
        }
        #endregion
        /// <summary>
        /// 每页显示条数（设置）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tbl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tbl = sender as TextBlock;
            if (tbl == null)
            {
                return;
            }
            int index = int.Parse(tbl.Text.ToString());
            pIndex = index;
            if (index > MaxIndex)
            {
                pIndex = MaxIndex;

            }
            if (index < 1)
            {
                pIndex = 1;
            }
            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //为空：返回
                if (gotoPageNoTb.Text == "")
                {
                    MessageBox.Show("请输入要跳转的页", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (System.Convert.ToInt32(gotoPageNoTb.Text) == pIndex)
                {
                    MessageBox.Show("跳转的页与当前页一致", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (System.Convert.ToInt32(gotoPageNoTb.Text) > MaxIndex)
                {
                    MessageBox.Show("跳转的页大于最大页", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                if (System.Convert.ToInt32(gotoPageNoTb.Text) <= 0)
                {
                    MessageBox.Show("跳转的页不能为负数", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                pIndex = System.Convert.ToInt32(gotoPageNoTb.Text);
                SetMaxIndex();
                ReadDataTable();
            }
            catch (Exception)
            {
                return;
            }
        }
        /// <summary>
        /// 末页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            pIndex = MaxIndex;
            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pIndex >= MaxIndex)
                return;
            pIndex++;
            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void prePageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pIndex <= 1)
                return;
            pIndex--;
            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void firstPageBtn_Click(object sender, RoutedEventArgs e)
        {
            pIndex = 1;
            SetMaxIndex();
            ReadDataTable();
        }
        /// <summary>
        /// 设置每页显示条数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setPageSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(pageSizeTb.Text.Trim()))
                {
                    pageNum = Convert.ToInt32(pageSizeTb.Text.Trim());
                    pIndex = 1;
                    SetMaxIndex();
                    ReadDataTable();
                }
                else
                {
                    MessageBox.Show("请输入显示条数", "系统提示！", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
