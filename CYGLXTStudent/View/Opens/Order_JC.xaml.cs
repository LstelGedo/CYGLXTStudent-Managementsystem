using System.Windows;

namespace CYGLXTStudent.Views.Opens
{
    /// <summary>
    /// Order_JC.xaml 的交互逻辑
    /// </summary>
    public partial class Order_JC : Window
    {
        public Order_JC()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.Opens.OrderJCViewModel();
        }
    }
}
