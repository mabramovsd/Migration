namespace Migration.Shipbuilding.DTO
{
    public class EmployeeShipbuilding
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Can be a carpenter
        /// </summary>
        public bool CanCarpentry { get; set; }
        /// <summary>
        /// Can design a ship
        /// </summary>
        public bool CanDesignShip { get; set; }
        /// <summary>
        /// Can weld
        /// </summary>
        public bool CanWeld { get; set; }
    }
}
