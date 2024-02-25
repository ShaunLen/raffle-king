using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaffleKing.Data.Models;

public class WinnerModel
{
    /// <summary>
    /// Auto-generated primary key for the winning entry.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; private init; }
    
    /// <summary>
    /// (Nullable) The identifier of the winning entry.
    /// </summary>
    [ForeignKey(nameof(Entry))]
    public int? EntryId { get; set; }
    
    /// <summary>
    /// The identifier of the prize won.
    /// </summary>
    [ForeignKey(nameof(Prize))]
    public int PrizeId { get; init; }
    
    /// <summary>
    /// Navigation property to the associated entry.
    /// </summary>
    public virtual EntryModel? Entry { get; set; }
    
    /// <summary>
    /// Navigation property to the associated prize.
    /// </summary>
    public virtual PrizeModel? Prize { get; set; }
}