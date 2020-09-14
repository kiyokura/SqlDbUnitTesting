CREATE PROCEDURE [dbo].[spGetMember]
AS
  SELECT Id, Name, BirthDay FROM Member ORDER BY BirthDay DESC
RETURN
