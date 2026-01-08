using Microsoft.EntityFrameworkCore;
using SubmiSoonProject.Data;
using SubmiSoonProject.DTOs.Common;
using SubmiSoonProject.DTOs.Leaderboard;
using SubmiSoonProject.Models;
using SubmiSoonProject.Repositories;

namespace SubmiSoonProject.Services
{
    public interface ILeaderboardService
    {
        Task<ApiResponse<List<LeaderboardEntryDto>>> GetLeaderboardAsync(int page, int size, string? sortBy, string? sortOrder);
    }

    public class LeaderboardService : ILeaderboardService
    {
        private readonly IStudentRepository _studentRepository;

        public LeaderboardService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<ApiResponse<List<LeaderboardEntryDto>>> GetLeaderboardAsync(
            int page, int size, string? sortBy, string? sortOrder)
        {
            if (!string.IsNullOrEmpty(sortBy))
            {
                var validFields = new[] { "name", "assessment" };
                if (!validFields.Contains(sortBy.ToLower()))
                {
                    throw new ArgumentException(
                        $"Invalid sort_by '{sortBy}'. Valid options: {string.Join(", ", validFields)}");
                }
            }
            var leaderboard = _studentRepository.GetStudentsLeaderboardAsync().Result;

            // apply sorting
            leaderboard = ApplySorting(leaderboard, sortBy, sortOrder);

            // calculate pagination
            var totalItem = leaderboard.Count;
            var totalPage = (int)Math.Ceiling(totalItem / (double)size);

            // apply pagination
            var paginatedData = leaderboard
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return new ApiResponse<List<LeaderboardEntryDto>>
            {
                Data = paginatedData,
                Paging = new PagingDto
                {
                    Page = page,
                    Size = size,
                    TotalItem = totalItem,
                    TotalPage = totalPage
                },
                Success = true
            };
        }

        private List<LeaderboardEntryDto> ApplySorting(List<LeaderboardEntryDto> leaderboard, string? sortBy, string? sortOrder)
        {
            var isDescending = sortOrder?.ToLower() == "desc";

            return (sortBy?.ToLower()) switch
            {
                "name" => isDescending
                    ? leaderboard.OrderByDescending(l => l.Name).ToList()
                    : leaderboard.OrderBy(l => l.Name).ToList(),
                "assessment" => isDescending
                    ? leaderboard.OrderByDescending(l => l.TotalAssessmentsDone).ThenBy(l => l.Name).ToList()
                    : leaderboard.OrderBy(l => l.TotalAssessmentsDone).ThenBy(l => l.Name).ToList(),
                _ => leaderboard.OrderByDescending(l => l.TotalAssessmentsDone).ThenBy(l => l.Name).ToList() // default: by assessment - descending
            };
        }
    }
}
