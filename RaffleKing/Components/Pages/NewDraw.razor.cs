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

        if (_drawDate is null || _drawTime is null)
        {
            Snackbar.Add("You must specify a draw date and time.", Severity.Error);
            return;
        }

        var hostId = await UserService.GetUserId();
        if (hostId == null)
        {
            Snackbar.Add("Invalid user.", Severity.Error);
            return;
        }
        
        var newDraw = new DrawModel
        {
            Title = _title,
            Description = _description,
            DrawDate = _drawDate.Value.Add((TimeSpan) _drawTime),
            DrawType =_drawType,
            IsBundle = _isBundle,
            MaxEntriesTotal = _maxEntriesTotal,
            MaxEntriesPerUser = _maxEntriesPerUser,
            DrawHostId = hostId
        };
            
        try
        {
            var result = await DrawManagementService.AddNewDraw(newDraw);
            if (!result.Success)
            {
                Snackbar.Add(result.Message, Severity.Error);
                return;
            }
            
            Snackbar.Add("Draw created successfully.", Severity.Success);
            NavigationManager.NavigateTo($"/draws/draw-details/{result.Data}");
        }
        catch (Exception exception)
        {
            Snackbar.Add($"Draw could not be created: {exception.Message}" , Severity.Error);
        }
    }
}