using System.Text;
using System.Xml;
using System.Xml.Linq;
using ScheduleLab.Services;

namespace ScheduleLab.Saving;


public interface IFragmentWriter
{
    void Write(XDocument fragment, Stream output);
}

public class XmlFragmentWriter : IFragmentWriter
{
    public void Write(XDocument fragment, Stream output)
    {
        var settings = new XmlWriterSettings { Encoding = new UTF8Encoding(false), Indent = true };
        using var writer = XmlWriter.Create(output, settings);
        fragment.Save(writer);
    }
}

public class HtmlFragmentWriter : IFragmentWriter
{
    private readonly HtmlTransformer _transformer;
    private readonly string _xslPath;

    public HtmlFragmentWriter(HtmlTransformer transformer, string xslPath)
    {
        _transformer = transformer;
        _xslPath = xslPath;
    }

    public void Write(XDocument fragment, Stream output) =>
        _transformer.Transform(fragment, _xslPath, output);
}
