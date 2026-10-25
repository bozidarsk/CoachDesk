using CoachDesk.Data;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IClientService
{
	Task<CoachModel?> GetClientAsync(ApplicationUser user);
	Task<CoachModel?> GetClientAsync(Guid id);
	Task AddClientAsync(string userId);
}
