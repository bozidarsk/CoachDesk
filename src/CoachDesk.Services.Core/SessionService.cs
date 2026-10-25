using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Services.Core.Contracts;
using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core;

public class SessionService : ISessionService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<SessionModel?> GetSessionAsync(Guid id)
	{
		var session = await dbContext.Sessions.SingleOrDefaultAsync(x => x.Id == id);

		if (session == null)
			return null;

		return new()
		{
			Id = session.Id,
			Date = session.Date,
			Duration = session.Duration,
			CoachId = session.CoachId,
			ClientId = session.ClientId,
		};
	}

	public async Task AddSessionAsync(DateTime date, TimeSpan duration, Guid coachId, Guid clientId)
	{
		await dbContext.Sessions.AddAsync(
			new()
			{
				Date = date,
				Duration = duration,
				CoachId = coachId,
				ClientId = clientId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteSessionsAsync(params Guid[] ids)
	{
		await dbContext.Sessions
			.Where(x => ids.Contains(x.Id))
			.ExecuteDeleteAsync()
		;
	}

	public SessionService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
