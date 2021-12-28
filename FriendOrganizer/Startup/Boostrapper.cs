using Autofac;
using FriendOrganizer.Data;
using FriendOrganizer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FriendOrganizer.Startup
{
    public class Boostrapper
    {
        // this class is responsible for creating the autofac container
        // the container knows about all the type and is used to create all the instances
        public IContainer Boostrap() 
        {
            // using autofac 4.2.1
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            // whenever a IFriendDataService is required, a FriendDataService will be sent back
            builder.RegisterType<FriendDataService>().As<IFriendDataService>();

            return builder.Build();
        }
    }
}
