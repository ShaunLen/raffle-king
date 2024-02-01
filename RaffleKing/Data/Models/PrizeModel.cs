using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

/// <summary>
/// Represents an individual prize in a raffle.
/// This model is used to store and manage details of each prize associated with a raffle.
/// </summary>
/// <param name="name">The name of the prize, max 30 characters.</param>
/// <param name="description">A detailed description of the prize, max 500 characters.</param>
/// <param name="quantity">The quantity of this prize available in the raffle.</param>
/// <param name="raffleId">The identifier of the raffle to which this prize is associated.</param>
public class PrizeModel
    (string name, string description, int quantity, int raffleId)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrizeId { get; set; } // auto-generated

    [Required]
    [StringLength(30)]
    public string Name { get; set; } = name;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = description;

    [Required]
    public int Quantity { get; set; } = quantity;

    [ForeignKey("RaffleModel")]
    public int RaffleId { get; set; } = raffleId;

    // Raffle reference
    public virtual RaffleModel? Raffle { get; set; }
}