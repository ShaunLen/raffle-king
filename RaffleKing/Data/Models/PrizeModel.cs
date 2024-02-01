using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public class PrizeModel
{
    /// <summary>
    /// Auto-generated primary key for the prize.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrizeId { get; private init; }

    /// <summary>
    /// The title of the prize, up to 30 characters.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Title { get; set; } = "Untitled Prize";

    /// <summary>
    /// The description of the prize, up to 500 characters.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "Missing description";

    /// <summary>
    /// The quantity of this prize available in the raffle.
    /// </summary>
    [Required]
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// The identifier of the raffle to which this prize is associated.
    /// </summary>
    [ForeignKey("RaffleModel")]
    public int RaffleId { get; init; }

    /// <summary>
    /// Navigation property to the associated raffle.
    /// </summary>
    public virtual RaffleModel? Raffle { get; set; }
}