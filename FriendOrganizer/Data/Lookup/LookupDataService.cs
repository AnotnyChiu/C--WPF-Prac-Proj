using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Lookup
{
    public class LookupDataService : ILookupDataService
        , IProgrammingLanguageLookupDataService
        , IMeetingLookupDataService
    {
        private readonly Func<FriendOrganizerDbContext> _context;

        public LookupDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _context = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            // get access to db using "using" block
            using (var ctx = _context())
            {
                return await ctx.Friends.AsNoTracking()
                    .Select(f => new LookupItem
                    {
                        Id = f.Id,
                        DisplayMember = f.FirstName + " " + f.LastName
                    })
                    .ToListAsync();
            }
        }

        // use for load meeting
        public async Task<List<LookupItem>> GetMeetingLookupAsync()
        {
            using (var ctx = _context())
            {
                return await ctx.Meetings.AsNoTracking()
                    .Select(m => new LookupItem
                    {
                        Id = m.Id,
                        DisplayMember = m.Title
                    })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync()
        {
            // get access to db using "using" block
            using (var ctx = _context())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking()
                    .Select(f => new LookupItem
                    {
                        Id = f.Id,
                        DisplayMember = f.Name
                    })
                    .ToListAsync();
            }
        }
    }
}
