using System;
using System.Collections;
using System.Runtime.Serialization;
using InAndOut.Helpers;
using LiteDB;

namespace InAndOut.Model
{
    public class TimeEntry
    {
        private TimeEntryType _type;
        private TimeSpan _duration;
        private DateTime _outTime;
        private DateTime _inTime;
        private DateTime _timeStamp;
        private int _userId;

        public TimeEntry()
        {
            _inTime = default(DateTime);
            _outTime = default(DateTime);
            _timeStamp = default(DateTime);
            _duration = TimeSpan.FromMilliseconds(0);
            _type = TimeEntryType.Unknown;
        }

        public TimeEntry(int userId, DateTime inTime, DateTime outTime, DateTime timestamp, TimeEntryType type)
        {
            _userId = userId;
            _inTime = inTime;
            _outTime = outTime;
            _timeStamp = timestamp;
            _duration = outTime.Equals(default(DateTime)) || inTime.Equals(default(DateTime)) ? TimeSpan.FromMilliseconds(0) : outTime.Subtract(inTime).RoundToSeconds();
            _type = type;
        }

        public TimeEntry(int userId, DateTime inTime, DateTime outTime, DateTime timestamp, TimeSpan duration, TimeEntryType type)
        {
            _userId = userId;
            _inTime = inTime;
            _outTime = outTime;
            _timeStamp = timestamp;
            _duration = duration.RoundToSeconds();
            _type = type;
        }
        

        [BsonId]
        public ObjectId Id { get; set; }
        public int UserId { get => _userId; set => _userId = value; }
        public DateTime TimeStamp { get => _timeStamp; set => _timeStamp = value; }
        public DateTime InTime { get => _inTime; set => _inTime = value; }
        public DateTime OutTime { get => _outTime; set => _outTime = value; }
        public TimeSpan Duration { get => _duration; set => _duration = value; }
        public TimeEntryType Type { get => _type; set => _type = value; }
    }
}
