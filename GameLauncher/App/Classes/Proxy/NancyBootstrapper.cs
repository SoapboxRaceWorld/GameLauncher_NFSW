using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Configuration;

namespace GameLauncher.App.Classes.Proxy
{
    class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IEnumerable<Type> ViewEngines {
            get {
                return new List<Type>();
            }
        }

        public override void Configure(INancyEnvironment environment)
        {
            base.Configure(environment);

            environment.Tracing(true, true);
        }
    }
}
