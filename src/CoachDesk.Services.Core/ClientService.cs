using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Data.Models;
using CoachDesk.Services.Models;
using CoachDesk.Services.Core.Contracts;

namespace CoachDesk.Services.Core;

public class ClientService : IClientService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMembershipService membershipService;

	public async Task<ClientModel?> GetClientAsync(ApplicationUser user)
	{
		var client = await dbContext.Clients
			.SingleOrDefaultAsync(x => x.UserId == user.Id)
		;

		if (client == null)
			return null;

		return new()
		{
			Id = client.Id,
			FullName = user.FullName,
			MemberSince = user.MemberSince,
			Bio = user.Bio,
		};
	}

	public async Task<ClientModel?> GetClientAsync(Guid id)
	{
		var client = await dbContext.Clients
			.Include(x => x.User)
			.SingleOrDefaultAsync(x => x.Id == id)
		;

		if (client == null)
			return null;

		return new()
		{
			Id = client.Id,
			FullName = client.User?.FullName ?? "Deleted User",
			MemberSince = client.User?.MemberSince ?? default,
			Bio = client.User?.Bio,
		};
	}

	public async Task<IReadOnlyList<MembershipModel>> GetMembershipsAsync(Guid id)
	{
		return await dbContext.Memberships
			.Where(x => x.ClientId == id)
			.Select(x =>
				new MembershipModel()
				{
					Id = x.Id,
					ActivatedOn = x.ActivatedOn,
					TimesUsed = x.TimesUsed,
					MembershipPlanId = x.MembershipPlanId,
					ClientId = id,
					Status = membershipService.GetMembershipStatus(
						activatedOn: x.ActivatedOn,
						timesUsed: x.TimesUsed,
						duration: x.MembershipPlan.Duration,
						count: x.MembershipPlan.Count
					),
				}
			)
			.ToListAsync()
		;
	}

	public async Task<IReadOnlyList<SessionModel>> GetUpcomingSessionsAsync(Guid id, Guid? withCoachId)
	{
		return await dbContext.Sessions
			.Where(x => ((withCoachId == null) ? x.ClientId == id : x.ClientId == id && x.CoachId == withCoachId) && x.Date >= DateTime.UtcNow)
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

	public async Task AddClientAsync(string userId)
	{
		await dbContext.Clients.AddAsync(
			new()
			{
				UserId = userId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public ClientService(ApplicationDbContext dbContext, IMembershipService membershipService)
	{
		this.dbContext = dbContext;
		this.membershipService = membershipService;
	}
}
