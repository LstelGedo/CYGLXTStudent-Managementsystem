using System.Windows.Controls;

namespace CYGLXTStudent.Views.DiningTables
{
    /// <summary>
    /// DiningTable_XS_UC.xaml 的交互逻辑
    /// </summary>
    public partial class DiningTable_XS_UC : UserControl
    {
        public DiningTable_XS_UC()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.DiningTables.DiningTableXSViewModel();
        }
    }
}
