﻿#pragma checksum "..\..\MathEditor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "771C110CDA70F1ADFA6BD96C09B930DD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using ClientView;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ClientView {
    
    
    /// <summary>
    /// MathEditor
    /// </summary>
    public partial class MathEditor : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 19 "..\..\MathEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas editorCanvas;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\MathEditor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox caretTextBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ClientView;component/matheditor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MathEditor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 12 "..\..\MathEditor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.EquationTypeButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 13 "..\..\MathEditor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.SuperscriptTypeButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 14 "..\..\MathEditor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.RadicalTypeButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 15 "..\..\MathEditor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NextPartButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 16 "..\..\MathEditor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BackSpaceButton_Clicked);
            
            #line default
            #line hidden
            return;
            case 6:
            this.editorCanvas = ((System.Windows.Controls.Canvas)(target));
            
            #line 19 "..\..\MathEditor.xaml"
            this.editorCanvas.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.editorCanvas_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 19 "..\..\MathEditor.xaml"
            this.editorCanvas.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.editorCanvas_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.caretTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 20 "..\..\MathEditor.xaml"
            this.caretTextBox.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.caretTextBox_PreviewTextInput);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

