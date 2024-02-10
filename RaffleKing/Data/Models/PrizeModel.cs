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
    public int Id { get; private init; }

    /// <summary>
    /// The title of the prize, up to 30 characters.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Title { get; init; } = null!;

    /// <summary>
    /// The description of the prize, up to 500 characters.
    /// </summary>
    [Required]
    [StringLength(500)]

    public string Description { get; init; } = null!;

    /// <summary>
    /// The quantity of this prize available in the draw.
    /// </summary>
    [Required]
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// The identifier of the draw to which this prize is associated.
    /// </summary>
    [ForeignKey(nameof(Draw))]
    public int DrawId { get; init; }

    /// <summary>
    /// Navigation property to the associated draw.
    /// </summary>
    public virtual DrawModel? Draw { get; set; }
}