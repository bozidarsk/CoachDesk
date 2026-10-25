using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface ISessionService
{
	Task<SessionModel?> GetSessionAsync(Guid id);
	Task AddSessionAsync(DateTime date, TimeSpan duration, Guid coachId, Guid clientId);
	Task DeleteSessionsAsync(params Guid[] ids);
}
