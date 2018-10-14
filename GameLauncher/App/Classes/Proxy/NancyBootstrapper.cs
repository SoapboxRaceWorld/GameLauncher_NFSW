using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.Proxy
{
    class NancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IEnumerable<Type> ViewEngines {
            get {
                return new List<Type>();
            }
        }
    }
}
