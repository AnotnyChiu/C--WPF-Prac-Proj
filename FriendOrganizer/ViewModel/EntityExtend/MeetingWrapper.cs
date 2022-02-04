using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        public MeetingWrapper(Meeting model):base(model)
        {
        }

        public int Id { get => Model.Id; }

        public string Title 
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public DateTime DateFrom 
        { 
          get => GetValue<DateTime>();
          set 
            {
                SetValue(value);
                // check date validation
                if (DateTo < DateFrom) 
                {
                    DateTo = DateFrom;
                }
            } 
        }

        public DateTime DateTo
        {
            get => GetValue<DateTime>();
            set
            {
                SetValue(value);
                // check date validation
                if (DateTo < DateFrom)
                {
                    DateFrom = DateTo;
                }
            }
        }
    }
}
