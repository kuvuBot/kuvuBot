﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kuvuBot.Data;

namespace kuvuBot.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20190127132152_AddKuvuWarnAndKuvuUser")]
    partial class AddKuvuWarnAndKuvuUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

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

                    b.Property<uint>("Exp");

                    b.Property<uint>("Level");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuWarn", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<int?>("GuildId");

                    b.Property<int?>("UserId");

                    b.Property<int>("Weight");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.HasIndex("UserId");

                    b.ToTable("Warns");
                });

            modelBuilder.Entity("kuvuBot.Data.KuvuWarn", b =>
                {
                    b.HasOne("kuvuBot.Data.KuvuGuild", "Guild")
                        .WithMany()
                        .HasForeignKey("GuildId");

                    b.HasOne("kuvuBot.Data.KuvuUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
