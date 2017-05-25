using Entities;
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
          
        }

        private void InputCommandButton_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            
        }

        private void serializeTestButton_Clicked(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;

            MathProblem problem = new MathProblem();
            problem.ID = Guid.NewGuid().ToString();
            problem.Type = "选择题";
            problem.Grade = "初一年级";
            problem.Chapter = "直线方程";
            problem.Knowledges = "直线函数，方程";
            problem.Publisher = "北京师范大学出版社";
            problem.Problem = mathEditor.GetEditorTypeInString();
            problem.Graphic = "";

            string problemJson = Newtonsoft.Json.JsonConvert.SerializeObject(problem);

            MathProblemService.SystemManagementServiceClient serviceClient = new MathProblemService.SystemManagementServiceClient();
            serviceClient.InsertMathProblem(problemJson); 

        }

        private void MathKeyBoard_Clicked(object sender, MouseButtonEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            if (null==btn)
            {
                return;
            }
            var parameter=btn.Tag.ToString();
            if (parameter=="char")
            {
                MathKeyBoardCharInputed(btn);
            }
            else if (parameter=="cmd")
            {
                MathKeyBoardCommandInputed(btn);
            }
        }

        private void MathKeyBoardCommandInputed(Button btn)
        {
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;
            if (mathEditor.caretTextBox.IsFocused)
            {
                string parameter = btn.CommandParameter as string;
                if (parameter == "FractionCommand")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.FractionCommand);
                }
                else if (parameter == "Exponential")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.Exponential);
                }
                else if (parameter == "Radical")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.Radical);
                }
                else if (parameter == "NextCommand")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.NextCommand);
                }
                else if (parameter == "DeleteCommand")
                {
                    mathEditor.AcceptInputCommand(MathData.InputCommands.DeleteCommand);
                }
            }
        }

        private void MathKeyBoardCharInputed(Button btn)
        {
            TextBlock buttonText = btn.Content as TextBlock;
            var tabItem = friendAnswerBoardTab.Items[0] as TabItem;
            var mathEditor = tabItem.Content as MathEditor;
            if (mathEditor.caretTextBox.IsFocused)
            {
                double offsetX = mathEditor.AcceptEnglishInputText(0, 0, buttonText.Text, mathEditor.caretTextBox.FontSize, buttonText.FontFamily, buttonText.FontStyle);
                mathEditor.RefreshAfterInput(offsetX, 0);
            }
        }
    }
}
