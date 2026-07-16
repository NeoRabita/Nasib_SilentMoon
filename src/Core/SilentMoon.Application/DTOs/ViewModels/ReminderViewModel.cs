using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilentMoon.Application.DTOs.ViewModels
{
    public class ReminderViewModel
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public int Days { get; set; }
        public string[] DayNames { get; set; } 
        public bool IsActive { get; set; }
    }
}
