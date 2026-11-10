using System.Text;

namespace ScheduleLab.Models;

public class Lesson
{
    
    public Dictionary<string, string> Attributes { get; } = new();

    public string Subject { get; set; } = "";
    public string TeacherName { get; set; } = "";
    public string Faculty { get; set; } = "";
    public string Department { get; set; } = "";
    public string Position { get; set; } = "";
    public string ClassroomNumber { get; set; } = "";
    public string Building { get; set; } = "";
    public string Capacity { get; set; } = "";
    public string Semester { get; set; } = "";
    public string Hours { get; set; } = "";
    public string Control { get; set; } = "";
    public string Group { get; set; } = "";
    public string Course { get; set; } = "";
    public List<string> Students { get; } = new();

    public string Id => GetAttribute("id");

    public string GetAttribute(string name) =>
        Attributes.TryGetValue(name, out var value) ? value : "";

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Заняття №{Id}: {GetAttribute("day")}, {GetAttribute("time")}");
        sb.AppendLine($"  Дисципліна: {Subject}");
        sb.AppendLine($"  Викладач: {TeacherName} ({Position})");
        sb.AppendLine($"  Факультет: {Faculty}");
        sb.AppendLine($"  Кафедра: {Department}");
        sb.AppendLine($"  Аудиторія: {ClassroomNumber}, {Building} (місць: {Capacity})");
        sb.AppendLine($"  Навчальний план: семестр {Semester}, {Hours} год., {Control}");
        sb.AppendLine($"  Група: {Group} ({Course} курс)");
        sb.AppendLine($"  Студенти: {string.Join(", ", Students)}");
        sb.AppendLine($"  Атрибути ({Attributes.Count}): " +
                      string.Join("; ", Attributes.Select(a => $"{a.Key}={a.Value}")));
        return sb.ToString();
    }
}
