using System.Xml.Linq;
using ScheduleLab.Models;

namespace ScheduleLab.Strategies;


public class LinqSearchStrategy : ISearchStrategy
{
    public string Name => "LINQ to XML";

    public List<Lesson> Search(string xmlPath, SearchCriteria c)
    {
        XDocument doc = XDocument.Load(xmlPath);
        IEnumerable<XElement> query = doc.Root?.Elements("Lesson") ?? Enumerable.Empty<XElement>();

        if (c.Day != null)
            query = query.Where(x => (string?)x.Attribute("day") == c.Day);
        if (c.Type != null)
            query = query.Where(x => (string?)x.Attribute("type") == c.Type);
        if (c.Subject != null)
            query = query.Where(x => (string?)x.Element("Subject") == c.Subject);
        if (c.Teacher != null)
            query = query.Where(x => (string?)x.Element("Teacher")?.Element("FullName") == c.Teacher);
        if (c.Department != null)
            query = query.Where(x => (string?)x.Element("Teacher")?.Element("Department") == c.Department);
        if (c.Group != null)
            query = query.Where(x => (string?)x.Element("Students")?.Attribute("group") == c.Group);

        return query.Select(Map).ToList();
    }

    private static Lesson Map(XElement e)
    {
        var l = new Lesson();
        foreach (var a in e.Attributes())
            l.Attributes[a.Name.LocalName] = a.Value;

        l.Subject = (string?)e.Element("Subject") ?? "";

        var t = e.Element("Teacher");
        l.TeacherName = (string?)t?.Element("FullName") ?? "";
        l.Faculty = (string?)t?.Element("Faculty") ?? "";
        l.Department = (string?)t?.Element("Department") ?? "";
        l.Position = (string?)t?.Element("Position") ?? "";

        var room = e.Element("Classroom");
        l.ClassroomNumber = (string?)room?.Attribute("number") ?? "";
        l.Building = (string?)room?.Attribute("building") ?? "";
        l.Capacity = (string?)room?.Attribute("capacity") ?? "";

        var plan = e.Element("Curriculum");
        l.Semester = (string?)plan?.Attribute("semester") ?? "";
        l.Hours = (string?)plan?.Attribute("hours") ?? "";
        l.Control = (string?)plan?.Attribute("control") ?? "";

        var st = e.Element("Students");
        l.Group = (string?)st?.Attribute("group") ?? "";
        l.Course = (string?)st?.Attribute("course") ?? "";
        if (st != null)
            l.Students.AddRange(st.Elements("Student").Select(s => s.Value));

        return l;
    }
}
