﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using rabbithole;

namespace rabbithole.Migrations
{
    [DbContext(typeof(EventSinkContext))]
    partial class EventSinkContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("rabbithole.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .HasColumnName("content")
                        .HasColumnType("jsonb");

                    b.Property<string>("Exchange")
                        .HasColumnName("exchange");

                    b.Property<DateTime>("Recieved")
                        .HasColumnName("timestamp");

                    b.Property<string>("RoutingKey")
                        .HasColumnName("routing_key");

                    b.HasKey("Id");

                    b.ToTable("Events");
                });
#pragma warning restore 612, 618
        }
    }
}