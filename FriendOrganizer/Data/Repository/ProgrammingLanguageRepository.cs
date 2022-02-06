using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{
    public class ProgrammingLanguageRepository 
        : GenericRepository<ProgrammingLanguage, FriendOrganizerDbContext> 
        , IProgrammingLanguageRepository
    {
        public ProgrammingLanguageRepository(FriendOrganizerDbContext context)
            : base(context)
        {
        }

        public async Task<LanguageReferenced> IsReferencedByFriendAsync(int programmingLanguageId)
        {
            var languageReferenced = new LanguageReferenced();
            languageReferenced.IsReferenced = await _context.Friends.AsNoTracking()
                .AnyAsync(f => f.FavoriteLanguageId == programmingLanguageId);

            languageReferenced.WhoReference = await _context.Friends.AsNoTracking()
                .Where(f => f.FavoriteLanguageId == programmingLanguageId)
                .Select(f => f.FirstName).ToListAsync();

            return languageReferenced;
        }

        public class LanguageReferenced 
        {
            public bool IsReferenced { get; set; }
            public List<string> WhoReference { get; set; }
        }
    }
}
