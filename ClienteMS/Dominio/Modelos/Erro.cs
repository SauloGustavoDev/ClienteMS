using System.ComponentModel.DataAnnotations;
namespace ClienteMS.Modelos
{
    public class Erro
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid ClienteId { get; set; }
        public string Tipo { get; set; }
        public DateTime DtErro { get; set; } = DateTime.Now;
        public string Mensagem { get; set; }
        public string StackTrace { get; set; }
    }
}
