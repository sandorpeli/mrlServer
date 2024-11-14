﻿using System.ComponentModel.DataAnnotations;

namespace MRLserver.Models
{
    public class MRLclass
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public string? telepitesHelye { get; set; }
        public string? telepitestVegezte { get; set; }
        public string? telepitesPozicioja { get; set; }
        public string? karbantarto { get; set; }

        [DataType(DataType.Date)]
        public DateTime telepitesIdeje { get; set; }
        [DataType(DataType.Date)]
        public DateTime utolsoKarbantartasIdeje { get; set; }
        [DataType(DataType.Date)]
        public DateTime kovetkezoKarbantartas { get; set; }
        [DataType(DataType.Date)]
        public DateTime utolsoKapcsolataLifttel { get; set; }
    }
}
