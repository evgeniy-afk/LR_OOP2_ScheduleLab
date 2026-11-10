using ScheduleLab.Models;

namespace ScheduleLab.Strategies;

public interface ISearchStrategy
{
    string Name { get; }
    List<Lesson> Search(string xmlPath, SearchCriteria criteria);
}
