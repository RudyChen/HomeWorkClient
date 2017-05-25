using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWorkCommon
{
    public class ViewModelBaseEx
    {
        public RelayCommand LoadedCommand { get; protected set; }
        public RelayCommand SendMsg { get; private set; }
        public RelayCommand Chinese { get; private set; }
        public RelayCommand English { get; private set; }

        public ViewModelBaseEx()
        {
            LoadedCommand = new RelayCommand(OnLoaded);
            SendMsg = new RelayCommand(SendMsgIml, CanSendMsg);         
        }

        protected virtual void OnLoaded()
        {
        }

        protected virtual bool CanSendMsg()
        {
            return true;
        }

        protected virtual void SendMsgIml()
        {
        }
    }
}
