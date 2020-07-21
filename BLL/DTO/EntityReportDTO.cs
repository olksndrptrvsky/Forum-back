using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class EntityReportDTO<TEntity>
    {
        public TEntity Entity { get; set; }
        public IEnumerable<ReportDTO> Reports { get; set; }
    }
}
