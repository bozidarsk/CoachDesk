using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Services.Core.Contracts;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core;

public class MembershipService : IMembershipService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<MembershipModel?> GetMembershipAsync(Guid id)
	{
		var membership = await dbContext.Memberships
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (membership == null)
			return null;

		return new()
		{
			Id = membership.Id,
			ActivatedOn = membership.ActivatedOn,
			TimesUsed = membership.TimesUsed,
			MembershipPlanId = membership.MembershipPlanId,
			ClientId = membership.ClientId,
			Status = await GetMembershipStatusAsync(membership.Id),
		};
	}

	public async Task<MembershipModel?> GetMembershipAsync(Guid? membershipPlanId, Guid? coachId, Guid? clientId)
	{
		var membership = await dbContext.Memberships
			.SingleOrDefaultAsync(x =>
				(membershipPlanId == null || x.MembershipPlanId == membershipPlanId)
				&& (coachId == null || x.MembershipPlan.CoachId == coachId)
				&& (clientId == null || x.ClientId == clientId)
			)
		;

		if (membership == null)
			return null;

		return new()
		{
			Id = membership.Id,
			ActivatedOn = membership.ActivatedOn,
			TimesUsed = membership.TimesUsed,
			MembershipPlanId = membership.MembershipPlanId,
			ClientId = membership.ClientId,
			Status = await GetMembershipStatusAsync(membership.Id),
		};
	}

	public async Task<string?> GetMembershipStatusAsync(Guid id)
	{
		var membership = await dbContext.Memberships
			.Include(x => x.MembershipPlan)
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (membership == null)
			return null;

		if (membership.MembershipPlan.Duration != null && membership.ActivatedOn.AddDays((int)membership.MembershipPlan.Duration) <= DateTime.UtcNow)
			return "Expired";

		if (membership.MembershipPlan.Count != null && membership.TimesUsed >= membership.MembershipPlan.Count)
			return "OutOfSessions";

		return "Active";
	}

	public async Task UseMembersipAsync(Guid id)
	{
		var membership = await dbContext.Memberships
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (membership == null)
			return;

		membership.TimesUsed++;

		await dbContext.SaveChangesAsync();
	}

	public async Task AddMembershipAsync(Guid membershipPlanId, Guid clientId)
	{
		await dbContext.Memberships.AddAsync(
			new()
			{
				ActivatedOn = DateTime.UtcNow,
				TimesUsed = 0,
				MembershipPlanId = membershipPlanId,
				ClientId = clientId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteMembershipsAsync(params Guid[] ids)
	{
		await dbContext.Memberships
			.Where(x => ids.Contains(x.Id))
			.ExecuteDeleteAsync()
		;
	}

	public MembershipService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
