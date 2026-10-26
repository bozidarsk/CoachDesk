namespace CoachDesk.ViewModels.Search;

public class SearchCoachesInputModel
{
	public string? Name { set; get; }

	public decimal? MinPrice { set; get; }
	public decimal? MaxPrice { set; get; }
	public int? MinDurationDays { set; get; }
	public int? MinSessions { set; get; }

	public List<DayOfWeek> Days { set; get; } = [];
	public TimeOnly? AvailableFrom { set; get; }
	public TimeOnly? AvailableTo { set; get; }

	public int Page { set; get; } = 1;

	public IReadOnlyList<SearchCoachesViewModel> Results { set; get; } = [];
	public int TotalResults { set; get; }
	public int TotalPages { set; get; }
}
