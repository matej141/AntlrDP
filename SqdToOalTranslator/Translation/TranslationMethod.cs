namespace SqdToOalTranslator.Translation;

public class TranslationMethod
{
    public string Name { get; set; }
    public string Code { get;  set; }
    public List<TranslationClass> Instances = new();
    public bool IsSelfMethod = new ();
    
    
    public TranslationMethod()
    {
    }
    
    public TranslationMethod(TranslationMethod original)
    {
        Name = original.Name;
        Code = original.Code;
        Instances = new List<TranslationClass>(original.Instances);
        IsSelfMethod = original.IsSelfMethod;
    }
}