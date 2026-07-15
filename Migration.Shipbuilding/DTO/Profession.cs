namespace Migration.Shipbuilding.DTO
{
    public class Profession
    {
        /// <summary>
        /// Profession Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Profession title
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Column in table <see cref="EmployeeShipbuilding"/>
        /// </summary>
        public required string Column { get; set; }
    }
}
