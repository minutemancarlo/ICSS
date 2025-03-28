﻿using ICSS.Server.Repository;
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

                foreach (var item in schedule)
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

                foreach (var item in schedule)
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

                foreach (var item in schedule)
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



    }
}
