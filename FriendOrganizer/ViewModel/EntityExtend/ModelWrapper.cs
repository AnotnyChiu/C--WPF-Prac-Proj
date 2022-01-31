using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    // make a universal model wrapper class
    // 注意generic class建立方式!
    public class ModelWrapper<T> : NotifyDataErrorInfoBase
    {
        public T Model { get; set; }
        public ModelWrapper(T model)
        {
            Model = model;
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName] string propertyName = null) // use null to make it optional
        {
            // set TValue here otherwise it'll collide with ModelWrapper<T>'s T
            return (TValue)typeof(T).GetProperty(propertyName).GetValue(Model);

            /*
             * 解說:
             * 1. 使用 System.Runtime.CompilerServices 中的 CallerMemberName decorator
             *    可以在runtime時知道是哪一個子項目call了這個function
             * 2. 取出這個子項目( type(T) )的property(根據傳進來的property name)後取得Value
             * 3. 最後因為GetValue是回傳object，再把他casting回去原本的TValue即可
             */
        }

        protected virtual void SetValue<TValue>(
            TValue value,
            [CallerMemberName] string propertyName = null) // use null to make it optional
        {
            // set TValue here otherwise it'll collide with ModelWrapper<T>'s T
            typeof(T).GetProperty(propertyName).SetValue(Model, value);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);
            
            /*
             * 解說:
             * 1. 使用 System.Runtime.CompilerServices 中的 CallerMemberName decorator
             *    可以在runtime時知道是哪一個子項目call了這個function
             * 2. 設定property value
             * 3. 同時執行OnPropertyChanged，就不用一直寫一樣的東西
             * 4. 設定好值之後，validation一樣在這邊做!
             */
        }

        private void ValidateCustomErrors(string propertyName)
        {
            // 2. Validate Custom Errors
            //// grap out the errors from child class
            //// call this method that override by different sub classes
            var errors = ValidateProperty(propertyName);

            if (errors != null)
            {
                foreach (var e in errors)
                {
                    AddErrors(propertyName, e);
                }
            }
        }

        private void ValidatePropertyInternal(string propertyName, object curValue)
        {
            ClearErrors(propertyName);
            ValidateDataAnnotations(propertyName, curValue);
            ValidateCustomErrors(propertyName);
        }

        private void ValidateDataAnnotations(string propertyName, object curValue)
        {
            // 1. Validate Data Annotations
            /*
                說明: 在Entity的地方已經有設定好Data annotation，問題是要如何把那邊
                (entity framework在後面check的annotation)跟前面的UI做連結? 是這邊要做的事             
             */

            // 去抓這個entity的這個property的annotation
            var context = new ValidationContext(Model) { MemberName = propertyName };
            // 建一個空List，validate完之後會把結果存這邊
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(curValue, context, results);

            // 去把得來的results加入error list就能跟UI介面做連結
            foreach (var result in results)
            {
                AddErrors(propertyName, result.ErrorMessage);
            }
        }

        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            return null;
            // for subclasses to override this method
        }
    }
}
