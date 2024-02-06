using Microsoft.AspNetCore.Components;

namespace RaffleKing.Components.Shared;

public partial class DrawCard
{
    [Parameter] public string Title { get; set; } = "Missing Title";
    [Parameter] public string Description { get; set; } = "Missing description.";
}