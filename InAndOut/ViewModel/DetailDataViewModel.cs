using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using InAndOut.Model;
using LiteDB;

namespace InAndOut.ViewModel
{
    public class DetailDataViewModel : ViewModelBase
    {
        private List<Model.MenuItem> _menuItems;

        private void InitMenuItems()
        {
            var menu = new List<Model.MenuItem> {new Model.MenuItem() {Name = "Back", CallBackAction = new RelayCommand(
                () =>
                {
                    Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),"clock",MainViewModel.ContentPropertyName));
                })}};
            _menuItems = menu;
        }

        public DetailDataViewModel()
        {
            GetRawData();
            InitMenuItems();
            Messenger.Default.Send(
                new PropertyChangedMessage<List<Model.MenuItem>>(default(List<MenuItem>), _menuItems, MainViewModel.MenuItemsPropertyName));
        }

        private void GetRawData()
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var col = db.GetCollection<TimeEntry>("work-entries");
                // Insert document to collection - if collection do not exits, create now
                RawData = col.FindAll().ToList();
                RaisePropertyChanged(nameof(RawData));
            }
        }

        public List<TimeEntry> RawData { get; set; }
    }
}
