using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Services.Models;
using CoachDesk.Services.Core.Contracts;

namespace CoachDesk.Services.Core;

public class ClientService : IClientService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<CoachModel?> GetClientAsync(ApplicationUser user)
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

	public async Task<CoachModel?> GetClientAsync(Guid id)
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
			FullName = client.User.FullName,
			MemberSince = client.User.MemberSince,
			Bio = client.User.Bio,
		};
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

	public ClientService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
