using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration;
using Autofac.Integration.WebApi;

namespace Card
{
    public class IoConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            var domain = AppDomain.CurrentDomain;
            var currentPath = Path.Combine(domain.BaseDirectory, "bin");
            //Load all dlls from bin folder in every cases
            PreLoad(currentPath);
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
         

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => new HttpContextWrapper(HttpContext.Current))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();

            var assemblys = domain.GetAssemblies();

            //Register all repositories
           

            //Register all services
            var services = assemblys.FirstOrDefault(c => c.FullName.StartsWith("SMS.Data"));

            builder.RegisterAssemblyTypes(services)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            //We build the container.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container)); //Set the MVC DependencyResolver
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container);
        }

        private static void PreLoad(string p)
        {
            //all try/catch blocks are elided for brevity
            string[] files = Directory.GetFiles(p, "*.dll", SearchOption.AllDirectories);
            foreach (var s in files)
            {
                if(!s.Contains("roslyn"))
                {
                    var assemblyName = AssemblyName.GetAssemblyName(s);
                    var assemblys = AppDomain.CurrentDomain.GetAssemblies();
                    if (
                        !assemblys.Any(assembly => AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), assemblyName)))
                        Assembly.LoadFrom(s);
                }
               
            }
        }
    }
}