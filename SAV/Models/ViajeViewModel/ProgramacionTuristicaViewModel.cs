using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ProgramacionTuristicaViewModel
    {
        [Required(ErrorMessage = "Debe ingresar una programación turística")]
        [Display(Name = "Datos programación turística:")]
        public string ProgramacionTuristica { get; set; }

        public int ViajeID { get; set; }
    }
}