using SqdToOalTranslator.Builder;

namespace SqdToOalTranslator;

internal abstract class Program
{
    private static void Main()
    {
        var builder = new SqdToOalTranslatorBuilder(selfMessagesAreEmpty: false);
        var director = new SqdToOalTranslatorDirector(builder);
        director.Construct("files/FowlerUMLDistilled.json", "files/AntlrGeneratedAnim.json", "files/OalGenerated.txt");
    }
}