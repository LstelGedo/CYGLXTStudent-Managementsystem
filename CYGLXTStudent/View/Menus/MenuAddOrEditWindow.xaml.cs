using System.Windows;

namespace CYGLXTStudent.Views.Menus
{
    /// <summary>
    /// MenuAddOrEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MenuAddOrEditWindow : Window
    {
        public MenuAddOrEditWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Menus.MenuAddOrEditViewModel();
        }
    }
}
