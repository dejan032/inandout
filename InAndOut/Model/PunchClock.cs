using System;

namespace InAndOut.Model
{
    public class PunchClock
    {
        private PunchClock() { }
        static PunchClock() { }

        public static PunchClock Instance { get; } = new PunchClock();

        public void PunchIn() {
        }
        public void PunchOut() {
        }
        public void BreakStart() {
        }
        public void BreakEnd() {
        }

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
            switch (State)
            {
                case PunchClockStates.PunchedIn:
                case PunchClockStates.OnBreak:
                    State = PunchClockStates.PunchedOut;
                    break;
                case PunchClockStates.PunchedOut:
                    State = PunchClockStates.PunchedIn;
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
                    State = PunchClockStates.OnBreak;
                    break;
                case PunchClockStates.OnBreak:
                    State = PunchClockStates.PunchedIn;
                    break;
                case PunchClockStates.PunchedOut:
                case PunchClockStates.Unknown:
                default:
                    break;
            }
        }
    }


}
