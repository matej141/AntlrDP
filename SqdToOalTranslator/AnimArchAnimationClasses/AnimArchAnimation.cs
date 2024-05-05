namespace SqdToOalTranslator.AnimArchAnimationClasses;

public class AnimArchAnimation
{
    public string Code { get; set; }
    public string AnimationName { get; set; }
    public string StartClass { get; set; }
    public string StartMethod { get; set; }
    public List<AnimationMethodCode> MethodsCodes { get; set; }
}