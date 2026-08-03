using System.ComponentModel.DataAnnotations;

namespace Migration.Contracts.DTO.Resources
{
    /// <summary>
    /// Forecast: how much resource will be produced over a period
    /// </summary>
    public record ResourceForecastDTO
    {
        /// <summary>
        /// Company Alias
        /// </summary>
        [Required]
        public required string Company { get; init; }

        /// <summary>
        /// Resource title
        /// </summary>
        [Required]
        [MaxLength(50)]
        public required string Resource { get; init; }

        /// <summary>
        /// Current resource amount on warehouse
        /// </summary>
        [Required]
        public decimal CurrentAmount { get; init; }

        /// <summary>
        /// Unit of measurement
        /// </summary>
        [Required]
        [MaxLength(20)]
        public required string Unit { get; init; }

        /// <summary>
        /// Forecast period in days
        /// </summary>
        [Required]
        public int Days { get; init; }

        /// <summary>
        /// Forecasted produced amount (only the increment)
        /// </summary>
        [Required]
        public decimal ProducedAmount { get; init; }

        /// <summary>
        /// Total amount after the period (current + produced)
        /// </summary>
        [Required]
        public decimal TotalAmount { get; init; }
    }
}
