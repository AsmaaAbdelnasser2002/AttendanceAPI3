﻿// <auto-generated />
using System;
using AttendanceAPI3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AttendanceAPI3.Migrations
{
    [DbContext(typeof(AttendanceContext))]
    partial class AttendanceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AttendanceAPI3.Models.AttendanceRecord", b =>
                {
                    b.Property<int>("AttendanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttendanceId"), 1L, 1);

                    b.Property<int>("Session_Id")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("TimeIn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TimeOut")
                        .HasColumnType("datetime2");

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.HasKey("AttendanceId");

                    b.HasIndex("Session_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("AttendanceRecords");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Package", b =>
                {
                    b.Property<int>("PackageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PackageId"), 1L, 1);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("FacesFolder")
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PackageDescription")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("PackageName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte[]>("Sheet")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.Property<byte[]>("VoicesFolder")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("PackageId");

                    b.HasIndex("User_Id");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Sequance", b =>
                {
                    b.Property<int>("SequanceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SequanceId"), 1L, 1);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("FacesFolder")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("Package_Id")
                        .HasColumnType("int");

                    b.Property<string>("SequanceDescription")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("SequanceName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte[]>("Sheet")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.Property<byte[]>("VoicesFolder")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("SequanceId");

                    b.HasIndex("Package_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Sequances");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Session", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SessionId"), 1L, 1);

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("FacesFolder")
                        .HasColumnType("varbinary(max)");

                    b.Property<int?>("Sequance_Id")
                        .HasColumnType("int");

                    b.Property<string>("SessionDescription")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SessionName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SessionPlace")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("Sheet")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("User_Id")
                        .HasColumnType("int");

                    b.Property<byte[]>("VoicesFolder")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("SessionId");

                    b.HasIndex("Sequance_Id");

                    b.HasIndex("User_Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserRole")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.AttendanceRecord", b =>
                {
                    b.HasOne("AttendanceAPI3.Models.Session", "Session")
                        .WithMany("AttendanceRecords")
                        .HasForeignKey("Session_Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AttendanceAPI3.Models.User", "User")
                        .WithMany("AttendanceRecords")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Session");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Package", b =>
                {
                    b.HasOne("AttendanceAPI3.Models.User", "User")
                        .WithMany("Packages")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Sequance", b =>
                {
                    b.HasOne("AttendanceAPI3.Models.Package", "Package")
                        .WithMany("Sequances")
                        .HasForeignKey("Package_Id");

                    b.HasOne("AttendanceAPI3.Models.User", "User")
                        .WithMany("Sequances")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Package");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Session", b =>
                {
                    b.HasOne("AttendanceAPI3.Models.Sequance", "Sequance")
                        .WithMany("Session")
                        .HasForeignKey("Sequance_Id");

                    b.HasOne("AttendanceAPI3.Models.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("User_Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Sequance");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Package", b =>
                {
                    b.Navigation("Sequances");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Sequance", b =>
                {
                    b.Navigation("Session");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.Session", b =>
                {
                    b.Navigation("AttendanceRecords");
                });

            modelBuilder.Entity("AttendanceAPI3.Models.User", b =>
                {
                    b.Navigation("AttendanceRecords");

                    b.Navigation("Packages");

                    b.Navigation("Sequances");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
