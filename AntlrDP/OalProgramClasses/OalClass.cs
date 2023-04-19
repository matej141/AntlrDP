namespace AntlrDP;

public class OalClass
{
    public string Id { get; set; }
    public string Name { get; set; }

    public readonly List<OalClassMethod> OalClassMethods = new();
    
}