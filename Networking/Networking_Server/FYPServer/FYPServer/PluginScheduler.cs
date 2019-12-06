using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DarkRift.Server;

namespace FYPServer
{
    class PluginScheduler : Plugin
    {
        public override bool ThreadSafe => false;

        public override Version Version => new Version(1, 0, 0);
        public PluginScheduler(PluginLoadData pluginLoadData) : base(pluginLoadData)
        {
            InitPluginsAsync(PluginManager);
            Console.WriteLine("Scheduler has been instantiated");
        }

        private static async Task InitPluginsAsync(IPluginManager pluginManager)
        {
            await Task.Yield();
            bool arePluginsLoaded = false;
            while (!arePluginsLoaded)
            {
                try
                {
                    pluginManager.GetPluginByType<PluginScheduler>();
                    arePluginsLoaded = true;
                }
                catch (Exception)
                {
                    //Console.WriteLine("Darkrift hasn't finished to load plugins");
                }
            }
            Console.WriteLine("The plugins are being Initialized ");

            //Insert plugins to be initialized here
            LoadPlugin<SessionManager>(pluginManager);
            LoadPlugin<NetworkEntityManager>(pluginManager);

            Console.WriteLine("Completed Initializing the plugins");

        }

        private static void LoadPlugin<T>(IPluginManager pluginManager)where T : Plugin,ISchedulable
        {
            pluginManager.GetPluginByType<T>().Init();
            Console.WriteLine("Loaded " + typeof(T).Name);
        }
    }
}
