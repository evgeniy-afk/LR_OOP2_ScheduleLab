using System.Xml;
using ScheduleLab.Models;

namespace ScheduleLab.Strategies;

public class SaxSearchStrategy : ISearchStrategy
{
    public string Name => "SAX API (XmlReader)";

    public List<Lesson> Search(string xmlPath, SearchCriteria criteria)
    {
        var result = new List<Lesson>();
        var settings = new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true };

        using var reader = XmlReader.Create(xmlPath, settings);
        Lesson? current = null;
        string currentElement = "";

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    currentElement = reader.Name;
                    if (reader.Name == "Lesson")
                    {
                        current = new Lesson();
                        while (reader.MoveToNextAttribute())
                            current.Attributes[reader.Name] = reader.Value;
                        reader.MoveToElement();
                    }
                    else if (current != null)
                    {
                        ReadElementAttributes(reader, current);
                    }
                    break;

                case XmlNodeType.Text:
                    if (current != null)
                        ReadText(currentElement, reader.Value, current);
                    break;

                case XmlNodeType.EndElement:
                    if (reader.Name == "Lesson" && current != null)
                    {
                        if (criteria.Matches(current))
                            result.Add(current);
                        current = null;
                    }
                    break;
            }
        }
        return result;
    }

    private static void ReadElementAttributes(XmlReader r, Lesson l)
    {
        switch (r.Name)
        {
            case "Classroom":
                l.ClassroomNumber = r.GetAttribute("number") ?? "";
                l.Building = r.GetAttribute("building") ?? "";
                l.Capacity = r.GetAttribute("capacity") ?? "";
                break;
            case "Curriculum":
                l.Semester = r.GetAttribute("semester") ?? "";
                l.Hours = r.GetAttribute("hours") ?? "";
                l.Control = r.GetAttribute("control") ?? "";
                break;
            case "Students":
                l.Group = r.GetAttribute("group") ?? "";
                l.Course = r.GetAttribute("course") ?? "";
                break;
        }
    }

    private static void ReadText(string element, string value, Lesson l)
    {
        switch (element)
        {
            case "Subject": l.Subject = value; break;
            case "FullName": l.TeacherName = value; break;
            case "Faculty": l.Faculty = value; break;
            case "Department": l.Department = value; break;
            case "Position": l.Position = value; break;
            case "Student": l.Students.Add(value); break;
        }
    }
}
