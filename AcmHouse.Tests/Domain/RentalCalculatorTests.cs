using System;
using AcmHouse.Domain;
using Xunit;

namespace AcmHouse.Tests.Domain
{
    public class RentalCalculatorTests
    {
        [Fact]
        public void CountDays_WorksAcrossNewYear()
        {
            int days = RentalCalculator.CountDays(new DateTime(2025, 12, 30), new DateTime(2026, 1, 2));

            Assert.Equal(3, days);
        }

        [Fact]
        public void CountDays_IgnoresTimeOfDay()
        {
            int days = RentalCalculator.CountDays(new DateTime(2026, 5, 1, 18, 0, 0), new DateTime(2026, 5, 3, 9, 0, 0));

            Assert.Equal(2, days);
        }

        [Fact]
        public void Calculate_MultipliesDaysByDayPrice()
        {
            RentalQuote quote = RentalCalculator.Calculate(new DateTime(2026, 5, 1), new DateTime(2026, 5, 8), 120);

            Assert.True(quote.IsValid);
            Assert.Equal(7, quote.Days);
            Assert.Equal(840, quote.Total);
        }

        [Fact]
        public void Calculate_AllowsTheMaximumPeriod()
        {
            DateTime start = new DateTime(2026, 5, 1);

            RentalQuote quote = RentalCalculator.Calculate(start, start.AddDays(RentalCalculator.MaxRentalDays), 100);

            Assert.True(quote.IsValid);
            Assert.Equal(RentalCalculator.MaxRentalDays, quote.Days);
        }

        [Fact]
        public void Calculate_RejectsPeriodLongerThanMaximum()
        {
            DateTime start = new DateTime(2026, 5, 1);

            RentalQuote quote = RentalCalculator.Calculate(start, start.AddDays(RentalCalculator.MaxRentalDays + 1), 100);

            Assert.False(quote.IsValid);
            Assert.Contains(RentalCalculator.MaxRentalDays.ToString(), quote.Error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Calculate_RejectsPeriodShorterThanOneDay(int days)
        {
            DateTime start = new DateTime(2026, 5, 10);

            RentalQuote quote = RentalCalculator.Calculate(start, start.AddDays(days), 100);

            Assert.False(quote.IsValid);
        }

        [Fact]
        public void Calculate_ThrowsForNegativePrice()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => RentalCalculator.Calculate(new DateTime(2026, 5, 1), new DateTime(2026, 5, 2), -1));
        }

        [Fact]
        public void CalculateAgentProfit_IsCommissionMinusTax()
        {
            // 15% commission of 1000 = 150, minus 30% tax = 105.
            Assert.Equal(105, RentalCalculator.CalculateAgentProfit(1000), 6);
        }
    }
}
