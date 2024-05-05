namespace SqdToOalTranslator.Builder;

public interface ISqdToOalTranslatorBuilder
{
    void ReadJson(string path);
    void ParseSequenceDiagram();
    void VisitParsedTreeOfSequenceDiagramElements();
    void TranslateSqdToOalCode();
    void CreateAnimationFile(string path);
    void CreateFileWithOalCode(string path);
}