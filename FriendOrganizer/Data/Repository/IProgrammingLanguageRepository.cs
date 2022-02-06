using FriendOrganizer.Model;
using System.Threading.Tasks;
using static FriendOrganizer.UI.Data.Repository.ProgrammingLanguageRepository;

namespace FriendOrganizer.UI.Data.Repository
{
    public interface IProgrammingLanguageRepository
        : IGenericRepository<ProgrammingLanguage>
    {
        // check if a language is referenced by a friend
        Task<LanguageReferenced> IsReferencedByFriendAsync(int programmingLanguageId);
    }
}