using FriendOrganizer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        // use base class's ctor (ModelWrapper)
        public FriendWrapper(Friend model) : base(model)
        {
        }

        // set the property
        public int Id { get => Model.Id; }

        public string FirstName
        {
            get => GetValue<string>(); // !! 如果只是單純return的話可以用 arrow function!
            set => SetValue(value);
        }

        public string LastName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string Email
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case (nameof(FirstName)):
                    if (string.Equals(FirstName, "Robot", StringComparison.OrdinalIgnoreCase))
                    {
                        // why yeild return here?
                        // yield return 是不把東西跑完 只要有就先return?
                        yield return "Robots are not valid friends";
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
