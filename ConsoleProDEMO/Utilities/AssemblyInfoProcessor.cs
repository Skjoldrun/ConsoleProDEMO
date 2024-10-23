using System.Reflection;

namespace ConsoleProDEMO.Utilities
{
    public static class AssemblyInfoProcessor
    {
        /// <summary>
        /// Gets the entry assembly name.
        /// </summary>
        /// <returns>Name of the entry assembly</returns>
        public static string GetEntryAssemblyName()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        /// <summary>
        /// Gets the entry assembly version.
        /// </summary>
        /// <returns>Verson of the entry assembly</returns>
        public static string GetEntryAssemblyVersion()
        {
            return $"{Assembly.GetEntryAssembly().GetName().Version}";
        }
    }
}