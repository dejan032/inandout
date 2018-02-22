using System;
using LiteDB;

namespace InAndOut.Model
{
    public class PunchClock
    {
        private PunchClock() { }
        static PunchClock() { }

        public static PunchClock Instance { get; } = new PunchClock();

        private DateTime _workStartTime;
        private DateTime _workEndTime;

        private DateTime _breakStartTime;
        private DateTime _breakEndTime;


        public void PunchIn() {
            State = PunchClockStates.PunchedIn;
            _workStartTime = DateTime.Now;
        }

        public void PunchOut() {
            State = PunchClockStates.PunchedOut;
            _workEndTime = DateTime.Now;
            AddWorkTimeEntry();
            ClearCurrentWorkTime();
        }

        public void BreakStart() {
            State = PunchClockStates.OnBreak;
            _breakStartTime = DateTime.Now;
        }

        public void BreakEnd() {
            State = PunchClockStates.PunchedIn;
            _breakEndTime = DateTime.Now;
            AddBreakTimeEntry();
            ClearCurrentBreakTime();
        }

        private void ClearCurrentBreakTime()
        {
            _breakStartTime = new DateTime();
            _breakEndTime = new DateTime();
        }

        private void ClearCurrentWorkTime()
        {
            _workStartTime = new DateTime();
            _workEndTime = new DateTime();
        }

        private void AddWorkTimeEntry()
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var col = db.GetCollection<TimeEntry>("time-entries");
                // Insert document to collection - if collection do not exits, create now
                var entry = new TimeEntry(0,_workStartTime, _workEndTime, DateTime.Now, TimeEntryType.Work);
                col.Insert(entry);
            }
        }

        private void AddBreakTimeEntry()
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var col = db.GetCollection<TimeEntry>("time-entries");
                // Insert document to collection - if collection do not exits, create now
                var entry = new TimeEntry(0,_breakStartTime, _breakEndTime, DateTime.Now, TimeEntryType.Break);
                col.Insert(entry);
            }
        }

        public PunchClockStates State { get; set; }

        public enum PunchClockStates
        {
            Unknown,
            PunchedIn,
            PunchedOut,
            OnBreak
        }

        public void Toggle()
        {
            switch (State)
            {
                case PunchClockStates.PunchedIn:
                    PunchOut();
                    break;
                case PunchClockStates.OnBreak:
                    BreakEnd();
                    PunchOut();
                    break;
                case PunchClockStates.PunchedOut:
                    PunchIn();
                    break;
                case PunchClockStates.Unknown:
                default:
                    break;
            }
        }

        internal void ToggleBreak()
        {
            switch (State)
            {
                case PunchClockStates.PunchedIn:
                    BreakStart();
                    break;
                case PunchClockStates.OnBreak:
                    BreakEnd();
                    break;
                case PunchClockStates.PunchedOut:
                case PunchClockStates.Unknown:
                default:
                    break;
            }
        }

        public void TestFillDataSet(int numberOfEntries)
        {
            for (var i = 0; i < numberOfEntries; i++)
            {
                var randomDay = DateTime.UtcNow
                    .Subtract(TimeSpan.FromDays(new Random().Next(2 * 365)))
                    .Date
                    .AddHours(new Random().Next(6, 9))
                    .AddMinutes(new Random().Next(0, 45))
                    .AddSeconds(new Random().Next(0, 45));
                var randomStartTime = randomDay.AddSeconds(new Random().Next(0, 5));
                var randomBreakStartTime = randomStartTime.AddHours(new Random().Next(2, 4));
                var randomBreakEndTime = randomBreakStartTime.AddMinutes(new Random().Next(0, 45));
                var randomEndTime = randomBreakEndTime.AddHours(new Random().Next(4, 6));

                // Open database (or create if doesn't exist)
                using (var db = new LiteDatabase(@"MyData.db"))
                {
                    // Get collection instance
                    var col = db.GetCollection<TimeEntry>("time-entries");
                    var breakentry = new TimeEntry(0, randomBreakStartTime, randomBreakEndTime, randomDay, TimeEntryType.Break);
                    col.Insert(breakentry);
                    // Insert punchin time
                    var workentry = new TimeEntry(0, randomStartTime, randomEndTime, randomDay, TimeEntryType.Work);
                    col.Insert(workentry);
                }
            }
        }
    }
}
