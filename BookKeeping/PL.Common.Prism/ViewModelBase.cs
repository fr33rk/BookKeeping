using System.Runtime.CompilerServices;
using Prism.Mvvm;
using System.Linq;
using Prism.Commands;

namespace PL.Common.Prism
{
    /// <summary>Base class for ViewModels, introducing the NotifyPropertyChanged method which automatically resolves the property name.
    /// Also, the commands of the view model will be updated when a single property changes.
    /// </summary>
    public abstract class ViewModelBase : BindableBase
    {
        #region INotifyPropertyChanged

        /// <summary>Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of a property.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            this.InvokeChangeCanExecute();
            UpdateCommandCanExecute();
        }

        #endregion INotifyPropertyChanged

        private void UpdateCommandCanExecute()
        {
            foreach (var command in this.GetType()
                .GetProperties()
                .Where(pi => pi.PropertyType == typeof(DelegateCommand))
                .Select(pi => pi.GetValue(this))
                .Cast< DelegateCommand >())
            {
                command.RaiseCanExecuteChanged();
            }           
        }
    }
}
