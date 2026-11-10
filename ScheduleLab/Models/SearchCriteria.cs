namespace ScheduleLab.Models;

public class SearchCriteria
{
    public string? Day { get; set; }
    public string? Type { get; set; }
    public string? Subject { get; set; }
    public string? Teacher { get; set; }
    public string? Department { get; set; }
    public string? Group { get; set; }

  
    public bool Matches(Lesson l) =>
        (Day == null || l.GetAttribute("day") == Day) &&
        (Type == null || l.GetAttribute("type") == Type) &&
        (Subject == null || l.Subject == Subject) &&
        (Teacher == null || l.TeacherName == Teacher) &&
        (Department == null || l.Department == Department) &&
        (Group == null || l.Group == Group);

  
    public override string ToString()
    {
        var parts = new List<string>();
        if (Day != null) parts.Add($"«День» – {Day}");
        if (Type != null) parts.Add($"«Тип» – {Type}");
        if (Subject != null) parts.Add($"«Дисципліна» – {Subject}");
        if (Teacher != null) parts.Add($"«Викладач» – {Teacher}");
        if (Department != null) parts.Add($"«Кафедра» – {Department}");
        if (Group != null) parts.Add($"«Група» – {Group}");
        return parts.Count == 0 ? "не задані (усі заняття)" : string.Join(", ", parts);
    }
}
