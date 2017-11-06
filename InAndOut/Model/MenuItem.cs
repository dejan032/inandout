using System;
using GalaSoft.MvvmLight.CommandWpf;

namespace InAndOut.Model
{
    public class MenuItem
    {
        public string Name { get; set; }
        public RelayCommand CallBackAction { get; set; }
    }
}
