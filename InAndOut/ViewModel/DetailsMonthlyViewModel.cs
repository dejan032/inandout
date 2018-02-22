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
    public class DetailsMonthlyViewModel : ViewModelBase
    {
        private List<Model.MenuItem> _menuItems;
        private DateTime _selectedMonth;
        private Dictionary<DateTime, List<DailyEntry>> Rawdata { get; set; }
        private List<DailyEntry> _displayData;
        private const string ClassName = nameof(DetailsMonthlyViewModel);

        public List<DateTime> Months { get; set; }
        public DateTime SelectedMonth { get => _selectedMonth; set { _selectedMonth = value; SelectedMonthChanged(); } }

        public TimeSpan TotalWorkTime { get; private set; }
        public TimeSpan TotalBreakTime { get; private set; }

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

        public DetailsMonthlyViewModel()
        {
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
            Rawdata = new Dictionary<DateTime, List<DailyEntry>>();
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var timeEntries = db.GetCollection<TimeEntry>("time-entries").FindAll().ToList();
                Months = timeEntries.OrderByDescending(x => x.TimeStamp.Date)
                    .GroupBy(x => new { x.TimeStamp.Year, x.TimeStamp.Month })
                    .Select(p => new DateTime(p.Key.Year, p.Key.Month, 1))
                    .ToList();


                foreach (var date in Months)
                {
                    //TODO make nice linq statemant this one doesnt work
                    //var monthData = timeEntries.Where(t =>
                    //        t.TimeStamp.Year == date.Year && t.TimeStamp.Month == date.Month)
                    //    .GroupBy(t => t.TimeStamp.Date)
                    //    .Select(t =>

                    //    new DailyEntry()
                    //    {
                    //        Date = t.Key.Date,
                    //        StartTime = t.Where(r=>r.Type == TimeEntryType.Work).Min(r=>r.InTime),
                    //        EndTime = t.Where(r => r.Type == TimeEntryType.Work).Max(r => r.OutTime),
                    //        BreakDuration = TimeSpan.FromSeconds(t.Where(r => r.Type == TimeEntryType.Break).Distinct().Sum(r => r.Duration.TotalSeconds)),
                    //        WorkDuration = TimeSpan.FromSeconds(t.Where(r => r.Type == TimeEntryType.Work).Distinct().Sum(r => r.Duration.TotalSeconds))
                    //    }

                    //);

                    var dailyEntries = new List<DailyEntry>();
                    var monthData = timeEntries.Where(x =>
                            x.TimeStamp.Year == date.Year && x.TimeStamp.Month == date.Month)
                        .GroupBy(x => x.TimeStamp.Date).ToList();
                    
                    monthData.ForEach(item =>
                    {
                        var workentries = item.Where(x => x.Type == TimeEntryType.Work).Distinct().ToList();
                        var breakentries = item.Where(x => x.Type == TimeEntryType.Break).Distinct().ToList();
                        var start = workentries.Min(x => x.InTime);
                        var end = workentries.Max(x => x.OutTime);
                        var breakspan = TimeSpan.FromSeconds(breakentries.Distinct().Sum(x => x.Duration.TotalSeconds));
                        var workspan = TimeSpan.FromSeconds(workentries.Distinct().Sum(x => x.Duration.TotalSeconds)).Subtract(breakspan);
                        var dailyEntry = new DailyEntry
                        {
                            Date = item.Key.Date,
                            StartTime = start,
                            EndTime = end,
                            WorkDuration = workspan,
                            BreakDuration = breakspan
                        };
                        dailyEntries.Add(dailyEntry);
                    });

                    Rawdata.Add(date, dailyEntries);
                }

                SelectedMonth = Months.ElementAt(0);
            }
        }

        public class DailyEntry
        {
            public DateTime Date { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public TimeSpan BreakDuration { get; set; }
            public TimeSpan WorkDuration { get; set; }
        }

        public List<DailyEntry> DisplayData { get => _displayData; set { _displayData = value; UpdateTotalHours(); RaisePropertyChanged(nameof(DisplayData)); } }

        private void UpdateTotalHours()
        {
            TimeSpan workTime = new TimeSpan();
            TimeSpan breakTime = new TimeSpan();
            DisplayData.ForEach(t =>
            {
                workTime = workTime.Add(t.WorkDuration.RoundToSeconds());
                breakTime = breakTime.Add(t.BreakDuration.RoundToSeconds());
            });

            TotalWorkTime = workTime;
            TotalBreakTime = breakTime;
        }

        private void SelectedMonthChanged()
        {
            Rawdata.TryGetValue(SelectedMonth, out var entries);
            DisplayData = entries;
        }
    }
}
