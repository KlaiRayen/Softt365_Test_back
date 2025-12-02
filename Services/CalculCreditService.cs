using System;
using System.Collections.Generic;
using Softt365Test.Models.DTOs;

namespace Softt365Test.Services
{
    public class CalculCreditService : ICalculCreditService
    {
        public CalculCreditResponseDto Calculer(CalculCreditRequestDto request)
        {
            decimal fraisAchat = Math.Round(request.MontantAchat * 0.10m, 2);

            decimal montantEmprunterBrut =
                Math.Round(request.MontantAchat + fraisAchat - request.FondsPropres, 2);

            decimal fraisHypotheque =
                Math.Round(montantEmprunterBrut * 0.02m, 2);

            decimal montantEmprunterNet =
                Math.Round(montantEmprunterBrut + fraisHypotheque, 2);

            decimal tauxMensuelDecimal = CalculerTauxMensuelArrondi(request.TauxAnnuel);

            decimal tauxMensuelAffiche = Math.Round(tauxMensuelDecimal * 100m, 3);

            decimal mensualite =
                CalculerMensualite(montantEmprunterNet, tauxMensuelDecimal, request.DureeMois);

            var tableau = GenererAmortissement(
                montantEmprunterNet, tauxMensuelDecimal, mensualite, request.DureeMois);

            return new CalculCreditResponseDto
            {
                MontantEmprunterBrut = montantEmprunterBrut,
                FraisHypotheque = fraisHypotheque,
                MontantEmprunterNet = montantEmprunterNet,
                TauxMensuel = tauxMensuelAffiche,
                Mensualite = mensualite,
                TableauAmortissement = tableau
            };
        }

        private decimal CalculerTauxMensuelArrondi(decimal tauxAnnuel)
        {
            double tA = (double)tauxAnnuel / 100.0;

            double mensuelRaw = Math.Pow(1 + tA, 1.0 / 12.0) - 1;

            double mensuelPercent = Math.Round(mensuelRaw * 100, 3);

            double tauxDecimal = mensuelPercent / 100;

            return (decimal)tauxDecimal;
        }

        private decimal CalculerMensualite(decimal capital, decimal t, int n)
        {
            if (t == 0)
                return Math.Round(capital / n, 2, MidpointRounding.AwayFromZero);

            double C = (double)capital;
            double tm = (double)t;
            double facteur = Math.Pow(1 + tm, n);

            double mensualite = C * tm * facteur / (facteur - 1);

            return Math.Round((decimal)mensualite, 2, MidpointRounding.AwayFromZero);
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
                decimal debut = Math.Round(solde, 2);

                decimal interet = Math.Round(debut * tauxMensuel, 2, MidpointRounding.AwayFromZero);

                decimal capitalRembourse = mensualite - interet;
                decimal fin = Math.Round(debut - capitalRembourse, 2);

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
                    CapitalRembourse = Math.Round(capitalRembourse, 2),
                    SoldeFin = fin
                });

                solde = fin;
            }

            return lignes;
        }
    }
}
