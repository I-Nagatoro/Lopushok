using NUnit.Framework;
using ClassLibraryMaterial;

namespace Testi
{
    [TestFixture]
    public class CalculationTests
    {
        // Позитивные тесты
        [Test]
        public void CalculateMaterial_ProductType3MaterialType1_ReturnsCorrectValue()
        {
            // Arrange
            int expected = 114148;

            // Act
            int actual = Calculation.CalculateMaterial(3, 1, 15, 20, 45);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_ProductType1MaterialType2_ReturnsCorrectValue()
        {
            // Arrange
            int expected = 276; // 10 * (5*5) * 1.1 / (1 - 0.0012) = 275.33 → 276

            // Act
            int actual = Calculation.CalculateMaterial(1, 2, 10, 5, 5);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CalculateMaterial_ProductType2MaterialType1_ReturnsCorrectValue()
        {
            // Arrange
            int expected = 627; // 10 * (5*5) * 2.5 / (1 - 0.003) ≈ 626.880 → 627

            // Act
            int actual = Calculation.CalculateMaterial(2, 1, 10, 5, 5);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        // Тесты на граничные значения
        [Test]
        public void CalculateMaterial_MinimumValues_ReturnsCorrectValue()
        {
            // Arrange
            int expected = 2; // 1 * (1*1) * 1.1 / (1 - 0.003) = 1.103 → 2

            // Act
            int actual = Calculation.CalculateMaterial(1, 1, 1, 1, 1);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        // Тесты на невалидные данные
        [Test]
        public void CalculateMaterial_InvalidProductType_ReturnsMinusOne()
        {
            // Act
            int result = Calculation.CalculateMaterial(4, 1, 10, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_InvalidMaterialType_ReturnsMinusOne()
        {
            // Act
            int result = Calculation.CalculateMaterial(1, 3, 10, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_ZeroCount_ReturnsMinusOne()
        {
            // Act
            int result = Calculation.CalculateMaterial(1, 1, 0, 5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_NegativeWidth_ReturnsMinusOne()
        {
            // Act
            int result = Calculation.CalculateMaterial(1, 1, 10, -5, 5);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        [Test]
        public void CalculateMaterial_ZeroLength_ReturnsMinusOne()
        {
            // Act
            int result = Calculation.CalculateMaterial(1, 1, 10, 5, 0);

            // Assert
            Assert.That(result, Is.EqualTo(-1));
        }

        // Тесты на точность вычислений
        [Test]
        public void CalculateMaterial_FractionalDimensions_CorrectRounding()
        {
            // Arrange
            int expected = 34; // 10 * (2.5*1.3) * 1.1 / (1 - 0.003) = 35.825 → 36

            // Act
            int actual = Calculation.CalculateMaterial(1, 1, 10, 2.5f, 1.3f);

            // Assert
            Assert.That(actual, Is.EqualTo(36));
        }

        // Тест на максимальные значения (по необходимости)
        [Test]
        public void CalculateMaterial_LargeValues_NoOverflow()
        {
            // Act
            int result = Calculation.CalculateMaterial(3, 1, int.MaxValue, float.MaxValue, float.MaxValue);

            // Assert
            Assert.That(result, Is.Not.EqualTo(-1));
        }
    }
}