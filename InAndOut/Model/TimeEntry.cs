using System;
using System.Runtime.Serialization;
using LiteDB;

namespace InAndOut.Model
{
    public class TimeEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
    }
}
