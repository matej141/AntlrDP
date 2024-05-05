using SqdToOalTranslator.PreOalCodeElements.FragmentTypes;

namespace SqdToOalTranslator.PreOalCodeElements;

public class Statement : PreOalCodeElement
{
    public List<PreOalCodeElement> StatementElements = new();
    public required StatementType StatementType { get; init; }
    public bool IsFirst = new();
    public bool IsLast = new();
}