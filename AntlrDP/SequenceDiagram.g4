
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
   | combinedFragment
   | interactionOperator
   | interactionOperand
   | guard
   | interactionConstraint
   | specification
   | opaqueExpression
   | operand
   | fragments
   | body
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

combinedFragment
    : STRING ':' '"uml:CombinedFragment"'
    ;

interactionOperand
    : STRING ':' '"uml:InteractionOperand"'
    ;
    
guard
    : '"guard"' ':' value
    ;

fragments
    : '"fragment"' ':' value
    ;
    
specification
    : '"specification"' ':' value
    ;

interactionConstraint
    : STRING ':' '"uml:InteractionConstraint"'
    ;

opaqueExpression
    : STRING ':' '"uml:OpaqueExpression"'
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

body
    : '"body"' ':' value
    ;
 
interactionOperator
    : '"interactionOperator"' ':' value
    ;

operand
    : '"operand"' ':' value
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
   : '0' | [1-9] [0-9]*
   ;


fragment EXP
   : [Ee] [+\-]? [0-9]+
   ;


WS
   : [ \t\n\r] + -> skip
   ;