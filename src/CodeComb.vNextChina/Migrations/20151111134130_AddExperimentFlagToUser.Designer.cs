using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using CodeComb.vNextChina.Models;

namespace CodeComb.vNextChina.Migrations
{
    [DbContext(typeof(vNextChinaContext))]
    [Migration("20151111134130_AddExperimentFlagToUser")]
    partial class AddExperimentFlagToUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc2-16311");

            modelBuilder.Entity("CodeComb.vNextChina.Hub.Models.Node", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Alias")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<int>("Port");

                    b.Property<string>("PrivateKey")
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<string>("Server")
                        .HasAnnotation("MaxLength", 64);

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "Nodes");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Blob", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ContentLength");

                    b.Property<string>("ContentType")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<byte[]>("File");

                    b.Property<string>("FileName")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("ContentLength");

                    b.HasIndex("ContentType");

                    b.HasIndex("Time");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.CISet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationTime");

                    b.Property<DateTime?>("LastBuildingTime");

                    b.Property<string>("Title");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Contest", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime>("Begin");

                    b.Property<long>("CompetitorCount");

                    b.Property<string>("Description");

                    b.Property<DateTime>("End");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 64);

                    b.HasKey("Id");

                    b.HasIndex("Begin");

                    b.HasIndex("End");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.ContestExperiment", b =>
                {
                    b.Property<string>("ContestId")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<long>("ExperimentId");

                    b.Property<int>("Point");

                    b.HasKey("ContestId", "ExperimentId");

                    b.HasIndex("Point");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Experiment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Accepted");

                    b.Property<byte[]>("AnswerArchive");

                    b.Property<bool>("CheckPassed");

                    b.Property<string>("Description");

                    b.Property<int>("Difficulty");

                    b.Property<string>("Namespace")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("NuGet");

                    b.Property<int>("OS");

                    b.Property<int>("Submitted");

                    b.Property<byte[]>("TestArchive");

                    b.Property<long>("TimeLimit");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("Version")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("CheckPassed");

                    b.HasIndex("Difficulty");

                    b.HasIndex("Title");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Forum", b =>
                {
                    b.Property<string>("Id")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<string>("Description");

                    b.Property<bool>("IsReadOnly");

                    b.Property<int>("PRI");

                    b.Property<string>("ParentId")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<long>("PostCount");

                    b.Property<long>("ThreadCount");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("PRI");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<Guid?>("ParentId");

                    b.Property<long>("ThreadId");

                    b.Property<DateTime>("Time");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("Time");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdditionalEnvironmentVariables");

                    b.Property<string>("Alias");

                    b.Property<Guid>("CISetId");

                    b.Property<int>("CurrentVersion");

                    b.Property<string>("NuGetHost");

                    b.Property<string>("NuGetPrivateKey");

                    b.Property<int>("PRI");

                    b.Property<bool>("RunWithLinux");

                    b.Property<bool>("RunWithOsx");

                    b.Property<bool>("RunWithWindows");

                    b.Property<string>("VersionRule");

                    b.Property<string>("ZipUrl");

                    b.HasKey("Id");

                    b.HasIndex("PRI");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Status", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Accepted");

                    b.Property<byte[]>("Archive");

                    b.Property<long?>("ExperimentId");

                    b.Property<string>("LinuxOutput");

                    b.Property<int>("LinuxResult");

                    b.Property<long>("MemoryUsage");

                    b.Property<string>("NuGet");

                    b.Property<string>("OsxOutput");

                    b.Property<int>("OsxResult");

                    b.Property<Guid?>("ProjectId");

                    b.Property<int>("Result");

                    b.Property<bool>("RunWithLinux");

                    b.Property<bool>("RunWithOsx");

                    b.Property<bool>("RunWithWindows");

                    b.Property<DateTime>("Time");

                    b.Property<long>("TimeUsage");

                    b.Property<int>("Total");

                    b.Property<int>("Type");

                    b.Property<long>("UserId");

                    b.Property<string>("WindowsOutput");

                    b.Property<int>("WindowsResult");

                    b.HasKey("Id");

                    b.HasIndex("LinuxResult");

                    b.HasIndex("OsxResult");

                    b.HasIndex("Result");

                    b.HasIndex("RunWithLinux");

                    b.HasIndex("RunWithOsx");

                    b.HasIndex("Time");

                    b.HasIndex("Type");

                    b.HasIndex("WindowsResult");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.StatusDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Method");

                    b.Property<int>("OS");

                    b.Property<int>("Result");

                    b.Property<long>("StatusId");

                    b.Property<float>("Time");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("OS");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("PRI");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("PRI");

                    b.HasIndex("Title");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Thread", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("ForumId")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<bool>("IsAnnouncement");

                    b.Property<bool>("IsLocked");

                    b.Property<bool>("IsTop");

                    b.Property<DateTime>("LastReplyTime");

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<long>("UserId");

                    b.Property<long>("Visit");

                    b.HasKey("Id");

                    b.HasIndex("CreationTime");

                    b.HasIndex("IsAnnouncement");

                    b.HasIndex("IsTop");

                    b.HasIndex("LastReplyTime");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<byte[]>("Avatar");

                    b.Property<string>("AvatarContentType")
                        .HasAnnotation("MaxLength", 32);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("ExperimentFlags");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Motto")
                        .HasAnnotation("MaxLength", 512);

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("Organization")
                        .HasAnnotation("MaxLength", 128);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("RegisteryTime");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("WebSite")
                        .HasAnnotation("MaxLength", 128);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasIndex("RegisteryTime");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole<long>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("RoleId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<long>("UserId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<long>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<long>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.ContestExperiment", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Contest")
                        .WithMany()
                        .HasForeignKey("ContestId");

                    b.HasOne("CodeComb.vNextChina.Models.Experiment")
                        .WithMany()
                        .HasForeignKey("ExperimentId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Forum", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Forum")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Post", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Post")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.HasOne("CodeComb.vNextChina.Models.Thread")
                        .WithMany()
                        .HasForeignKey("ThreadId");

                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Project", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.CISet")
                        .WithMany()
                        .HasForeignKey("CISetId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Status", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Experiment")
                        .WithMany()
                        .HasForeignKey("ExperimentId");

                    b.HasOne("CodeComb.vNextChina.Models.Project")
                        .WithMany()
                        .HasForeignKey("ProjectId");

                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.StatusDetail", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Status")
                        .WithMany()
                        .HasForeignKey("StatusId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Tag", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Tag")
                        .WithMany()
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("CodeComb.vNextChina.Models.Thread", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.Forum")
                        .WithMany()
                        .HasForeignKey("ForumId");

                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole<long>")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<long>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole<long>")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("CodeComb.vNextChina.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
