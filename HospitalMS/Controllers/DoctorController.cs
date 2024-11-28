using HospitalMS.Data;
using HospitalMS.Models;
using HospitalMS.Repository;
using HospitalMS.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
namespace HospitalMS.Controllers
{
    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public class DoctorController : Controller
    {

        IPatientRepository patientRepository;
        IDoctorRepository DoctorRepository;
        IMedicalRecordRepository MedicalRecordRepository;
        IDepartmentRepository departmentRepository;
        IBookingRepository bookingRepository;
        public DoctorController(IDoctorRepository DoctorRepository, IPatientRepository patientRepository, IMedicalRecordRepository MedicalRecordRepository, IDepartmentRepository departmentRepository, IBookingRepository bookingRepository)
        {
            this.DoctorRepository = DoctorRepository;
            this.patientRepository = patientRepository;
            this.MedicalRecordRepository = MedicalRecordRepository;
            this.departmentRepository = departmentRepository;
            this.bookingRepository = bookingRepository;
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        //<-------------------------------------------------Add-------------------------------------------------->
public IActionResult AddMedicalRecord(int DocId)
{
    DocId = 4;
    MedicalRecordWithPatientIdList viewModel = new MedicalRecordWithPatientIdList();
    viewModel.patientlist = patientRepository.GetAllPatientByDocId(DocId);
    viewModel.DoctorId = DocId;
    return View("AddPatientMedicalRecord", viewModel);
}

public IActionResult AddPatientMedicalRecord(MedicalRecord record)
{
    ModelState.Remove("Doctor");
    ModelState.Remove("Patient");

    if (ModelState.IsValid)
    {
        MedicalRecordRepository.Add(record);
        MedicalRecordRepository.Save();
        return View("Index");
    }
   
        MedicalRecordWithPatientIdList viewModel = new MedicalRecordWithPatientIdList();
        viewModel.Id = record.Id;
        viewModel.DoctorId = record.DoctorId;
        viewModel.Date = record.Date;
        viewModel.PatientId = record.PatientId;
        viewModel.Note = record.Note;
        viewModel.patientlist = patientRepository.GetAllPatientByDocId(record.DoctorId);
        return View ("AddPatientMedicalRecord",viewModel);
    
}
       


        //--------------------------------------Delete---------------------------------------------------------/

        public IActionResult DeleteMedicalRecord(int DocId)
        {
            DocId = 4;
            MedicalRecordWithPatientIdList medical = new MedicalRecordWithPatientIdList();
            medical.patientlist = patientRepository.GetAllPatientByDocId(DocId) ?? new List<Patient>();
            medical.MedicalRecords = MedicalRecordRepository.GetMedicalRecordsByDocId(DocId) ?? new List<MedicalRecord>();
            return View("DeletePatientMedicalRecord", medical);
        }

        public IActionResult DeletePatientMedicalRecord(int id, int medicalrecordId)
        {
            MedicalRecordWithPatientIdList medical = new MedicalRecordWithPatientIdList();
            List<MedicalRecord> medicallist = MedicalRecordRepository.GetListByMedicalRecordId(medicalrecordId);
            medical.MedicalRecords = medicallist;

            var obj = MedicalRecordRepository.GetById(id);
            MedicalRecordRepository.Delete(obj);
            MedicalRecordRepository.Save();
            return RedirectToAction("Index");
        }
        //--------------------------------------ViewAppointment--------------------------------------------------------/
        public IActionResult ViewAppointment(int DocId)
        {
            DocId = 4;
            List<Booking> BookingLst = bookingRepository.GetBookingListByDocId(DocId);
            return View("ViewPatientAppointment", BookingLst);
        }

        //--------------------------------------------------Edit Profile-----------------------------------------------------------/

        public IActionResult DoctorEditView(int DocId)
        {
            DocId = 4;
            return View("EditDoctor", DoctorRepository.GetById(DocId));
        }


        public ActionResult SaveEdit(Doctor Doc)
        {
            if (ModelState.IsValid)
            {
                DoctorRepository.Update(Doc);
                DoctorRepository.Save();
                return RedirectToAction("Index");
            }
            return View("EditDoctor", Doc);
        }

    }
}