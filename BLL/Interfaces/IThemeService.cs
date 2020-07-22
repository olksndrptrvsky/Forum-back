using BLL.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IThemeService
    {
        IEnumerable<ThemeListItemDTO> GetPopularThemes(int pagingNumber, int pagingSize);
        IEnumerable<ThemeListItemDTO> GetLatestThemes(int pagingNumber, int pagingSize);

        ThemeDTO GetThemeById(int id);

        Task<ThemeDTO> CreateAsync(ThemeDTO themeDTO, int authorId);


        IEnumerable<ThemeListItemDTO> GetThemesWithoutModers(int pagingNumber, int pagingSize);

        Task ReportThemeAsync(ReportDTO report);

        Task AddModerToThemeAsync(ThemeModerDTO themeModerDTO);

        Task DeleteThemeAsync(int id);

        bool UserCanDeleteTheme(int userId, int themeId);


        IEnumerable<ThemeListItemDTO> SearchThemes(string search, int pagingNumber, int pagingSize);

        bool IsThemeExist(int themeId);

        bool UserIsAuthor(int themeId, int userId);

        Task UpdateAsync(int id, ThemeDTO themeDTO);


        int GetUnmoderatedThemeCount();


        IEnumerable<EntityReportDTO<ThemeListItemDTO>> GetReportedThemesWithReports(int moderId);

        bool IsModeratingThemeReport(int moderId, int reportId);

        Task<ReportDTO> CheckReportAsync(int reportId);

    }
}
