using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIFEIYU
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
