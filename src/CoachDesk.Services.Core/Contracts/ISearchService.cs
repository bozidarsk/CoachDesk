using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface ISearchService
{
	Task<IReadOnlyList<CoachModel>> SearchCoaches(
		string? name,
		DayOfWeek? dayOfWeek,
		TimeOnly? availableFrom,
		TimeOnly? availableTo
	);

	Task<IReadOnlyList<MembershipPlanModel>> SearchCoachMembershipPlans(
		Guid id,
		decimal? minPrice,
		decimal? maxPrice,
		int? minDurationDays,
		int? minSessions
	);
}
