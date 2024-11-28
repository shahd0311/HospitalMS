using HospitalMS.Models;
using System.ComponentModel.DataAnnotations;

namespace HospitalMS.ViewModel
{
    public class AdminAddNurse2ViewModel
    {
        [Key]
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int Salary { get; set; }
        [Required]
        public string? Imag { get; set; }
        [Required(ErrorMessage ="Select Doctor Please ..")]
        public int DoctorId { get; set; }
    }
}
