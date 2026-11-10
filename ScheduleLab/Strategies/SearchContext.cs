using ScheduleLab.Models;

namespace ScheduleLab.Strategies;

public class SearchContext
{
    private ISearchStrategy _strategy;

    public SearchContext(ISearchStrategy strategy) => _strategy = strategy;

    public void SetStrategy(ISearchStrategy strategy) => _strategy = strategy;

    public List<Lesson> ExecuteSearch(string xmlPath, SearchCriteria criteria) =>
        _strategy.Search(xmlPath, criteria);
}
