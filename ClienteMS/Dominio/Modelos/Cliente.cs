using System.ComponentModel.DataAnnotations;
namespace ClienteMS.Modelos
{
    public class Cliente
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public decimal Renda { get; set; }
        public List<Proposta> Proposta { get; set; }
        public List<Cartao> Cartao { get; set; }
    }
}
