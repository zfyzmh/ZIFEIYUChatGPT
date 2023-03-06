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

        //public async Task<List<TodoItem>> GetItemsAsync()
        //{
        //    await Init();
        //    return await Database.Table<TodoItem>().ToListAsync();
        //}

        //public async Task<List<TodoItem>> GetItemsNotDoneAsync()
        //{
        //    await Init();
        //    return await Database.Table<TodoItem>().Where(t => t.Done).ToListAsync();

        //    // SQL queries are also possible
        //    //return await Database.QueryAsync<TodoItem>("SELECT * FROM [TodoItem] WHERE [Done] = 0");
        //}

        //public async Task<TodoItem> GetItemAsync(int id)
        //{
        //    await Init();
        //    return await Database.Table<TodoItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        //}

        //public async Task<int> SaveItemAsync(TodoItem item)
        //{
        //    await Init();
        //    if (item.ID != 0)
        //        return await Database.UpdateAsync(item);
        //    else
        //        return await Database.InsertAsync(item);
        //}

        //public async Task<int> DeleteItemAsync(TodoItem item)
        //{
        //    await Init();
        //    return await Database.DeleteAsync(item);
        //}
    }
}