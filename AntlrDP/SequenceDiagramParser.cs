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
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public partial class SequenceDiagramParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, STRING=19, NUMBER=20, WS=21;
	public const int
		RULE_json = 0, RULE_obj = 1, RULE_pair = 2, RULE_arr = 3, RULE_value = 4, 
		RULE_name = 5, RULE_xmiId = 6, RULE_xmiIdRef = 7, RULE_lifeline = 8, RULE_message = 9, 
		RULE_occurenceSpecification = 10, RULE_covered = 11, RULE_receiveEvent = 12, 
		RULE_sendEvent = 13;
	public static readonly string[] ruleNames = {
		"json", "obj", "pair", "arr", "value", "name", "xmiId", "xmiIdRef", "lifeline", 
		"message", "occurenceSpecification", "covered", "receiveEvent", "sendEvent"
	};

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

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static SequenceDiagramParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public SequenceDiagramParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public SequenceDiagramParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class JsonContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(SequenceDiagramParser.Eof, 0); }
		public JsonContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_json; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitJson(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public JsonContext json() {
		JsonContext _localctx = new JsonContext(Context, State);
		EnterRule(_localctx, 0, RULE_json);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 28;
			value();
			State = 29;
			Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ObjContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public PairContext[] pair() {
			return GetRuleContexts<PairContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public PairContext pair(int i) {
			return GetRuleContext<PairContext>(i);
		}
		public ObjContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_obj; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitObj(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ObjContext obj() {
		ObjContext _localctx = new ObjContext(Context, State);
		EnterRule(_localctx, 2, RULE_obj);
		int _la;
		try {
			State = 44;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 31;
				Match(T__0);
				State = 32;
				pair();
				State = 37;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 33;
					Match(T__1);
					State = 34;
					pair();
					}
					}
					State = 39;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				State = 40;
				Match(T__2);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 42;
				Match(T__0);
				State = 43;
				Match(T__2);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class PairContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(SequenceDiagramParser.STRING, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public NameContext name() {
			return GetRuleContext<NameContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public LifelineContext lifeline() {
			return GetRuleContext<LifelineContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public XmiIdContext xmiId() {
			return GetRuleContext<XmiIdContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public XmiIdRefContext xmiIdRef() {
			return GetRuleContext<XmiIdRefContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public MessageContext message() {
			return GetRuleContext<MessageContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public OccurenceSpecificationContext occurenceSpecification() {
			return GetRuleContext<OccurenceSpecificationContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public CoveredContext covered() {
			return GetRuleContext<CoveredContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ReceiveEventContext receiveEvent() {
			return GetRuleContext<ReceiveEventContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public SendEventContext sendEvent() {
			return GetRuleContext<SendEventContext>(0);
		}
		public PairContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_pair; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitPair(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public PairContext pair() {
		PairContext _localctx = new PairContext(Context, State);
		EnterRule(_localctx, 4, RULE_pair);
		try {
			State = 58;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,2,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 46;
				Match(STRING);
				State = 47;
				Match(T__3);
				State = 48;
				value();
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 49;
				name();
				}
				break;
			case 3:
				EnterOuterAlt(_localctx, 3);
				{
				State = 50;
				lifeline();
				}
				break;
			case 4:
				EnterOuterAlt(_localctx, 4);
				{
				State = 51;
				xmiId();
				}
				break;
			case 5:
				EnterOuterAlt(_localctx, 5);
				{
				State = 52;
				xmiIdRef();
				}
				break;
			case 6:
				EnterOuterAlt(_localctx, 6);
				{
				State = 53;
				message();
				}
				break;
			case 7:
				EnterOuterAlt(_localctx, 7);
				{
				State = 54;
				occurenceSpecification();
				}
				break;
			case 8:
				EnterOuterAlt(_localctx, 8);
				{
				State = 55;
				covered();
				}
				break;
			case 9:
				EnterOuterAlt(_localctx, 9);
				{
				State = 56;
				receiveEvent();
				}
				break;
			case 10:
				EnterOuterAlt(_localctx, 10);
				{
				State = 57;
				sendEvent();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ArrContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext[] value() {
			return GetRuleContexts<ValueContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value(int i) {
			return GetRuleContext<ValueContext>(i);
		}
		public ArrContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_arr; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitArr(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ArrContext arr() {
		ArrContext _localctx = new ArrContext(Context, State);
		EnterRule(_localctx, 6, RULE_arr);
		int _la;
		try {
			State = 73;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,4,Context) ) {
			case 1:
				EnterOuterAlt(_localctx, 1);
				{
				State = 60;
				Match(T__4);
				State = 61;
				value();
				State = 66;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 62;
					Match(T__1);
					State = 63;
					value();
					}
					}
					State = 68;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				State = 69;
				Match(T__5);
				}
				break;
			case 2:
				EnterOuterAlt(_localctx, 2);
				{
				State = 71;
				Match(T__4);
				State = 72;
				Match(T__5);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ValueContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(SequenceDiagramParser.STRING, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMBER() { return GetToken(SequenceDiagramParser.NUMBER, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ObjContext obj() {
			return GetRuleContext<ObjContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ArrContext arr() {
			return GetRuleContext<ArrContext>(0);
		}
		public ValueContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_value; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitValue(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ValueContext value() {
		ValueContext _localctx = new ValueContext(Context, State);
		EnterRule(_localctx, 8, RULE_value);
		try {
			State = 82;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case STRING:
				EnterOuterAlt(_localctx, 1);
				{
				State = 75;
				Match(STRING);
				}
				break;
			case NUMBER:
				EnterOuterAlt(_localctx, 2);
				{
				State = 76;
				Match(NUMBER);
				}
				break;
			case T__0:
				EnterOuterAlt(_localctx, 3);
				{
				State = 77;
				obj();
				}
				break;
			case T__4:
				EnterOuterAlt(_localctx, 4);
				{
				State = 78;
				arr();
				}
				break;
			case T__6:
				EnterOuterAlt(_localctx, 5);
				{
				State = 79;
				Match(T__6);
				}
				break;
			case T__7:
				EnterOuterAlt(_localctx, 6);
				{
				State = 80;
				Match(T__7);
				}
				break;
			case T__8:
				EnterOuterAlt(_localctx, 7);
				{
				State = 81;
				Match(T__8);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class NameContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public NameContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_name; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitName(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public NameContext name() {
		NameContext _localctx = new NameContext(Context, State);
		EnterRule(_localctx, 10, RULE_name);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 84;
			Match(T__9);
			State = 85;
			Match(T__3);
			State = 86;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class XmiIdContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public XmiIdContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_xmiId; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitXmiId(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public XmiIdContext xmiId() {
		XmiIdContext _localctx = new XmiIdContext(Context, State);
		EnterRule(_localctx, 12, RULE_xmiId);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 88;
			Match(T__10);
			State = 89;
			Match(T__3);
			State = 90;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class XmiIdRefContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public XmiIdRefContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_xmiIdRef; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitXmiIdRef(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public XmiIdRefContext xmiIdRef() {
		XmiIdRefContext _localctx = new XmiIdRefContext(Context, State);
		EnterRule(_localctx, 14, RULE_xmiIdRef);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 92;
			Match(T__11);
			State = 93;
			Match(T__3);
			State = 94;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LifelineContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(SequenceDiagramParser.STRING, 0); }
		public LifelineContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_lifeline; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitLifeline(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public LifelineContext lifeline() {
		LifelineContext _localctx = new LifelineContext(Context, State);
		EnterRule(_localctx, 16, RULE_lifeline);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 96;
			Match(STRING);
			State = 97;
			Match(T__3);
			State = 98;
			Match(T__12);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class MessageContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(SequenceDiagramParser.STRING, 0); }
		public MessageContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_message; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitMessage(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public MessageContext message() {
		MessageContext _localctx = new MessageContext(Context, State);
		EnterRule(_localctx, 18, RULE_message);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 100;
			Match(STRING);
			State = 101;
			Match(T__3);
			State = 102;
			Match(T__13);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class OccurenceSpecificationContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING() { return GetToken(SequenceDiagramParser.STRING, 0); }
		public OccurenceSpecificationContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_occurenceSpecification; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitOccurenceSpecification(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public OccurenceSpecificationContext occurenceSpecification() {
		OccurenceSpecificationContext _localctx = new OccurenceSpecificationContext(Context, State);
		EnterRule(_localctx, 20, RULE_occurenceSpecification);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 104;
			Match(STRING);
			State = 105;
			Match(T__3);
			State = 106;
			Match(T__14);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class CoveredContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public CoveredContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_covered; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitCovered(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public CoveredContext covered() {
		CoveredContext _localctx = new CoveredContext(Context, State);
		EnterRule(_localctx, 22, RULE_covered);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 108;
			Match(T__15);
			State = 109;
			Match(T__3);
			State = 110;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ReceiveEventContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public ReceiveEventContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_receiveEvent; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitReceiveEvent(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ReceiveEventContext receiveEvent() {
		ReceiveEventContext _localctx = new ReceiveEventContext(Context, State);
		EnterRule(_localctx, 24, RULE_receiveEvent);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 112;
			Match(T__16);
			State = 113;
			Match(T__3);
			State = 114;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class SendEventContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ValueContext value() {
			return GetRuleContext<ValueContext>(0);
		}
		public SendEventContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_sendEvent; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ISequenceDiagramVisitor<TResult> typedVisitor = visitor as ISequenceDiagramVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitSendEvent(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public SendEventContext sendEvent() {
		SendEventContext _localctx = new SendEventContext(Context, State);
		EnterRule(_localctx, 26, RULE_sendEvent);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 116;
			Match(T__17);
			State = 117;
			Match(T__3);
			State = 118;
			value();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,21,121,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,2,7,
		7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,1,0,1,0,1,
		0,1,1,1,1,1,1,1,1,5,1,36,8,1,10,1,12,1,39,9,1,1,1,1,1,1,1,1,1,3,1,45,8,
		1,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,3,2,59,8,2,1,3,1,3,1,
		3,1,3,5,3,65,8,3,10,3,12,3,68,9,3,1,3,1,3,1,3,1,3,3,3,74,8,3,1,4,1,4,1,
		4,1,4,1,4,1,4,1,4,3,4,83,8,4,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,6,1,7,1,7,1,
		7,1,7,1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,10,1,10,1,10,1,10,1,11,1,11,1,
		11,1,11,1,12,1,12,1,12,1,12,1,13,1,13,1,13,1,13,1,13,0,0,14,0,2,4,6,8,
		10,12,14,16,18,20,22,24,26,0,0,125,0,28,1,0,0,0,2,44,1,0,0,0,4,58,1,0,
		0,0,6,73,1,0,0,0,8,82,1,0,0,0,10,84,1,0,0,0,12,88,1,0,0,0,14,92,1,0,0,
		0,16,96,1,0,0,0,18,100,1,0,0,0,20,104,1,0,0,0,22,108,1,0,0,0,24,112,1,
		0,0,0,26,116,1,0,0,0,28,29,3,8,4,0,29,30,5,0,0,1,30,1,1,0,0,0,31,32,5,
		1,0,0,32,37,3,4,2,0,33,34,5,2,0,0,34,36,3,4,2,0,35,33,1,0,0,0,36,39,1,
		0,0,0,37,35,1,0,0,0,37,38,1,0,0,0,38,40,1,0,0,0,39,37,1,0,0,0,40,41,5,
		3,0,0,41,45,1,0,0,0,42,43,5,1,0,0,43,45,5,3,0,0,44,31,1,0,0,0,44,42,1,
		0,0,0,45,3,1,0,0,0,46,47,5,19,0,0,47,48,5,4,0,0,48,59,3,8,4,0,49,59,3,
		10,5,0,50,59,3,16,8,0,51,59,3,12,6,0,52,59,3,14,7,0,53,59,3,18,9,0,54,
		59,3,20,10,0,55,59,3,22,11,0,56,59,3,24,12,0,57,59,3,26,13,0,58,46,1,0,
		0,0,58,49,1,0,0,0,58,50,1,0,0,0,58,51,1,0,0,0,58,52,1,0,0,0,58,53,1,0,
		0,0,58,54,1,0,0,0,58,55,1,0,0,0,58,56,1,0,0,0,58,57,1,0,0,0,59,5,1,0,0,
		0,60,61,5,5,0,0,61,66,3,8,4,0,62,63,5,2,0,0,63,65,3,8,4,0,64,62,1,0,0,
		0,65,68,1,0,0,0,66,64,1,0,0,0,66,67,1,0,0,0,67,69,1,0,0,0,68,66,1,0,0,
		0,69,70,5,6,0,0,70,74,1,0,0,0,71,72,5,5,0,0,72,74,5,6,0,0,73,60,1,0,0,
		0,73,71,1,0,0,0,74,7,1,0,0,0,75,83,5,19,0,0,76,83,5,20,0,0,77,83,3,2,1,
		0,78,83,3,6,3,0,79,83,5,7,0,0,80,83,5,8,0,0,81,83,5,9,0,0,82,75,1,0,0,
		0,82,76,1,0,0,0,82,77,1,0,0,0,82,78,1,0,0,0,82,79,1,0,0,0,82,80,1,0,0,
		0,82,81,1,0,0,0,83,9,1,0,0,0,84,85,5,10,0,0,85,86,5,4,0,0,86,87,3,8,4,
		0,87,11,1,0,0,0,88,89,5,11,0,0,89,90,5,4,0,0,90,91,3,8,4,0,91,13,1,0,0,
		0,92,93,5,12,0,0,93,94,5,4,0,0,94,95,3,8,4,0,95,15,1,0,0,0,96,97,5,19,
		0,0,97,98,5,4,0,0,98,99,5,13,0,0,99,17,1,0,0,0,100,101,5,19,0,0,101,102,
		5,4,0,0,102,103,5,14,0,0,103,19,1,0,0,0,104,105,5,19,0,0,105,106,5,4,0,
		0,106,107,5,15,0,0,107,21,1,0,0,0,108,109,5,16,0,0,109,110,5,4,0,0,110,
		111,3,8,4,0,111,23,1,0,0,0,112,113,5,17,0,0,113,114,5,4,0,0,114,115,3,
		8,4,0,115,25,1,0,0,0,116,117,5,18,0,0,117,118,5,4,0,0,118,119,3,8,4,0,
		119,27,1,0,0,0,6,37,44,58,66,73,82
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
