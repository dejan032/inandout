/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:InAndOut.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace InAndOut.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<PunchClockViewModel>();
            SimpleIoc.Default.Register<DetailsDailyViewModel>();
            SimpleIoc.Default.Register<DetailsWeeklyViewModel>();
            SimpleIoc.Default.Register<DetailsMonthlyViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public PunchClockViewModel PunchClock => ServiceLocator.Current.GetInstance<PunchClockViewModel>();
        public DetailsDailyViewModel DailyDetailsData => ServiceLocator.Current.GetInstance<DetailsDailyViewModel>();
        public DetailsWeeklyViewModel WeeklyDetailsData => ServiceLocator.Current.GetInstance<DetailsWeeklyViewModel>();
        public DetailsMonthlyViewModel MothlyDetailsData => ServiceLocator.Current.GetInstance<DetailsMonthlyViewModel>();

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}