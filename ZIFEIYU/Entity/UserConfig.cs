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

        public bool IsDarkMode { get; set; }

        public string ChatModel { get; } = "";

        public string ApiKey { get; set; } = "";

        public int Timeout { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsProxy { get; set; } = false;

        public string ProxyAddress { get; set; } = "";

        public int ProxyPort { get; set; }
    }
}