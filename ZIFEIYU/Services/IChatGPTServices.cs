using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZIFEIYU.Model;

namespace ZIFEIYU.Services
{
    public interface IChatGPTServices
    {
        public Task<DavinciOutput> GetDavinci(DavinciInput davinciInput);
        
    }
}
