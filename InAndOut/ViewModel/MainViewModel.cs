using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using InAndOut.Model;
using System;
using System.Windows;
using System.Windows.Threading;

namespace InAndOut.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        PunchClock punchClock;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            punchClock = PunchClock.Instance;
            PunchClockButtonCommand = new RelayCommand(new Action(ExecutePunchClockButtonCommand));
            BreakClockButtonCommand = new RelayCommand(new Action(ExecuteBreakClockButtonCommand));
        }

        private void ExecuteBreakClockButtonCommand()
        {
            if (BreakSecondsCount == 0)
                BreakTimePassedText = DEFAULT_ZERO_TIME_TEXT;

            BreakTimerVisibility = Visibility.Visible;
            punchClock.ToggleBreak();
            
            BreakButtonText = punchClock.State == PunchClock.PunchClockStates.OnBreak ? "Stop Break" : "Break";
            ToggleBreakTimer();
        }

        private void ToggleBreakTimer()
        {
            if (WorkTimer != null)
            {
                if (WorkTimer.IsEnabled)
                    WorkTimer.Stop();
                else
                    WorkTimer.Start();
            }

            if (BreakTimer == null)
            {
                StartBreakTimer();            }
            else
            {
                StopBreakTimer();
            }

        }

        private void StopBreakTimer()
        {
            if (BreakTimer == null) return;
            BreakTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            BreakTimer.Tick += BreakTimer_Tick;
            BreakTimer.Start();
        }

        private void StartBreakTimer()
        {
            if (BreakTimer != null) return;
            BreakTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            BreakTimer.Tick += BreakTimer_Tick;
            BreakTimer.Start();
        }

        private void BreakTimer_Tick(object sender, EventArgs e)
        {
            BreakSecondsCount++;
            BreakTimePassedText = TimeSpan.FromSeconds(BreakSecondsCount).ToString();
        }

        private void ExecutePunchClockButtonCommand()
        {
            if (WorkingSecondsCount == 0)
            {
                WorkTimePassedText = DEFAULT_ZERO_TIME_TEXT;
                punchClock.State = PunchClock.PunchClockStates.PunchedOut;
            }

            punchClock.Toggle();
            Boolean punchedIn = punchClock.State == PunchClock.PunchClockStates.PunchedIn;
            PunchClockButtonText = punchedIn ? "Punch Out" : "Punch In";
            BreakButtonIsEnabled = punchedIn;
            SetBreakEnabled(punchedIn);
            ToggleWorkTimer();
        }

        private void SetBreakEnabled(Boolean value)
        {
            BreakButtonText = value ? "Start Break" : "Stop break";
            BreakButtonIsEnabled = value;
            if (value)
                StartBreakTimer();
            else
                StopBreakTimer();
        }

        private void ToggleWorkTimer()
        {
            if (WorkTimer == null)
            {
                WorkTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                WorkTimer.Tick += WorkTimer_Tick;
                WorkTimer.Start();
            }
            else
            {
                WorkTimer.Tick -= WorkTimer_Tick;
                WorkTimer.Stop();
                WorkTimer = null;
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            WorkingSecondsCount++;
            WorkTimePassedText = TimeSpan.FromSeconds(WorkingSecondsCount).ToString();
            
        }

        private int WorkingSecondsCount = 0;
        private int BreakSecondsCount = 0;
        private const string DEFAULT_ZERO_TIME_TEXT = "00:00:00";
        
        private DispatcherTimer WorkTimer;
        private DispatcherTimer BreakTimer;

        /// <summary>
        /// The <see cref="WorkTimePassedText" /> property's name.
        /// </summary>
        public const string WorkTimePassedTextPropertyName = "WorkTimePassedText";

        private String _workTimePassedText = DEFAULT_ZERO_TIME_TEXT;

        /// <summary>
        /// Gets the WorkTimePassedText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public String WorkTimePassedText
        {
            get
            {
                return _workTimePassedText;
            }

            set
            {
                if (_workTimePassedText == value)
                {
                    return;
                }

                _workTimePassedText = value;
                RaisePropertyChanged(WorkTimePassedTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="WorkTimerVisibility" /> property's name.
        /// </summary>
        public const string WorkTimerVisibilityPropertyName = "WorkTimerVisibility";

        private Visibility _workTimerVisibility = Visibility.Visible;

        /// <summary>
        /// Gets the WorkTimerVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility WorkTimerVisibility
        {
            get
            {
                return _workTimerVisibility;
            }

            set
            {
                if (_workTimerVisibility == value)
                {
                    return;
                }

                _workTimerVisibility = value;
                RaisePropertyChanged(WorkTimerVisibilityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BreakTimePassedText" /> property's name.
        /// </summary>
        public const string BreakTimePassedTextPropertyName = "BreakTimePassedText";

        private String _breakTimePassedText = DEFAULT_ZERO_TIME_TEXT;

        /// <summary>
        /// Gets the BreakTimePassedText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public String BreakTimePassedText
        {
            get
            {
                return _breakTimePassedText;
            }

            set
            {
                if (_breakTimePassedText == value)
                {
                    return;
                }

                _breakTimePassedText = value;
                RaisePropertyChanged(BreakTimePassedTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BreakTimerVisibility" /> property's name.
        /// </summary>
        public const string BreakTimerVisibilityPropertyName = "BreakTimerVisibility";

        private Visibility _breakTimerVisibility = Visibility.Collapsed;

        /// <summary>
        /// Gets the BreakTimerVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility BreakTimerVisibility
        {
            get
            {
                return _breakTimerVisibility;
            }

            set
            {
                if (_breakTimerVisibility == value)
                {
                    return;
                }

                _breakTimerVisibility = value;
                RaisePropertyChanged(BreakTimerVisibilityPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="PunchClockButtonText" /> property's name.
        /// </summary>
        public const string PunchClockButtonTextPropertyName = "PunchClockButtonText";

        private String _punchClockButtonText = "Punch In";

        /// <summary>
        /// Gets the PunchClockButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public String PunchClockButtonText
        {
            get
            {
                return _punchClockButtonText;
            }

            set
            {
                if (_punchClockButtonText == value)
                {
                    return;
                }

                _punchClockButtonText = value;
                RaisePropertyChanged(PunchClockButtonTextPropertyName);
            }
        }
        
        /// <summary>
        /// The <see cref="PunchClockButtonCommand" /> property's name.
        /// </summary>
        public const string PunchClockButtonCommandPropertyName = "PunchClockButtonCommand";

        private RelayCommand _punchClockButtonCommand;

        /// <summary>
        /// Gets the PunchClockButtonCommand property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public RelayCommand PunchClockButtonCommand
        {
            get
            {
                return _punchClockButtonCommand;
            }

            set
            {
                if (_punchClockButtonCommand == value)
                {
                    return;
                }

                _punchClockButtonCommand = value;
                RaisePropertyChanged(PunchClockButtonCommandPropertyName);
            }
        }

        

        /// <summary>
        /// The <see cref="BreakButtonIsEnabled" /> property's name.
        /// </summary>
        public const string BreakButtonIsEnabledPropertyName = "BreakButtonIsEnabled";

        private bool _breakButtonIsEnabled = false;

        /// <summary>
        /// Gets the BreakButtonIsEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool BreakButtonIsEnabled
        {
            get
            {
                return _breakButtonIsEnabled;
            }

            set
            {
                if (_breakButtonIsEnabled == value)
                {
                    return;
                }

                _breakButtonIsEnabled = value;
                RaisePropertyChanged(BreakButtonIsEnabledPropertyName);
            }
        }


        /// <summary>
        /// The <see cref="BreakButtonText" /> property's name.
        /// </summary>
        public const string BreakButtonTextPropertyName = "BreakButtonText";

        private String _breakButtonText = "Start Break";

        /// <summary>
        /// Gets the BreakButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public String BreakButtonText
        {
            get
            {
                return _breakButtonText;
            }

            set
            {
                if (_breakButtonText == value)
                {
                    return;
                }

                _breakButtonText = value;
                RaisePropertyChanged(BreakButtonTextPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="BreakClockButtonCommand" /> property's name.
        /// </summary>
        public const string BreakClockButtonCommandPropertyName = "BreakClockButtonCommand";

        private RelayCommand _breakClockButtonCommand;

        /// <summary>
        /// Gets the BreakClockButtonCommand property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public RelayCommand BreakClockButtonCommand
        {
            get
            {
                return _breakClockButtonCommand;
            }

            set
            {
                if (_breakClockButtonCommand == value)
                {
                    return;
                }

                _breakClockButtonCommand = value;
                RaisePropertyChanged(BreakClockButtonCommandPropertyName);
            }
        }
        
        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}



    }

   
}