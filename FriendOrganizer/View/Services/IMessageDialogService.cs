namespace FriendOrganizer.UI.View.Services
{
    public interface IMessageDialogService
    {
        // remember go to boostrapper to register the interface
        MessageDialogResult ShowOkCancelDialog(string text, string title);
    }
}