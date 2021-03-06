using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Model
{
    // many to many relationship
    // friend can have many meetings
    // meeting can have many friends
    public class Meeting
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public ICollection<Friend> Friends { get; set; }

        public Meeting()
        {
            Friends = new Collection<Friend>();
        }
    }
}
