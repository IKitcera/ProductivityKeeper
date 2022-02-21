using ProductivityKeeperModels.TaskRelated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProductivityKeeperModels
{
    public class UserStatistic
    {
        public int Id { get; set; }
        public Dictionary<DateTime, float> DonePerDay = new Dictionary<DateTime, float>();
        public float PercentOfDoneToday { get; set; }
        public float PercentOfDoneTotal { get; set; }
        public int CountOfDoneToday { get; set; }
        public int CountOfDoneTotal { get; set; }
        public int CountOfExpiredTotal { get; set; }

        public void CalculateCommonStatistic(User user)
        {
            List<Task> AllTasks = new List<Task>();

            //user.Categories.ForEach(c => c.Subcategories.ForEach(s =>
            //{
            //    s.Tasks.ForEach(t => AllTasks.Add(t));

            //    if (s.Name == "Today")
            //        PercentOfDoneToday = s.Tasks.Where(t => t.IsChecked).Count() / s.Tasks.Count;
            //}));

            //PercentOfDoneTotal = AllTasks.Where(t => t.IsChecked).Count() / AllTasks.Count;
            //CountOfDoneToday = AllTasks.Where(t => t.DoneDate.Date == DateTime.Now.Date).Count();
            //CountOfExpiredTotal = AllTasks.Where(t => t.DoneDate > t.Deadline || (DateTime.Now > t.Deadline && !t.IsChecked)).Count();
            //CountOfDoneTotal = AllTasks.Where(t => t.IsChecked).Count();

            //var grouppedByDoneDate = AllTasks.GroupBy(task => task.DoneDate).ToDictionary(x => x.Key, x => x.ToList());

            //DonePerDay.Clear();

            //foreach(var date in grouppedByDoneDate.Keys)
            //{
            //    var tasksOnToday = AllTasks.Where(t => t.Deadline == date).Count();
            //    var doneToday = grouppedByDoneDate[date].Count;
            //    DonePerDay.Add(date, doneToday / tasksOnToday);
            //}
        }
    }
}
