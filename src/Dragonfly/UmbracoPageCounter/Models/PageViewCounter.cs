using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;


namespace Dragonfly.UmbracoPageCounter.Models
{
    [TableName(SqlTableName)]
    [PrimaryKey("NodeId", autoIncrement = false)]
    public class PageViewCounter
    {
        internal const string SqlTableName = "Dragonfly_PageViewCounter";
      
        [PrimaryKeyColumn(AutoIncrement = false)]
        public int NodeId { get; set; }

        [NullSetting(NullSetting = NullSettings.NotNull)]
        public int Counter { get; set; }

        public DateTime LastVisit { get; set; }
    }
}
