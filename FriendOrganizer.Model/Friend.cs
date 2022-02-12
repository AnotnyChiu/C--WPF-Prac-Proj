using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [EmailAddress]
        public string Email { get; set; }

        // programming language
        public int? FavoriteLanguageId { get; set; }
        public ProgrammingLanguage FarvoriteLanguage { get; set; }

        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }

        public ICollection<Meeting> Meetings { get; set; }

        public Friend()
        {
            PhoneNumbers = new Collection<FriendPhoneNumber>();
            Meetings = new Collection<Meeting>();
        }

        // row version for optimistic concurrency
        // 說明: 當同時有許多使用者針對同一個Friend修改並儲存時
        // 這個property會去check他的版本，如果現有DB版本跟UI的版本不一時代表有其他人修改過資料
        // 此時會丟出exception，然後再針對這個exception去跟使用者確認是否覆蓋即可
        // [Timestamp] // 這個decorator說明這個property是row version
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        // [ConcurrencyCheck]
        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[NotMapped]
        public string RowVersion { get; set; }
    }
}
