using System.Xml.Linq;
using ScheduleLab.Services;

namespace ScheduleLab.Saving;


public abstract class FragmentSaver
{
    private readonly GoogleDriveService _drive;

    protected FragmentSaver(GoogleDriveService drive) => _drive = drive;

    public abstract string Extension { get; }
    protected abstract string MimeType { get; }

    protected abstract IFragmentWriter CreateWriter();

    public async Task<string> SaveAsync(XDocument fragment, string fileName)
    {
        string fullName = fileName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase)
            ? fileName
            : fileName + Extension;

        
        byte[] content;
        using (var file = new MemoryStream())
        {
            IFragmentWriter writer = CreateWriter();
            writer.Write(fragment, file);
            content = file.ToArray();
        }

        using var upload = new MemoryStream(content);
        await _drive.UploadAsync(fullName, upload, MimeType);
        return fullName;
    }
}

public class XmlDriveSaver : FragmentSaver
{
    public XmlDriveSaver(GoogleDriveService drive) : base(drive) { }

    public override string Extension => ".xml";
    protected override string MimeType => "application/xml";
    protected override IFragmentWriter CreateWriter() => new XmlFragmentWriter();
}

public class HtmlDriveSaver : FragmentSaver
{
    private readonly HtmlTransformer _transformer;
    private readonly string _xslPath;

    public HtmlDriveSaver(GoogleDriveService drive, HtmlTransformer transformer, string xslPath)
        : base(drive)
    {
        _transformer = transformer;
        _xslPath = xslPath;
    }

    public override string Extension => ".html";
    protected override string MimeType => "text/html";
    protected override IFragmentWriter CreateWriter() => new HtmlFragmentWriter(_transformer, _xslPath);
}
