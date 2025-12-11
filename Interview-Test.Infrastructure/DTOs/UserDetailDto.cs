using System;
using System.Collections.Generic;

namespace Interview_Test.Infrastructure.DTOs
{
    // Role สำหรับใช้ใน Response (GetUserById)
    public class UserRoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    // Response DTO หลักสำหรับ GetUserById / CreateUser (response)
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? Age { get; set; }

        // รายชื่อ Role ที่ user มี
        public List<UserRoleDto> Roles { get; set; } = new();

        // Flatten permission codes ทั้งหมด
        public List<string> Permissions { get; set; } = new();
    }

    // ==== DTO สำหรับ CreateUser (Request) ====

    public class CreatePermissionDto
    {
        public string PermissionCode { get; set; } = default!;
    }

    public class CreateRoleDto
    {
        public int RoleId { get; set; }              // ถ้า design ให้ client เลือก role เดิม
        public string RoleName { get; set; } = default!;
        public List<CreatePermissionDto> Permissions { get; set; } = new();
    }

    public class CreateUserProfileDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public int? Age { get; set; }
    }

    public class CreateUserDto
    {
        public string UserId { get; set; } = default!;
        public string Username { get; set; } = default!;
        public CreateUserProfileDto UserProfile { get; set; } = default!;
        public List<CreateRoleDto> Roles { get; set; } = new();
    }
}
