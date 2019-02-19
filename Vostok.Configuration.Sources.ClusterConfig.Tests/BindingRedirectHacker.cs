using System;
using System.Reflection;

namespace Vostok.Configuration.Sources.ClusterConfig.Tests
{
    internal static class BindingRedirectHacker
    {
        public static void Setup()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var assemblyName = new AssemblyName(eventArgs.Name);

                var zeroVersion = new Version();

                if (assemblyName.Version.Equals(zeroVersion))
                    return null;

                assemblyName.Version = zeroVersion;
                return Assembly.LoadFrom(assemblyName.Name + ".dll");
            };
        }
    }
}