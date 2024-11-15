using System.ComponentModel.DataAnnotations;

namespace MRLserver.Models
{
    public class MRLtelemetryModel
    {
        public int ID { get; set; }
        public required string UID { get; set; }
        [DataType(DataType.Date)]
        public DateTime? utolsoKapcsolataLifttel { get; set; }
        public int DoorStateA { get; set; }
        public int DoorStateB { get; set; }
        public int ElevatorState { get; set; }
        public int Travel1 { get; set; }
        public int Travel2 { get; set; }
        public string VVVFErrors { get; set; }
        public string Errors { get; set; }        
    }
}