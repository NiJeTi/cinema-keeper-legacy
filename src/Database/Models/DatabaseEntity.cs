using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaKeeper.Database.Models;

public abstract class DatabaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}
