using FriendOrganizer.Model;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public interface IFriendDetailViewModel
    {
        Task LoadAsync(int? friendId); // add a question mark to make it nullable
        bool HasChanges { get; }
    }
}