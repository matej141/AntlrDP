//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.11.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/ciern/RiderProjects/ANTLR_DP/AntlrDP/AntlrDP/AntlrDP\SequenceDiagram.g4 by ANTLR 4.11.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public partial class SequenceDiagramLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, STRING=19, NUMBER=20, WS=21;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"T__17", "STRING", "ESC", "UNICODE", "HEX", "SAFECODEPOINT", "NUMBER", 
		"INT", "EXP", "WS"
	};


	public SequenceDiagramLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public SequenceDiagramLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'{'", "','", "'}'", "':'", "'['", "']'", "'true'", "'false'", "'null'", 
		"'\"name\"'", "'\"XmiId\"'", "'\"XmiIdRef\"'", "'\"uml:Lifeline\"'", "'\"uml:Message\"'", 
		"'\"uml:OccurrenceSpecification\"'", "'\"covered\"'", "'\"receiveEvent\"'", 
		"'\"sendEvent\"'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, "STRING", "NUMBER", "WS"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "SequenceDiagram.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static SequenceDiagramLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,21,271,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,1,0,1,0,1,1,1,1,
		1,2,1,2,1,3,1,3,1,4,1,4,1,5,1,5,1,6,1,6,1,6,1,6,1,6,1,7,1,7,1,7,1,7,1,
		7,1,7,1,8,1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,1,9,1,9,1,10,1,10,1,10,1,
		10,1,10,1,10,1,10,1,10,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,11,1,
		11,1,11,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,12,1,
		12,1,12,1,12,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,13,1,
		13,1,13,1,13,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,
		14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,14,1,
		14,1,14,1,14,1,14,1,14,1,15,1,15,1,15,1,15,1,15,1,15,1,15,1,15,1,15,1,
		15,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,16,1,
		16,1,16,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,17,1,
		18,1,18,1,18,5,18,209,8,18,10,18,12,18,212,9,18,1,18,1,18,1,19,1,19,1,
		19,3,19,219,8,19,1,20,1,20,1,20,1,20,1,20,1,20,1,21,1,21,1,22,1,22,1,23,
		3,23,232,8,23,1,23,1,23,1,23,4,23,237,8,23,11,23,12,23,238,3,23,241,8,
		23,1,23,3,23,244,8,23,1,24,1,24,1,24,5,24,249,8,24,10,24,12,24,252,9,24,
		3,24,254,8,24,1,25,1,25,3,25,258,8,25,1,25,4,25,261,8,25,11,25,12,25,262,
		1,26,4,26,266,8,26,11,26,12,26,267,1,26,1,26,0,0,27,1,1,3,2,5,3,7,4,9,
		5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,
		35,18,37,19,39,0,41,0,43,0,45,0,47,20,49,0,51,0,53,21,1,0,8,8,0,34,34,
		47,47,92,92,98,98,102,102,110,110,114,114,116,116,3,0,48,57,65,70,97,102,
		3,0,0,31,34,34,92,92,1,0,48,57,1,0,49,57,2,0,69,69,101,101,2,0,43,43,45,
		45,3,0,9,10,13,13,32,32,276,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,
		0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,
		0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,
		1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,47,1,0,0,
		0,0,53,1,0,0,0,1,55,1,0,0,0,3,57,1,0,0,0,5,59,1,0,0,0,7,61,1,0,0,0,9,63,
		1,0,0,0,11,65,1,0,0,0,13,67,1,0,0,0,15,72,1,0,0,0,17,78,1,0,0,0,19,83,
		1,0,0,0,21,90,1,0,0,0,23,98,1,0,0,0,25,109,1,0,0,0,27,124,1,0,0,0,29,138,
		1,0,0,0,31,168,1,0,0,0,33,178,1,0,0,0,35,193,1,0,0,0,37,205,1,0,0,0,39,
		215,1,0,0,0,41,220,1,0,0,0,43,226,1,0,0,0,45,228,1,0,0,0,47,231,1,0,0,
		0,49,253,1,0,0,0,51,255,1,0,0,0,53,265,1,0,0,0,55,56,5,123,0,0,56,2,1,
		0,0,0,57,58,5,44,0,0,58,4,1,0,0,0,59,60,5,125,0,0,60,6,1,0,0,0,61,62,5,
		58,0,0,62,8,1,0,0,0,63,64,5,91,0,0,64,10,1,0,0,0,65,66,5,93,0,0,66,12,
		1,0,0,0,67,68,5,116,0,0,68,69,5,114,0,0,69,70,5,117,0,0,70,71,5,101,0,
		0,71,14,1,0,0,0,72,73,5,102,0,0,73,74,5,97,0,0,74,75,5,108,0,0,75,76,5,
		115,0,0,76,77,5,101,0,0,77,16,1,0,0,0,78,79,5,110,0,0,79,80,5,117,0,0,
		80,81,5,108,0,0,81,82,5,108,0,0,82,18,1,0,0,0,83,84,5,34,0,0,84,85,5,110,
		0,0,85,86,5,97,0,0,86,87,5,109,0,0,87,88,5,101,0,0,88,89,5,34,0,0,89,20,
		1,0,0,0,90,91,5,34,0,0,91,92,5,88,0,0,92,93,5,109,0,0,93,94,5,105,0,0,
		94,95,5,73,0,0,95,96,5,100,0,0,96,97,5,34,0,0,97,22,1,0,0,0,98,99,5,34,
		0,0,99,100,5,88,0,0,100,101,5,109,0,0,101,102,5,105,0,0,102,103,5,73,0,
		0,103,104,5,100,0,0,104,105,5,82,0,0,105,106,5,101,0,0,106,107,5,102,0,
		0,107,108,5,34,0,0,108,24,1,0,0,0,109,110,5,34,0,0,110,111,5,117,0,0,111,
		112,5,109,0,0,112,113,5,108,0,0,113,114,5,58,0,0,114,115,5,76,0,0,115,
		116,5,105,0,0,116,117,5,102,0,0,117,118,5,101,0,0,118,119,5,108,0,0,119,
		120,5,105,0,0,120,121,5,110,0,0,121,122,5,101,0,0,122,123,5,34,0,0,123,
		26,1,0,0,0,124,125,5,34,0,0,125,126,5,117,0,0,126,127,5,109,0,0,127,128,
		5,108,0,0,128,129,5,58,0,0,129,130,5,77,0,0,130,131,5,101,0,0,131,132,
		5,115,0,0,132,133,5,115,0,0,133,134,5,97,0,0,134,135,5,103,0,0,135,136,
		5,101,0,0,136,137,5,34,0,0,137,28,1,0,0,0,138,139,5,34,0,0,139,140,5,117,
		0,0,140,141,5,109,0,0,141,142,5,108,0,0,142,143,5,58,0,0,143,144,5,79,
		0,0,144,145,5,99,0,0,145,146,5,99,0,0,146,147,5,117,0,0,147,148,5,114,
		0,0,148,149,5,114,0,0,149,150,5,101,0,0,150,151,5,110,0,0,151,152,5,99,
		0,0,152,153,5,101,0,0,153,154,5,83,0,0,154,155,5,112,0,0,155,156,5,101,
		0,0,156,157,5,99,0,0,157,158,5,105,0,0,158,159,5,102,0,0,159,160,5,105,
		0,0,160,161,5,99,0,0,161,162,5,97,0,0,162,163,5,116,0,0,163,164,5,105,
		0,0,164,165,5,111,0,0,165,166,5,110,0,0,166,167,5,34,0,0,167,30,1,0,0,
		0,168,169,5,34,0,0,169,170,5,99,0,0,170,171,5,111,0,0,171,172,5,118,0,
		0,172,173,5,101,0,0,173,174,5,114,0,0,174,175,5,101,0,0,175,176,5,100,
		0,0,176,177,5,34,0,0,177,32,1,0,0,0,178,179,5,34,0,0,179,180,5,114,0,0,
		180,181,5,101,0,0,181,182,5,99,0,0,182,183,5,101,0,0,183,184,5,105,0,0,
		184,185,5,118,0,0,185,186,5,101,0,0,186,187,5,69,0,0,187,188,5,118,0,0,
		188,189,5,101,0,0,189,190,5,110,0,0,190,191,5,116,0,0,191,192,5,34,0,0,
		192,34,1,0,0,0,193,194,5,34,0,0,194,195,5,115,0,0,195,196,5,101,0,0,196,
		197,5,110,0,0,197,198,5,100,0,0,198,199,5,69,0,0,199,200,5,118,0,0,200,
		201,5,101,0,0,201,202,5,110,0,0,202,203,5,116,0,0,203,204,5,34,0,0,204,
		36,1,0,0,0,205,210,5,34,0,0,206,209,3,39,19,0,207,209,3,45,22,0,208,206,
		1,0,0,0,208,207,1,0,0,0,209,212,1,0,0,0,210,208,1,0,0,0,210,211,1,0,0,
		0,211,213,1,0,0,0,212,210,1,0,0,0,213,214,5,34,0,0,214,38,1,0,0,0,215,
		218,5,92,0,0,216,219,7,0,0,0,217,219,3,41,20,0,218,216,1,0,0,0,218,217,
		1,0,0,0,219,40,1,0,0,0,220,221,5,117,0,0,221,222,3,43,21,0,222,223,3,43,
		21,0,223,224,3,43,21,0,224,225,3,43,21,0,225,42,1,0,0,0,226,227,7,1,0,
		0,227,44,1,0,0,0,228,229,8,2,0,0,229,46,1,0,0,0,230,232,5,45,0,0,231,230,
		1,0,0,0,231,232,1,0,0,0,232,233,1,0,0,0,233,240,3,49,24,0,234,236,5,46,
		0,0,235,237,7,3,0,0,236,235,1,0,0,0,237,238,1,0,0,0,238,236,1,0,0,0,238,
		239,1,0,0,0,239,241,1,0,0,0,240,234,1,0,0,0,240,241,1,0,0,0,241,243,1,
		0,0,0,242,244,3,51,25,0,243,242,1,0,0,0,243,244,1,0,0,0,244,48,1,0,0,0,
		245,254,5,48,0,0,246,250,7,4,0,0,247,249,7,3,0,0,248,247,1,0,0,0,249,252,
		1,0,0,0,250,248,1,0,0,0,250,251,1,0,0,0,251,254,1,0,0,0,252,250,1,0,0,
		0,253,245,1,0,0,0,253,246,1,0,0,0,254,50,1,0,0,0,255,257,7,5,0,0,256,258,
		7,6,0,0,257,256,1,0,0,0,257,258,1,0,0,0,258,260,1,0,0,0,259,261,7,3,0,
		0,260,259,1,0,0,0,261,262,1,0,0,0,262,260,1,0,0,0,262,263,1,0,0,0,263,
		52,1,0,0,0,264,266,7,7,0,0,265,264,1,0,0,0,266,267,1,0,0,0,267,265,1,0,
		0,0,267,268,1,0,0,0,268,269,1,0,0,0,269,270,6,26,0,0,270,54,1,0,0,0,13,
		0,208,210,218,231,238,240,243,250,253,257,262,267,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
