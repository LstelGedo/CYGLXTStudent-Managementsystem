using System.Windows;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// Dining_Options.xaml 的交互逻辑
    /// </summary>
    public partial class Dining_Options : Window
    {
        public Dining_Options()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.DiningOptionsViewModel();
        }
    }
}
