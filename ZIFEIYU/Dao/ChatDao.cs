using SQLite;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;

namespace ZIFEIYU.Dao
{
    public class ChatDao
    {
        private readonly SQLiteAsyncConnection _database;

        public ChatDao(ZFYDatabase database)
        {
            this._database = database.Database;
        }

        public async Task<List<ChatEntity>> GetAllChat()
        {
            return await _database.Table<ChatEntity>().ToListAsync();
        }

        public async Task<ChatEntity> GetChatById(int id)
        {
            return await _database.Table<ChatEntity>().Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> CreateChat(ChatEntity chatEntity)
        {
            return await _database.InsertAsync(chatEntity);
        }

        public async Task<int> SaveChat(ChatEntity chatEntity)
        {
            if (chatEntity.Id <= 0)
            {
                return await _database.InsertAsync(chatEntity);
            }
            else
            {
                return await _database.UpdateAsync(chatEntity);
            }
        }

        public async Task<ChatEntity> GetChatCurrent()
        {
            var count = await _database.Table<ChatEntity>().CountAsync();
            return await _database.Table<ChatEntity>().Skip(count - 1).FirstOrDefaultAsync();
        }
    }
}