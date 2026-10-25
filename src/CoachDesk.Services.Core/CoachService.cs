using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Data.Models;
using CoachDesk.Services.Core.Contracts;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core;

public class CoachService : ICoachService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<CoachModel?> GetCoachAsync(ApplicationUser user)
	{
		var coach = await dbContext.Coaches
			.SingleOrDefaultAsync(x => x.UserId == user.Id)
		;

		if (coach == null)
			return null;

		return new()
		{
			Id = coach.Id,
			FullName = user.FullName,
			MemberSince = user.MemberSince,
			Bio = user.Bio,
		};
	}

	public async Task<CoachModel?> GetCoachAsync(Guid id)
	{
		var coach = await dbContext.Coaches
			.Include(x => x.User)
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (coach == null)
			return null;

		return new()
		{
			Id = coach.Id,
			FullName = coach.User.FullName,
			MemberSince = coach.User.MemberSince,
			Bio = coach.User.Bio,
		};
	}

	public async Task<bool> IsAvailable(Guid id, DateTime date, TimeSpan duration)
	{
		var coach = await dbContext.Coaches
			.Include(x => x.BookedSessions)
			.Include(x => x.WorkPeriods)
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (coach == null)
			return false;

		// TODO: check if date + duration overflows to the next day

		if (!coach.WorkPeriods.Any(x =>
				x.DayOfWeek == date.DayOfWeek
				&& date.TimeOfDay >= x.Start.ToTimeSpan()
				&& date.TimeOfDay + duration < x.End.ToTimeSpan()
			)
		) return false;

		if (coach.BookedSessions.Any(x => date >= x.Date && date < x.Date + x.Duration))
			return false;

		return true;
	}

	public async Task<IReadOnlyList<ClientModel>> GetClientsAsync(Guid id)
	{
		return await dbContext.Clients
			.Where(x => x.ActiveMemberships.Any(x => x.MembershipPlan.CoachId == id))
			.Select(x =>
				new ClientModel()
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

	public async Task<IReadOnlyList<MembershipPlanModel>> GetMembershipPlansAsync(Guid id)
	{
		return await dbContext.MembershipPlans
			.Where(x => x.CoachId == id)
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

	public async Task<IReadOnlyList<SessionModel>> GetUpcomingSessionsAsync(Guid id, Guid? withClientId)
	{
		return await dbContext.Sessions
			.Where(x => ((withClientId == null) ? x.CoachId == id : x.CoachId == id && x.ClientId == withClientId) && x.Date >= DateTime.UtcNow)
			.OrderBy(x => x.Date)
			.Select(x =>
				new SessionModel()
				{
					Id = x.Id,
					CoachId = x.CoachId,
					ClientId = x.ClientId,
					Date = x.Date,
					Duration = x.Duration,
				}
			)
			.ToListAsync()
		;
	}

	public async Task<IReadOnlyList<WorkPeriodModel>> GetWorkPeriodsAsync(Guid id)
	{
		return await dbContext.WorkPeriods
			.Where(x => x.CoachId == id)
			.Select(x =>
				new WorkPeriodModel()
				{
					Id = x.Id,
					DayOfWeek = x.DayOfWeek,
					Start = x.Start,
					End = x.End,
					CoachId = id,
				}
			)
			.ToListAsync()
		;
	}

	public async Task AddCoachAsync(string userId)
	{
		await dbContext.Coaches.AddAsync(
			new()
			{
				UserId = userId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public CoachService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
