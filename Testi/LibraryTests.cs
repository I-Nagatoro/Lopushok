using NUnit.Framework;
using WSUniversalLib;

namespace Testi
{
    [TestFixture]
    public class CalculationTests
    {
        [Test]
        public void CalculateMaterial_ProductType3MaterialType1_ReturnsCorrectValue()
        {
            int expected = 114148;

            int actual = Calculation.CalculateMaterial(3, 1, 15, 20, 45);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_ProductType1MaterialType2_ReturnsCorrectValue()
        {
            int expected = 276;

            int actual = Calculation.CalculateMaterial(1, 2, 10, 5, 5);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_ProductType2MaterialType1_ReturnsCorrectValue()
        {
            int expected = 627;

            int actual = Calculation.CalculateMaterial(2, 1, 10, 5, 5);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_MinimumValues_ReturnsCorrectValue()
        {
            int expected = 2; 

            int actual = Calculation.CalculateMaterial(1, 1, 1, 1, 1);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_InvalidProductType_ReturnsMinusOne()
        {
            int result = Calculation.CalculateMaterial(4, 1, 10, 5, 5);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_InvalidMaterialType_ReturnsMinusOne()
        {
            int result = Calculation.CalculateMaterial(1, 3, 10, 5, 5);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_ZeroCount_ReturnsMinusOne()
        {
            int result = Calculation.CalculateMaterial(1, 1, 0, 5, 5);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_NegativeWidth_ReturnsMinusOne()
        {
            int result = Calculation.CalculateMaterial(1, 1, 10, -5, 5);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_ZeroLength_ReturnsMinusOne()
        {
            int result = Calculation.CalculateMaterial(1, 1, 10, 5, 0);

            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_FractionalDimensions_CorrectRounding()
        {
            int expected = 34;

            int actual = Calculation.CalculateMaterial(1, 1, 10, 2.5f, 1.3f);

            Assert.That(actual, Is.EqualTo(36));
        }

        [Test]
        public void CalculateMaterial_LargeValues_NoOverflow()
        {
            int result = Calculation.CalculateMaterial(3, 1, int.MaxValue, float.MaxValue, float.MaxValue);

            Assert.That(result, Is.Not.EqualTo(-1));
        }
    }
}