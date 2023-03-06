using SQLite;
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
            CreateTableResult result = Database.CreateTableAsync<Test>().Result;
        }
    }
}