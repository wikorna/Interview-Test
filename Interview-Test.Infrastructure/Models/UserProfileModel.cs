using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Interview_Test.Models;
[Table("UserProfileTb")]
public class UserProfileModel
{
    //[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid ProfileId { get; set; }
    
    //[Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; } =default!;
    //[Required]
    [Column(TypeName = "varchar(100)")]
    public string LastName { get; set; }=default!;
    public int? Age { get; set; }

    //[ForeignKey("Id")]
    //[Required]
    public UserModel? User { get; set; } = default!;
}