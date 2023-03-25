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
        /// 获取对话列表以更新时间倒序,分页,
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<ChatEntity>> GetChatListDesc(int pageSize, int pageNumber)
        {
            return await _database.Table<ChatEntity>().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<ChatEntity> GetChatById(int id)
        {
            return await _database.Table<ChatEntity>().Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChat(ChatEntity chatEntity)
        {
            if (chatEntity.Id <= 0)
            {
                return await _database.InsertAsync(chatEntity, chatEntity.GetType());
            }
            else
            {
                var chat = await _database.Table<ChatEntity>().Where(m => m.Id == chatEntity.Id).FirstAsync();
                chat.DialogJson = chatEntity.DialogJson;
                chat.UpdateDate = DateTime.Now;
                return await _database.UpdateAsync(chat);
            }
        }

        /// <summary>
        /// 获取当前聊天
        /// </summary>
        /// <returns></returns>
        public async Task<ChatEntity> GetChatCurrent()
        {
            return await _database.Table<ChatEntity>().OrderByDescending(m => m.UpdateDate).FirstOrDefaultAsync();
        }

        public async Task DeleteChat(long chatId)
        {
            await _database.DeleteAsync<ChatEntity>(chatId);
        }

        public async Task DeleteChat(long[] chatIds)
        {
            foreach (var chatId in chatIds)
            {
                await _database.DeleteAsync<ChatEntity>(chatId);
            }
        }
    }
}