using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Validations;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_API.Test.UnitTesting
{
    [TestClass]
    public class CapitalFirstLetterAttributeTest
    {
        [TestMethod]
        public void IfFirtsLetterIsLower_ReturnError()
        {
            //Preparation

            var capitalFirstLetter = new CapitalFirstLetterAttribute();
            var value = "wendy";
            var valContext = new ValidationContext(new { Name = value });

            //Execution
            var result = capitalFirstLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.AreEqual("The first letter must be capital case", result.ErrorMessage);
        }

        [TestMethod]
        public void IfFirtsLetterIsCapital_DoesNotReturnError()
        {
            //Preparation

            var capitalFirstLetter = new CapitalFirstLetterAttribute();
            var value = "Wendy";
            var valContext = new ValidationContext(new { Name = value });

            //Execution
            var result = capitalFirstLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }

        [TestMethod]
        public void IfNull_DoesNotReturnError()
        {
            //Preparation

            var capitalFirstLetter = new CapitalFirstLetterAttribute();
            string value = null;
            var valContext = new ValidationContext(new { Name = value });

            //Execution
            var result = capitalFirstLetter.GetValidationResult(value, valContext);

            // Verification
            Assert.IsNull(result);
        }
    }
}