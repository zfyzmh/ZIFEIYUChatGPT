using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;

namespace ZIFEIYU.Dao
{
    public class ChatDao
    {
        private readonly ZFYDatabase _database;

        public ChatDao(ZFYDatabase database)
        {
            this._database = database;
        }

        public async Task<List<ChatEntity>> GetAllChat()
        {
            return await _database.Database.Table<ChatEntity>().ToListAsync();
        }

        public async Task<ChatEntity> GetChatById(int id)
        {
            return await _database.Database.Table<ChatEntity>().Where(m => m.Id == id).FirstOrDefaultAsync();
        }
    }
}