namespace AntlrDP.Translation;

public class TranslationContext
{
    public List<string> MethodNames = new();
    public TranslationClass Sender { get; set; }
    public TranslationClass Receiver { get; set; }
    public string CurrentCode { get; set; }
    public List<TranslationClass> Instances = new();
    public List<string> LastMethodsCalled = new();

    public TranslationContext()
    {
    }

    public TranslationContext(TranslationContext original)
    {
        MethodNames = original.MethodNames;
        Sender = original.Sender;
        Receiver = original.Receiver;
        CurrentCode = original.CurrentCode;
        Instances = new List<TranslationClass>(original.Instances);
        LastMethodsCalled = new List<string>(original.LastMethodsCalled);
    }
}