using HospitalMS.Models;
using HospitalMS.Repository;
using HospitalMS.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace HospitalMS.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly IAdminRepository adminRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IBookingRepository bookingRepository;
        private readonly INurseRepository nurseRepository;

        public SuperAdminController
            (IAdminRepository _adminRepository, IDepartmentRepository _departmentRepository,
            IDoctorRepository _doctorRepository, IBookingRepository _bookingRepository,
            INurseRepository _nurseRepository)
        {
            adminRepository = _adminRepository;
            departmentRepository = _departmentRepository;
            doctorRepository = _doctorRepository;
            bookingRepository = _bookingRepository;
            nurseRepository = _nurseRepository;
        }



        public IActionResult SuperAdminPage()
        {
            return View("SuperAdminPage");
        }


        //view all admins with their departments --
        public IActionResult ViewAllAdmins()
        {
            return View("ViewAllAdmins", adminRepository.GetAll());
        }



        //EditAdmin --
        public IActionResult EditAdmin(int AdminId)
        {
            ViewData["DeptList"] = departmentRepository.GetAll();
            Admin admin = adminRepository.GetById(AdminId);
            SuperAdminAddAdminViewModel ViewModel = new();
            ViewModel.Id= AdminId;
            ViewModel.FName=admin.FName;
            ViewModel.LName=admin.LName;
            ViewModel.Username = admin.Username;
            ViewModel.Password = admin.Password;
            ViewModel.BirthDate = admin.BirthDate;
            ViewModel.Email = admin.Email;
            ViewModel.Phone = admin.Phone;
            ViewModel.Image = admin.Imag;
            ViewModel.Gender = admin.Gender;
            return View("EditAdmin", ViewModel);
        }
        [HttpPost]
        public IActionResult SaveAdminEdit(SuperAdminAddAdminViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                if (ViewModel.DepartmentId != 0)
                {
                    Admin admin = new Admin();
                    admin.Id = ViewModel.Id;
                    admin.DepartmentId = ViewModel.DepartmentId;
                    admin.FName = ViewModel.FName;
                    admin.LName = ViewModel.LName;
                    admin.Username = ViewModel.Username;
                    admin.Password = ViewModel.Password;
                    admin.BirthDate = ViewModel.BirthDate;
                    admin.Email = ViewModel.Email;
                    admin.Phone = ViewModel.Phone;
                    admin.Imag = ViewModel.Image;
                    admin.Gender = ViewModel.Gender;
                    admin.SuperAdminId = 1;/////////

                    adminRepository.Update(admin);
                    adminRepository.Save();
                    return RedirectToAction("ViewAllAdmins");
                }

                ModelState.AddModelError("DepartmentId", "Please select a valid department.");
            }
            ViewData["DeptList"] = departmentRepository.GetAll();
            return View("EditAdmin", ViewModel);

        }


        //AddNewDepartment---

        public IActionResult AddNewDepartment()
        {
            return View("AddNewDepartment");
        }

        public IActionResult SaveNewDepartment(string Name, string Description)
        {
            Department department = new Department();
            department.Name = Name;
            department.Description = Description;
            if (ModelState.IsValid)
            {
                departmentRepository.Add(department);
                departmentRepository.Save();
                return RedirectToAction("ViewAllDepartments");
            }
            return View("AddNewDepartment", department);
        }


        //view all departments--
        public IActionResult ViewAllDepartments()
        {
            return View("ViewAllDepartments", departmentRepository.GetAll());
        }


        //EditDepartment--
        public IActionResult EditDepartment(int DeptId)
        {
            return View("EditDepartment", departmentRepository.GetById(DeptId));
        }

        public IActionResult SaveDepartmentEdit(Department department)
        {
            if (ModelState.IsValid)
            {
                departmentRepository.Update(department);
                departmentRepository.Save();
                return RedirectToAction("ViewAllDepartments");
            }
            return View("EditDepartment", department);
        }


        //view all doctors with their department--
        public ActionResult ViewAllDeptDoctors(int DeptId)
        {
            if (DeptId == 0)
            {
                return View("ViewAllDeptDoctors", doctorRepository.GetAll());
            }
            return View("ViewAllDeptDoctors", doctorRepository.GetListByDeptId(DeptId));
        }

        //view all nurses with their department--
        public ActionResult ViewAllDeptNurses(int DeptId)
        {
            if (DeptId == 0)
            {
                return View("ViewAllDeptNurses", nurseRepository.GetAll());
            }
            return View("ViewAllDeptNurses", nurseRepository.GetByDeptId(DeptId));
        }

        //view all recorded appointments--
        public ActionResult ViewDoctorBookedAppointments(int DocId)
        {
            return View("ViewDoctorBookedAppointments", bookingRepository.GetDocBookingListWithPatients(DocId));
        }
        

        //AddNewAdmin ---
        public IActionResult AddNewAdmin()
        {
            ViewBag.DeptList= departmentRepository.GetAll();

            return View("AddNewAdmin", new SuperAdminAddAdminViewModel());
        }
        int SuperAdminId = 1;

        [HttpPost]
        public IActionResult SaveAdmin(SuperAdminAddAdminViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                if (ViewModel.DepartmentId != 0)
                {
                    Admin admin= new Admin();
                    admin.DepartmentId = ViewModel.DepartmentId;
                    admin.FName = ViewModel.FName;
                    admin.LName = ViewModel.LName;
                    admin.Username = ViewModel.Username;
                    admin.Password = ViewModel.Password;
                    admin.BirthDate = ViewModel.BirthDate;
                    admin.Email = ViewModel.Email;
                    admin.Phone = ViewModel.Phone;
                    admin.Imag= ViewModel.Image;
                    admin.Gender = ViewModel.Gender;
                    admin.SuperAdminId= SuperAdminId;
                    adminRepository.Add(admin);
                    adminRepository.Save();
                    return RedirectToAction("ViewAllAdmins");
                }

                ModelState.AddModelError("DepartmentId", "Please select a valid department.");
            }

            ViewData["DeptList"] = departmentRepository.GetAll();
            return View("AddNewAdmin", ViewModel);

        }

        public ActionResult CheckUserName(string Username, int Id)
        {
            if (adminRepository.GetByUserNameAndId(Username, Id) == null)
                return Json(true);
            return Json(false);
        }
        public ActionResult CustomBirthDateValidation(DateOnly BirthDate)
        {
            if (BirthDate.Year < 2005 && BirthDate.Year > 1960)
                return Json(true);
            else return Json(false);
        }

    }
}
