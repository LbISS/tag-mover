grammar FilterQuery;

query: expression;

expression: expression 'AND' expression #AndOperator
	| expression 'OR' expression		#OrOperator
	| '(' expression ')'				#BracketOperator
	| 'NOT' expression					#NotOperator
	| FIELD 'MISSING'					#MissingOperator
	| FIELD 'PRESENT'					#PresentOperator
	| FIELD 'HAS' STRING				#HasOperator
	| FIELD 'IS' STRING					#IsOperator
	| FIELD 'GREATER' NUMBER			#GreaterOperator
	| FIELD 'EQUAL' NUMBER				#EqualOperator
	| FIELD 'LESS' NUMBER				#LessOperator
	| FIELD 'MATCHES' REGEX				#MatchesOperator
	| STRING							#String;

FIELD: CHAR+;
STRING: CHAR+;
NUMBER: [0-9]+(.[0-9]+);
REGEX: CHAR+;
CHAR: [a-zA-Z];

WS : [ \t\r\n]+ -> skip ; // skip spaces, tabs, newlines