using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using RaffleKing.Data.Models;

namespace RaffleKing.Components.Pages;

public partial class DrawDetails
{
    [Parameter]
    public int DrawId { get; set; }

    private DrawModel? _draw;
    private List<PrizeModel>? _prizes;
    private int _availableEntries;
    private List<int>? _availableLuckyNumbers;
    private string? _detailsText;
    

    protected override async Task OnInitializedAsync()
    {
        _draw = await DrawService.GetDrawById(DrawId);
        _prizes = await PrizeService.GetPrizesForDraw(DrawId);
        _availableEntries = GetAvailableEntries();
        _detailsText = GetDetailsText();
        _availableLuckyNumbers = GetAvailableLuckyNumbers();
    }

    /// <summary>
    /// Returns the number of available entries for this draw. Returns the difference between MaxEntriesPerUser and
    /// the current number of entries made by this user, or the number of total entries remaining, whichever is lower. 
    /// </summary>
    /// <returns>Maximum available entries.</returns>
    private int GetAvailableEntries()
    {
        if (_draw is null) return 0;
        
        return _draw.MaxEntriesPerUser;
    }

    /// <summary>
    /// Returns a list of the available lucky numbers for this draw (if lottery). The list will only contain
    /// </summary>
    private List<int> GetAvailableLuckyNumbers()
    {
        var availableLuckyNumbers = new List<int>();
        if (_draw?.DrawType != DrawTypeEnum.Lottery) return availableLuckyNumbers;
        
        for (var i = 1; i < 100; i++)
        {
            // Temporarily just add all lucky numbers until entry logic is added
            availableLuckyNumbers.Add(i);
        }

        return availableLuckyNumbers;
    }

    /// <summary>
    /// Returns the text to be displayed underneath the draw description. Contains information about the number of
    /// prizes available, and whether or not the draw is a bundle (single winner for all prizes or individual winner
    /// for each prize).
    /// </summary>
    /// <returns>The draw details text.</returns>
    private string GetDetailsText()
    {
        var numberOfPrizes = _prizes?.Sum(prize => prize.Quantity);
        var text = new StringBuilder();

        text.Append($"There are a total of {numberOfPrizes} prizes to be won in this {_draw?.DrawType.ToString()}. ");

        if (_draw is { IsBundle: true })
        {
            text.Append($"This {_draw?.DrawType.ToString()} is a bundle, so a single winner will be drawn as the" +
                        $" recipient of all {numberOfPrizes} prizes.");
        }
        else
        {
            text.Append($"An individual winner will be drawn for each prize in this {_draw?.DrawType.ToString()}, " +
                        $"so each entry gives you {numberOfPrizes} chances to win!");
        }
        
        return text.ToString();
    }
}