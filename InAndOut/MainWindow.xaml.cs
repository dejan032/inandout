using System.Windows.Input;
using InAndOut.Model;
using InAndOut.ViewModel;
using LiteDB;
using MahApps.Metro.Controls;

namespace InAndOut
{
    /// <inheritdoc cref="MetroWindow" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void ContentHolder_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sideBar.IsOpen)
            {
                sideBar.IsOpen = false;
            }

            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<TimeEntry>("break-entries");
                var timeEntries = col.FindAll();

                var col2 = db.GetCollection<TimeEntry>("work-entries");
                var findAll = col2.FindAll();
            }
        }
    }
}