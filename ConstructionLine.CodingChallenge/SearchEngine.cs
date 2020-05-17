using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly IReadOnlyList<Shirt> _shirts;
        private readonly IReadOnlyDictionary<Guid, Shirt> _shirtsByGuid;
        private readonly IReadOnlyDictionary<Guid, HashSet<Guid>> _shirtsByColor;
        private readonly IReadOnlyDictionary<Guid, HashSet<Guid>> _shirtsBySize;

        public SearchEngine(List<Shirt> shirts)
        {
            // data preparation and initialisation of additional data structures to improve performance goes here.
            _shirts = shirts ?? throw new ArgumentNullException(nameof(shirts));
            _shirtsByGuid = _shirts.ToDictionary(s => s.Id, s => s);
            _shirtsByColor = _shirts.GroupBy(s => s.Color.Id).ToDictionary(s => s.Key, s => s.Select(s => s.Id).ToHashSet());
            _shirtsBySize = _shirts.GroupBy(s => s.Size.Id).ToDictionary(s => s.Key, s => s.Select(s => s.Id).ToHashSet());
        }

        public SearchResults Search(SearchOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var coloredShirts = Search(_shirtsByColor, options.Colors.Select(s => s.Id).ToHashSet());
            var sizedShirts = Search(_shirtsBySize, options.Sizes.Select(s => s.Id).ToHashSet());

            var matchedShirts = coloredShirts
                .Intersect(sizedShirts)
                .Select(ms => _shirtsByGuid[ms])
                .ToList();

            var colorCounts = GetColorCount(matchedShirts);
            var sizeCounts = GetSizeCount(matchedShirts);

            return new SearchResults
            {
                Shirts = matchedShirts,
                ColorCounts = colorCounts,
                SizeCounts = sizeCounts
            };
        }

        private static HashSet<Guid> Search(IReadOnlyDictionary<Guid, HashSet<Guid>> shirtsBy, HashSet<Guid> lookupValues)
        {
            if (!lookupValues.Any())
            {
                return shirtsBy
                    .SelectMany(s => s.Value)
                    .ToHashSet();
            }

            var result = new HashSet<Guid>();
            foreach (var lookupValue in lookupValues)
            {
                if (shirtsBy.TryGetValue(lookupValue, out var foundShirtsBy))
                {
                    result.UnionWith(foundShirtsBy);
                }
            }

            return result;
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