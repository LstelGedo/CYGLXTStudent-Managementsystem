using System.Windows.Controls;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// DMenu_UC.xaml 的交互逻辑
    /// </summary>
    public partial class DMenu_UC : UserControl
    {
        public DMenu_UC()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.DMenuUCViewModel();
        }
    }
}
