using GalaSoft.MvvmLight;
using System.Windows.Controls;
using InAndOut.View;
using InAndOut.View.Menu;

namespace InAndOut.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            //Menu = new MainMenu();
            Content = new PunchClockView();
            //Content = new LoginView();
        }

        public UserControl Menu { get; set; }

        public UserControl Content { get; set; }
    }
}