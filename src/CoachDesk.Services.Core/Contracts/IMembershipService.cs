using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IMembershipService
{
	Task<MembershipModel?> GetMembershipAsync(Guid id);
	Task<MembershipModel?> GetMembershipAsync(Guid? membershipPlanId, Guid? coachId, Guid? clientId);
	Task<string?> GetMembershipStatusAsync(Guid id);
	string GetMembershipStatus(DateTime activatedOn, int timesUsed, int? duration, int? count);
	Task UseMembersipAsync(Guid id);
	Task AddMembershipAsync(Guid membershipPlanId, Guid clientId);
	Task DeleteMembershipsAsync(params Guid[] ids);
}
