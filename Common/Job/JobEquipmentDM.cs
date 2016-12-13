namespace Common
{
    public class JobEquipmentDM
    {
        public int JobEquipmentID { get; set; }
        public int EquipmentID { get; set; }
        public int JobID { get; set; }

        public EquipmentDM EquipmentDM { get; set; }

        public JobDM JobDM { get; set; }
    }
}
