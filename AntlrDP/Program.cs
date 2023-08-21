using Antlr4.Runtime;

namespace AntlrDP
{
    class Program
    {
        private static void Main(string[] args)
        {
            var json = File.ReadAllText("files/SimpleLoop.json");
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