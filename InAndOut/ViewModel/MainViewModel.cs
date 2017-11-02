using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using InAndOut.Model;
using System;
using System.Windows;
using System.Windows.Threading;

namespace InAndOut.ViewModel
{
    /// <inheritdoc />
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly PunchClock _punchClock;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _punchClock = PunchClock.Instance;
        }

        private void ExecuteBreakClockButtonCommand()
        {
            if (_breakSecondsCount == 0)
                BreakTimerPassedText = DefaultZeroTimeText;

            BreakTimerVisibility = Visibility.Visible;
            _punchClock.ToggleBreak();

            UpdateBreakButtonText(_punchClock.State);
            ToggleBreakTimer();
        }

        private void UpdateBreakButtonText(PunchClock.PunchClockStates state)
        {
            switch (state)
            {
                case PunchClock.PunchClockStates.OnBreak:
                    BreakButtonText = "Stop Break";
                    break;
                case PunchClock.PunchClockStates.PunchedIn:
                case PunchClock.PunchClockStates.PunchedOut:
                case PunchClock.PunchClockStates.Unknown:
                default:
                    BreakButtonText = "Start Break";
                    break;
            }
        }

        private void ToggleBreakTimer()
        {
            if (_workTimer != null)
            {
                if (_workTimer.IsEnabled)
                    _workTimer.Stop();
                else
                    _workTimer.Start();
            }

            if (_breakTimer == null)
            {
                StartBreakTimer();
            }
            else
            {
                StopBreakTimer();
            }
        }

        private void StopBreakTimer()
        {
            if (_breakTimer == null) return;

            _breakTimer.Stop();
            _breakTimer.Tick -= BreakTimer_Tick;
            _breakTimer = null;
        }

        private void StartBreakTimer()
        {
            if (_breakTimer != null) return;
            _breakTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _breakTimer.Tick += BreakTimer_Tick;
            _breakTimer.Start();
        }

        private void BreakTimer_Tick(object sender, EventArgs e)
        {
            _breakSecondsCount++;
            BreakTimerPassedText = TimeSpan.FromSeconds(_breakSecondsCount).ToString();
        }

        private void ExecutePunchClockButtonCommand()
        {
            if (_workingSecondsCount == 0)
            {
                WorkTimerPassedText = DefaultZeroTimeText;
                _punchClock.State = PunchClock.PunchClockStates.PunchedOut;
            }

            _punchClock.Toggle();
            var punchedIn = _punchClock.State == PunchClock.PunchClockStates.PunchedIn;
            PunchClockButtonText = punchedIn ? "Punch Out" : "Punch In";
            SetBreakEnabled(punchedIn);
            ToggleWorkTimer();
        }

        private void SetBreakEnabled(bool value)
        {
            UpdateBreakButtonText(_punchClock.State);
            BreakButtonIsEnabled = value;
        }

        private void ToggleWorkTimer()
        {
            if (_workTimer == null)
            {
                _workTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _workTimer.Tick += WorkTimer_Tick;
                _workTimer.Start();
            }
            else
            {
                _workTimer.Tick -= WorkTimer_Tick;
                _workTimer.Stop();
                _workTimer = null;
                StopBreakTimer();
            }
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            _workingSecondsCount++;
            WorkTimerPassedText = TimeSpan.FromSeconds(_workingSecondsCount).ToString();
        }

        private int _workingSecondsCount;
        private int _breakSecondsCount;
        private const string DefaultZeroTimeText = "00:00:00";
        
        private DispatcherTimer _workTimer;
        private DispatcherTimer _breakTimer;

        private string _workTimerPassedText = DefaultZeroTimeText;

        /// <summary>
        /// Gets the WorkTimePassedText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WorkTimerPassedText
        {
            get => _workTimerPassedText;

            set
            {
                if (_workTimerPassedText == value)
                {
                    return;
                }

                _workTimerPassedText = value;
                RaisePropertyChanged(nameof(WorkTimerPassedText));
            }
        }

        private Visibility _workTimerVisibility = Visibility.Visible;

        /// <summary>
        /// Gets the WorkTimerVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility WorkTimerVisibility
        {
            get => _workTimerVisibility;

            set
            {
                if (_workTimerVisibility == value)
                {
                    return;
                }

                _workTimerVisibility = value;
                RaisePropertyChanged(nameof(WorkTimerVisibility));
            }
        }

        private string _breakTimerPassedText = DefaultZeroTimeText;

        /// <summary>
        /// Gets the BreakTimerPassedText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BreakTimerPassedText
        {
            get => _breakTimerPassedText;

            set
            {
                if (_breakTimerPassedText == value)
                {
                    return;
                }

                _breakTimerPassedText = value;
                RaisePropertyChanged(nameof(BreakTimerPassedText));
            }
        }

        private Visibility _breakTimerVisibility = Visibility.Collapsed;

        /// <summary>
        /// Gets the BreakTimerVisibility property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public Visibility BreakTimerVisibility
        {
            get => _breakTimerVisibility;

            set
            {
                if (_breakTimerVisibility == value)
                {
                    return;
                }

                _breakTimerVisibility = value;
                RaisePropertyChanged(nameof(BreakTimerVisibility));
            }
        }

        private string _punchClockButtonText = "Punch In";

        /// <summary>
        /// Gets the PunchClockButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string PunchClockButtonText
        {
            get => _punchClockButtonText;

            set
            {
                if (_punchClockButtonText == value)
                {
                    return;
                }

                _punchClockButtonText = value;
                RaisePropertyChanged(nameof(PunchClockButtonText));
            }
        }

        public RelayCommand PunchClockButtonCommand => new RelayCommand(ExecutePunchClockButtonCommand);


        private bool _breakButtonIsEnabled;

        /// <summary>
        /// Gets the BreakButtonIsEnabled property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public bool BreakButtonIsEnabled
        {
            get => _breakButtonIsEnabled;

            set
            {
                if (_breakButtonIsEnabled == value)
                {
                    return;
                }

                _breakButtonIsEnabled = value;
                RaisePropertyChanged(nameof(BreakButtonIsEnabled));
            }
        }

        private string _breakButtonText = "Start Break";

        /// <summary>
        /// Gets the BreakButtonText property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string BreakButtonText
        {
            get => _breakButtonText;

            set
            {
                if (_breakButtonText == value)
                {
                    return;
                }

                _breakButtonText = value;
                RaisePropertyChanged(nameof(BreakButtonText));
            }
        }

        public RelayCommand BreakClockButtonCommand => new RelayCommand(ExecuteBreakClockButtonCommand);
    }
}