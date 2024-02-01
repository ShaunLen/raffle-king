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
    public int EntryId { get; private init; }

    /// <summary>
    /// The identifier of the associated raffle.
    /// </summary>
    [ForeignKey("RaffleModel")]
    public int RaffleId { get; init; }

    /// <summary>
    /// (Nullable) The identifier of the user, if the entry is made by a registered user.
    /// </summary>
    [ForeignKey("AspNetUsers")]
    public string? UserId { get; set; }

    /// <summary>
    /// Indicates whether the entry is made by a guest.
    /// </summary>
    [Required]
    public bool IsGuest { get; set; }

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
    /// Navigation property to the associated ApplicationUser (if any).
    /// </summary>
    public virtual ApplicationUser? User { get; set; }
    
    /// <summary>
    /// Navigation property to the associated Raffle.
    /// </summary>
    public virtual RaffleModel? Raffle { get; set; }
}