using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConstructionLine.CodingChallenge.Tests.SampleData;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    public class SearchEnginePerformanceTests : SearchEngineTestsBase
    {
        private List<Shirt> _shirts;
        private SearchEngine _searchEngine;

        [SetUp]
        public void Setup()
        {
            
            var dataBuilder = new SampleDataBuilder(50000);

            _shirts = dataBuilder.CreateShirts();

            _searchEngine = new SearchEngine(_shirts);
        }

        [TestCase(100)]
        public void Search_WhenSingleColorOptionProvided_ShouldNotTakeLongerThanExpected(int maxDurationInMilliseconds)
        {
            var sw = new Stopwatch();
            sw.Start();

            var options = new SearchOptions
            {
                Colors = new List<Color> { Color.Red }
            };

            var results = _searchEngine.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds");

            AssertResults(results.Shirts, options);
            AssertSizeCounts(_shirts, options, results.SizeCounts);
            AssertColorCounts(_shirts, options, results.ColorCounts);

            Assert.IsTrue(sw.ElapsedMilliseconds < maxDurationInMilliseconds, $"Expected Duration should be less than {maxDurationInMilliseconds}ms, but elapsed {sw.ElapsedMilliseconds}ms.");
        }

        [TestCase(100)]
        public void Search_WhenMultiColorMultiSizeOptionProvided_ShouldNotTakeLongerThanExpected(int maxDurationInMilliseconds)
        {
            var sw = new Stopwatch();
            sw.Start();

            var options = new SearchOptions
            {
                Colors = Color.All,
                Sizes = Size.All
            };

            var results = _searchEngine.Search(options);

            sw.Stop();
            Console.WriteLine($"Test fixture finished in {sw.ElapsedMilliseconds} milliseconds");

            AssertResults(results.Shirts, options);
            AssertSizeCounts(_shirts, options, results.SizeCounts);
            AssertColorCounts(_shirts, options, results.ColorCounts);

            Assert.IsTrue(sw.ElapsedMilliseconds < maxDurationInMilliseconds, $"Expected Duration should be less than {maxDurationInMilliseconds}ms, but elapsed {sw.ElapsedMilliseconds}ms.");
        }
    }
}
