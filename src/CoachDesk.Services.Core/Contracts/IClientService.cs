using CoachDesk.Data.Models;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IClientService
{
	Task<ClientModel?> GetClientAsync(ApplicationUser user);
	Task<ClientModel?> GetClientAsync(Guid id);
	Task<IReadOnlyList<MembershipModel>> GetMembershipsAsync(Guid id);
	Task<IReadOnlyList<SessionModel>> GetUpcomingSessionsAsync(Guid id, Guid? withCoachId);
	Task AddClientAsync(string userId);
}
