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

        public int IsDarkMode { get; set; }
    }
}