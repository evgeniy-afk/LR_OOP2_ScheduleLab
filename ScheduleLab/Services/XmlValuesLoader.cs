using System.Xml.Linq;

namespace ScheduleLab.Services;

public class ScheduleValues
{
    public List<string> Days { get; init; } = new();
    public List<string> Types { get; init; } = new();
    public List<string> Subjects { get; init; } = new();
    public List<string> Teachers { get; init; } = new();
    public List<string> Departments { get; init; } = new();
    public List<string> Groups { get; init; } = new();
}


public class XmlValuesLoader
{
    public ScheduleValues Load(string xmlPath)
    {
        XDocument doc = XDocument.Load(xmlPath);
        var lessons = doc.Root?.Elements("Lesson").ToList() ?? new List<XElement>();
        if (lessons.Count == 0)
            throw new InvalidDataException("У файлі не знайдено жодного елемента <Lesson>.");

        return new ScheduleValues
        {
            Days = Unique(lessons.Select(l => (string?)l.Attribute("day"))),
            Types = Unique(lessons.Select(l => (string?)l.Attribute("type"))),
            Subjects = Unique(lessons.Select(l => (string?)l.Element("Subject"))),
            Teachers = Unique(lessons.Select(l => (string?)l.Element("Teacher")?.Element("FullName"))),
            Departments = Unique(lessons.Select(l => (string?)l.Element("Teacher")?.Element("Department"))),
            Groups = Unique(lessons.Select(l => (string?)l.Element("Students")?.Attribute("group")))
        };
    }

    private static List<string> Unique(IEnumerable<string?> values) =>
        values.Where(v => !string.IsNullOrWhiteSpace(v))
              .Select(v => v!.Trim())
              .Distinct()
              .ToList();
}
