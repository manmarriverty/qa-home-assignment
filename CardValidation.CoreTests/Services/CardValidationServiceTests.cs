using Microsoft.VisualStudio.TestTools.UnitTesting;
using CardValidation.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reqnroll.Assist;
using CardValidation.Core.Enums;
using System.ComponentModel.DataAnnotations;
using Reqnroll.CommonModels;

namespace CardValidation.Core.Services.Tests
{
   
    [TestClass()]
    public class CardValidationServiceTests
    {

        private CardValidationService service;

        [TestInitialize]
        public void Setup()

        { 
            service = new CardValidationService();
        }
    
        // Validate Owner
        [TestMethod()]
        public void ValidateOwner_OneWord()
        {
            bool result = service.ValidateOwner("John");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateOwner_ThreeWords()
        {
            bool result = service.ValidateOwner("John Doe Jim");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateOwner_WordAndNumber()
        {
            bool result = service.ValidateOwner("John123");
            Assert.IsFalse(result);
        }


        [TestMethod()]
        public void ValidateOwner_EmptyString()
        {
            bool result = service.ValidateOwner("");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateOwner_DoubleSpaces()
        {
            bool result = service.ValidateOwner("Jon  Dohn");
            Assert.IsFalse(result);
        }
        // Validate Date
         [TestMethod()]
        public void ValidateDate_FutureDate_MMYYYY()
        {
            bool result = service.ValidateIssueDate("12/2028");
            Assert.IsTrue(result);
        }
        [TestMethod()]
        public void ValidateDate_FutureDate_MMYY()
        {
            bool result = service.ValidateIssueDate("12/28");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateDate_PastDate_MMYYYY()
        {
            bool result = service.ValidateIssueDate("11/2021");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_PastDate_MMYY()
        {
            bool result = service.ValidateIssueDate("11/21");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_EmptyString()
        {
            bool result = service.ValidateIssueDate("");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_DoubleSpaces()
        {
            bool result = service.ValidateIssueDate("  ");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_IncorrectFormat()
        {
            bool result = service.ValidateIssueDate("11-25");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_CurrentMonth()
        {
            bool result = service.ValidateIssueDate("01/25");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateDate_InvalidMonth_MMYYYY()
        {
            bool result = service.ValidateIssueDate("18/2025");
            Assert.IsFalse(result);
        }

        // Validate CVC
        [TestMethod()]
        public void ValidateCvc_ThreeDigits()
        {
            bool result = service.ValidateCvc("789");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateCvc_FourDigits()
        {
            bool result = service.ValidateCvc("7890");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateCvc_TwoDigits()
        {
            bool result = service.ValidateCvc("12");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateCvc_FiveDigits()
        {
            bool result = service.ValidateCvc("65873");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateCvc_TwoDigitsAndOneCharacter()
        {
            bool result = service.ValidateCvc("71d");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateCvc_Characters()
        {
            bool result = service.ValidateCvc("vcd");
            Assert.IsFalse(result);
        }


        // Validate Number
        [TestMethod()]
        public void ValidateNumber_ValidAmericanExpress()
        {
            bool result = service.ValidateNumber("378282246310005");
            Assert.IsTrue(result);
        }
        [TestMethod()]
        public void ValidateNumber_ValidVisaCard()
        {
            bool result = service.ValidateNumber("4111111111111111");
            Assert.IsTrue(result);
        }
        [TestMethod()]
        public void ValidateNumber_ValidMasterCard()
        {
            bool result = service.ValidateNumber("5555555555554444");
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void ValidateNumber_InValidAmericanExpress()
        {
            bool result = service.ValidateNumber("378280005");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateNumber_InValidVisaCard()
        {
            bool result = service.ValidateNumber("8111111111111111");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateNumber_InValidMasterCard()
        {
            bool result = service.ValidateNumber("555555554444");
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ValidateNumber_ForEmptyString()
        {
           
            bool result = service.ValidateNumber("");
            Assert.IsFalse(result); 
        }

        [TestMethod()]
        public void ValidateNumber_ForWhitespaceInput()
        {
            
            bool result = service.ValidateNumber("   ");
            Assert.IsFalse(result);
        }

        // Validate Payment Type
        [TestMethod()]
        public void GetPaymentSystemType_ReturnsVisa()
        {
           var visaCard = service.GetPaymentSystemType("4111111111111111");
            Assert.AreEqual(PaymentSystemType.Visa, visaCard);
        }

        [TestMethod()]
        public void GetPaymentSystemType_ReturnsMasterCard()
        {
            var masterCard = service.GetPaymentSystemType("5555555555554444");
            Assert.AreEqual(PaymentSystemType.MasterCard, masterCard);
        }

        [TestMethod()]
        public void GetPaymentSystemType_ReturnsAmericanExpress()
        {
            var americanExpressCard = service.GetPaymentSystemType("378282246310005");
            Assert.AreEqual(PaymentSystemType.AmericanExpress, americanExpressCard);
        }

        [TestMethod()]
        public void GetPaymentSystemType_ReturnsException()
        {
            var exception = Assert.ThrowsException<NotImplementedException>(() => service.GetPaymentSystemType("6011000990123456"));
            Assert.AreEqual("The method or operation is not implemented.", exception.Message);
        }
    }
}