﻿using System.Windows.Controls;

namespace CYGLXTStudent.Views
{
    /// <summary>
    /// Menu.xaml 的交互逻辑
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
            this.DataContext = new ViewModel.MenuViewModel();
        }
    }
}
