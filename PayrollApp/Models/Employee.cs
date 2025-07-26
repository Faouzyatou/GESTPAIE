using System;
using System.ComponentModel.DataAnnotations;

namespace PayrollApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string Function { get; set; }

        public string Position { get; set; }

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string Category { get; set; }

        public string Echelon { get; set; }
    }
}
