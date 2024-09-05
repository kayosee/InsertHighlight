grammar Select;
select:SELECT fields from? where? groupby? orderby? ';'?;
from:FROM tables;
SELECT:'select';
FROM:'from';
WHERE:'where';
AS:'as';
INNER:'inner';
LEFT:'left';
RIGHT:'right';
JOIN:'join';
ON:'on';
CASE:'case';
WHEN:'when';
THEN:'then';
END:'end';
GROUPBY:'group by';
ORDERBY:'order by';
HAVING:'having';
ASC:'asc';
DESC:'desc';
COUNT:'count';
SUM:'sum';
AVG:'avg';
fields:'*'|field(','field)*;
field:
CONSTANT #constantField
|subquery #queryField
|tableName'.*' #allField
|tableName'.'columnName #fullQualifiedField
|columnName #noQualifiedField
|field AS? ID #renameField
|func #functionField
|var #variableField
|case #caseField;
case:CASE field (WHEN CONSTANT THEN field)+ END ((AS)? ID)?|
CASE (WHEN expr THEN field)+ END ((AS)? ID)?;
var:'@'('@')?ID;
func:(arc'.')?ID'('(args)?')'|aggrFunc'('args')';
arc:ID;
args:arg(','arg)*;
arg:CONSTANT|field;
columnName: '['ID']'|ID;
ID: [_a-zA-Z][0-9_a-zA-Z]*;
tables:tableName(','tableName)*|joinTables;
joinTables:tableName ((LEFT|INNER|RIGHT) JOIN tableName ON expr)+;
tableName:('@'|'#')ID #varTable
|ID #noalias
|arc'.'ID #withArc
|tableName AS? ID #alias
|subquery AS? ID #subsql
;
subquery:'('select')';
conditions:expr+;
expr:field'='right
|field'!='right
|field'<>'right
|field'>'right
|field'<'right
|field'>='right
|field'<='right
|expr('and'|'or')expr
|'('expr')'
;
where:WHERE conditions;
groupby:GROUPBY field(','field)* (having)?;
having:HAVING aggrExpr+;
aggrExpr:aggrFunc'('(field|'*')')'('>'|'>='|'<'|'<='|'!=')CONSTANT|('and'|'or')aggrExpr;
aggrFunc:COUNT|SUM|AVG;
orderby:ORDERBY orderField(','orderField)*;
orderField:field (ASC|DESC);
right:field|CONSTANT;

CONSTANT:'\''.+?'\''|[0-9]+;
WS:[ \t\n\r]+->skip;