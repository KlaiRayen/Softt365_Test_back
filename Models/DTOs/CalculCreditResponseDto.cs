using System.Collections.Generic;

namespace Softt365Test.Models.DTOs
{
    public class CalculCreditResponseDto
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public decimal MontantEmprunterBrut { get; set; }
        public decimal FraisHypotheque { get; set; }
        public decimal MontantEmprunterNet { get; set; }
        public decimal TauxMensuel { get; set; }
        public decimal Mensualite { get; set; }
        public List<LigneAmortissementDto> TableauAmortissement { get; set; }
    }
}
