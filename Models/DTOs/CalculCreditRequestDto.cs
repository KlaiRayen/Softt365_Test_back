using System.ComponentModel.DataAnnotations;

namespace Softt365Test.Models.DTOs
{
    public class CalculCreditRequestDto
    {
        [Required(ErrorMessage = "Le montant d'achat est requis.")]
        [Range(1, double.MaxValue, ErrorMessage = "Le montant d'achat doit être supérieur à 0.")]
        public decimal MontantAchat { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Les fonds propres ne peuvent pas être négatifs.")]
        public decimal FondsPropres { get; set; }

        [Range(1, 600, ErrorMessage = "La durée doit être comprise entre 1 et 600 mois.")]
        public int DureeMois { get; set; }

        [Range(0, 100, ErrorMessage = "Le taux annuel doit être compris entre 0% et 100%.")]
        public decimal TauxAnnuel { get; set; }
    }
}
