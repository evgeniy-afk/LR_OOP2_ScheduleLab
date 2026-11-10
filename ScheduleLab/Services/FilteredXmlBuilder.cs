using System.Xml.Linq;
using ScheduleLab.Models;

namespace ScheduleLab.Services;

public static class FilteredXmlBuilder
{
   
    public static XDocument Build(string xmlPath, IEnumerable<Lesson>? found)
    {
        XDocument doc = XDocument.Load(xmlPath);
        if (found == null) return doc;

        var ids = found.Select(l => l.Id).ToHashSet();
        doc.Root?.Elements("Lesson")
            .Where(e => !ids.Contains((string?)e.Attribute("id") ?? ""))
            .Remove();
        return doc;
    }
}
