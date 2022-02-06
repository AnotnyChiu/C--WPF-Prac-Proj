using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.EntityExtend
{
    public class ProgrammingLanguageWrapper : ModelWrapper<ProgrammingLanguage>
    {
        public ProgrammingLanguageWrapper(ProgrammingLanguage model): base(model)
        {

        }

        public int Id { get => Model.Id; }

        public string Name 
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
