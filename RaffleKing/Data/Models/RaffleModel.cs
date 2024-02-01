using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

/// <summary>
/// Represents a raffle with its associated details.
/// This model is used to store and manage details of a raffle in the database.
/// </summary>
/// <param name="title">The title of the raffle, max 30 words.</param>
/// <param name="description">A detailed description of the raffle, max 500 words.</param>
/// <param name="drawDate">The scheduled draw date/time of the raffle.</param>
/// <param name="isBundle">Indicates whether the raffle is a bundle (one winner for all prizes) or not (one winner for
/// each prize individually).</param>
/// <param name="raffleHolderId">The identifier of the raffle holder, linking to an ApplicationUser.</param>
public class RaffleModel
    (string title, string description, DateTime drawDate, bool isBundle, string raffleHolderId)
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int RaffleId { get; set; } // auto-generated
    
    [Required] 
    [StringLength(30)] 
    public string Title { get; set; } = title;
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = description;
    
    [Required]
    public DateTime DrawDate { get; set; } = drawDate;

    // Set to true when the raffle is published (after prizes are added)
    [Required] 
    public bool IsActive { get; set; } = false; 
    
    [Required]
    public bool IsBundle { get; set; } = isBundle;
    
    [ForeignKey("AspNetUsers")]
    public string RaffleHolderId { get; set; } = raffleHolderId;
    
    // Identity user reference
    public virtual ApplicationUser? RaffleHolder { get; set; }
}