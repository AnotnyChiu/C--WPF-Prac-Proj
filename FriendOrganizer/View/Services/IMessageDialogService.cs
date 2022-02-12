using System.Threading.Tasks;

namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        // remember go to boostrapper to register the interface
        Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);
        Task ShowInfoDialogAsync(string text);
    }
}