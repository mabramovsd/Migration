using Migration.Contracts.DTO.Professions;

namespace Migration.Shipbuilding.Entities
{
    public class EmployeeShipbuilding
    {
        /// <summary>
        /// Employee Identifier (unique)
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Flag to mark disabled employees
        /// </summary>
        public bool IsDeleted { get; set; }
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
        /// <summary>
        /// Can shipyard
        /// </summary>
        public bool CanShipyard { get; set; }
        /// <summary>
        /// Can paint
        /// </summary>
        public bool CanPaint { get; set; }
        /// <summary>
        /// Can rig
        /// </summary>
        public bool CanRig { get; set; }

        /// <summary>
        /// Navigation property
        /// </summary>
        public ICollection<EmployeeProfession> EmployeeProfessions { get; set; } = null!;
    }
}
