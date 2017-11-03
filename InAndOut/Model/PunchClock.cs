using System;
using System.Diagnostics;
using System.Windows.Media;
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
                var col = db.GetCollection<TimeEntry>("work-entries");
                // Insert document to collection - if collection do not exits, create now
                col.Insert(new TimeEntry() { InTime = _workStartTime,OutTime = _workEndTime,TimeStamp = DateTime.Now});
            }
        }

        private void AddBreakTimeEntry()
        {
            // Open database (or create if doesn't exist)
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get collection instance
                var col = db.GetCollection<TimeEntry>("break-entries");
                // Insert document to collection - if collection do not exits, create now
                col.Insert(new TimeEntry() { InTime = _breakStartTime, OutTime = _breakEndTime, TimeStamp = DateTime.Now });
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
    }


}
