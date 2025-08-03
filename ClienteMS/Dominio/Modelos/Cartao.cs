using ClienteMS.Enum;
using System.ComponentModel.DataAnnotations;

namespace ClienteMS.Modelos
{
    public class Cartao
    {
        [Key]
        public Guid Id { get; set; }
        public decimal Limite { get; set; }
        public string Numero { get; set; }
        public string Cvv { get; set; }
        public StatusCartao Status { get; set; }
    }
}
