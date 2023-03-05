using ZIFEIYU.Model;
using ZIFEIYU.util;

namespace ZIFEIYU.Services
{
    public class ChatGPTServices
    {
        private Dictionary<string, string> Headers = new Dictionary<string, string>();

        public ChatGPTServices()
        {
            Headers.Add("Authorization", "Bearer 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");
        }

        public async Task<DavinciOutput> GetDavinci(DavinciInput davinciInput)
        {
            return await HttpHelper.HttpPostAsync<DavinciOutput>("https://api.openai.com/v1/completions", JsonHelper.SerializeObject(davinciInput), headers: Headers);
        }
    }
}