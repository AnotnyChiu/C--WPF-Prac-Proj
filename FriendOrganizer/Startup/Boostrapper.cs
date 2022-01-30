using Autofac;
using FriendOrganizer.UI.Data;
using FriendOrganizer.DataAccess;
using FriendOrganizer.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace FriendOrganizer.UI.Startup
{
    public class Boostrapper
    {
        // this class is responsible for creating the autofac container
        // the container knows about all the type and is used to create all the instances
        public IContainer Boostrap() 
        {
            // using autofac 4.2.1
            var builder = new ContainerBuilder();

            // register event aggregator
            builder.RegisterType<EventAggregator>()
                   .As<IEventAggregator>()
                   .SingleInstance();

            // register window and view model
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<FriendDetailViewModel>().As<IFriendDetailViewModel>();

            // register db context
            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();
            // AsSelf() >> provide its own concrete type as service

            // whenever a IFriendDataService is required, a FriendDataService will be sent back
            builder.RegisterType<FriendDataService>().As<IFriendDataService>();

            // builder.RegisterType<LookupDataService>().As<IFriendLookupDataService>();
            // 上面這個As<Interface> 只能指定一個interface，但這個lookupDataService會有其他功能並繼承其他interfaces
            // 所以要用以下這種寫法較佳
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            return builder.Build();
        }
    }
}
