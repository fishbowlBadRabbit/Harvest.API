using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class UserAssignment : BaseModel
    {
        public Project Project { get; set; }
        public User User { get; set; }
        public bool IsActive { get; set; }
        public bool IsProjectManger { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal Budget { get; set; }
    }

    public class UserAssignmentsResponse : PagedList
    {
        public UserAssignment[] UserAssignments { get; set; }
    }
}
