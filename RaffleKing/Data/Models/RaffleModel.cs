using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public class RaffleModel
{
    /// <summary>
    /// Auto-generated primary key for the raffle.
    /// </summary>
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int RaffleId { get; private init; }

    /// <summary>
    /// The title of the raffle.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Title { get; set; } = "Untitled Raffle";

    /// <summary>
    /// The description of the raffle.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = "Missing description.";
    
    /// <summary>
    /// The scheduled draw date/time of the raffle.
    /// </summary>
    [Required]
    public DateTime DrawDate { get; set; }

    /// <summary>
    /// Whether the raffle is active or not. Set to false by default. Should be set to true when the raffle is
    /// published (after prizes are added) then set to false again when the raffle has been drawn.
    /// </summary>
    [Required] 
    public bool IsActive { get; set; } = false; 
    
    /// <summary>
    /// Indicates whether the raffle is a bundle (single winner drawn for all prizes) or not (individual winner drawn
    /// for each prize).
    /// </summary>
    [Required]
    public bool IsBundle { get; init; } = true;
    
    /// <summary>
    /// The identifier of the raffle holder, linking to an ApplicationUser.
    /// </summary>
    [ForeignKey("AspNetUsers")]
    public string RaffleHolderId { get; init; } = null!;
    
    /// <summary>
    /// Navigation property to the ApplicationUser who is the holder of the raffle.
    /// </summary>
    public virtual ApplicationUser? RaffleHolder { get; set; }
    
    /// <summary>
    /// Collection of PrizeModel objects associated with the raffle.
    /// Represents the prizes available in this raffle.
    /// </summary>
    public virtual ICollection<PrizeModel> Prizes { get; set; } = new HashSet<PrizeModel>();
    
    /// <summary>
    /// Collection of EntryModel objects associated with the raffle.
    /// Represents all entries submitted for this raffle.
    /// </summary>
    public virtual ICollection<EntryModel> Entries { get; set; } = new HashSet<EntryModel>();
}