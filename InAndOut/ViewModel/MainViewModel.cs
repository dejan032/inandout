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
            OldMenuItems = propertyDetails.NewValue;
            RaisePropertyChanged(nameof(OldMenuItems));
        }

        private void ContentChanged(PropertyChangedMessage<string> propertyDetails)
        {
            if (propertyDetails.PropertyName.Equals(ContentPropertyName))
            {
                ChangeView(propertyDetails.NewValue);
            }
        }

        public UserControl Content { get; set; }

        public List<Model.MenuItem> OldMenuItems { get; set; }
        

        private void ChangeView(string name = "clock")
        {
            if (name.Equals("clock"))
            {
                Content = new PunchClockView();
                RaisePropertyChanged(nameof(Content));
            }
            else if (name.Equals("details"))
            {
                Content = new DetailDataView();
                RaisePropertyChanged(nameof(Content));
            }
        }
    }
}