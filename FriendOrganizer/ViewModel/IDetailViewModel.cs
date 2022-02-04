using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    // implement all detail view model's base class to make it reusable
    public interface IDetailViewModel 
    {
        Task LoadAsync(int? id); // add a question mark to make it nullable
        bool HasChanges { get; }
    }
}