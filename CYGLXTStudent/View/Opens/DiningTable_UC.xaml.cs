using System.Windows.Controls;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// DiningTable_UC.xaml 的交互逻辑
    /// </summary>
    public partial class DiningTable_UC : UserControl
    {
        public DiningTable_UC()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.DiningTableViewModel();
        }
    }
}
