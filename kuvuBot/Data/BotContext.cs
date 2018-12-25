using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace kuvuBot.Data
{
    public class BotContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var mysqlconfig = Program.Config.MySQL;
            string connectionString = $"SERVER={mysqlconfig.Ip};DATABASE={mysqlconfig.Database};UID={mysqlconfig.User};PASSWORD={mysqlconfig.Password};persistsecurityinfo=True;port={mysqlconfig.Port};SslMode=none";

            optionsBuilder.UseMySql(connectionString, mysqlOptions =>
            {
                
            });
        }
    }
}
