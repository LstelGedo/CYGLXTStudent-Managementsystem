using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// DiningTable.xaml 的交互逻辑
    /// </summary>
    public partial class DiningTable : UserControl
    {
        public DiningTable()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.DiningTableViewModel();
        }
    }
}
