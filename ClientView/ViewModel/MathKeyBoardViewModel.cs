using ClientView.Model;
using GalaSoft.MvvmLight;
using HomeWorkCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace ClientView.ViewModel
{
    public class MathKeyBoardViewModel : ViewModelBase
    {

        private ObservableCollection<MathKey> _numberKeys=new ObservableCollection<MathKey>();

        public ObservableCollection<MathKey> NumberKeys
        {
            get { return _numberKeys; }
            set { _numberKeys = value; }
        }

        private ObservableCollection<MathKey> _symbolKeys=new ObservableCollection<MathKey>();

        public ObservableCollection<MathKey> SymbolKeys
        {
            get { return _symbolKeys; }
            set { _symbolKeys = value; }
        }


        private ObservableCollection<MathKey> _operatorKeys = new ObservableCollection<MathKey>();

        public ObservableCollection<MathKey> OperatorKeys
        {
            get { return _operatorKeys; }
            set { _operatorKeys = value; }
        }

        internal void LoadKey()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\KeyBoardKeys.xml");
            XmlNode xn = doc.SelectSingleNode("Keys");
            foreach (XmlNode item in xn.ChildNodes)
            {
                XmlElement xe = (XmlElement)item;
                var group = xe.GetAttribute("Group").ToString();
                var charactor = xe.GetAttribute("Charactor").ToString();
                MathKey key = new MathKey() { Group = group, Charactor = charactor };
                if (key.Group == "Operator")
                {
                    OperatorKeys.Add(key);
                }
                else if (key.Group == "Number")
                {
                    NumberKeys.Add(key);
                }
                else if (key.Group == "Symbol")
                {
                    SymbolKeys.Add(key);
                }
                else if (key.Group == "Letter")
                {
                    LetterKeys.Add(key);
                }
            }

           
        }

        private ObservableCollection<MathKey> _letterKeys=new ObservableCollection<MathKey>();

        public ObservableCollection<MathKey> LetterKeys
        {
            get { return _letterKeys; }
            set { _letterKeys = value; }
        }


       

    }
}
