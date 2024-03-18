using AntlrDP.OalCodeElements.FragmentTypes;

namespace AntlrDP.OalCodeElements;

public class Statement : OalCodeElement
{
    public List<OalCodeElement> StatementElements = new();
    public required StatementType StatementType { get; init; }
    public bool IsLast = new();
}