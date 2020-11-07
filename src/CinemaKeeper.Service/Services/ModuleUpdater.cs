using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

using CinemaKeeper.Service.Configurations;

using Discord.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace CinemaKeeper.Service.Services
{
    internal class ModuleUpdater : BackgroundService
    {
        private readonly IServiceProvider _services;

        private readonly CommandService _commandService;

        private readonly ServiceConfiguration _serviceConfiguration;

        public ModuleUpdater(IServiceProvider services)
        {
            _services = services;

            _commandService = services.GetService<CommandService>();
            _serviceConfiguration = services.GetService<ServiceConfiguration>();
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Log.Information("Begin loading modules");

                try
                {
                    await UnloadOldModules();

                    var moduleAssemblies = ReadModuleAssemblies();

                    await LoadNewModules(moduleAssemblies);

                    Log.Information("Done loading modules");
                }
                catch (Exception e)
                {
                    Log.Warning(e, "Error while loading modules");

                    return;
                }

                await Task.Delay(_serviceConfiguration.ModulesUpdateRate, cancellationToken);
            }
        }

        private async Task UnloadOldModules()
        {
            var removeTasks = _commandService.Modules
               .AsParallel()
               .Select(_commandService.RemoveModuleAsync);

            await Task.WhenAll(removeTasks);
        }

        private async Task LoadNewModules(IEnumerable<Assembly> assemblies)
        {
            var addTasks = assemblies
               .AsParallel()
               .Select(a => _commandService.AddModulesAsync(a, _services));

            await Task.WhenAll(addTasks);
        }

        private IEnumerable<Assembly> ReadModuleAssemblies()
        {
            string modulesPath = Path.Combine(
                Environment.CurrentDirectory,
                _serviceConfiguration.ModuleAssembliesDirectory);

            var assemblies = Directory
               .GetFiles(modulesPath, _serviceConfiguration.ModulesAssemblyNamePattern, SearchOption.TopDirectoryOnly)
               .AsParallel()
               .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            return assemblies;
        }
    }
}