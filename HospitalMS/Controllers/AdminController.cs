using AutoMapper;
using HospitalMS.Data;
using HospitalMS.Models;
using HospitalMS.Repository;
using HospitalMS.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HospitalMS.Controllers
{
    public class AdminController : Controller
    {
        IAdminRepository AdminRepository;
        IDepartmentRepository DepartmentRepository;
        IDoctorRepository DoctorRepository;
        INurseRepository NurseRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IPatientRepository patientRepository;

        public AdminController(IAdminRepository adminRepository, IDepartmentRepository departmentRepository, 
            IDoctorRepository doctorRepository, INurseRepository nurseRepository, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager,IMapper mapper,SignInManager<ApplicationUser> signInManager,
            IPatientRepository patientRepository)
        {
            this.AdminRepository = adminRepository;
            this.DepartmentRepository = departmentRepository;
            DoctorRepository = doctorRepository;
            NurseRepository = nurseRepository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.patientRepository = patientRepository;
        }
        public IActionResult AdminPage()
        {
            return View("Index");
        }

        //<-------------------------------------------Doctor--------------------------------------------------------->

        public IActionResult AddDoctor()
        {
            AdminNurseViewModel viewModel = new AdminNurseViewModel();
            viewModel.DeptList = DepartmentRepository.GetAll();
            return View("AddDoctor", viewModel);
        }

       

        [HttpPost]
        public async Task<IActionResult> SaveDoctorData(Doctor doctor)
        {
            
            ModelState.Remove("Bookings");
            ModelState.Remove("Admins");
            ModelState.Remove("Nurses");
            ModelState.Remove("MedicalRecords");
            ModelState.Remove("Department");
            ModelState.Remove("DoctorNurse");
            if (ModelState.IsValid)
            {
                //Save to DB
                
                var Userapp = mapper.Map<ApplicationUser>(doctor);
                Userapp.Id = Guid.NewGuid().ToString();
                IdentityResult result = await userManager.CreateAsync(Userapp, doctor.Password);
                if (result.Succeeded)
                {
                    DoctorRepository.Add(doctor);
                    DoctorRepository.Save();
                    //add Role
                    IdentityResult result1 =await userManager.AddToRoleAsync(Userapp, "Doctor");
                    if (result1.Succeeded)
                    {
                      await signInManager.SignInAsync(Userapp,false);
                        
                        return View("Index");
                    }
                    foreach (var item in result1.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            ModelState.AddModelError("DepartmentId", "Select Department");
            var view = mapper.Map<AdminNurseViewModel>(doctor);
            view.DeptList = DepartmentRepository.GetAll();
            return View("AddDoctor", view);
        }
       
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = DoctorRepository.GetById(id);
            var viewModel = mapper.Map<AdminNurseViewModel>(doctor);
            viewModel.Id = doctor.Id;
            viewModel.CurrentImage = doctor.Imag;
            viewModel.DeptList = DepartmentRepository.GetAll();
            viewModel.userid = (await userManager.FindByNameAsync(doctor.Username)).Id;

            return View("EditDoctor", viewModel);

        }
        public async Task<IActionResult> SaveDoctorEdit(AdminNurseViewModel doctor)
        {
           
            if (ModelState.IsValid)
            {

                var MapedDoctor=mapper.Map<Doctor>(doctor);
                if (MapedDoctor.Imag == null) {
                    MapedDoctor.Imag = doctor.CurrentImage;
                }
                var user = await userManager.FindByIdAsync(doctor.userid);
                if (user != null) {
                    user.Id = doctor.userid;
                    user.FName = doctor.FName;
                    user.LName = doctor.LName;
                    user.Image = MapedDoctor.Imag;
                    user.BirthDate = doctor.BirthDate;
                    user.Email = doctor.Email;
                    user.PhoneNumber = doctor.Phone;
                    user.UserName = doctor.Username;
                    user.PasswordHash = doctor.Password;
                    user.Gender = doctor.Gender;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    
                    if (result.Succeeded)
                    {
                        DoctorRepository.Update(MapedDoctor);
                        DoctorRepository.Save();
                        return RedirectToAction("DoctorEditView");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            var viewModel = mapper.Map<AdminNurseViewModel>(doctor);
            ModelState.AddModelError("DepartmentId", "Select Department");
            viewModel.DeptList = DepartmentRepository.GetAll();
            return View("EditDoctor",viewModel);
        }



        public ActionResult DoctorEditView()
        {
            List<Doctor> doctorList = DoctorRepository.GetAll();
            return View("EditDoctor1", doctorList);
        }
        public IActionResult DeleteDoctor()
        {
            List<Doctor> doctorList = DoctorRepository.GetAll();
            return View("DeleteDoctor", doctorList);
        }
        public async Task<IActionResult> EnsureDoctorDelete(int id)
        {
            var doc = DoctorRepository.GetByIdWithMedicalRcord(id);
            if (id != 0)
            {
                var user= await userManager.FindByNameAsync(doc.Username);
                if (user != null && !doc.Bookings.Any(d=>d.DoctorId==id) )
                {
                    if (!doc.MedicalRecords.Any(d=>d.DoctorId==id))
                    {
                        await userManager.DeleteAsync(user);
                        DoctorRepository.Remove(id);
                        DoctorRepository.Save();
                        return RedirectToAction("DeleteDoctor");
                    }
                    ViewBag.errorMedical = "true";
                }                
            }
            if(doc.Bookings.Any()) { ViewBag.error = "true"; }
            ViewBag.doctorname = doc.FName+" "+doc.LName;
            List<Doctor> doctorList = DoctorRepository.GetAll();
            return View("DeleteDoctor", doctorList);
        }


        //<-------------------------------------------Nurse--------------------------------------------------------->

        public IActionResult AddNurse()
        {
            AdminNurseViewModel viewModel = new AdminNurseViewModel();
            viewModel.DeptList = DepartmentRepository.GetAll();
            return View("AddNurse", viewModel);
        }

        public async Task<IActionResult> SaveNurseData (Nurse nurse)

        {
            ModelState.Remove("Admins");
            ModelState.Remove("Doctors");
            ModelState.Remove("Patients");
            ModelState.Remove("Department");
            ModelState.Remove("DoctorNurse");

            if (ModelState.IsValid)
            {
                
                NurseRepository.Add(nurse);
                NurseRepository.Save();
                var Userapp = mapper.Map<ApplicationUser>(nurse);
                Userapp.Id = Guid.NewGuid().ToString(); 
                IdentityResult result = await userManager.CreateAsync(Userapp, nurse.Password);
                if (result.Succeeded)
                {
                    //add Role
                    IdentityResult result1 = await userManager.AddToRoleAsync(Userapp, "Nurse");
                    if (result1.Succeeded)
                    {
                        //add cookie
                        await signInManager.SignInAsync(Userapp, false);
                        var editid=await userManager.FindByNameAsync(nurse.Username);
                        
                        return View("Index");
                    }
                    foreach (var item in result1.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                
            }
            
                AdminNurseViewModel viewModel = new AdminNurseViewModel();
            ModelState.AddModelError("DepartmentId", "Select Department");
            viewModel.DeptList = DepartmentRepository.GetAll();
                viewModel.FName = nurse.FName;
                viewModel.LName = nurse.LName;
                viewModel.Salary = nurse.Salary;
                viewModel.Imag = nurse.Imag;
                viewModel.BirthDate = nurse.BirthDate;
                viewModel.Email = nurse.Email;
                viewModel.DepartmentId = nurse.DepartmentId;
                viewModel.Phone = nurse.Phone;
                viewModel.Username = nurse.Username;
                viewModel.Password = nurse.Password;
                viewModel.Gender = nurse.Gender;
                viewModel.DeptList = DepartmentRepository.GetAll();
                return View("AddNurse", viewModel);
         //_______________________________________________________________________________   
        }
         public async Task<IActionResult> EditNurse(int id)
        {
            var nurse = NurseRepository.GetById(id);
            var viewModel = mapper.Map<AdminNurseViewModel>(nurse);
            viewModel.Id = nurse.Id;
            viewModel.CurrentImage = nurse.Imag;
            viewModel.DeptList = DepartmentRepository.GetAll();
            viewModel.userid = (await userManager.FindByNameAsync(nurse.Username)).Id;

            return View("EditNurse", viewModel);

        }
        public async Task<IActionResult> SaveNurseEdit(AdminNurseViewModel nurse)
        {
           
            if (ModelState.IsValid)
            {

                var MapedNurse=mapper.Map<Nurse>(nurse);
                if (MapedNurse.Imag == null) {
                    MapedNurse.Imag = nurse.CurrentImage;
                }
                var user = await userManager.FindByIdAsync(nurse.userid);
                if (user != null) {
                    user.Id = nurse.userid;
                    user.FName = nurse.FName;
                    user.LName = nurse.LName;
                    user.Image = MapedNurse.Imag;
                    user.BirthDate = nurse.BirthDate;
                    user.Email = nurse.Email;
                    user.PhoneNumber = nurse.Phone;
                    user.UserName = nurse.Username;
                    user.PasswordHash = nurse.Password;
                    user.Gender = nurse.Gender;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    
                    if (result.Succeeded)
                    {
                        NurseRepository.Update(MapedNurse);
                        NurseRepository.Save();
                        return RedirectToAction("NurseEditView");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            var viewModel = mapper.Map<AdminNurseViewModel>(nurse);
            viewModel.DeptList = DepartmentRepository.GetAll();
            return View("EditNurse", viewModel);
        }
      
        public ActionResult NurseEditView()
        {
            List<Nurse> NurseList = NurseRepository.GetAll();
            return View("EditNurse1", NurseList);
        }

        public IActionResult DeleteNurse()
        {
            List<Nurse> nurseList = NurseRepository.GetAll();
            return View("DeleteNurse", nurseList);
        }


        
        public async Task<IActionResult> EnsureNurseDelete(int id)
        {

            if (id != 0)
            {
                var nurse = NurseRepository.GetById(id);
                var user = await userManager.FindByNameAsync(nurse.Username);
               
                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                    NurseRepository.RemoveById(id);
                    NurseRepository.Save();
                    List<Nurse> nurseList = NurseRepository.GetAll();
                    return View("DeleteNurse", nurseList);
                }
            }
            List<Nurse> nurses = NurseRepository.GetAll();
            return View("DeleteNurse",nurses);
        }


       

        //-------------------------------Validation---------------------------------------
        public async Task<ActionResult> CheckUserName(string UserName, int Id)
        {
            if (DoctorRepository.GetById(Id) == null || NurseRepository.GetById(Id) == null)
            {
                bool check  = DoctorRepository.SearchByUserName(UserName);
                bool check2 = NurseRepository.SearchByUserName(UserName);
                bool check3 = false;
                if( await userManager.FindByNameAsync(UserName)!=null)
                {
                     check3 = true;
                }
                
                return Json(!(check || check2 || check3));
            }
            return Json(false);

        }
        public ActionResult CustomBirthDateValidation(DateOnly BirthDate)
        {
            if (BirthDate.Year < 2005 && BirthDate.Year > 1960)
                return Json(true);
            else return Json(false);


        }
        public ActionResult ConfirmPassword(string ConfirmPassword, string Password)
        {

            if (!string.IsNullOrEmpty(ConfirmPassword))
            {
                if (ConfirmPassword.Equals(Password))
                    return Json(true);
            }
            return Json(false);
        }

        //---------------------------------------Patient------------------------------------------
        public ActionResult AllPatients()
        {
            List<Patient> patients = patientRepository.GetAll();
            return View("AllPatients", patients);
        }

    }
   


}

