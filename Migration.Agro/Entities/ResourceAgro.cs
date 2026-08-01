namespace Migration.Agro.Entities
{
    public class ResourceAgro
    {
        /// <summary>
        /// Resource Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Resource title/name
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Resource count/amount
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// Unit of measurement (kg, units, etc.)
        /// </summary>
        public required string Unit { get; set; }
    }
}
