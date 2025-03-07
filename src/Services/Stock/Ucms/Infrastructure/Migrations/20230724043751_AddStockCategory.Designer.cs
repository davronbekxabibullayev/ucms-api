﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ucms.Stock.Infrastructure.EntityFramework;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    [DbContext(typeof(StockDbContext))]
    [Migration("20230724043751_AddStockCategory")]
    partial class AddStockCategory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Income", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("IncomeDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("IncomeStatus")
                        .HasColumnType("integer");

                    b.Property<int>("IncomeType")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Note")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<Guid>("StockId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("StockId");

                    b.ToTable("Incomes");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.IncomeItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<Guid>("IncomeId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("SkuId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("IncomeId");

                    b.HasIndex("SkuId");

                    b.ToTable("IncomeItems");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.MeasurementUnit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<double>("Multiplier")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameEn")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameKa")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("MeasurementUnits");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Outcome", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Note")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<DateTimeOffset>("OutcomeDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("OutcomeStatus")
                        .HasColumnType("integer");

                    b.Property<int>("OutcomeType")
                        .HasColumnType("integer");

                    b.Property<Guid>("StockId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("StockId");

                    b.ToTable("Outcomes");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.OutcomeItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Amount")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("OutcomeId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SkuId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("OutcomeId");

                    b.HasIndex("SkuId");

                    b.ToTable("OutcomeItems");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Sku", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MeasurementUnitId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameEn")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameKa")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("MeasurementUnitId");

                    b.ToTable("Skus");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Stock", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameEn")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameKa")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("NameRu")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<int>("StockCategory")
                        .HasColumnType("integer");

                    b.Property<int>("StockType")
                        .HasColumnType("integer");

                    b.Property<int>("StorageCondition")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockDemand", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DemandDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DemandStatus")
                        .HasColumnType("integer");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Note")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SenderId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("SenderId");

                    b.ToTable("StockDemands");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockDemandItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MeasurementUnitId")
                        .HasColumnType("uuid");

                    b.Property<bool>("NotApproved")
                        .HasColumnType("boolean");

                    b.Property<string>("Note")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StockDemandId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("MeasurementUnitId");

                    b.HasIndex("StockDemandId");

                    b.ToTable("StockDemandItems");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockSku", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<Guid>("SkuId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StockId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("SkuId");

                    b.HasIndex("StockId");

                    b.ToTable("StockSkus");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Income", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.IncomeItem", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Income", "Outcome")
                        .WithMany("IncomeItems")
                        .HasForeignKey("IncomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ucms.Stock.Domain.Models.Sku", "Sku")
                        .WithMany()
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Outcome");

                    b.Navigation("Sku");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Outcome", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.OutcomeItem", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Outcome", "Outcome")
                        .WithMany("OutcomeItems")
                        .HasForeignKey("OutcomeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ucms.Stock.Domain.Models.Sku", "Sku")
                        .WithMany()
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Outcome");

                    b.Navigation("Sku");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Sku", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.MeasurementUnit", "MeasurementUnit")
                        .WithMany()
                        .HasForeignKey("MeasurementUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MeasurementUnit");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Stock", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Parent")
                        .WithMany("Childs")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockDemand", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockDemandItem", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.MeasurementUnit", "MeasurementUnit")
                        .WithMany()
                        .HasForeignKey("MeasurementUnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ucms.Stock.Domain.Models.StockDemand", "StockDemand")
                        .WithMany("StockDemandItems")
                        .HasForeignKey("StockDemandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MeasurementUnit");

                    b.Navigation("StockDemand");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockSku", b =>
                {
                    b.HasOne("Ucms.Stock.Domain.Models.Sku", "Sku")
                        .WithMany()
                        .HasForeignKey("SkuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Ucms.Stock.Domain.Models.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sku");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Income", b =>
                {
                    b.Navigation("IncomeItems");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Outcome", b =>
                {
                    b.Navigation("OutcomeItems");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.Stock", b =>
                {
                    b.Navigation("Childs");
                });

            modelBuilder.Entity("Ucms.Stock.Domain.Models.StockDemand", b =>
                {
                    b.Navigation("StockDemandItems");
                });
#pragma warning restore 612, 618
        }
    }
}
