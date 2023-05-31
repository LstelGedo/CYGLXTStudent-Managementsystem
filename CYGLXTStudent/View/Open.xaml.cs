using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// Open.xaml 的交互逻辑
    /// </summary>
    public partial class Open : UserControl
    {
        public Open()
        {
            InitializeComponent();
            this.DataContext =new ViewModel.OpenViewModel();
        }
    }
}
