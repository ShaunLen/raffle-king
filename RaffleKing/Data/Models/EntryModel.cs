using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public class EntryModel
{
    /// <summary>
    /// Represents an entry in a raffle, which can be made by registered users or guests.
    /// </summary>
    /// <param name="raffleId">The identifier of the associated raffle.</param>
    /// <param name="userId">(Nullable) The identifier of the user, if the entry is made by a registered user.</param>
    /// <param name="isGuest">Indicates whether the entry is made by a guest.</param>
    /// <param name="guestEmail">(Nullable) The email address of the guest, if the entry is made by a guest.</param>
    public EntryModel(int raffleId, string? userId, bool isGuest, string? guestEmail)
    {
        RaffleId = raffleId;
        UserId = userId;
        IsGuest = isGuest;

        // Properties below are only relevant for guests
        if (!IsGuest) return;
        
        GuestEmail = guestEmail;
        // Randomly generated 32 character string
        GuestReferenceCode = Guid.NewGuid().ToString("N");
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EntryId { get; set; } // auto-generated

    [ForeignKey("RaffleModel")]
    public int RaffleId { get; set; }

    // Nullable for guest entries
    [ForeignKey("AspNetUsers")]
    public string? UserId { get; set; }

    [Required]
    public bool IsGuest { get; set; }

    // Nullable, used if IsGuest is true
    [EmailAddress]
    [StringLength(255)]
    public string? GuestEmail { get; set; }

    // Nullable, unique reference for guest entries, used if IsGuest is true
    [StringLength(32)]
    public string? GuestReferenceCode { get; set; }

    // Identity user reference
    public virtual ApplicationUser? User { get; set; }
    
    // Raffle reference
    public virtual RaffleModel? Raffle { get; set; }
}