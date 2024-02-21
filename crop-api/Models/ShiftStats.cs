using CROP.API.Data;

namespace CROP.API.Models
{
    public record ShiftSum(string? StationId, DateTimeOffset StartTime, DateTimeOffset EndTime, int PlanCount, int CutCount, int CarCount, int WeightSum);
    public record ShiftDist(string? StationId, DateTimeOffset StartTime, DateTimeOffset EndTime, List<ShiftSum> Dist);
    public record ShiftList(string? StationId, DateTimeOffset StartTime, DateTimeOffset EndTime, List<ShiftData> List);
}
