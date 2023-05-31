using System.Windows.Controls;

namespace CYGLXTStudent.Views.Menus
{
    /// <summary>
    /// Menu_UC.xaml 的交互逻辑
    /// </summary>
    public partial class Menu_UC : UserControl
    {
        public Menu_UC()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Menus.MenuUCViewModel();
        }
    }
}
