namespace SqdToOalTranslator.Builder;

public class SqdToOalTranslatorDirector
{
    private ISqdToOalTranslatorBuilder _builder;

    public SqdToOalTranslatorDirector(ISqdToOalTranslatorBuilder builder)
    {
        _builder = builder;
    }

    public void Construct(string jsonInputFilePath, string animationFileOutputPath, string oalFileOutputPath)
    {
        _builder.ReadJson(jsonInputFilePath);
        _builder.ParseSequenceDiagram();
        _builder.VisitParsedTreeOfSequenceDiagramElements();
        _builder.TranslateSqdToOalCode();
        _builder.CreateAnimationFile(animationFileOutputPath);
        _builder.CreateFileWithOalCode(oalFileOutputPath);
    }
}