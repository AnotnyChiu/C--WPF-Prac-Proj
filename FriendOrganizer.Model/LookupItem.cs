using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Model
{
    // 注意他的Entity不是用資料夾，而是直接開另一個Proj做管理
    public class LookupItem
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
