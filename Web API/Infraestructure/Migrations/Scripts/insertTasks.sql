DECLARE @i INT = 0;

WHILE @i < 4000
BEGIN
    INSERT INTO dbo.tasks (personid, title, description, location, severity, startdate, enddate)
    VALUES (
        FLOOR(RAND() * 105) + 1,
        'Tarefa ' + CAST((@i + 1) AS NVARCHAR(10)),
        NULL,
        'Sala ' + CAST((FLOOR(RAND() * 10) + 1) AS NVARCHAR(10)),
        CAST((FLOOR(RAND() * 4) + 1) AS NVARCHAR(10)),
        GETDATE(),
        NULL
    );

    SET @i = @i + 1;
END
