﻿using AutoMapper;
using log4net;
using NHibernate;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using PIMTool.Services.Constants;
using PIMTool.Services.Service;
using PIMTool.Services.Service.Map;
using PIMTool.Services.Service.Pattern;
using PIMTool.Services.Service.Pattern.SessionFactory;
using PIMTool.Services.Service.Repository;

namespace PIMTool.Services.DependencyInjection
{
    public class ServiceBindingModule : NinjectModule
    {
        public override void Load()
        {
            ConfigureLog4Net();
            ConfigureNHibernate();
            BindServices();
        }

        private void ConfigureLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();

            var logger = LogManager.GetLogger(WebConstants.WEB_SERVER_TRACE);
            Bind<ILog>().ToConstant(logger);
        }

        private void ConfigureNHibernate()
        {
            Bind<ISessionFactory>().ToProvider<PIMToolSessionFactoryProvider<PIMToolSessionFactory>>()
                .InSingletonScope();
            Bind<IUnitOfWorkProvider>()
                .To<UnitOfWorkProvider>()
                .WithConstructorArgument(WebConstants.SessionFactory, x => x.Kernel.Get<ISessionFactory>());
            Bind<ISession>().ToMethod(x => x.Kernel.Get<ISessionFactory>().OpenSession()).InRequestScope();
        }

        private void BindServices()
        {
            Bind<IProjectService>().To<ProjectService>().InSingletonScope();
            Bind<IProjectRepository>().To<ProjectRepository>();
            Bind<IMapper>()
                .ToMethod(context =>
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<EntityToResourceMap>();
                        cfg.AddProfile<ResourceToEntityMap>();
                    });
                    return config.CreateMapper();
                }).InSingletonScope();
        }
    }
}