using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace CodeComb.vNextChina.Migrations
{
    public partial class AddExperimentFlagToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ContestExperiment_Experiment_ExperimentId", table: "ContestExperiment");
            migrationBuilder.DropForeignKey(name: "FK_Post_Thread_ThreadId", table: "Post");
            migrationBuilder.DropForeignKey(name: "FK_Post_User_UserId", table: "Post");
            migrationBuilder.DropForeignKey(name: "FK_Project_CISet_CISetId", table: "Project");
            migrationBuilder.DropForeignKey(name: "FK_Status_User_UserId", table: "Status");
            migrationBuilder.DropForeignKey(name: "FK_StatusDetail_Status_StatusId", table: "StatusDetail");
            migrationBuilder.DropForeignKey(name: "FK_Thread_User_UserId", table: "Thread");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<long>_IdentityRole<long>_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<long>_User_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<long>_User_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<long>_IdentityRole<long>_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<long>_User_UserId", table: "AspNetUserRoles");
            migrationBuilder.AddColumn<string>(
                name: "ExperimentFlags",
                table: "AspNetUsers",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_ContestExperiment_Experiment_ExperimentId",
                table: "ContestExperiment",
                column: "ExperimentId",
                principalTable: "Experiment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Post_Thread_ThreadId",
                table: "Post",
                column: "ThreadId",
                principalTable: "Thread",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Post_User_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Project_CISet_CISetId",
                table: "Project",
                column: "CISetId",
                principalTable: "CISet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Status_User_UserId",
                table: "Status",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_StatusDetail_Status_StatusId",
                table: "StatusDetail",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_Thread_User_UserId",
                table: "Thread",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<long>_IdentityRole<long>_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<long>_User_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<long>_User_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<long>_IdentityRole<long>_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<long>_User_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_ContestExperiment_Experiment_ExperimentId", table: "ContestExperiment");
            migrationBuilder.DropForeignKey(name: "FK_Post_Thread_ThreadId", table: "Post");
            migrationBuilder.DropForeignKey(name: "FK_Post_User_UserId", table: "Post");
            migrationBuilder.DropForeignKey(name: "FK_Project_CISet_CISetId", table: "Project");
            migrationBuilder.DropForeignKey(name: "FK_Status_User_UserId", table: "Status");
            migrationBuilder.DropForeignKey(name: "FK_StatusDetail_Status_StatusId", table: "StatusDetail");
            migrationBuilder.DropForeignKey(name: "FK_Thread_User_UserId", table: "Thread");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<long>_IdentityRole<long>_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<long>_User_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<long>_User_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<long>_IdentityRole<long>_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<long>_User_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "ExperimentFlags", table: "AspNetUsers");
            migrationBuilder.AddForeignKey(
                name: "FK_ContestExperiment_Experiment_ExperimentId",
                table: "ContestExperiment",
                column: "ExperimentId",
                principalTable: "Experiment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Post_Thread_ThreadId",
                table: "Post",
                column: "ThreadId",
                principalTable: "Thread",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Post_User_UserId",
                table: "Post",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Project_CISet_CISetId",
                table: "Project",
                column: "CISetId",
                principalTable: "CISet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Status_User_UserId",
                table: "Status",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_StatusDetail_Status_StatusId",
                table: "StatusDetail",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Thread_User_UserId",
                table: "Thread",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<long>_IdentityRole<long>_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<long>_User_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<long>_User_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<long>_IdentityRole<long>_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<long>_User_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
