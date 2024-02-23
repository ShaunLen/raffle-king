namespace RaffleKing.Services.BLL.Interfaces;

public interface IEmailService
{
    Task SendEmail(string recipient, string subject, string body);
    void SendGuestEntranceEmail(string recipient, string guestRef);
    void SendUserEntranceEmail(string userId);
    void SendWinnerEmail(string recipient, int prizeId);
}