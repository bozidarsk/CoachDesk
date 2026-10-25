using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Services.Models;
using CoachDesk.Services.Core.Contracts;

namespace CoachDesk.Services.Core;

public class MembershipPlanService : IMembershipPlanService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<MembershipPlanModel?> GetMembershipPlanAsync(Guid id)
	{
		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == id);

		if (membershipPlan == null)
			return null;

		return new()
		{
			Id = membershipPlan.Id,
			Name = membershipPlan.Name,
			Price = membershipPlan.Price,
			Count = membershipPlan.Count,
			Duration = membershipPlan.Duration,
			CoachId = membershipPlan.CoachId ?? throw new NotImplementedException(),
		};
	}

	public async Task<IReadOnlyList<MembershipModel>> GetMembershipsAsync(Guid id)
	{
		return await dbContext.Memberships
			.Where(x => x.MembershipPlanId == id)
			.Select(x =>
				new MembershipModel()
				{
					Id = x.Id,
					ActivatedOn = x.ActivatedOn,
					TimesUsed = x.TimesUsed,
					MembershipPlanId = x.MembershipPlanId,
					ClientId = x.ClientId,
				}
			)
			.ToListAsync()
		;
	}

	public async Task<int> CountMembershipsAsync(Guid id)
	{
		return await dbContext.Memberships.CountAsync(x => x.MembershipPlanId == id);
	}

	public async Task AddMembershipPlanAsync(string name, decimal price, int? count, int? duration, Guid coachId)
	{
		await dbContext.MembershipPlans.AddAsync(
			new()
			{
				Name = name,
				Price = price,
				Count = count,
				Duration = duration,
				CoachId = coachId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public async Task EditMembershipPlanAsync(Guid id, string name, decimal price, int? count, int? duration)
	{
		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == id);

		if (membershipPlan == null)
			return;

		membershipPlan.Name = name;
		membershipPlan.Price = price;
		membershipPlan.Count = count;
		membershipPlan.Duration = duration;

		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteMembershipPlansAsync(params Guid[] ids)
	{
		await dbContext.MembershipPlans
			.Where(x => ids.Contains(x.Id))
			.ExecuteDeleteAsync()
		;
	}

	public MembershipPlanService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
