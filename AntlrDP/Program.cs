using Antlr4.Runtime;

namespace AntlrDP
{
    // inspiracia z: https://tomassetti.me/getting-started-with-antlr-in-csharp/
    class Program
    {
        private static void Main(string[] args)
        {
            var json = File.ReadAllText("files/HelloMessageImproved1.json");
            var inputStream = new AntlrInputStream(json);
            var speakLexer = new SequenceDiagramLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(speakLexer);
            
            var sequenceDiagramParser = new SequenceDiagramParser(commonTokenStream);
            var context = sequenceDiagramParser.json();
            var visitor = new SequenceDiagramRepairedVisitor();
            visitor.Visit(context);
            
            var jsonGenerated =
                Newtonsoft.Json.JsonConvert.SerializeObject(visitor.OalProgram.CreateAnimArchAnimationObject());
            Console.Write(jsonGenerated);
            File.WriteAllText("files/AntlrGeneratedAnim.json", jsonGenerated);
        }
    }
}