using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    public class NotifyDataErrorInfoBase : ViewModelBase, INotifyDataErrorInfo
    {
        // create a dict to store errors for all the properties
        private Dictionary<string, List<string>> _errorsByPropertyName
            = new Dictionary<string, List<string>>();

        // INotifyErrorInfo interface
        public bool HasErrors => _errorsByPropertyName.Any(); // 用 Any() 找出任何問題

        // subscribe to its own event
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName)
              ? _errorsByPropertyName[propertyName]
              : null;
        }

        // publish event if errors are added or removed
        protected virtual void OnErrorsChanged(string propertyName)
        {
            // use ? to check whether the error is null
            // "this" here says the sender is the class itself
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

            // raise event if error changed
            base.OnPropertyChanged(nameof(HasErrors));
        }

        // Add and Remove Errors
        protected void AddErrors(string propertyName, string error)
        {
            // check if property is new
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            // check if error is new
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                // also raise the ErrorsChanged event
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            // check if property is new
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
 