using Microsoft.AspNetCore.Components;
using MudBlazor;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Shared;

public partial class DrawCard
{
    /// <summary>
    /// The title of the draw.
    /// </summary>
    [Parameter] public string Title { get; set; } = "Missing Title";
    
    /// <summary>
    /// The description of the draw, first 100 characters will be used if exceeded.
    /// </summary>
    [Parameter] public string Description { get; set; } = "Missing description.";
    
    /// <summary>
    /// The type of list the DrawCard will be displayed in (Active/Hosted/Entered).
    /// </summary>
    [Parameter] public DrawListType ListType { get; set; }
    
    /// <summary>
    /// The date/time that winners will be drawn.
    /// </summary>
    [Parameter] public DateTime DrawDateTime { get; set; }

    // Private fields
    private string? _buttonText;
    private string? _dateString;
    private bool _expired;
    private Color _dateColor;

    protected override void OnInitialized()
    {
        _dateString = GetFormattedDate(DrawDateTime);
        _expired = DrawDateTime < DateTime.Now;
        _dateColor = _expired ? Color.Error : Color.Info;
        
        // Restrict description displayed to 100 characters.
        if (Description.Length > 100)
        {
            Description = $"{Description[..100]}...";
        }

        // Conditional text for the button on the card
        _buttonText = ListType switch
        {
            DrawListType.ActiveDraws => "View Draw",
            DrawListType.HostedDraws => _expired ? "View Draw" : "Edit Draw",
            DrawListType.EnteredDraws => "View Draw",
            _ => "View Draw"
        };
    }

    /// <summary>
    /// Convert the draw date to a user-friendly display string. Allows users to more easily determine when a draw
    /// will be taking place.
    /// </summary>
    /// <param name="date">The date that the winner(s) will be drawn.</param>
    /// <returns>The formatted date string.</returns>
    private static string GetFormattedDate(DateTime date)
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        if (date.Date == today)
        {
            return date < DateTime.Now ? "Expired" : "Today";
        }
        else if (date.Date < today)
        {
            return "Expired";
        }
        else if (date.Date == today.AddDays(1))
        {
            return "Tomorrow";
        }
        else if (date > weekStart && date < weekEnd)
        {
            return date.ToString("dddd");
        }
        else if (date.Year == today.Year)
        {
            return date.ToString("MMMM d");
        }
        else
        {
            return date.ToString("MMMM d, yyyy");
        }
    }
}