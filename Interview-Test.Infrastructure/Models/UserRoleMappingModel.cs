using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Interview_Test.Models;

[Table("UserRoleMappingTb")]
public class UserRoleMappingModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid UserRoleMappingId { get; set; }

    // FK → User
    [ForeignKey("UserId")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    
    public UserModel? User { get; set; }

    // FK → Role
    [ForeignKey("RoleId")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public RoleModel? Role { get; set; }
}