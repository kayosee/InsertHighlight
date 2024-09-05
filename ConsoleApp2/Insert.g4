grammar Insert;
import Select;
insert:INSERT INTO? insertTableName insertFieldList? insertValueList;
INSERT:'insert';
INTO:'into';
insertTableName:tableName;
insertFieldList:'('insertFields')';
insertFields:insertField(','insertField)*;
insertField:ID;
insertValueList:VALUES'('insertValues')' #insertValueFromConstant
|select #insertValueFromSelect;
VALUES:'values';
insertValues:CONSTANT(','CONSTANT)*;



