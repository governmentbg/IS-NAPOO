using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Framework
{
    public interface IDataMigration
    {
        public int? OldId { get; set; }
        public string? MigrationNote{ get; set; }
    }
}
