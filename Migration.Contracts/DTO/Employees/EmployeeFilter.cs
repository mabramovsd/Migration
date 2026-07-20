using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Employees
{
    /// <summary>
    /// Filter for employees
    /// </summary>
    public record EmployeeFilter
    {
        /// <summary>
        /// Company
        /// </summary>
        [FromQuery]
        [Display(Name = "Company")]
        public string? Company { get; init; }

        /// <summary>
        /// Profession
        /// </summary>
        [FromQuery]
        [Display(Name = "Profession")]
        public string? Profession { get; init; }
    }
}
