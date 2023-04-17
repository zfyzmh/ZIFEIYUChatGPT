using SQLite;
using System.Reflection;
using ZIFEIYU.Entity;
using ZIFEIYU.util;

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

        public async void Init()
        {
            _ = Database.CreateTablesAsync(CreateFlags.AutoIncPK, Assembly.GetExecutingAssembly().GetTypes().Where(m => m.Namespace == "ZIFEIYU.Entity" & m.IsClass).ToArray()).Result;
            if (await Database.Table<UserConfig>().CountAsync() == 0)
            { await Database.InsertAsync(new UserConfig() { IsDarkMode = true, UserId = 0 }); }
        }

        public void ErrorLog(Exception exception)
        {
            ErrorLogEntity entity = new ErrorLogEntity();
            entity.Message = exception.Message;
            entity.StackTrace = exception.StackTrace;
            entity.ErrorInfo = exception.ToJson();
            _database.InsertAsync(entity);
        }
    }
}