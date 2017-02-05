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

namespace ClientView
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveEquationsButton_Clicked(object sender, RoutedEventArgs e)
        {
            //mathEditor.SaveMathEquationText();
        }

        private void OpenEquationsButton_Clicked(object sender, RoutedEventArgs e)
        {
            //mathEditor.OpenMathEquationText();
        }

        private void CareerButton_Clicked(object sender, RoutedEventArgs e)
        {
            mainPage.Visibility = Visibility.Collapsed;
            careerPage.Visibility = Visibility.Visible;
            workPage.Visibility = Visibility.Collapsed;
        }

        private void CareerReturnButton_Clicked(object sender, RoutedEventArgs e)
        {
            mainPage.Visibility = Visibility.Visible;
            careerPage.Visibility = Visibility.Collapsed;
            workPage.Visibility = Visibility.Collapsed;
        }

        private void SectionButton_Clicked(object sender, RoutedEventArgs e)
        {
            mainPage.Visibility = Visibility.Collapsed;
            careerPage.Visibility = Visibility.Collapsed;
            workPage.Visibility = Visibility.Visible;
        }

        private void WorkPageReturnButton_Clicked(object sender, RoutedEventArgs e)
        {
            mainPage.Visibility = Visibility.Visible;
            careerPage.Visibility = Visibility.Collapsed;
            workPage.Visibility = Visibility.Collapsed;
        }

        private void InputCharButton_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;
            if (mathEditor.caretTextBox.IsFocused)
            {
                double offsetX = mathEditor.AcceptEnglishInputText(0, 0, buttonText.Text, mathEditor.caretTextBox.FontSize, buttonText.FontFamily, buttonText.FontStyle);
                mathEditor.RefreshAfterInput(offsetX, 0);
            }
        }

        private void InputCommandButton_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;
            if (mathEditor.caretTextBox.IsFocused)
            {
                string tagString = btn.Tag as string;
                if (tagString == "1")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.FractionCommand);
                }
                else if (tagString=="2")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.Exponential);
                }
                else if (tagString=="3")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.Radical);
                }
                else if (tagString=="4")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.NextCommand);
                }
                else if (tagString=="5")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.DeleteCommand);
                }
            }
        }

        private void serializeTestButton_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;
            mathEditor.SaveMathEquationText();
        }
    }
}
