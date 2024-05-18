using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanificacionReceta.Models
{
    public class RecetaModel
    {
        public string Nombre { get; set; } = null!;
        public string Imagen { get; set; } = null!;
        public string Ingredientes { get; set; } = null!;
        public int TiempoCoccion { get; set; }
        public int TiempoPreparacion {  get; set; }
        public string Instrucciones { get; set; } = null!;

        public string Dia { get; set; }=null!;
    }
}
