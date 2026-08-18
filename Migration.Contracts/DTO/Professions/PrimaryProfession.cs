using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Professions
{
    /// <summary>
    /// Primary profession of employee
    /// </summary>
    public record PrimaryProfession
    {
        [Required]
        public string Column { get; init; }

        [Required]
        public DateTime HireDate { get; init; }

        public DateTime? FireDate { get; init; }
    }
}
