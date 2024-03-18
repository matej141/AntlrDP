using Antlr4.Runtime;
using AntlrDP.OalCodeElements;
using AntlrDP.Translation;

namespace AntlrDP
{
    class Program
    {
        private static void Main(string[] args)
        {
            var json = File.ReadAllText("files/FowlerUMLDistilled.json");
            var inputStream = new AntlrInputStream(json);
            var speakLexer = new SequenceDiagramLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(speakLexer);
            
            var sequenceDiagramParser = new SequenceDiagramParser(commonTokenStream);
            var context = sequenceDiagramParser.json();
            var visitor = new SequenceDiagramRepairedArchivedVisitor();
            visitor.Visit(context);
            
            // var jsonGenerated =
            //     Newtonsoft.Json.JsonConvert.SerializeObject(visitor.OalProgram.CreateAnimArchAnimationObject());
            // Console.Write(jsonGenerated);
            // File.WriteAllText("files/AntlrGeneratedAnim.json", jsonGenerated);

            var newVisitor = new SequenceDiagramCustomVisitor();
            newVisitor.Visit(context);
            var sequenceDiagram = newVisitor.SequenceDiagram;

            var oalCode = new OalCode(sequenceDiagram);
            var translator = new Translator(oalCode);
            var animArchAnimationObject = translator.CreateAnimArchAnimationObject();
            var jsonGenerated =
                Newtonsoft.Json.JsonConvert.SerializeObject(animArchAnimationObject);
            Console.Write(jsonGenerated);
            File.WriteAllText("files/AntlrGeneratedAnim.json", jsonGenerated);
            var oalCodeTxt = translator.GetCompleteOALCode();
            File.WriteAllText("files/AntlrGeneratedAnim.json", oalCodeTxt);
        }
    }
}