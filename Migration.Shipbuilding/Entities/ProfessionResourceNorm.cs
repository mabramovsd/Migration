namespace Migration.Shipbuilding.Entities
{
    /// <summary>
    /// Production norm: how much resource is produced per profession work time
    /// </summary>
    public class ProfessionResourceNorm
    {
        public Guid Id { get; set; }
        public Guid ProfessionId { get; set; }
        public Guid ResourceId { get; set; }
        public int Hours { get; set; }
        public int QuantityProduced { get; set; }

        public Profession? Profession { get; set; }
        public ResourceShipbuilding? Resource { get; set; }
    }
}
