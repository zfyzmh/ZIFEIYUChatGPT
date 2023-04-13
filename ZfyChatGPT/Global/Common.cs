using System;
using System.IO;
using System.Threading.Tasks;

namespace ZfyChatGPT.Global
{
    public static class Common
    {
        private static ServiceProvider serviceProvider;

        public static ServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
            set
            {
                serviceProvider ??= value;
            }
        }
    }
}