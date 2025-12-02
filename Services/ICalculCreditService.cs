using Softt365Assessment.Models.DTOs;

namespace Softt365Assessment.Services
{
    public interface ICalculCreditService
    {
        CalculCreditResponseDto Calculer(CalculCreditRequestDto request);
    }
}
