-- Name: Team B - Ana Quijano, Tanner Adkins, Roman Mashak, Kay Singh
-- Abstract: Gamified Habit Tracker 

-- Options

USE [HabitHero.Api_db];
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
--	CREATE TABLES 1/2
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
	,strImageName				VARCHAR(255)	NOT NULL
	,strAvatarName				VARCHAR(255)	NOT NULL
	,intHappiness				INTEGER			NOT NULL DEFAULT 100
	,intHealth					INTEGER			NOT NULL DEFAULT 100
	,CONSTRAINT TAvatars_PK PRIMARY KEY (intAvatarID)
)


CREATE TABLE TAppRestrictions
(
     intAppRestrictionID  INTEGER       IDENTITY
    ,strAppName           VARCHAR(100)  NOT NULL
    ,blnRestricted        BIT           NOT NULL DEFAULT 1
    ,CONSTRAINT TAppRestrictions_PK PRIMARY KEY (intAppRestrictionID)
)

CREATE TABLE TUsers
(
	 intUserID					INTEGER			IDENTITY
	,strUserName				VARCHAR(100)	NOT NULL
	,strEmail					VARCHAR(255)	NOT NULL
	,strPassword				VARCHAR(255)	NOT NULL
	,intPoints					INTEGER			NOT NULL DEFAULT 0
	,intTotalPoints				INTEGER			NOT NULL DEFAULT 0
	,monCash					MONEY			NOT NULL DEFAULT 0
	,intAppRestrictionID		INTEGER			NULL
	,intAvatarID				INTEGER			NULL
	,CONSTRAINT TUsers_PK	PRIMARY KEY ( intUserID )
)

CREATE TABLE THabits
(
	 intHabitID					INTEGER			IDENTITY
	,intUserID					INTEGER			NOT NULL
	,intScheduleID				INTEGER			NULL
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

CREATE TABLE TStatuses
(
     intStatusID       INTEGER         IDENTITY
    ,strStatus         VARCHAR(30)     NOT NULL    
    ,CONSTRAINT TStatuses_PK PRIMARY KEY (intStatusID)
);

CREATE TABLE THabitOccurrences
(
     intHabitOccurrenceID  INTEGER      IDENTITY
    ,intHabitID				INTEGER     NULL
	,intQuestHabitID	   INTEGER      NULL
    ,dtmDate	          DATETIME		NULL 
    ,intStatusID           INTEGER      NOT NULL DEFAULT 1
    ,CONSTRAINT THabitOccurrences_PK PRIMARY KEY (intHabitOccurrenceID)
);

CREATE TABLE TQuestHabits
(
     intQuestHabitID	INTEGER         IDENTITY
    ,intQuestID			INTEGER         NOT NULL
    ,intScheduleID		INTEGER         NOT NULL
	,strHabitName		VARCHAR(100)	NOT NULL
	,dtmReminderTime	TIME			NULL
	,dtmStartDate		DATE			NULL
	,dtmEndDate			DATE			NULL
	,strDescription		VARCHAR(500)	NULL
    ,CONSTRAINT TQuestHabits_PK PRIMARY KEY (intQuestHabitID)
);

CREATE TABLE TAchievements
(
     intAchievementID   INTEGER         IDENTITY
    ,strAchievement     VARCHAR(150)    NOT NULL
    ,CONSTRAINT TAchievements_PK PRIMARY KEY (intAchievementID)
);

CREATE TABLE TUserAchievements
(
     intUserAchievementID  INTEGER      IDENTITY
    ,intUserID             INTEGER      NOT NULL
    ,intAchievementID      INTEGER      NOT NULL
    ,CONSTRAINT TUserAchievements_PK PRIMARY KEY (intUserAchievementID)
);

CREATE TABLE TItemTypes
(
     intItemTypeID     INTEGER         IDENTITY
    ,strItemType       VARCHAR(50)     NOT NULL     
    ,CONSTRAINT TItemTypes_PK PRIMARY KEY (intItemTypeID)
)

CREATE TABLE TItemTiers
(
     intItemTierID    INTEGER        IDENTITY
    ,strItemTier      VARCHAR(50)    NOT NULL          
    ,CONSTRAINT TItemTiers_PK PRIMARY KEY (intItemTierID)
);

CREATE TABLE TItems
(
     intItemID			INTEGER         IDENTITY
    ,strItem			VARCHAR(100)    NOT NULL
	,strSlug			VARCHAR(255)	NULL
	,intPrice			INTEGER			NOT NULL
	,intItemTypeID		INTEGER			NOT NULL
	,intItemTierID		INTEGER			NOT NULL
    ,CONSTRAINT TItems_PK PRIMARY KEY (intItemID)
);

CREATE TABLE TAvatarItems
(
	 intAvatarItemID	INTEGER		IDENTITY
	,intAvatarID		INTEGER		NOT NULL
	,intItemID			INTEGER		NOT NULL
	,intQuantity		INTEGER		NOT NULL DEFAULT 0
	,CONSTRAINT TAvatarItems_PK PRIMARY KEY (intAvatarItemID)
)


-- --------------------------------------------------------------------------------
--	REFERENTIAL INTEGRITY 1/2
-- --------------------------------------------------------------------------------
-- #	Child					Parent					Column
-- 1	TUsers					TAvatars				intAvatarID
-- 2	THabits					TSchedules				intScheduleID
-- 3	THabits					TUsers					intUserID
-- 4    TUserQuests				TQuests					intQuestID
-- 5    TUserQuests				TUsers					intUserID
-- 6	THabitOccurrences		THabits					intUserHabitID
-- 7	THabitOccurrences		TQuestHabits			intQuestHabitID
-- 8	THabitOccurrences		TStatuses				intStatusID
-- 9	TQuestHabits			TQuests					intQuestID
-- 10	TQuestHabits			TSchedules				intScheduleID
-- 11	TItems					TItemTiers				intItemTierID
-- 12	TItems					TItemTypes				intItemTypeID
-- 13	TUsers					TAppRestrictions		intAppRestrictionID
-- 14	TUserAchievements		TUsers					intUserID
-- 15	TUserAchievements		TAchievements			intAchievementID
-- 16	TUsers					TAppRestriction			intAppRestrictionID
-- 17	TAvatarItems			TAvatars				intAvatarID

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

-- 6
ALTER TABLE THabitOccurrences ADD CONSTRAINT THabitOccurrences_THabits_FK
    FOREIGN KEY (intHabitID) REFERENCES THabits (intHabitID);

-- 7
ALTER TABLE THabitOccurrences ADD CONSTRAINT THabitOccurrences_TQuestHabits_FK
    FOREIGN KEY (intQuestHabitID) REFERENCES TQuestHabits (intQuestHabitID);

-- 8
ALTER TABLE THabitOccurrences ADD CONSTRAINT THabitOccurrences_TStatuses_FK
    FOREIGN KEY (intStatusID) REFERENCES TStatuses (intStatusID);

-- 9
ALTER TABLE TQuestHabits ADD CONSTRAINT TQuestHabits_TQuests_FK
    FOREIGN KEY (intQuestID) REFERENCES TQuests (intQuestID);

-- 10
ALTER TABLE TQuestHabits ADD CONSTRAINT TQuestHabits_TSchedules_FK
    FOREIGN KEY (intScheduleID) REFERENCES TSchedules (intScheduleID);

-- 11
ALTER TABLE TItems ADD CONSTRAINT TItems_TItemTiers_FK
    FOREIGN KEY (intItemTierID) REFERENCES TItemTiers (intItemTierID);

-- 12
ALTER TABLE TItems ADD CONSTRAINT TItems_TItemTypes_FK
    FOREIGN KEY (intItemTypeID) REFERENCES TItemTypes (intItemTypeID);

-- 13
--ALTER TABLE TUsers ADD CONSTRAINT TUsers_TAppRestrictions_FK
--FOREIGN KEY (intAppRestrictionID) REFERENCES TAppRestrictions (intAppRestrictionID);

-- 14
ALTER TABLE TUserAchievements ADD CONSTRAINT TUserAchievements_TUsers_FK
FOREIGN KEY (intUserID) REFERENCES TUsers (intUserID)

-- 15
ALTER TABLE TUserAchievements ADD CONSTRAINT TUserAchievements_TAchievements_FK
FOREIGN KEY (intAchievementID) REFERENCES TAchievements (intAchievementID)

-- 16
ALTER TABLE TUsers ADD CONSTRAINT TUsers_TAppRestrictions_FK
FOREIGN KEY (intAppRestrictionID) REFERENCES TAppRestrictions (intAppRestrictionID)

-- 17
ALTER TABLE TAvatarItems ADD CONSTRAINT TAvatarItems_TAvatars_FK
FOREIGN KEY (intAvatarID) REFERENCES TAvatars (intAvatarID)

-- 18
ALTER TABLE TAvatarItems ADD CONSTRAINT TAvatarItems_TItems_FK
FOREIGN KEY (intItemID) REFERENCES TItems (intItemID)

-- --------------------------------------------------------------------------------
--	INSERT STATEMENTS 1/2
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


INSERT INTO TUsers
		 (strUserName, strEmail, strPassword)
VALUES	 ('Hero123', 'hero123@heromail.com', 'Hero123')
		,('PlayerABC', 'playerabc@playermail.com', 'PlayerABC')

INSERT INTO THabits 
		  (intUserID, intScheduleID, strHabit, strDescription, dtmStartDate, dtmEndDate, dtmReminderTime)
VALUES
		  (1, 2, 'Read 10 pages', 'Read every night before bed', '2025-10-01', NULL, '21:00')
		 ,(1, 4, '30-min exercise', 'Light workout / walk', '2025-10-03', NULL, '18:00')      
		 ,(2, 6, 'Practice coding', 'Leetcode / project work', '2025-10-05', NULL, '20:00')   

INSERT INTO TStatuses
		 (strStatus)
VALUES	 ('To Do')
		,('Done')
		,('Missed')

INSERT INTO THabitOccurrences
    (intHabitID, intQuestHabitID, dtmDate, intStatusID)
VALUES
     (1, NULL, GETDATE(), 1)	-- Read 10 pages (To Do)
    ,(2, NULL, DATEADD(DAY, 1, GETDATE()), 1)	-- 30-min exercise (To Do)
    ,(3, NULL, GETDATE(), 1)	-- Practice coding (To Do)

--Select TH.strHabit, TH.strDescription, THO.dtmDate, TS.strStatus
--From THabits as TH JOIN THabitOccurrences as THO
--	ON TH.intHabitID = THO.intHabitID
--	Join TStatuses as TS
--	ON TS.intStatusID = THO.intStatusID
--WHERE TH.intUserID = 1