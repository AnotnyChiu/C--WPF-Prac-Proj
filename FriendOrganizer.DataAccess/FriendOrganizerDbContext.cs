using FriendOrganizer.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.DataAccess
{
    // entity framework setup
    // first defind DbContext class and inherit DbContext from System.Data.Entity
    public class FriendOrganizerDbContext : DbContext
    {
        public DbSet<Friend> Friends { get; set; }

        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }

        // use base constructor to specify connection string
        // but put the connString setting inside app.config file
        public FriendOrganizerDbContext() : base("FriendOrganizerDb")
        {

        }

        // custom setting: 設定建立起的db table 名稱不要是複數，使用單數即可
        // override OnModelCreating method
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // table will be created call "Friend" though our property is called Friends here
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // add fluent api db column constraint
            // 作法一是直接寫在這邊，但不好，可以根據不同table拉出去寫
            //modelBuilder.Entity<Friend>()
            //    .Property(f => f.FirstName)
            //    .IsRequired()
            //    .HasMaxLength(50);

            // add constraint class
            //modelBuilder.Configurations.Add(new FriendConfiguration());
        }
    }

    // 正確連結postgres模式
    // 1. 安裝 npgsql(版本4.0.12) 再安裝 EntityFramework6.Npgsql
    // 2. 安裝好之後app.config那邊會自動設定好provider，只需去設定sonnectionString即可
    // 3. 在繼承DbContext的class下方再加上下面這段configurationg設定檔即可
    // 4. 去manager console 先 enable migration >> init database >> update database
    // 詳細參見: https://www.npgsql.org/ef6/
    class NpgSqlConfiguration : DbConfiguration
    {
        public NpgSqlConfiguration()
        {
            var name = "Npgsql";

            SetProviderFactory(providerInvariantName: name,
                               providerFactory: NpgsqlFactory.Instance);

            SetProviderServices(providerInvariantName: name,
                                provider: NpgsqlServices.Instance);

            SetDefaultConnectionFactory(connectionFactory: new NpgsqlConnectionFactory());
        }
    }

    //// 這個proj用annotation來設定constraint，所以這邊先comment掉
    // add table constraint class
    //public class FriendConfiguration : EntityTypeConfiguration<Friend> 
    //{
    //    // add constraint in constructor's "Property"
    //    public FriendConfiguration()
    //    {
    //        Property(f => f.FirstName)
    //            .IsRequired()
    //            .HasMaxLength(50);
    //    }
    //}
}

/*
Setup:
1. 用tool裡面的nuget package manager 叫出console來執行
2. 輸入enable-migration來建立default的configuration檔
3. 在configuration檔裡面建立一開始的資料後，在console輸入: Add-Migration "migration 名字"
EX: Add-Migration InitialDatabase
 */