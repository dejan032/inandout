using System.Collections.Generic;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using InAndOut.View;
using System;
using MenuItem = InAndOut.Model.MenuItem;

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
        public static string MenuItemsPropertyName = "MenuItems";
        public static string ContentPropertyName = "Content";

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            Messenger.Default.Register<PropertyChangedMessage<string>>(this, ContentChanged);
            Messenger.Default.Register<PropertyChangedMessage<List<Model.MenuItem>>>(this, MenuItemsChanged);
        }

        private void MenuItemsChanged(PropertyChangedMessage<List<Model.MenuItem>> propertyDetails)
        {
            if (!propertyDetails.PropertyName.Equals(MenuItemsPropertyName)) return;
            MenuItems = propertyDetails.NewValue;
            RaisePropertyChanged(nameof(MenuItems));
        }

        private void ContentChanged(PropertyChangedMessage<string> propertyDetails)
        {
            if (propertyDetails.PropertyName.Equals(ContentPropertyName))
            {
                ChangeView(propertyDetails.NewValue);
            }
        }

        public UserControl Content { get; set; }

        public List<Model.MenuItem> MenuItems { get; set; }
        

        private void ChangeView(string name = ViewNames.PunchClock)
        {
            if (name.Equals(ViewNames.PunchClock))
            {
                Content = new PunchClockView();
            }
            else if (name.Equals(ViewNames.DetailsDaily))
            {
                Content = new DetailsDailyView();
            }
            else if (name.Equals(ViewNames.DetailsWeekly))
            {
                Content = new DetailsWeeklyView();
            }
            else if (name.Equals(ViewNames.DetailsMonthly))
            {
                Content = new DetailsMonthlyView();
            }

            RaisePropertyChanged(nameof(Content));
        }
    }
}