using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Interactivity;
using Lopushok;
using NUnit.Framework;

namespace Testi
{
    [TestFixture]
    public class ChangePriceWindowTests
    {
        [OneTimeSetUp]
        public void InitAvalonia()
        {
            try
            {
                AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                    .SetupWithoutStarting();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to initialize Avalonia: {ex}");
            }
        }

        [Test]
        public void Constructor_ShouldInitializeWithSuggestedPrice()
        {
            // Arrange
            const decimal testPrice = 123.45m;

            // Act
            var window = new ChangePriceWindow(testPrice);

            // Assert
            Assert.That(window.PriceBox.Text, Is.EqualTo(testPrice.ToString("0.##", CultureInfo.InvariantCulture)));
            Assert.That(window.EnteredPrice, Is.Null);
        }

        [Test]
        public void Apply_Click_WithValidPrice_ShouldSetEnteredPriceAndClose()
        {
            // Arrange
            const decimal testPrice = 150.75m;
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = testPrice.ToString(CultureInfo.InvariantCulture) }
            };

            bool closed = false;
            window.Closed += (_, _) => closed = true;

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(testPrice));
            Assert.That(closed, Is.True);
        }

        [Test]
        public void Apply_Click_WithCommaDecimalSeparator_ShouldParseCorrectly()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "150,75" }
            };

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(150.75m));
        }

        [Test]
        public void Apply_Click_WithDotDecimalSeparator_ShouldParseCorrectly()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "150.75" }
            };

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(150.75m));
        }

        [Test]
        public void Apply_Click_WithNegativePrice_ShouldNotCloseWindow()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "-10" }
            };

            bool closed = false;
            window.Closed += (_, _) => closed = true;

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.Null);
            Assert.That(closed, Is.False);
        }

        [Test]
        public void Apply_Click_WithInvalidText_ShouldNotCloseWindow()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "abc" }
            };

            bool closed = false;
            window.Closed += (_, _) => closed = true;

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.Null);
            Assert.That(closed, Is.False);
        }

        [Test]
        public void Apply_Click_WithEmptyText_ShouldNotCloseWindow()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "" }
            };

            bool closed = false;
            window.Closed += (_, _) => closed = true;

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.Null);
            Assert.That(closed, Is.False);
        }

        [Test]
        public void Apply_Click_ShouldRoundToTwoDecimalPlaces()
        {
            // Arrange
            var window = new ChangePriceWindow(100m)
            {
                PriceBox = { Text = "123.4567" }
            };

            // Act
            window.Apply_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.EqualTo(123.46m));
        }

        [Test]
        public void Cancel_Click_ShouldCloseWithNullResult()
        {
            // Arrange
            var window = new ChangePriceWindow(100m);
            bool closed = false;
            window.Closed += (_, _) => closed = true;

            // Act
            window.Cancel_Click(null, null);

            // Assert
            Assert.That(window.EnteredPrice, Is.Null);
            Assert.That(closed, Is.True);
        }
    }
}