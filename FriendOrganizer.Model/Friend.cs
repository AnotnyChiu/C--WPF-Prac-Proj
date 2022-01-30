using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Model
{
    public class Friend
    {
        [Key] // >> define the primary key (but ef detect Id as default PK)
        public int Id { get; set; }
        [Required]
        // [MaxLength(50)] >> for other data type also 
        [StringLength(50)] // >> specific for string
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
    }
}
