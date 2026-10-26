using CoachDesk.Data;
using CoachDesk.Services.Models;
using CoachDesk.Services.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CoachDesk.Services.Core;

public class SearchService : ISearchService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<IReadOnlyList<CoachModel>> SearchCoaches(
		string? name,
		DayOfWeek? dayOfWeek,
		TimeOnly? availableFrom,
		TimeOnly? availableTo
	)
	{
		return await dbContext.Coaches
			.Where(x =>
				(name == null || EF.Functions.Like(matchExpression: x.User.FullName, pattern: name))
				&&
				x.WorkPeriods.Any(x =>
					(dayOfWeek == null || x.DayOfWeek == dayOfWeek)
					&&
					(availableFrom == null || x.Start >= availableFrom)
					&&
					(availableTo == null || x.End <= availableTo)
				)
			)
			.Select(x =>
				new CoachModel()
				{
					Id = x.Id,
					FullName = x.User.FullName,
					MemberSince = x.User.MemberSince,
					Bio = x.User.Bio,
				}
			)
			.ToListAsync()
		;
	}

	public async Task<IReadOnlyList<MembershipPlanModel>> SearchCoachMembershipPlans(
		Guid id,
		decimal? minPrice,
		decimal? maxPrice,
		int? minDurationDays,
		int? minSessions
	)
	{
		return await dbContext.MembershipPlans
			.Where(x =>
				x.CoachId == id
				&&
				(minPrice == null || x.Price >= minPrice)
				&&
				(maxPrice == null || x.Price <= maxPrice)
				&&
				(minDurationDays == null || x.Duration == null || x.Duration >= minDurationDays)
				&&
				(minSessions == null || x.Count == null || x.Count >= minSessions)
			)
			.Select(x =>
				new MembershipPlanModel()
				{
					Id = x.Id,
					Name = x.Name,
					Price = x.Price,
					Duration = x.Duration,
					Count = x.Count,
					CoachId = id,
				}
			)
			.ToListAsync()
		;
	}

	public SearchService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
