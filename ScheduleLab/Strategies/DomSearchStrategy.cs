using System.Text;
using System.Xml;
using ScheduleLab.Models;

namespace ScheduleLab.Strategies;


public class DomSearchStrategy : ISearchStrategy
{
    public string Name => "DOM API (XmlDocument + XPath)";

    public List<Lesson> Search(string xmlPath, SearchCriteria criteria)
    {
        var doc = new XmlDocument();
        doc.Load(xmlPath);

        string xpath = BuildXPath(criteria);
        var result = new List<Lesson>();

        XmlNodeList? nodes = doc.SelectNodes(xpath);
        if (nodes == null) return result;

        foreach (XmlNode node in nodes)
            if (node is XmlElement element)
                result.Add(Map(element));

        return result;
    }

    
    private static string BuildXPath(SearchCriteria c)
    {
        var sb = new StringBuilder("/Schedule/Lesson");
        if (c.Day != null) sb.Append($"[@day={Literal(c.Day)}]");
        if (c.Type != null) sb.Append($"[@type={Literal(c.Type)}]");
        if (c.Subject != null) sb.Append($"[Subject={Literal(c.Subject)}]");
        if (c.Teacher != null) sb.Append($"[Teacher/FullName={Literal(c.Teacher)}]");
        if (c.Department != null) sb.Append($"[Teacher/Department={Literal(c.Department)}]");
        if (c.Group != null) sb.Append($"[Students/@group={Literal(c.Group)}]");
        return sb.ToString();
    }

   
    private static string Literal(string value)
    {
        if (!value.Contains('\'')) return $"'{value}'";
        if (!value.Contains('"')) return $"\"{value}\"";
        return "concat('" + value.Replace("'", "', \"'\", '") + "')";
    }

    private static Lesson Map(XmlElement e)
    {
        var l = new Lesson();
        foreach (XmlAttribute a in e.Attributes)
            l.Attributes[a.Name] = a.Value;

        l.Subject = e["Subject"]?.InnerText ?? "";

        if (e["Teacher"] is XmlElement t)
        {
            l.TeacherName = t["FullName"]?.InnerText ?? "";
            l.Faculty = t["Faculty"]?.InnerText ?? "";
            l.Department = t["Department"]?.InnerText ?? "";
            l.Position = t["Position"]?.InnerText ?? "";
        }
        if (e["Classroom"] is XmlElement room)
        {
            l.ClassroomNumber = room.GetAttribute("number");
            l.Building = room.GetAttribute("building");
            l.Capacity = room.GetAttribute("capacity");
        }
        if (e["Curriculum"] is XmlElement plan)
        {
            l.Semester = plan.GetAttribute("semester");
            l.Hours = plan.GetAttribute("hours");
            l.Control = plan.GetAttribute("control");
        }
        if (e["Students"] is XmlElement st)
        {
            l.Group = st.GetAttribute("group");
            l.Course = st.GetAttribute("course");
            foreach (XmlNode s in st.SelectNodes("Student")!)
                l.Students.Add(s.InnerText);
        }
        return l;
    }
}
