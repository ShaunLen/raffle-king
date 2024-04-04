using Microsoft.AspNetCore.Components;
using MudBlazor;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Shared;

public partial class DrawCard
{
    /// <summary>
    /// The Id of the Draw.
    /// </summary>
    [Parameter] public int DrawId { get; set; }
    
    /// <summary>
    /// The type of list the DrawCard will be displayed in (Active/Hosted/Entered).
    /// </summary>
    [Parameter] public DrawListType ListType { get; set; }

    // Private fields
    private DrawModel? _draw;
    private string? _description;
    private string? _buttonText;
    private string? _dateString;
    private bool _expired;
    private Color _dateColor;
    private double _percentageEntriesRemaining;

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawManagementService.GetDrawById(DrawId);

        if (_draw is null) return;
        
        _dateString = GetFormattedDate(_draw.DrawDate);
        _expired = _draw.DrawDate < DateTime.Now;
        _dateColor = _expired ? Color.Error : Color.Info;
        _percentageEntriesRemaining = await EntryManagementService.GetPercentageEntriesRemainingByDraw(DrawId);
        
        // Restrict description displayed to 100 characters.
        _description = _draw.Description.Length > 100 ? $"{_draw.Description[..100]}..." : _draw.Description;

        _buttonText = ListType switch
        {
            DrawListType.ActiveDraws => $"View {_draw.DrawType}",
            DrawListType.HostedDraws => _expired ? $"View {_draw.DrawType}" : $"Edit {_draw.DrawType}",
            DrawListType.EnteredDraws => $"View {_draw.DrawType}",
            _ => $"View {_draw.DrawType}"
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
            return date < DateTime.Now ? "Expired" : "Today";
        
        if (date.Date < today)
            return "Expired";
        
        if (date.Date == today.AddDays(1))
            return "Tomorrow";
        
        if (date > weekStart && date < weekEnd)
            return date.ToString("dddd");

        return date.ToString(date.Year == today.Year ? "MMMM d" : "MMMM d, yyyy");
    }
}