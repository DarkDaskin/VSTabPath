using System;
using System.Reflection;

namespace VSTabPath.Interop.Contracts
{
    // Since VS 17.0, many interop types were moved to different assemblies, so code interacting with them
    // must have diferent references for different VS versions. Such code was moved to proxy assemblies.
    public static class DteInteropResolver
    {
        public static IDteInterop Interop { get; }

        static DteInteropResolver()
        {
            var assemblyName = Environment.Is64BitProcess ? "VSTabPath.Interop.X64" : "VSTabPath.Interop.X86";
            var assembly = Assembly.Load(assemblyName);
            var type = assembly.GetType("VSTabPath.Interop.DteInterop");
            Interop = (IDteInterop)Activator.CreateInstance(type);
        }
    }
}