using Softt365Assessment.Models.DTOs;
using Softt365Assessment.Services.Constants;
using System.Collections.Generic;

namespace Soft365Assessment.Services.Validators
{
    public class CalculCreditValidator
    {
        public List<string> Valider(CalculCreditRequestDto request)
        {
            var errors = new List<string>();

            if (request.MontantAchat <= 0)
                errors.Add("Le montant d'achat doit être supérieur à 0.");

            if (request.DureeMois < 1 || request.DureeMois > 600)
                errors.Add("La durée doit être comprise entre 1 et 600 mois.");

            if (request.TauxAnnuel < 0 || request.TauxAnnuel > 100)
                errors.Add("Le taux annuel doit être compris entre 0% et 100%.");

            if (!request.MontantEmprunterOverride.HasValue)
            {
                if (!request.FondsPropres.HasValue)
                {
                    errors.Add("Les fonds propres sont requis.");
                    return errors;
                }

                if (request.FondsPropres.Value < 0)
                    errors.Add("Les fonds propres ne peuvent pas être négatifs.");

                if (request.FondsPropres.Value > request.MontantAchat)
                    errors.Add("Les fonds propres ne peuvent pas dépasser le montant d'achat.");

                if (request.FondsPropres.Value >= request.MontantAchat)
                {
                    errors.Add("Les fonds propres couvrent déjà le montant d'achat, aucun crédit n'est nécessaire.");
                    return errors;
                }

                decimal fraisAchat = request.FraisAchatOverride ??
                    (request.MontantAchat > CreditConstants.SEUIL_FRAIS_ACHAT
                        ? request.MontantAchat * CreditConstants.FRAIS_ACHAT_TAUX
                        : 0);

                decimal montantBrut = request.MontantAchat + fraisAchat - request.FondsPropres.Value;
                if (montantBrut <= 0)
                    errors.Add("Le montant à emprunter doit être supérieur à 0.");
            }
            else
            {
                if (request.MontantEmprunterOverride.Value <= 0)
                    errors.Add("Le montant d'emprunt doit être supérieur à 0.");

                decimal fraisAchatAuto = (request.MontantAchat > CreditConstants.SEUIL_FRAIS_ACHAT)
                    ? request.MontantAchat * CreditConstants.FRAIS_ACHAT_TAUX
                    : 0;

                if (request.MontantEmprunterOverride.Value > request.MontantAchat + fraisAchatAuto)
                    errors.Add("Le montant d'emprunt est incohérent par rapport au montant d'achat.");
            }

            if (request.FraisAchatOverride.HasValue)
            {
                if (request.FraisAchatOverride.Value < 0)
                    errors.Add("Les frais d'achat ne peuvent pas être négatifs.");

                if (request.FraisAchatOverride.Value >= request.MontantAchat)
                    errors.Add("Les frais d'achat manuels sont incohérents.");
            }

            return errors;
        }
    }
}
