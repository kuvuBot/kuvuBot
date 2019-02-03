﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kuvuBot.Data;

namespace kuvuBot.Migrations
{
    [DbContext(typeof(BotContext))]
    partial class BotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Steam.Models.SteamAppModel", b =>
                {
                    b.Property<uint>("AppId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("AppId");

                    b.ToTable("SteamAppsCache");
                });

            modelBuilder.Entity("kuvuBot.Data.CacheInfo", b =>
                {
                    b.Property<string>("Type");

                    b.Property<DateTime>("RefreshedTime");

                    b.HasKey("Type");

                    b.ToTable("CacheInfos");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuGuild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong?>("AutoRole");

                    b.Property<ulong?>("GoodbyeChannel");

                    b.Property<string>("GoodbyeMessage");

                    b.Property<ulong?>("GreetingChannel");

                    b.Property<string>("GreetingMessage");

                    b.Property<ulong>("GuildId");

                    b.Property<string>("Lang");

                    b.Property<ulong?>("LogChannel");

                    b.Property<ulong?>("MuteRole");

                    b.Property<string>("Prefix");

                    b.HasKey("Id");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuStat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Channels");

                    b.Property<DateTime>("Date");

                    b.Property<int>("Guilds");

                    b.Property<int>("Users");

                    b.HasKey("Id");

                    b.ToTable("Statistics");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<ulong>("DiscordUser");

                    b.Property<int>("Exp");

                    b.Property<int?>("GuildId");

                    b.Property<DateTime?>("LastExpMessage");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuWarn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Reason");

                    b.Property<int?>("UserId");

                    b.Property<ulong>("Warning");

                    b.Property<int>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Warns");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuUser", b =>
                {
                    b.HasOne("kuvuBot.Data.KuvuGuild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuWarn", b =>
                {
                    b.HasOne("kuvuBot.Data.KuvuUser", "User")
                        .WithMany("Warns")
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
