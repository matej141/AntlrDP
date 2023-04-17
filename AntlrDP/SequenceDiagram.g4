
/** Taken from "The Definitive ANTLR 4 Reference" by Terence Parr */

// Derived from https://json.org
grammar SequenceDiagram;

json
   : value EOF
   ;

obj
   : '{' pair (',' pair)* '}'
   | '{' '}'
   ;

pair
   : STRING ':' value
   | name
   | lifeline
   | xmiId
   | xmiIdRef
   | message
   | occurenceSpecification
   | covered
   | receiveEvent
   | sendEvent
   ;

arr
   : '[' value (',' value)* ']'
   | '[' ']'
   ;

value
   : STRING
   | NUMBER
   | obj
   | arr
   | 'true'
   | 'false'
   | 'null'
   ;

name 
    : '"name"' ':' value
    ;
    
xmiId 
    : '"XmiId"' ':' value
    ;
    
xmiIdRef 
    : '"XmiIdRef"' ':' value
    ;

lifeline
    : STRING ':' '"uml:Lifeline"'
    ;
    
message
    : STRING ':' '"uml:Message"'
    ;

occurenceSpecification
    : STRING ':' '"uml:OccurrenceSpecification"'
    ;
   
covered
    : '"covered"' ':' value
    ;
    
receiveEvent
    : '"receiveEvent"' ':' value
    ;
    
sendEvent
    : '"sendEvent"' ':' value
    ;

//NAME
//   : '"'~('"')*':name"'
//   ;

STRING
   : '"' (ESC | SAFECODEPOINT)* '"'
   ;


fragment ESC
   : '\\' (["\\/bfnrt] | UNICODE)
   ;


fragment UNICODE
   : 'u' HEX HEX HEX HEX
   ;


fragment HEX
   : [0-9a-fA-F]
   ;


fragment SAFECODEPOINT
   : ~ ["\\\u0000-\u001F]
   ;


NUMBER
   : '-'? INT ('.' [0-9] +)? EXP?
   ;


fragment INT
   // integer part forbis leading 0s (e.g. `01`)
   : '0' | [1-9] [0-9]*
   ;

// no leading zeros

fragment EXP
   // exponent number permits leading 0s (e.g. `1e01`)
   : [Ee] [+\-]? [0-9]+
   ;

// \- since - means "range" inside [...]

WS
   : [ \t\n\r] + -> skip
   ;