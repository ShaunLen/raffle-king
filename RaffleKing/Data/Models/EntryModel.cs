using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public class EntryModel
{
    /// <summary>
    /// Auto-generated primary key for the entry.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private init; }

    /// <summary>
    /// The identifier of the associated draw.
    /// </summary>
    [ForeignKey(nameof(Draw))]
    public int DrawId { get; init; }

    /// <summary>
    /// (Nullable) The identifier of the user, if the entry is made by a registered user.
    /// </summary>
    [ForeignKey(nameof(User))]
    public string? UserId { get; init; }

    /// <summary>
    /// Indicates whether the entry is made by a guest.
    /// </summary>
    [Required]
    public bool IsGuest { get; init; }

    /// <summary>
    /// (Nullable) The email address of the guest, if the entry is made by a guest.
    /// </summary>
    [EmailAddress]
    [StringLength(255)]
    public string? GuestEmail { get; set; }

    /// <summary>
    /// (Nullable) A unique reference for guest entries, if the entry is made by a guest.
    /// </summary>
    [StringLength(32)]
    public string? GuestReferenceCode { get; set; }
    
    /// <summary>
    /// (Nullable) Only relevant for lotteries, the selected "lucky number" associated with the entry.
    /// </summary>
    public int? LuckyNumber { get; init; }

    /// <summary>
    /// Whether or not this entry has won a prize, false by default. When set to true, PrizeId will also be
    /// populated.
    /// </summary>
    public bool IsWinner { get; set; }
    
    /// <summary>
    /// (Nullable) The identifier of the prize won by this entry, if any.
    /// </summary>
    public int? PrizeId { get; set; }

    /// <summary>
    /// Navigation property to the associated ApplicationUser (if any).
    /// </summary>
    public virtual ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Navigation property to the associated draw.
    /// </summary>
    public virtual DrawModel? Draw { get; set; }
    
    /// <summary>
    /// Navigation property to the associated prize, if this entry is a winner.
    /// </summary>
    public virtual PrizeModel? Prize { get; set; }
}