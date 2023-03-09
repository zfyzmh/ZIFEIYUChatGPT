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

        public void Init()
        {
            CreateTablesResult result = Database.CreateTablesAsync(CreateFlags.AutoIncPK, Assembly.GetExecutingAssembly().GetTypes().Where(m => m.Namespace == "ZIFEIYU.Entity" & m.IsClass).ToArray()).Result;
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