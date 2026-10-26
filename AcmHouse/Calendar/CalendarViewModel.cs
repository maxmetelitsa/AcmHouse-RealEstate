using AcmHouse.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace AcmHouse.Profile
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public SQLiteManager manager = new SQLiteManager();

        private ObservableCollection<CalendarEvent> _events;
        public ObservableCollection<CalendarEvent> Events
        {
            get { return _events; }
            set { _events = value; OnPropertyChanged("Events"); }
        }

        public CalendarViewModel()
        {
            Events = new ObservableCollection<CalendarEvent>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void AddEvent(CalendarEvent calendarEvent)
        {
            Events.Add(calendarEvent);
        }

        public void RemoveEvent(CalendarEvent calendarEvent)
        {
            Events.Remove(calendarEvent);
        }

    }
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
