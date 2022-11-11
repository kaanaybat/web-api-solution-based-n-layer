using System.Reflection;
using Autofac;
using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;
using Data.Context;
using Data.Repositories;
using Data.UnitOfWorks;
using Service.Mapping;
using Service.Services;
using Module = Autofac.Module;

namespace Api.Modules
{
    public class ServiceModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();

            var apiAssembly = Assembly.GetExecutingAssembly();
            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            builder.RegisterAssemblyTypes(apiAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

        }
    }
}