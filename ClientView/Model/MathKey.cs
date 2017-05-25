using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientView.Model
{
   public class MathKey
    {
        private string _group;

        private string _charactor;

        public string Charactor
        {
            get { return _charactor; }
            set { _charactor = value; }
        }
        

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

    }
}
