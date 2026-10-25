using CoachDesk.Data;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface ICoachService
{
	Task<CoachModel?> GetCoachAsync(ApplicationUser user);
	Task<CoachModel?> GetCoachAsync(Guid id);
	Task<bool> IsAvailable(Guid id, DateTime date, TimeSpan duration);
	Task<IReadOnlyList<ClientModel>> GetClientsAsync(Guid id);
	Task<IReadOnlyList<MembershipPlanModel>> GetMembershipPlansAsync(Guid id);
	Task<IReadOnlyList<SessionModel>> GetUpcomingSessionsAsync(Guid id, Guid? withClientId);
	Task<IReadOnlyList<WorkPeriodModel>> GetWorkPeriodsAsync(Guid id);
	Task AddCoachAsync(string userId);
}
