﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Harvest.Api
{
    public class TaskAssignment : BaseModel
    {
        public bool Billable { get; set; }
        public bool IsActive { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? Budget { get; set; }

        public IdNameModel Task { get; set; }
        public IdNameCodeModel Project { get; set; }
    }

    public class TaskAssignmentsResponse : PagedList
    {
        public TaskAssignment[] TaskAssignments { get; set; }
    }
}
