namespace ClienteMS.Dominio.Models.DTOs
{
    public class ReturnoDto<T>
    {
        public T Dados { get; set; }
        public Exception Excecao { get; set; }
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
    }
}
