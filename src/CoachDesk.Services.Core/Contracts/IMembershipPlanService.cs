using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IMembershipPlanService
{
	Task<MembershipPlanModel?> GetMembershipPlanAsync(Guid id);
	Task<IReadOnlyList<MembershipModel>> GetMembershipsAsync(Guid id);
	Task<int> CountMembershipsAsync(Guid id);
	Task AddMembershipPlanAsync(string name, decimal price, int? count, int? duration, Guid coachId);
	Task EditMembershipPlanAsync(Guid id, string name, decimal price, int? count, int? duration);
	Task DeleteMembershipPlansAsync(params Guid[] ids);
}
