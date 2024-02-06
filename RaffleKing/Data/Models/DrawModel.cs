using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public enum DrawTypeEnum
{
    Raffle,
    Lottery
}

public class DrawModel
{
    /// <summary>
    /// Auto-generated primary key for the draw.
    /// </summary>
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; private init; }

    /// <summary>
    /// The title of the draw, up to 30 characters.
    /// </summary>
    [Required]
    [StringLength(30)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// The description of the draw, up to 500 characters.
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = null!;
    
    /// <summary>
    /// The scheduled draw date/time of the draw.
    /// </summary>
    [Required]
    public DateTime DrawDate { get; set; }

    /// <summary>
    /// Whether the draw is active or not. Set to false by default. Should be set to true when the draw is
    /// published (after prizes are added) then set to false again when winners have been drawn.
    /// </summary>
    [Required] 
    public bool IsActive { get; set; } = false; 
    
    /// <summary>
    /// Indicates whether the draw is a bundle (single winner drawn for all prizes) or not (individual winner drawn
    /// for each prize).
    /// </summary>
    [Required]
    public bool IsBundle { get; init; } = true;
    
    /// <summary>
    /// The type of the draw, either Raffle or Lottery.
    /// </summary>
    [Required]
    public DrawTypeEnum DrawType { get; init; }
    
    /// <summary>
    /// Indicates the total maximum number of entries for the draw. For raffles, this will dictate the number of
    /// tickets available. For lotteries, this will determine the available lucky number range
    /// (from 1 - MaxEntriesTotal).
    /// </summary>
    [Required]
    public int MaxEntriesTotal { get; init; }
    
    /// <summary>
    /// Indicates the maximum number of times that a particular user can enter the draw.
    /// </summary>
    [Required]
    public int MaxEntriesPerUser { get; init; }
    
    /// <summary>
    /// The identifier of the draw host, linking to an ApplicationUser.
    /// </summary>
    [ForeignKey(nameof(DrawHost))]
    public string DrawHostId { get; init; } = null!;
    
    /// <summary>
    /// Navigation property to the ApplicationUser who is the host of the draw.
    /// </summary>
    public virtual ApplicationUser? DrawHost { get; set; }
    
    /// <summary>
    /// Collection of PrizeModel objects associated with the draw.
    /// Represents the prizes available in this draw.
    /// </summary>
    public virtual ICollection<PrizeModel> Prizes { get; set; } = new HashSet<PrizeModel>();
    
    /// <summary>
    /// Collection of EntryModel objects associated with the draw.
    /// Represents all entries submitted for this draw.
    /// </summary>
    public virtual ICollection<EntryModel> Entries { get; set; } = new HashSet<EntryModel>();
}