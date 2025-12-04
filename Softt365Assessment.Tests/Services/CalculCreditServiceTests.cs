using NUnit.Framework;
using Softt365Assessment.Models.DTOs;
using Softt365Assessment.Services;
using System;
using System.Linq;

namespace Soft365Assessment.Tests.Services
{
    [TestFixture]
    public class CalculCreditServiceTests
    {
        private CalculCreditService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CalculCreditService();
        }

        [Test]
        public void Calculer_ShouldReturnSuccess_WhenRequestIsValid()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(598.48m, result.Mensualite);
            Assert.AreEqual(112000m, result.MontantEmprunterBrut);
        }

        [Test]
        public void Calculer_ShouldFail_WhenFondsPropresExceedMontantAchat()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 100000,
                FondsPropres = 150000, // erreur
                DureeMois = 240,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsNotEmpty(result.Errors);
        }


        [Test]
        public void Calculer_ShouldFail_WhenFondsPropresCoverEverything()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 120000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }


        [Test]
        public void Calculer_ShouldFail_WhenMontantEmprunteBrutBecomesNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 20000,
                FondsPropres = 50000,   // after frais, les fonds dépassent
                DureeMois = 120,
                TauxAnnuel = 2.1m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void Calculer_ShouldFail_WhenMontantAchatIsZero()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 0,
                FondsPropres = 1000,
                DureeMois = 240,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void Calculer_ShouldFail_WhenMontantAchatIsNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = -500,
                FondsPropres = 1000,
                DureeMois = 240,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }


        [Test]
        public void Calculer_ShouldFail_WhenFondsPropresAreNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = -1,
                DureeMois = 240,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }


        [Test]
        public void Calculer_ShouldFail_WhenDureeMoisIsZero()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 0,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }


        [Test]
        public void Calculer_ShouldFail_WhenDureeMoisExceeds600()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 700,
                TauxAnnuel = 2m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }


        [Test]
        public void Calculer_ShouldFail_WhenTauxAnnuelIsNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = -1m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }


        [Test]
        public void Calculer_ShouldFail_WhenTauxAnnuelExceeds100()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 150m
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
        }

        [Test]
        public void Calculer_ShouldUseManualFraisAchat_WhenOverrideIsProvided()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m,
                FraisAchatOverride = 8000
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(8000m, result.MontantEmprunterBrut - (request.MontantAchat - request.FondsPropres));
        }

        [Test]
        public void Calculer_ShouldFail_WhenManualFraisAchatIsNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m,
                FraisAchatOverride = -100
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void Calculer_ShouldFail_WhenManualFraisAchatIsIncoherent()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m,
                FraisAchatOverride = 200000
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void Calculer_ShouldRecalculateFondsPropres_WhenMontantEmprunterOverrideIsUsed()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m,
                FraisAchatOverride = null,
                MontantEmprunterOverride = 100000
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(100000m, result.MontantEmprunterBrut);
        }

        [Test]
        public void Calculer_ShouldComputeCorrectFondsPropres_WhenMontantEmprunterOverrideIsApplied()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 100000,
                FondsPropres = 0,
                DureeMois = 200,
                TauxAnnuel = 3m,
                MontantEmprunterOverride = 80000
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(80000m, result.MontantEmprunterBrut);
        }

        [Test]
        public void Calculer_ShouldFail_WhenMontantEmprunterOverrideIsNegative()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2m,
                MontantEmprunterOverride = -5000
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void Calculer_ShouldFail_WhenMontantEmprunterOverrideExceedsAllowedAmount()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2m,
                MontantEmprunterOverride = 200000
            };

            var result = _service.Calculer(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void Calculer_ShouldPass_WhenMontantEmprunterOverrideIsAtMaxAllowed()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 100000,
                FondsPropres = 0,
                DureeMois = 180,
                TauxAnnuel = 3.5m,
                MontantEmprunterOverride = 110000
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public void Calculer_ShouldRecalculateLargeFondsPropres_WhenMontantEmprunterOverrideIsSmall()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 0,
                DureeMois = 240,
                TauxAnnuel = 2.4m,
                MontantEmprunterOverride = 20000
            };

            var result = _service.Calculer(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(20000m, result.MontantEmprunterBrut);
        }


        [Test]
        public void DerniereLigne_ShouldHaveSoldeFinEgalZero()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var result = _service.Calculer(request);

            var lastLine = result.TableauAmortissement.Last();

            Assert.AreEqual(0m, lastLine.SoldeFin);
        }

        [Test]
        public void DerniereLigne_ShouldAdjustInteretCorrectly()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var result = _service.Calculer(request);

            var last = result.TableauAmortissement.Last();

            var expectedInteret = Math.Round(last.Mensualite - last.SoldeDebut, 2);

            Assert.AreEqual(expectedInteret, last.Interet);
        }

        [Test]
        public void DerniereLigne_CapitalRembourse_ShouldEqualSoldeDebut()
        {
            var request = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var result = _service.Calculer(request);

            var last = result.TableauAmortissement.Last();

            Assert.AreEqual(last.SoldeDebut, last.CapitalRembourse);
        }

        [Test]
        public void DerniereLigne_ShouldRespectMensualiteEquality()
        {
            var req = new CalculCreditRequestDto
            {
                MontantAchat = 120000,
                FondsPropres = 20000,
                DureeMois = 240,
                TauxAnnuel = 2.4m
            };

            var res = _service.Calculer(req);

            var last = res.TableauAmortissement.Last();

            var sum = Math.Round(last.Interet + last.CapitalRembourse, 2);

            Assert.AreEqual(last.Mensualite, sum);
        }

    }
}