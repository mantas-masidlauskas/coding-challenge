using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            // TODO: data preparation and initialisation of additional data structures to improve performance goes here.
            _shirts = shirts ?? throw new ArgumentNullException(nameof(shirts));
        }

        public SearchResults Search(SearchOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // TODO: search logic goes here.

            var colorCounts = GetColorCount(_shirts);
            var sizeCounts = GetSizeCount(_shirts);

            return new SearchResults
            {
                Shirts = _shirts,
                ColorCounts = colorCounts,
                SizeCounts = sizeCounts
            };
        }

        private static List<SizeCount> GetSizeCount(List<Shirt> matchedShirts)
        {
            var sizedShirtCount = matchedShirts
                          .GroupBy(g => g.Size.Id)
                          .ToDictionary(s => s.Key, s => s.Count());

            var sizeCounts = new List<SizeCount>();
            foreach (var s in Size.All)
            {
                sizedShirtCount.TryGetValue(s.Id, out int count);
                sizeCounts.Add(new SizeCount { Size = s, Count = count });
            }

            return sizeCounts;
        }

        private static List<ColorCount> GetColorCount(List<Shirt> matchedShirts)
        {
            var coloredShirtCount = matchedShirts
                          .GroupBy(g => g.Color.Id)
                          .ToDictionary(s => s.Key, s => s.Count());

            var colorCounts = new List<ColorCount>();
            foreach (var c in Color.All)
            {
                coloredShirtCount.TryGetValue(c.Id, out int count);
                colorCounts.Add(new ColorCount { Color = c, Count = count });
            }

            return colorCounts;
        }
    }
}