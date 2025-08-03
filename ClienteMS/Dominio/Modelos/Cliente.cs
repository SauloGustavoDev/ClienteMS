using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Modelos
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
