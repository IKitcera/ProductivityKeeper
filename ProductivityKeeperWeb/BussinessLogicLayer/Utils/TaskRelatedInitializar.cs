using ProductivityKeeperWeb.Models.TaskRelated;
using System.Collections.Generic;
using System.Linq;

namespace ProductivityKeeperWeb.BussinessLogicLayer.Utils
{
    public static class TaskRelatedInitializar
    {
        public static Unit FillUnitForNewcommer(Unit unit)
        {
            var timeCtg = new Category
            {
                Name = "Time",
                IsVisible = true,
                Position = 1,
                Subcategories = new List<Subcategory> {
                new Subcategory { Name = "Today", Position=0},
                new Subcategory { Name = "Tomorrow", Position = 1},
                new Subcategory {Name = "Week", Position=2},
                new Subcategory {Name = "Month", Position=3},
                new Subcategory {Name = "Year", Position=4}
            }
            };
            var aspectCtg = new Category
            {
                Name = "Aspects",
                IsVisible = true,
                Position = 0,
                Subcategories = new List<Subcategory> {
                    new Subcategory { Name = "Work", Position = 0, Tasks = new List<TaskItem> {
                        new TaskItem { Text = "Finish work" }
                    }
                    },
                    new Subcategory { Name = "Personal development", Position = 1, Tasks = new List<TaskItem>
                    {
                        new TaskItem { Text = "Read 30 pages of the book" }
                    }
                    },

                    new Subcategory { Name = "Home", Position = 2 },
                    new Subcategory { Name = "Relax", Position = 3, Tasks = new List<TaskItem>
                    {
                        new TaskItem { Text = "Be happy", IsChecked = true }
                    }
                    }
                }
            };

            timeCtg = FillCategory(timeCtg, unit.Categories);
            aspectCtg = FillCategory(aspectCtg, unit.Categories);

            unit.Categories.Add(aspectCtg);
            unit.Categories.Add(timeCtg);

            unit.Categories.ForEach(c =>
            {
                c.Subcategories.ForEach(s =>
                {
                    s = FillSubcategory(s, c.Subcategories);
                    s.Tasks.ForEach(t =>
                    {
                        t = FillTask(t);
                    });
                });
            });


            unit.Timer.Label = "Learn English";
            unit.Timer.Goal = 92 * 24 * 3600;
            unit.Timer.Ticked = 0;

            return unit;
        }

        public static Category FillCategory(Category category, IEnumerable<Category> allCategories)
        {
            if (string.IsNullOrEmpty(category.Name))
            {
                var lastGeneratedItem = allCategories.LastOrDefault(ctg => ctg.Name.Contains(nameof(Category)));
                var lastGeneratedName = lastGeneratedItem != null ? lastGeneratedItem.Name : $"{nameof(Category)} 0";
                string newGeneratedName = nameof(Category) + " " + (int.Parse(lastGeneratedName.Substring((nameof(Category).Length))) + 1).ToString();
                category.Name = newGeneratedName;
            }
            category.ColorHex ??= ColorUtil.GenerateColorHex();

            category.IsVisible = true;
            category.DateOfCreation = System.DateTime.Now;

            return category;
        }

        public static Subcategory FillSubcategory(Subcategory subcategory, IEnumerable<Subcategory> allSubcategoriesOfTheCategory)
        {
            if (string.IsNullOrEmpty(subcategory.Name))
            {
                var lastGeneratedItem = allSubcategoriesOfTheCategory.LastOrDefault(sub => sub.Name.Contains(nameof(Subcategory)));
                var lastGeneratedName = lastGeneratedItem != null ? lastGeneratedItem.Name : $"{nameof(Subcategory)} 0";
                string newGeneratedName = nameof(Subcategory) + " " + (int.Parse(lastGeneratedName.Substring((nameof(Subcategory).Length))) + 1).ToString();
                subcategory.Name = newGeneratedName;
            }
            subcategory.ColorHex ??= ColorUtil.GenerateColorHex();
            subcategory.DateOfCreation = System.DateTime.Now;

            return subcategory;
        }


        public static TaskItem FillTask(TaskItem task)
        {
            if (string.IsNullOrEmpty(task.Text))
                task.Text = "New task";

            task.DateOfCreation = System.DateTime.Now;
            return task;
        }
    }
}
