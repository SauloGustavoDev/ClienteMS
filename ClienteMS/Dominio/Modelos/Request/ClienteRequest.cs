namespace ClienteMS.Dominio.Models.Request
{
    public class ClienteRequest
    {
        public string Nome { get; set; }
        public decimal Renda { get; set; }
        public bool SimularErro { get; set; }
    }
}
