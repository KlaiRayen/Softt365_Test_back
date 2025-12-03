using Soft365Assessment.Services.Validators;
using Softt365Assessment.Models.DTOs;
using Softt365Assessment.Services.Constants;
using System;
using System.Collections.Generic;

namespace Softt365Assessment.Services
{
    public class CalculCreditService : ICalculCreditService
    {
        public CalculCreditResponseDto Calculer(CalculCreditRequestDto request)
        {
            var response = new CalculCreditResponseDto();

            var validator = new CalculCreditValidator();
            var errors = validator.Valider(request);

            if (errors.Count > 0)
            {
                response.Success = false;
                response.Errors = errors;
                return response;
            }

            response.Success = true;

            decimal fraisAchat;

            if (request.FraisAchatOverride.HasValue)
            {
                fraisAchat = request.FraisAchatOverride.Value;
            }
            else
            {
                if (request.MontantAchat > CreditConstants.SEUIL_FRAIS_ACHAT)
                {
                    fraisAchat = Math.Round(
                        request.MontantAchat * CreditConstants.FRAIS_ACHAT_TAUX,
                        CreditConstants.ARRONDI_DEUX
                    );
                }
                else
                {
                    fraisAchat = 0;
                }
            }

            decimal montantEmprunterBrut;

            if (request.MontantEmprunterOverride.HasValue)
            {
                montantEmprunterBrut = Math.Round(
                    request.MontantEmprunterOverride.Value,
                    CreditConstants.ARRONDI_DEUX
                );

                decimal fondsPropresRecalcules =
                    request.MontantAchat + fraisAchat - montantEmprunterBrut;

                request.FondsPropres = Math.Round(
                    fondsPropresRecalcules,
                    CreditConstants.ARRONDI_DEUX
                );
            }
            else
            {
                montantEmprunterBrut = Math.Round(
                    request.MontantAchat + fraisAchat - request.FondsPropres.Value,
                    CreditConstants.ARRONDI_DEUX
                );
            }


            decimal fraisHypotheque = Math.Round(
                montantEmprunterBrut * CreditConstants.FRAIS_HYPOTHEQUE_TAUX,
                CreditConstants.ARRONDI_DEUX
            );

            decimal montantEmprunterNet = Math.Round(
                montantEmprunterBrut + fraisHypotheque,
                CreditConstants.ARRONDI_DEUX
            );

            decimal tauxMensuelDecimal = CalculerTauxMensuelArrondi(request.TauxAnnuel);

            decimal tauxMensuelAffiche = Math.Round(
                tauxMensuelDecimal * 100m,
                CreditConstants.ARRONDI_TROIS
            );

            decimal mensualite =
                CalculerMensualite(montantEmprunterNet, tauxMensuelDecimal, request.DureeMois);

            var tableau = GenererAmortissement(
                montantEmprunterNet, tauxMensuelDecimal, mensualite, request.DureeMois);

            response.MontantEmprunterBrut = montantEmprunterBrut;
            response.FraisHypotheque = fraisHypotheque;
            response.FraisAchat = fraisAchat;
            response.MontantEmprunterNet = montantEmprunterNet;
            response.TauxMensuel = tauxMensuelAffiche;
            response.Mensualite = mensualite;
            response.TableauAmortissement = tableau;
            response.FondsPropres = request.FondsPropres.Value;

            return response;
        }

        private decimal CalculerTauxMensuelArrondi(decimal tauxAnnuel)
        {
            double tA = (double)tauxAnnuel / 100.0;
            double mensuelRaw = Math.Pow(1 + tA, 1.0 / 12.0) - 1;

            double mensuelPercent = Math.Round(
                mensuelRaw * 100,
                CreditConstants.ARRONDI_TROIS
            );

            double tauxDecimal = mensuelPercent / 100;

            return (decimal)tauxDecimal;
        }

        private decimal CalculerMensualite(decimal capital, decimal t, int n)
        {
            if (t == 0)
                return Math.Round(capital / n, CreditConstants.ARRONDI_DEUX, MidpointRounding.AwayFromZero);

            double C = (double)capital;
            double tm = (double)t;
            double facteur = Math.Pow(1 + tm, n);

            double mensualite = C * tm * facteur / (facteur - 1);

            return Math.Round(
                (decimal)mensualite,
                CreditConstants.ARRONDI_DEUX,
                MidpointRounding.AwayFromZero
            );
        }

        private List<LigneAmortissementDto> GenererAmortissement(
        decimal capitalInitial,
        decimal tauxMensuel,
        decimal mensualite,
        int duree)
        {
            var lignes = new List<LigneAmortissementDto>();
            decimal solde = capitalInitial;

            for (int periode = 1; periode <= duree; periode++)
            {
                decimal debut = Math.Round(solde, CreditConstants.ARRONDI_DEUX);

                decimal interet = Math.Round(
                    debut * tauxMensuel,
                    CreditConstants.ARRONDI_DEUX,
                    MidpointRounding.AwayFromZero
                );

                decimal capitalRembourse = mensualite - interet;
                decimal fin = Math.Round(debut - capitalRembourse, CreditConstants.ARRONDI_DEUX);

                if (periode == duree)
                {
                    capitalRembourse = debut;
                    interet = mensualite - capitalRembourse;
                    fin = 0;
                }

                lignes.Add(new LigneAmortissementDto
                {
                    Periode = periode,
                    SoldeDebut = debut,
                    Mensualite = mensualite,
                    Interet = interet,
                    CapitalRembourse = Math.Round(capitalRembourse, CreditConstants.ARRONDI_DEUX),
                    SoldeFin = fin
                });

                solde = fin;
            }

            return lignes;
        }

    }
}
