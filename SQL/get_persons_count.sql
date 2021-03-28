CREATE PROCEDURE get_persons_count
	@compound int, 
	@status varchar(4),
	@date varchar(10)
AS
BEGIN
	DECLARE @sqlStatement varchar(300);
	SET @sqlStatement = 'SELECT COUNT(id) FROM persons WHERE'
	
	IF @compound = 0
		SET @sqlStatement = @sqlStatement + ' date_employ=''' + @date + ''''
	
	IF @compound = 1
		SET @sqlStatement = @sqlStatement + ' date_uneploy=''' + @date + ''''
	
	IF @compound = 2
		SET @sqlStatement = @sqlStatement + ' (date_employ=''' + @date + ''' OR date_uneploy=''' + @date + ''')'
	
	IF @status <> 0
		SET @sqlStatement = @sqlStatement + ' AND status=' + @status

	EXEC(@sqlStatement)
END;