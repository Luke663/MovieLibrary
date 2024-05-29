using System.ComponentModel;

namespace MovieLibrary.ViewModels
{
    // Used to implement INotifyPropertyChanged on inheriting classes

    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
