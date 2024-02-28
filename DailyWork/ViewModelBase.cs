﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace DailyWork
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Window Close
        //
        //bool? _CloseWindowFlag;
        //public int _lastGeneratedMonthIndex = 12;
        //public string _lastGeneratedYearValue = string.Empty;
        //public bool? CloseWindowFlag
        //{
        //    get { return _CloseWindowFlag; }
        //    set
        //    {
        //        _CloseWindowFlag = value;
        //        RaisePropertyChanged("CloseWindowFlag");
        //    }
        //}
        //
        //public virtual void CloseWindow(bool? result = true)
        //{
        //    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        //    {
        //        CloseWindowFlag = CloseWindowFlag == null
        //            ? true
        //            : !CloseWindowFlag;
        //    }));
        //}
        //
        #endregion


        #region INotifyPropertyChanged

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
