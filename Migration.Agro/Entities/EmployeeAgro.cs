namespace Migration.Agro.Entities
{
    public class EmployeeAgro
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
        /// Has license to drive tracktor:)
        /// </summary>
        public bool HasTracktorLicense { get; set; }

        /// <summary>
        /// Is a milker
        /// </summary>
        public bool IsMilker { get; set; }

        /// <summary>
        /// Is a cattleman
        /// </summary>
        public bool IsCattleman { get; set; }

        /// <summary>
        /// Is a poultry farmer
        /// </summary>
        public bool IsPoultryFarmer { get; set; }

        /// <summary>
        /// Is a miller
        /// </summary>
        public bool IsMiller { get; set; }

        /// <summary>
        /// Is a vegetable grower
        /// </summary>
        public bool IsVegetableGrower { get; set; }
    }
}
