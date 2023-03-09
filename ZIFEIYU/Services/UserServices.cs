using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;

namespace ZIFEIYU.Services
{
    public class UserServices
    {
        private readonly ZFYDatabase database;

        public UserServices(ZFYDatabase database)
        {
            this.database = database;
        }

        public Task UpdateConfig(UserConfig userConfig)
        {
            return database.Database.UpdateAsync(userConfig);
        }

        public Task<UserConfig> GetConfig()
        {
            return database.Database.Table<UserConfig>().FirstAsync();
        }
    }
}