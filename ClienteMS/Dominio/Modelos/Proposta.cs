using ClienteMS.Enum;
using System.ComponentModel.DataAnnotations;

namespace ClienteMS.Modelos
{
    public class Proposta
    {
        [Key]
        public Guid Id { get; set; }
        public decimal ValorOfertado { get; set; }
        public StatusProposta Status {get; set;}
    }
}
