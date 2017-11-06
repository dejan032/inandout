using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using InAndOut.ViewModel;

namespace InAndOut
{
    /// <inheritdoc />
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool _firstRun = true;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (!_firstRun) return;
            Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),"clock",MainViewModel.ContentPropertyName));
            _firstRun = false;
        }
    }
}
