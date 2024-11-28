using HospitalMS.Data;
using HospitalMS.Models;
using HospitalMS.Repository;
using HospitalMS.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalMS.Controllers
{
    public class PatientController : Controller
    {

        private IPatientRepository patientRepository;
        private IDepartmentRepository departmentRepository;
        private IDoctorRepository doctorRepository;
        private IBookingRepository bookingRepository;

        public PatientController(IPatientRepository _patientRepository, IDepartmentRepository _departmentRepository, IDoctorRepository _doctorRepository, IBookingRepository _bookingRepository)
        {
            patientRepository = _patientRepository;
            departmentRepository = _departmentRepository;
            doctorRepository = _doctorRepository;
            bookingRepository = _bookingRepository;
        }


        private readonly int PID = 1;
        public IActionResult PatientProfile(int PatientId)
        {
            PatientId = PID;////////
            return View("PatientProfile", patientRepository.GetById(PatientId));
        }

        public IActionResult ShowDepartments()
        {
            return View("ShowDepartments", departmentRepository.GetAll());
        }

        public IActionResult ShowDepartmentDoctors(int DeptId)
        {
            return View("ShowDepartmentDoctors", doctorRepository.GetListByDeptId(DeptId));
        }

        public IActionResult ShowAllDoctors()
        {
            return View("ShowAllDoctors", doctorRepository.GetAll()); ;
        }

        public IActionResult BookAnAppoinment(int DocId)
        {
            ViewData["BookingLst"] = bookingRepository.GetDoctorBookingList(DocId);
            ViewData["DocId"] = DocId;
            return View("BookAnAppoinment");
        }

        public IActionResult SaveAppointment(DocDateTimeViewModel booking)
        {
            TimeOnly Time = TimeOnly.FromDateTime(booking.DateTimeAppointment);
            DateOnly Date = DateOnly.FromDateTime(booking.DateTimeAppointment);

            Booking appointment= bookingRepository.GetAppointment(booking.DocId, Date, Time);

            if (appointment != null)
            {
                appointment.PatientId = PID;//////
                bookingRepository.Save();
            }

            return RedirectToAction("ShowAppointments","Patient");
        }

        public IActionResult ShowAppointments(int PatientId)
        {
            PatientId = PID;////////
            return View("ShowAppointments", bookingRepository.GetPatientAppointmentList(PatientId));
        }

        public IActionResult CancelAppointment(int DocId, DateOnly dateOnly, TimeOnly timeOnly)
        {
            Booking? CancelAppointment = bookingRepository.GetAppointment(DocId, dateOnly, timeOnly);
            if (CancelAppointment != null)
            {
                CancelAppointment.PatientId = null;
                bookingRepository.Save();
            }
            return RedirectToAction("ShowAppointments", "Patient");
        }
    }
}