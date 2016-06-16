select 
	t1.TABLE_NAME as 表名,
	t2.TABLE_COMMENT as  表说明,
	t1.COLUMN_NAME as 字段名称,
	case t1.COLUMN_KEY
		WHEN 'PRI' THEN '√'
		WHEN 0 THEN ''
	end  as 主键,
	t1.COLUMN_TYPE as 字段类型,
	t1.COLUMN_COMMENT as 字段备注	
from COLUMNS t1
left JOIN `TABLES` t2 on t2.TABLE_NAME=t1.TABLE_NAME
WHERE t1.TABLE_SCHEMA='Bilin3d'
