using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InAndOut.Model
{

    public class PunchClock
    {
        private PunchClock() { }
        static PunchClock() { }

        private static PunchClock _instance = new PunchClock();
        public static PunchClock Instance { get { return _instance; } }

        public void PunchIn() { return; }
        public void PunchOut() { return; }
        public void BreakStart() { return; }
        public void BreakEnd() { return; }

        public PunchClockStates State { get; set; }

        public enum PunchClockStates
        {
            PunchedIn,
            PunchedOut,
            OnBreak,
            Unknown
        }

        public void Toggle()
        {
            if (State == PunchClockStates.PunchedIn)
                State = PunchClockStates.PunchedOut;
            else if (State == PunchClockStates.PunchedOut)
                State = PunchClockStates.PunchedIn;
        }

        internal void ToggleBreak()
        {
            if (State == PunchClockStates.PunchedIn)
                State = PunchClockStates.OnBreak;
            else if (State == PunchClockStates.OnBreak)
                State = PunchClockStates.PunchedIn;
        }
    }
   
  
}
