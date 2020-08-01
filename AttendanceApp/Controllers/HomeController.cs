using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceApp.DAL;
using AttendanceApp.Models;
using Newtonsoft.Json;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Globalization;

namespace AttendanceApp.Controllers
{
	public class HomeController : Controller
	{
		private EmployeeDBContext db = new EmployeeDBContext();
		[Authorize]
		public ActionResult Index(AttendanceViewModel vm)
		{
            //IEnumerable emp = db.Employee.Where(obj => obj.Email == "ahmed@ahmed.com");
            //var result = db.Employee
            //   .SingleOrDefault(c => c.Email == "ahmed@ahmed.com");
            //ViewBag.Message = result.Email;






            ////// look if user coming or not
            Employee userinfo = JsonConvert.DeserializeObject<Employee>(User.Identity.Name);
            DateTime dt = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todayDate = dt;
            bool AttendanceValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID);
			
			if (AttendanceValid == true)
			{
				vm.iscoming = true;
				bool AttendanceAllValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID && c.LeaveTime != null);
				if (AttendanceAllValid) {
					vm.isLeave = true;
				}

			}
			else {
				vm.iscoming = false;
			}

		


			return View(vm);


		}

		[HttpPost]
		[Authorize]
		public ActionResult Index(Attendance attendance , AttendanceViewModel vm)
		{
			Employee userinfo = JsonConvert.DeserializeObject<Employee>(User.Identity.Name);


            DateTime dt = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime myDateTime = dt;

            DateTime dt2 = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todayDate = dt2;



			/////////////////////////////implement

			bool AttendanceValid = db.Attendance.Any(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID);

			if (AttendanceValid)
			{
				var attendanceRow = db.Attendance.Where(c => c.DateOfDay == todayDate && c.EmployeeID == userinfo.ID).Single();
				vm.iscoming = true;
				vm.isLeave = true;
				attendanceRow.LeaveTime = myDateTime;


				db.Entry(attendanceRow).State = EntityState.Modified;
				db.SaveChanges();

				return View(vm);

			}
			else {
				int userID = userinfo.ID;

				DateTime now = DateTime.Now;
				vm.isLeave = false;
                DateTime dt3 = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                Attendance Attendance = new Attendance
				{
					EmployeeID = userID,
					ComingTime = myDateTime,
                    DateOfDay = dt3,
				
				};
				
				db.Attendance.Add(Attendance);
				db.SaveChanges();
				vm.iscoming = true;
				return View(vm);

			}




			
		}


		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}