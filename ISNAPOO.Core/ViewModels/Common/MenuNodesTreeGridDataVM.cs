using System;
namespace ISNAPOO.Core.ViewModels.Common
{
    public class MenuNodesTreeGridDataVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ApplicationRole { get; set; }

        public int? ParentId { get; set; }

        public int EntityId { get; set; }

        public string EntityType { get; set; }
    }
}

