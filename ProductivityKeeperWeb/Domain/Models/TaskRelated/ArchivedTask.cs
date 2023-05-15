using Microsoft.EntityFrameworkCore;
using System;

namespace ProductivityKeeperWeb.Domain.Models.TaskRelated
{
    // todo: refactor
    [Owned]
    public class ArchivedTask
    {
        public int Id { get; set; }
        public ArchievedTaskStatus Status { get; set; }
        public DateTime? DoneDate { get; set; }
        public DateTime? Deadline { get; set; }
    }

    public enum ArchievedTaskStatus
    {
        Undone,
        Done,
        Expired
    }
}
