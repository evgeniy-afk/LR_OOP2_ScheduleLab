using System.Text;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ScheduleLab.Services;

public class HtmlTransformer
{
    public void Transform(XDocument xml, string xslPath, Stream output)
    {
        var xslt = new XslCompiledTransform();
        xslt.Load(xslPath);
        using var reader = xml.CreateReader();
        xslt.Transform(reader, null, output);
    }

    public string TransformToString(XDocument xml, string xslPath)
    {
        using var ms = new MemoryStream();
        Transform(xml, xslPath, ms);
        return Encoding.UTF8.GetString(ms.ToArray()).TrimStart('\uFEFF');
    }
}
