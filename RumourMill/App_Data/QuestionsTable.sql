CREATE TABLE [dbo].[Table]
(
	[QuestionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [QuestionTitle] VARCHAR(50) NOT NULL, 
    [QuestionText] VARCHAR(MAX) NOT NULL, 
    [IsApproved] BIT NOT NULL, 
    [IsAnswered] BIT NOT NULL, 
    [AnsweredBy] VARCHAR(MAX) NULL
)
