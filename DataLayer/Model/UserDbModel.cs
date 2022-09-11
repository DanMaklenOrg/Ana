using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Ana.DataLayer.Model;

[Index(nameof(Username), IsUnique = true)]
public class UserDbModel
{
    [Key]
    public Guid Guid { get; set; }

    [MaxLength(50)]
    public string Username { get; set; } = default!;

    [MaxLength(172)]
    public string HashedPassword { get; set; } = default!;

    [MaxLength(24)]
    public string HashSalt { get; set; } = default!;
}
