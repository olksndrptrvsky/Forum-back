using System.Collections.Generic;

namespace BLL.DTO
{
    public class EntityReportDTO<TEntity>
    {
        public TEntity Entity { get; set; }
        public IEnumerable<ReportDTO> Reports { get; set; }
    }
}
