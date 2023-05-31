using System.Windows;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// Order_KT.xaml 的交互逻辑
    /// </summary>
    public partial class Order_KT : Window
    {
        public Order_KT()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.OrderKTViewModel();
        }
    }
}
