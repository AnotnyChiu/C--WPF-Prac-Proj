﻿using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repository
{

    // 可以直接從class extract interface出來
    // alt + enter 中就會有這個選項
    // Ctrl R R 可以快速從新命名class
    public class FriendRepository : GenericRepository<Friend,FriendOrganizerDbContext>,
                                    IFriendRepository
    {
        public FriendRepository(FriendOrganizerDbContext context) : base(context)
        {
            //!! 注意 1. 傳入的參數可以是call一個回傳generic的function >> Func<T>
            // 2.這邊這樣做DI是因為要讓Autofac去產生 FriendOrganizerDbContext 的instance
            // 3.記得要去boostrapper那邊設定autofac
        }

        // return single frined's detail info
        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            // load from db
            var firends = await _context.Friends
                .Include(f => f.PhoneNumbers) // join 作法
                .SingleAsync(f => f.Id == friendId);
                
            // as no tracking : 重新從DB取 不是先去快取找
            // 使用System.Data.Entity的ToListAsync，讓method變asyncronous

            // load phone numbers


            // async 測試
            // await Task.Delay(5000);

            return firends;
        }

        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            _context.FriendPhoneNumbers.Remove(model);
        }
    }
}
