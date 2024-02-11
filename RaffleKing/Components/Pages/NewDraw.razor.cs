using System.Security.Claims;
using MudBlazor;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages;

public partial class NewDraw
{
    // Form Fields
    private string? _title;
    private string? _description;
    private DrawTypeEnum _drawType;
    private bool _isBundle;
    private DateTime? _drawDate;
    private TimeSpan? _drawTime;
    private int _maxEntriesTotal = 50;
    private int _maxEntriesPerUser = 5;

    private async Task OnSubmit()
    {
        if (_title is null || _description is null)
            return;
        
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            Snackbar.Add("User must be logged in to create a draw.", Severity.Error);
            return;
        }
        
        var newDraw = new DrawModel()
        {
            Title = _title,
            Description = _description,
            DrawDate = _drawDate!.Value.Add((TimeSpan) _drawTime!),
            DrawType =_drawType,
            IsBundle = _isBundle,
            MaxEntriesTotal = _maxEntriesTotal,
            MaxEntriesPerUser = _maxEntriesPerUser,
            DrawHostId = userId
        };
            
        try
        {
            var drawId = await DrawService.AddNewDraw(newDraw);
            Snackbar.Add("Draw created successfully.", Severity.Success);
            NavigationManager.NavigateTo($"/draws/draw-details/{drawId}");
        }
        catch (Exception exception)
        {
            Snackbar.Add($"Draw could not be created: {exception.Message}" , Severity.Error);
        }
    }
}