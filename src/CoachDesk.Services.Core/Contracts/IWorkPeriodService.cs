using CoachDesk.Services.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IWorkPeriodService
{
	Task<WorkPeriodModel?> GetWorkPeriodAsync(Guid id);
	Task AddWorkPeriodAsync(DayOfWeek dayOfWeek, TimeOnly start, TimeOnly end, Guid coachId);
	Task DeleteWorkPeriodsAsync(params Guid[] ids);
}
