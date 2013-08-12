﻿namespace RssService.Application
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;

    using Castle.Facilities.Logging;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using PusherServer;

    using RssService.Business.Repos;
    using RssService.Business.Services;

    public class Bootstrapper
    {
        public static IWindsorContainer Container { get; private set; }

        public static void Initialize()
        {
            Container = new WindsorContainer();
            Container.AddFacility<WcfFacility>();
            Container.AddFacility<LoggingFacility>(f => f.UseNLog());

            var netNamedPipeBinding = new NetNamedPipeBinding
            {
                MaxBufferSize = 67108864,
                MaxReceivedMessageSize = 67108864,
                TransferMode = TransferMode.Streamed,
                ReceiveTimeout = new TimeSpan(0, 30, 0),
                SendTimeout = new TimeSpan(0, 30, 0)
            };

            var pusherAppId = ConfigurationManager.AppSettings["pusherAppId"];
            var pusherAppKey = ConfigurationManager.AppSettings["pusherAppKey"];
            var pusherAppSecret = ConfigurationManager.AppSettings["pusherAppSecret"];

            Container.Register(
                Component.For<ExceptionInterceptor>(),
                Component.For(typeof(IEntityRepository<>)).ImplementedBy(typeof(EntityRepository<>)),
                Component.For<Pusher>()
                         .DependsOn(
                             new Hashtable {
                                               { "appId", pusherAppId },
                                               { "appKey", pusherAppKey },
                                               { "appSecret", pusherAppSecret },
                                           }),
                Types.FromAssemblyNamed("RssService.Business")
                     .Pick()
                     .If(
                         type =>
                         type.GetInterfaces()
                             .Any(
                                 i =>
                                 i.IsDefined(typeof(ServiceContractAttribute), true)
                                 && i.Name != typeof(RssService).Name))
                     .Configure(
                         configurer =>
                         configurer.Named(configurer.Implementation.Name)
                                   .LifestyleSingleton()
                                   .AsWcfService(
                                       new DefaultServiceModel().AddEndpoints(
                                           WcfEndpoint.BoundTo(netNamedPipeBinding)
                                                      .At(
                                                          string.Format(
                                                              "net.pipe://localhost/{0}", configurer.Implementation.Name)))
                                                                .PublishMetadata()))
                     .WithService.Select(
                         (type, baseTypes) =>
                         type.GetInterfaces().Where(i => i.IsDefined(typeof(ServiceContractAttribute), true))));

            Thread.Sleep(1000);
            var service = Container.Resolve<IRssService>();
            service.Run();

        }
    }
}