﻿// <auto-generated />
using System;
using AbnNotifier.Data.Notifier;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AbnNotifier.Migrations
{
    [DbContext(typeof(AbnNotifierDbContext))]
    [Migration("20210216085953_RefactorNotificationModel")]
    partial class RefactorNotificationModel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AbnNotifier.Data.Notifier.Models.Approver", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmpNo");

                    b.Property<int>("Level");

                    b.Property<Guid?>("NotificationId");

                    b.Property<string>("Title");

                    b.Property<string>("UserCode");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId");

                    b.ToTable("Approver");
                });

            modelBuilder.Entity("AbnNotifier.Data.Notifier.Models.EmailSmsSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("CanSend");

                    b.Property<string>("Code");

                    b.Property<string>("Data");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<int>("Key");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.ToTable("EmailSmsSettings");
                });

            modelBuilder.Entity("AbnNotifier.Data.Notifier.Models.Notification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime?>("DateCreated");

                    b.Property<DateTime?>("DateModified");

                    b.Property<string>("Department");

                    b.Property<string>("DocNo");

                    b.Property<string>("Empno");

                    b.Property<bool>("IsFinalStatus");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("AbnNotifier.Data.Notifier.Models.Approver", b =>
                {
                    b.HasOne("AbnNotifier.Data.Notifier.Models.Notification")
                        .WithMany("Approvers")
                        .HasForeignKey("NotificationId");
                });
#pragma warning restore 612, 618
        }
    }
}
