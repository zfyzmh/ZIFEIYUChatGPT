using SQLite;
using System.Reflection;
using ZIFEIYU.Entity;

namespace ZIFEIYU.DataBase
{
    public class ZFYDatabase
    {
        private SQLiteAsyncConnection _database;

        public SQLiteAsyncConnection Database
        {
            get
            {
                _database ??= new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
                return _database;
            }
        }

        public ZFYDatabase()
        {
        }

        public void Init()
        {
            CreateTablesResult result = Database.CreateTablesAsync(CreateFlags.AutoIncPK, Assembly.GetExecutingAssembly().GetTypes().Where(m => m.Namespace == "ZIFEIYU.Entity" & m.IsClass).ToArray()).Result;
        }
    }
}