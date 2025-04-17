using ICSS.Server.Repository;
using ICSS.Shared;
using System.Data;
using System.Linq;

namespace ICSS.Server.Services
{
    public class ScheduleService
    {
        private readonly ScheduleRepository _scheduleRepository;

        public ScheduleService(ScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public async Task<DataTable> GetScheduleSingleByIdAsync(int? scheduleId, int? departmentId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Section", typeof(string));
            dt.Columns.Add("SubjectCode", typeof(string));
            dt.Columns.Add("SubjectDescription", typeof(string));
            dt.Columns.Add("Units", typeof(decimal));
            dt.Columns.Add("LoadUnits", typeof(decimal));
            dt.Columns.Add("Schedule", typeof(string));
            dt.Columns.Add("Days", typeof(string));
            dt.Columns.Add("Room", typeof(string));
            dt.Columns.Add("Instructor", typeof(string));
            dt.Columns.Add("SchoolYear", typeof(string));
            dt.Columns.Add("College", typeof(string));

            var schedule = await _scheduleRepository.GetReportScheduleAsync(scheduleId, departmentId);

            if (schedule != null)
            {
                var sortedSchedule = schedule
            .OrderBy(item => GetDayOrder(item.Days)) // Sort by Days (M -> T -> W -> Th -> F -> Sat)
            .ThenBy(item => ParseFirstTime(item.Schedule)) // Sort by the first time in the range
            .ToList();
                foreach (var item in sortedSchedule)
                {
                    DataRow row = dt.NewRow();
                    row["Section"] = item.Section;
                    row["SubjectCode"] = item.SubjectCode;
                    row["SubjectDescription"] = item.SubjectDescription;
                    row["Units"] = item.Units;
                    row["LoadUnits"] = item.LoadUnits;
                    row["Schedule"] = item.Schedule;
                    row["Days"] = item.Days;
                    row["Room"] = item.Room;
                    row["Instructor"] = item.Instructor;
                    row["SchoolYear"] = item.SchoolYear;
                    row["College"] = item.College;

                    dt.Rows.Add(row);
                }



            }

            return dt;
        }

        public async Task<DataTable> GetScheduleByFacultyIdAsync(int? facultyId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Section", typeof(string));
            dt.Columns.Add("SubjectCode", typeof(string));
            dt.Columns.Add("SubjectDescription", typeof(string));
            dt.Columns.Add("Units", typeof(decimal));
            dt.Columns.Add("LoadUnits", typeof(decimal));
            dt.Columns.Add("Schedule", typeof(string));
            dt.Columns.Add("Days", typeof(string));
            dt.Columns.Add("Room", typeof(string));
            dt.Columns.Add("Instructor", typeof(string));
            dt.Columns.Add("SchoolYear", typeof(string));            

            var schedule = await _scheduleRepository.GetReportScheduleAsync(null, null,facultyId,null);

            if (schedule != null)
            {
                var sortedSchedule = schedule
           .OrderBy(item => GetDayOrder(item.Days)) // Sort by Days (M -> T -> W -> Th -> F -> Sat)
           .ThenBy(item => ParseFirstTime(item.Schedule)) // Sort by the first time in the range
           .ToList();
                foreach (var item in sortedSchedule)
                {
                    DataRow row = dt.NewRow();
                    row["Section"] = item.Section;
                    row["SubjectCode"] = item.SubjectCode;
                    row["SubjectDescription"] = item.SubjectDescription;
                    row["Units"] = item.Units;
                    row["LoadUnits"] = item.LoadUnits;
                    row["Schedule"] = item.Schedule;
                    row["Days"] = item.Days;
                    row["Room"] = item.Room;
                    row["Instructor"] = item.Instructor;
                    row["SchoolYear"] = item.SchoolYear;
                    

                    dt.Rows.Add(row);
                }



            }

            return dt;
        }


        public async Task<DataTable> GetScheduleByRoomIdAsync(int? roomId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Section", typeof(string));
            dt.Columns.Add("SubjectCode", typeof(string));
            dt.Columns.Add("SubjectDescription", typeof(string));
            dt.Columns.Add("Units", typeof(decimal));
            dt.Columns.Add("LoadUnits", typeof(decimal));
            dt.Columns.Add("Schedule", typeof(string));
            dt.Columns.Add("Days", typeof(string));
            dt.Columns.Add("Room", typeof(string));
            dt.Columns.Add("Instructor", typeof(string));
            dt.Columns.Add("SchoolYear", typeof(string));

            var schedule = await _scheduleRepository.GetReportScheduleAsync(null, null, null, roomId);

            if (schedule != null)
            {
                var sortedSchedule = schedule
           .OrderBy(item => GetDayOrder(item.Days)) // Sort by Days (M -> T -> W -> Th -> F -> Sat)
           .ThenBy(item => ParseFirstTime(item.Schedule)) // Sort by the first time in the range
           .ToList();
                foreach (var item in sortedSchedule)
                {
                    DataRow row = dt.NewRow();
                    row["Section"] = item.Section;
                    row["SubjectCode"] = item.SubjectCode;
                    row["SubjectDescription"] = item.SubjectDescription;
                    row["Units"] = item.Units;
                    row["LoadUnits"] = item.LoadUnits;
                    row["Schedule"] = item.Schedule;
                    row["Days"] = item.Days;
                    row["Room"] = item.Room;
                    row["Instructor"] = item.Instructor;
                    row["SchoolYear"] = item.SchoolYear;


                    dt.Rows.Add(row);
                }



            }

            return dt;
        }


        // Helper method to map day names to sorting order
        private int GetDayOrder(string day)
        {
            return day switch
            {
                "M" => 1,
                "T" => 2,
                "W" => 3,
                "Th" => 4,
                "F" => 5,
                "Sat" => 6,
                _ => 7  // Unknown days go last
            };
        }

        // Helper method to parse time from Schedule column
        private TimeSpan ParseFirstTime(string schedule)
        {
            if (!string.IsNullOrWhiteSpace(schedule))
            {
                var firstTimeString = schedule.Split('-').FirstOrDefault()?.Trim(); // Extract the first time
                if (DateTime.TryParse(firstTimeString, out DateTime time))
                {
                    return time.TimeOfDay; // Convert to TimeSpan for sorting
                }
            }
            return TimeSpan.MaxValue; // If parsing fails, place it at the end
        }



    }
}
