namespace Softt365Test.Models.DTOs
{
    public class CalculCreditRequestDto
    {
        public decimal MontantAchat { get; set; }
        public decimal FondsPropres { get; set; }
        public int DureeMois { get; set; }
        public decimal TauxAnnuel { get; set; }
    }
}
