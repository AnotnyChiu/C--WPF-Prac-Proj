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

    public class NullLookupItem : LookupItem
    {
        // Null Lookup Item
        public new int? Id { get => null; }
        public new string DisplayMember { get; } = "<Empty>";
    }

    /*
     * new 關鍵字做為宣告修飾詞使用時，會明確隱藏繼承自基底類別的成員。 
     * 當您隱藏繼承的成員時，該成員的衍生版本就會取代基底類別版本
     * 白話: 子項目的property會蓋過母項目，可能兩者名字一樣但內容不同
     */
}
