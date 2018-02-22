using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using InAndOut.Helpers;
using InAndOut.Model;
using InAndOut.ViewModel.MVVMMSGS;
using LiteDB;

namespace InAndOut.ViewModel
{
    public class DetailsDailyViewModel : ViewModelBase
    {
        private List<Model.MenuItem> _menuItems;
        private DateTime _selectedDate;
        private Dictionary<DateTime, List<TimeEntry>> Rawdata { get; set; }
        private List<TimeEntry> _displayData;
        private const string ClassName = nameof(DetailsDailyViewModel);

        private void InitMenuItems()
        {
            var menu = new List<Model.MenuItem> {
                new Model.MenuItem() {Name = "Monthly", CallBackAction = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),ViewNames.DetailsMonthly,MainViewModel.ContentPropertyName));
                        Messenger.Default.Send(new UpdateMenuItems(nameof(DetailsMonthlyViewModel)));
                    })},
                new Model.MenuItem() {Name = "Weekly", CallBackAction = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),ViewNames.DetailsWeekly,MainViewModel.ContentPropertyName));
                        Messenger.Default.Send(new UpdateMenuItems(nameof(DetailsWeeklyViewModel)));
                    })},
                new Model.MenuItem() {Name = "Daily", CallBackAction = new RelayCommand(
                () =>
                {
                    Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),ViewNames.DetailsDaily,MainViewModel.ContentPropertyName));
                    Messenger.Default.Send(new UpdateMenuItems(nameof(DetailsDailyViewModel)));
                })},
                new Model.MenuItem() {Name = "Back", CallBackAction = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send(new PropertyChangedMessage<string>(default(string),ViewNames.PunchClock,MainViewModel.ContentPropertyName));
                        Messenger.Default.Send(new UpdateMenuItems(nameof(PunchClockViewModel)));
                    })}
            };
            _menuItems = menu;
        }

        public DetailsDailyViewModel()
        {
            GetRawData();
            GenerateReport();
            InitMenuItems();
            Messenger.Default.Send(
                new PropertyChangedMessage<List<Model.MenuItem>>(default(List<MenuItem>), _menuItems, MainViewModel.MenuItemsPropertyName));
            Messenger.Default.Register<UpdateMenuItems>(this, msg =>
            {
                if (ClassName.Equals(msg.ClassName))
                {
                    Messenger.Default.Send(new PropertyChangedMessage<List<Model.MenuItem>>(default(List<MenuItem>), _menuItems, MainViewModel.MenuItemsPropertyName));
                }
            });
        }

        private void GenerateReport()
        {
            Rawdata = new Dictionary<DateTime, List<TimeEntry>>();
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var timeEntries = db.GetCollection<TimeEntry>("time-entries").FindAll().ToList();

                // Insert document to collection - if collection do not exits, create now
                Dates = timeEntries.Select(p => p.TimeStamp.Date).Distinct().ToList();

                foreach (var date in Dates)
                {
                    var displaylist = new List<TimeEntry>();
                    var entries = timeEntries
                        .Where(r => r.TimeStamp.Date.Equals(date.Date))
                        .OrderBy(t=>t.InTime)
                        .ToList();
                    
                    entries.ForEach(t =>
                    {
                        displaylist.Add(new TimeEntry(t.UserId,t.InTime,default(DateTime),t.InTime,t.Type));
                        displaylist.Add(new TimeEntry(t.UserId,default(DateTime),t.OutTime,t.OutTime,t.Duration,t.Type));
                    });
                    
                    displaylist = displaylist.OrderBy(t => t.TimeStamp).ToList();
                   
                    Rawdata.Add(date, displaylist);
                }

                SelectedDate = Dates.ElementAt(0);
            }
        }

        private void GetRawData()
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var col = db.GetCollection<TimeEntry>("time-entries");
                // Insert document to collection - if collection do not exits, create now
                DisplayData = col.FindAll().ToList();
            }
        }

        public List<TimeEntry> DisplayData { get => _displayData; set { _displayData = value; UpdateTotalHours(); RaisePropertyChanged(nameof(DisplayData)); } }

        private void UpdateTotalHours()
        {
            TimeSpan workTime = new TimeSpan();
            TimeSpan breakTime = new TimeSpan();
            DisplayData.ForEach(t =>
            {
                if (t.Duration.TotalMilliseconds > 0)
                {
                    if (t.Type == TimeEntryType.Work)
                        workTime = workTime.Add(t.Duration.RoundToSeconds());
                    else if (t.Type == TimeEntryType.Break)
                        breakTime = breakTime.Add(t.Duration.RoundToSeconds());
                }
            });

            TotalWorkTime = workTime;
            TotalBreakTime = breakTime;
        }

        public List<DateTime> Dates { get; set; }
        public DateTime SelectedDate { get => _selectedDate; set { _selectedDate = value; SelectedDateChanged();} }

        public TimeSpan TotalWorkTime { get; private set; }
        public TimeSpan TotalBreakTime { get; private set; }

        private void SelectedDateChanged()
        {
            Rawdata.TryGetValue(SelectedDate, out List<TimeEntry> entries);
            DisplayData = entries;
        }
    }
}
