using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace CodeComb.vNextChina.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Alias = table.Column<string>(nullable: true),
                    Port = table.Column<int>(nullable: false),
                    PrivateKey = table.Column<string>(nullable: true),
                    Server = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Node", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Blob",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContentLength = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    File = table.Column<byte[]>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blob", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "CISet",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastBuildingTime = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CISet", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Contest",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Begin = table.Column<DateTime>(nullable: false),
                    CompetitorCount = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    End = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contest", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Experiment",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Accepted = table.Column<int>(nullable: false),
                    AnswerArchive = table.Column<byte[]>(nullable: true),
                    CheckPassed = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Difficulty = table.Column<int>(nullable: false),
                    Namespace = table.Column<string>(nullable: true),
                    NuGet = table.Column<string>(nullable: true),
                    OS = table.Column<int>(nullable: false),
                    Submitted = table.Column<int>(nullable: false),
                    TestArchive = table.Column<byte[]>(nullable: true),
                    TimeLimit = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiment", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Forum",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsReadOnly = table.Column<bool>(nullable: false),
                    PRI = table.Column<int>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    PostCount = table.Column<long>(nullable: false),
                    ThreadCount = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forum_Forum_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Forum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PRI = table.Column<long>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_Tag_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Avatar = table.Column<byte[]>(nullable: true),
                    AvatarContentType = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    Motto = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Organization = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    RegisteryTime = table.Column<DateTime>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    WebSite = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole<long>", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AdditionalEnvironmentVariables = table.Column<string>(nullable: true),
                    Alias = table.Column<string>(nullable: true),
                    CISetId = table.Column<Guid>(nullable: false),
                    CurrentVersion = table.Column<int>(nullable: false),
                    NuGetHost = table.Column<string>(nullable: true),
                    NuGetPrivateKey = table.Column<string>(nullable: true),
                    PRI = table.Column<int>(nullable: false),
                    RunWithLinux = table.Column<bool>(nullable: false),
                    RunWithOsx = table.Column<bool>(nullable: false),
                    RunWithWindows = table.Column<bool>(nullable: false),
                    VersionRule = table.Column<string>(nullable: true),
                    ZipUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_CISet_CISetId",
                        column: x => x.CISetId,
                        principalTable: "CISet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ContestExperiment",
                columns: table => new
                {
                    ContestId = table.Column<string>(nullable: false),
                    ExperimentId = table.Column<long>(nullable: false),
                    Point = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestExperiment", x => new { x.ContestId, x.ExperimentId });
                    table.ForeignKey(
                        name: "FK_ContestExperiment_Contest_ContestId",
                        column: x => x.ContestId,
                        principalTable: "Contest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContestExperiment_Experiment_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Thread",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    ForumId = table.Column<string>(nullable: true),
                    IsAnnouncement = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    IsTop = table.Column<bool>(nullable: false),
                    LastReplyTime = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    Visit = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thread", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thread_Forum_ForumId",
                        column: x => x.ForumId,
                        principalTable: "Forum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Thread_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<long>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<long>_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<long>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<long>_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<long>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<long>_IdentityRole<long>_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<long>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<long>_IdentityRole<long>_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<long>_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Accepted = table.Column<int>(nullable: false),
                    Archive = table.Column<byte[]>(nullable: true),
                    ExperimentId = table.Column<long>(nullable: true),
                    LinuxOutput = table.Column<string>(nullable: true),
                    LinuxResult = table.Column<int>(nullable: false),
                    MemoryUsage = table.Column<long>(nullable: false),
                    NuGet = table.Column<string>(nullable: true),
                    OsxOutput = table.Column<string>(nullable: true),
                    OsxResult = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: true),
                    Result = table.Column<int>(nullable: false),
                    RunWithLinux = table.Column<bool>(nullable: false),
                    RunWithOsx = table.Column<bool>(nullable: false),
                    RunWithWindows = table.Column<bool>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    TimeUsage = table.Column<long>(nullable: false),
                    Total = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    WindowsOutput = table.Column<string>(nullable: true),
                    WindowsResult = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_Experiment_ExperimentId",
                        column: x => x.ExperimentId,
                        principalTable: "Experiment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Status_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Status_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    ThreadId = table.Column<long>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Post_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Post_Thread_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Thread",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_User_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "StatusDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Method = table.Column<string>(nullable: true),
                    OS = table.Column<int>(nullable: false),
                    Result = table.Column<int>(nullable: false),
                    StatusId = table.Column<long>(nullable: false),
                    Time = table.Column<float>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatusDetail_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateIndex(
                name: "IX_Blob_ContentLength",
                table: "Blob",
                column: "ContentLength");
            migrationBuilder.CreateIndex(
                name: "IX_Blob_ContentType",
                table: "Blob",
                column: "ContentType");
            migrationBuilder.CreateIndex(
                name: "IX_Blob_Time",
                table: "Blob",
                column: "Time");
            migrationBuilder.CreateIndex(
                name: "IX_Contest_Begin",
                table: "Contest",
                column: "Begin");
            migrationBuilder.CreateIndex(
                name: "IX_Contest_End",
                table: "Contest",
                column: "End");
            migrationBuilder.CreateIndex(
                name: "IX_ContestExperiment_Point",
                table: "ContestExperiment",
                column: "Point");
            migrationBuilder.CreateIndex(
                name: "IX_Experiment_CheckPassed",
                table: "Experiment",
                column: "CheckPassed");
            migrationBuilder.CreateIndex(
                name: "IX_Experiment_Difficulty",
                table: "Experiment",
                column: "Difficulty");
            migrationBuilder.CreateIndex(
                name: "IX_Experiment_Title",
                table: "Experiment",
                column: "Title");
            migrationBuilder.CreateIndex(
                name: "IX_Forum_PRI",
                table: "Forum",
                column: "PRI");
            migrationBuilder.CreateIndex(
                name: "IX_Post_Time",
                table: "Post",
                column: "Time");
            migrationBuilder.CreateIndex(
                name: "IX_Project_PRI",
                table: "Project",
                column: "PRI");
            migrationBuilder.CreateIndex(
                name: "IX_Status_LinuxResult",
                table: "Status",
                column: "LinuxResult");
            migrationBuilder.CreateIndex(
                name: "IX_Status_OsxResult",
                table: "Status",
                column: "OsxResult");
            migrationBuilder.CreateIndex(
                name: "IX_Status_Result",
                table: "Status",
                column: "Result");
            migrationBuilder.CreateIndex(
                name: "IX_Status_RunWithLinux",
                table: "Status",
                column: "RunWithLinux");
            migrationBuilder.CreateIndex(
                name: "IX_Status_RunWithOsx",
                table: "Status",
                column: "RunWithOsx");
            migrationBuilder.CreateIndex(
                name: "IX_Status_Time",
                table: "Status",
                column: "Time");
            migrationBuilder.CreateIndex(
                name: "IX_Status_Type",
                table: "Status",
                column: "Type");
            migrationBuilder.CreateIndex(
                name: "IX_Status_WindowsResult",
                table: "Status",
                column: "WindowsResult");
            migrationBuilder.CreateIndex(
                name: "IX_StatusDetail_OS",
                table: "StatusDetail",
                column: "OS");
            migrationBuilder.CreateIndex(
                name: "IX_Tag_PRI",
                table: "Tag",
                column: "PRI");
            migrationBuilder.CreateIndex(
                name: "IX_Tag_Title",
                table: "Tag",
                column: "Title");
            migrationBuilder.CreateIndex(
                name: "IX_Thread_CreationTime",
                table: "Thread",
                column: "CreationTime");
            migrationBuilder.CreateIndex(
                name: "IX_Thread_IsAnnouncement",
                table: "Thread",
                column: "IsAnnouncement");
            migrationBuilder.CreateIndex(
                name: "IX_Thread_IsTop",
                table: "Thread",
                column: "IsTop");
            migrationBuilder.CreateIndex(
                name: "IX_Thread_LastReplyTime",
                table: "Thread",
                column: "LastReplyTime");
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
            migrationBuilder.CreateIndex(
                name: "IX_User_RegisteryTime",
                table: "AspNetUsers",
                column: "RegisteryTime");
            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Nodes");
            migrationBuilder.DropTable("Blob");
            migrationBuilder.DropTable("ContestExperiment");
            migrationBuilder.DropTable("Post");
            migrationBuilder.DropTable("StatusDetail");
            migrationBuilder.DropTable("Tag");
            migrationBuilder.DropTable("AspNetRoleClaims");
            migrationBuilder.DropTable("AspNetUserClaims");
            migrationBuilder.DropTable("AspNetUserLogins");
            migrationBuilder.DropTable("AspNetUserRoles");
            migrationBuilder.DropTable("Contest");
            migrationBuilder.DropTable("Thread");
            migrationBuilder.DropTable("Status");
            migrationBuilder.DropTable("AspNetRoles");
            migrationBuilder.DropTable("Forum");
            migrationBuilder.DropTable("Experiment");
            migrationBuilder.DropTable("Project");
            migrationBuilder.DropTable("AspNetUsers");
            migrationBuilder.DropTable("CISet");
        }
    }
}
