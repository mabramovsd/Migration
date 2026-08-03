using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Professions
{
    /// <summary>
    /// Norm: how much resource is produced per unit of profession work time
    /// </summary>
    public record ProfessionResourceNormDTO
    {
        /// <summary>
        /// Company Alias
        /// </summary>
        [Required]
        public required string Company { get; init; }

        /// <summary>
        /// Profession title
        /// </summary>
        [Required]
        [MaxLength(50)]
        public required string Profession { get; init; }

        /// <summary>
        /// Resource title
        /// </summary>
        [Required]
        [MaxLength(50)]
        public required string Resource { get; init; }

        /// <summary>
        /// Hours of work
        /// </summary>
        [Required]
        public decimal Hours { get; init; }

        /// <summary>
        /// Quantity produced for the given hours
        /// </summary>
        [Required]
        public decimal QuantityProduced { get; init; }
    }
}
