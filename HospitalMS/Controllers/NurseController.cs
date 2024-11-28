using AutoMapper;
using HospitalMS.AutoMapper;
using HospitalMS.Data;
using HospitalMS.Models;
using HospitalMS.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace HospitalMS.Controllers
{
    public class NurseController : Controller
    {

        IDoctorNurseRepository MapedNurseNurseRepository;
        IDepartmentRepository DepartmentRepository;
        IDoctorRepository DoctorRepository;
        INurseRepository NurseRepository;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        IBookingRepository bookingRepository;
        IMedicalRecordRepository MedicalRecordRepository;
        public NurseController(IMedicalRecordRepository _MedicalRecordRepository,
            IBookingRepository _bookingRepository, IDoctorNurseRepository _MapedNurseNurseRepository,
            IDepartmentRepository departmentRepository, IDoctorRepository MapedNurseRepository,
            INurseRepository nurseRepository, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            MedicalRecordRepository = _MedicalRecordRepository;
            bookingRepository = _bookingRepository;
            MapedNurseNurseRepository = _MapedNurseNurseRepository;
            this.DepartmentRepository = departmentRepository;
            DoctorRepository = MapedNurseRepository;
            NurseRepository = nurseRepository;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        int Nid = 1;


        public IActionResult Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> ViewAppointment(int NrsId)
        {
            NrsId = Nid;
            List<BookingNurseViewModel>? bookings = await bookingRepository
                .GetDepartmentAppointments(NurseRepository.GetById(Nid).DepartmentId);
            return View("ViewPatientAppointment", bookings);
        }

        public async Task<IActionResult> ViewPatientMedicalRecord(int NrsId)
        {
            NrsId = Nid;
            List<MedicalRecordNurseViewModel>? bookings = await bookingRepository
                .GetDepartmenMedicalRecord(NurseRepository.GetById(Nid).DepartmentId);
            return View("ViewPatientMedicalRecord", bookings );
        }

       
        public async Task<IActionResult> NurseEditView(int NrsId)
        {
            NrsId = Nid;
            Nurse nurse = NurseRepository.GetById(NrsId);
            var nurseModel = mapper.Map<AdminNurseViewModel>(nurse);
            nurseModel.CurrentImage = nurse.Imag;
            nurseModel.userid = (await userManager.FindByNameAsync(nurse.Username)).Id;
            return View("Edit", nurseModel);
        }


        public async Task<ActionResult> SaveEdit(AdminNurseViewModel Nrs)
        {
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                var MapedNurse = mapper.Map<Nurse>(Nrs);
                if (MapedNurse.Imag == null)
                {
                    MapedNurse.Imag = Nrs.CurrentImage;
                }
                var user = await userManager.FindByIdAsync(Nrs.userid);
                if (user != null)
                {
                    user.Id = Nrs.userid;
                    user.FName = MapedNurse.FName;
                    user.LName = MapedNurse.LName;
                    user.Image = MapedNurse.Imag;
                    user.BirthDate = MapedNurse.BirthDate;
                    user.Email = MapedNurse.Email;
                    user.PhoneNumber = MapedNurse.Phone;
                    user.UserName = MapedNurse.Username;
                    user.PasswordHash = MapedNurse.Password;
                    user.Gender = MapedNurse.Gender;
                    IdentityResult result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        NurseRepository.Update(MapedNurse);
                        NurseRepository.Save();
                        return View("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            var viewModel = mapper.Map<AdminNurseViewModel>(Nrs);
            return View("Edit", viewModel);
        }


    }
}