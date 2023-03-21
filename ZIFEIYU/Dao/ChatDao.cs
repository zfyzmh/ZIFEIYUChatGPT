using SQLite;
using ZIFEIYU.DataBase;
using ZIFEIYU.Entity;
using System.Linq;

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

        /// <summary>
        /// 获取对话列表倒序
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<ChatEntity>> GetChatListDesc(int pageSize, int pageNumber)
        {
            return await _database.Table<ChatEntity>().OrderByDescending(s => s.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
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

        /// <summary>
        /// 获取当前聊天
        /// </summary>
        /// <returns></returns>
        public async Task<ChatEntity> GetChatCurrent()
        {
            var count = await _database.Table<ChatEntity>().CountAsync();
            return await _database.Table<ChatEntity>().Skip(count - 1).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 切换聊天
        /// </summary>
        /// <returns></returns>
        public async Task<ChatEntity> SwitchChat(ChatEntity chat)
        {
            var count = await _database.Table<ChatEntity>().DeleteAsync(e => e.Id == chat.Id);
            chat.Id = 0;
            await SaveChat(chat);
            return chat;
        }
    }
}