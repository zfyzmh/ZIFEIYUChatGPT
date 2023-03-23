using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIFEIYU.Entity
{
    public class UserConfig
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        public bool IsDarkMode { get; set; } = true;

        public string ChatModel { get; set; } = "gpt-3.5-turbo";

        public string ApiKey { get; set; } = "";

        public int Timeout { get; set; } = 35;

        public bool IsProxy { get; set; } = false;

        public bool IsSSL { get; set; } = true;

        public string ProxyAddress { get; set; } = "127.0.0.1";

        public int ProxyPort { get; set; } = 7890;
    }
}