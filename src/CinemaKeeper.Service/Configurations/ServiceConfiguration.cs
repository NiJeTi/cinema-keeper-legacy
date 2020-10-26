using System;

namespace CinemaKeeper.Service.Configurations
{
    [Serializable]
    internal class ServiceConfiguration
    {
        public string ModulesAssemblyNamePattern { get; set; } = "CinemaKeeper.Modules*.dll";
        public string ModuleAssembliesDirectory { get; set; } = "Modules";
        public TimeSpan ModulesUpdateRate { get; set; } = TimeSpan.FromMinutes(5);
    }
}