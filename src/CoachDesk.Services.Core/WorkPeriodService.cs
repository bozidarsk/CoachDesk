using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Services.Models;
using CoachDesk.Services.Core.Contracts;

namespace CoachDesk.Services.Core;

public class WorkPeriodService : IWorkPeriodService
{
	private readonly ApplicationDbContext dbContext;

	public async Task<WorkPeriodModel?> GetWorkPeriodAsync(Guid id)
	{
		var workPeriod = await dbContext.WorkPeriods.SingleOrDefaultAsync(x => x.Id == id);

		if (workPeriod == null)
			return null;

		return new()
		{
			Id = workPeriod.Id,
			DayOfWeek = workPeriod.DayOfWeek,
			Start = workPeriod.Start,
			End = workPeriod.End,
			CoachId = workPeriod.CoachId,
		};
	}

	public async Task AddWorkPeriodAsync(DayOfWeek dayOfWeek, TimeOnly start, TimeOnly end, Guid coachId)
	{
		await dbContext.WorkPeriods.AddAsync(
			new()
			{
				DayOfWeek = dayOfWeek,
				Start = start,
				End = end,
				CoachId = coachId,
			}
		);

		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteWorkPeriodsAsync(params Guid[] ids)
	{
		await dbContext.WorkPeriods
			.Where(x => ids.Contains(x.Id))
			.ExecuteDeleteAsync()
		;
	}

	public WorkPeriodService(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
