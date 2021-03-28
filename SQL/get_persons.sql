CREATE PROCEDURE get_persons
	@status varchar(100),
	@post varchar(100),
	@department varchar(100),
	@last_name varchar(100)
AS
BEGIN
	DECLARE @sqlStatement varchar(720);
	SET @sqlStatement = 'SELECT first_name, second_name, last_name, status.name, deps.name, posts.name, date_employ, date_uneploy FROM persons, status, posts, deps WHERE status.id=persons.status AND deps.id=persons.id_dep AND persons.id_post=posts.id'
	if @status <> ''
		SET @sqlStatement = @sqlStatement + ' AND status.name=''' + @status + '''' 
	if @post <> ''
		SET @sqlStatement = @sqlStatement + ' AND posts.name=''' + @post + ''''
	if @department <> ''
		SET @sqlStatement = @sqlStatement + ' AND deps.name=''' + @department + ''''
	if @last_name <> ''
		SET @sqlStatement = @sqlStatement + ' AND first_name LIKE ''%' + @last_name + '%'';'
	EXEC(@sqlStatement);
END;

