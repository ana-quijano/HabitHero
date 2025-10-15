-- Name: Team B - Ana Quijano, Tanner Adkins, Roman Mashak, Kay Singh
-- Abstract: Gamified Habit Tracker 

-- Options

USE dbHabitHero;
SET NOCOUNT ON;

-- --------------------------------------------------------------------------------
-- DROP ALL FOREIGN KEY CONSTRAINTS 
-- --------------------------------------------------------------------------------
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += '
ALTER TABLE [' + s.name + '].[' + t.name + '] DROP CONSTRAINT [' + fk.name + '];'
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id
JOIN sys.schemas s ON t.schema_id = s.schema_id;

EXEC sp_executesql @sql;

-- --------------------------------------------------------------------------------
-- DROP ALL TABLES
-- --------------------------------------------------------------------------------
SET @sql = N'';

SELECT @sql += '
DROP TABLE [' + s.name + '].[' + t.name + '];'
FROM sys.tables t
JOIN sys.schemas s ON t.schema_id = s.schema_id
WHERE t.is_ms_shipped = 0;

EXEC sp_executesql @sql;

-- --------------------------------------------------------------------------------
--	CREATE TABLES
-- --------------------------------------------------------------------------------
CREATE TABLE TSchedules
(

	 intScheduleID				INTEGER			IDENTITY
	,strSchedule				VARCHAR(10)		NOT NULL
	,CONSTRAINT TSchedules_PK PRIMARY KEY (intScheduleID)
)

CREATE TABLE TAvatars
(
	 intAvatarID				INTEGER			IDENTITY
	,strAvatar					VARCHAR(255)	NOT NULL
	,CONSTRAINT TAvatars_PK PRIMARY KEY (intAvatarID)
)

CREATE TABLE TUsers
(
	 intUserID					INTEGER			IDENTITY
	,strUserName				VARCHAR(100)	NOT NULL
	,strEmail					VARCHAR(255)	NOT NULL
	,strPassword				VARCHAR(255)	NOT NULL
	,decPoints					DECIMAL(10,2)	NOT NULL DEFAULT 0
	,monCash					MONEY			NOT NULL DEFAULT 0
	,blnAppRestriction			BIT				NOT NULL DEFAULT 0
	,intAvatarID				INTEGER			NULL
	,CONSTRAINT TUsers_PK	PRIMARY KEY ( intUserID )
)

CREATE TABLE THabits
(
	 intHabitID					INTEGER			IDENTITY
	,intUserID					INTEGER			NOT NULL
	,intScheduleID				INTEGER			NOT NULL
	,strHabit					VARCHAR(255)	NOT NULL
	,strDescription				VARCHAR(500)	NULL
	,dtmStartDate				DATE			NULL
	,dtmEndDate					DATE			NULL
	,dtmReminderTime			TIME			NULL
	,CONSTRAINT THabits_PK PRIMARY KEY ( intHabitID )
)

CREATE TABLE TQuests
(
	 intQuestID					INTEGER			IDENTITY
	,strQuestName				VARCHAR(150)	NOT NULL
	,monMoneyPot				MONEY			NULL
	,decPointsPot				DECIMAL(10,2)	NULL
	,dtmStartDate				DATE			NULL
	,dtmEndDate					DATE			NULL
	,CONSTRAINT TQuests_PK PRIMARY KEY (intQuestID)
)

CREATE TABLE TUserQuests
(
	 intUserQuestID				INTEGER			IDENTITY
	,intUserID					INTEGER			NOT NULL
	,intQuestID					INTEGER			NOT NULL
	,CONSTRAINT TUserQuests_PK PRIMARY KEY (intUserQuestID)
)

-- --------------------------------------------------------------------------------
--	REFERENTIAL INTEGRITY
-- --------------------------------------------------------------------------------
-- #	Child					Parent					Column
-- 1	TUsers					TAvatars				intAvatarID
-- 2	THabits					TSchedules				intScheduleID
-- 3	THabits					TUsers					intUserID
-- 4    TUserQuests				TQuests					intQuestID
-- 5    TUserQuests				TUsers					intUserID

-- 1
ALTER TABLE TUsers ADD CONSTRAINT TUsers_TAvatars_FK
FOREIGN KEY (intAvatarID) REFERENCES TAvatars (intAvatarID)

-- 2
ALTER TABLE THabits ADD CONSTRAINT THabits_TSchedules_FK
FOREIGN KEY (intScheduleID) REFERENCES TSchedules (intScheduleID)

-- 3
ALTER TABLE THabits ADD CONSTRAINT THabits_TUsers_FK
FOREIGN KEY (intUserID) REFERENCES TUsers (intUserID)

-- 4
ALTER TABLE TUserQuests ADD CONSTRAINT TUserQuests_TQuests_FK
FOREIGN KEY (intQuestID) REFERENCES TQuests (intQuestID)

-- 5
ALTER TABLE TUserQuests ADD CONSTRAINT TUserQuests_TUsers_FK
FOREIGN KEY (intUserID) REFERENCES TUsers (intUserID)

-- --------------------------------------------------------------------------------
--	INSERT STATEMENTS
-- --------------------------------------------------------------------------------

INSERT INTO TSchedules 
		 (strSchedule)
VALUES	 ('Sunday')
		,('Monday')
		,('Tuesday')
		,('Wednesday')
		,('Thursday')
		,('Friday')
		,('Saturday')

INSERT INTO TAvatars -- SAMPLE ONLY (str entities will hold file names for each avatar)
		 (strAvatar)
VALUES	 ('bird.png')
		,('alien.png')
		,('robot.png')
		,('fish.png')
		,('penguin.png')

INSERT INTO TUsers
		 (strUserName, strEmail, strPassword, decPoints, monCash, blnAppRestriction, intAvatarID)
VALUES	 ('Hero123', 'hero123@heromail.com', 'Hero123', 0, 0, 0, 1)
		,('PlayerABC', 'playerabc@playermail.com', 'PlayerABC', 0, 0, 0, 2)

INSERT INTO THabits 
		  (intUserID, intScheduleID, strHabit, strDescription, dtmStartDate, dtmEndDate, dtmReminderTime)
VALUES
		  (1, 2, 'Read 10 pages', 'Read every night before bed', '2025-10-01', NULL, '21:00')
		 ,(1, 4, '30-min exercise', 'Light workout / walk', '2025-10-03', NULL, '18:00')      
		 ,(2, 6, 'Practice coding', 'Leetcode / project work', '2025-10-05', NULL, '20:00')   