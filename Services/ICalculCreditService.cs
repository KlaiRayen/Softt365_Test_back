using Softt365Test.Models.DTOs;

namespace Softt365Test.Services
{
    public interface ICalculCreditService
    {
        CalculCreditResponseDto Calculer(CalculCreditRequestDto request);
    }
}
