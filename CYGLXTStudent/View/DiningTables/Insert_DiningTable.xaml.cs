using System.Windows;

namespace CYGLXTStudent.Views.DiningTables
{
    /// <summary>
    /// Insert_DiningTable.xaml 的交互逻辑
    /// </summary>
    public partial class Insert_DiningTable : Window
    {
        public Insert_DiningTable()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.DiningTables.InsertTableViewModel();
        }
    }
}
