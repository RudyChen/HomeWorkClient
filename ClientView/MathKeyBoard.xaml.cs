using ClientView.Model;
using ClientView.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ClientView
{
    /// <summary>
    /// MathKeyBoard.xaml 的交互逻辑
    /// </summary>
    public partial class MathKeyBoard : UserControl
    {
        public MathKeyBoard()
        {
            InitializeComponent();
        }

        private void InputCharButton_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void InputCommandButton_Clicked(object sender, RoutedEventArgs e)
        {

        }

        private void MathKeyBoard_Loaded(object sender, RoutedEventArgs e)
        {
            var vm=this.DataContext as MathKeyBoardViewModel;
            vm.LoadKey();
        }
    }
}
