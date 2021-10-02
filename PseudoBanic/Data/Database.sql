/* POSTGRES DATABASE INITIALIZATION CODE */
DROP TABLE IF EXISTS "Users";
DROP TABLE IF EXISTS "Tasks";
DROP TABLE IF EXISTS "Versions";
DROP TABLE IF EXISTS "Assignments";
DROP TABLE IF EXISTS "TasksMetadata";
DROP TABLE IF EXISTS "Leaderboard";
DROP TABLE IF EXISTS "HistoricalLeaderboard";

CREATE TABLE "Users" (
	"ID" SERIAL PRIMARY KEY, 
	"Username" TEXT UNIQUE, 
	"APIKey" TEXT UNIQUE, 
	"AdminLevel" INTEGER, 
	"DiscordID" BIGINT
);

CREATE TABLE "Tasks" (
	"ID" SERIAL PRIMARY KEY,
	"MetadataID" INTEGER,
	"Status" SMALLINT,
	"QuorumLeft" INTEGER,
	"Priority" INTEGER DEFAULT floor(random() * 1000)::int,
	"InputData" TEXT,
	"Consensus" TEXT
);

CREATE TABLE "TasksMetadata" (
	"ID" SERIAL PRIMARY KEY,
	"Name" TEXT,
	"BinaryURL" TEXT,
	"FileHash" TEXT,
	"PassByFile" BOOLEAN
);

CREATE TABLE "Versions" (
	"VersionNumber" SERIAL PRIMARY KEY,
	"Codename" TEXT,
	"BinaryURL" TEXT,
	"FileHash" TEXT
);

CREATE TABLE "Assignments" (
	"ID" SERIAL PRIMARY KEY, 
	"TaskID" INTEGER, 
	"UserID" INTEGER, 
	"Deadline" TIMESTAMP, 
	"State" SMALLINT DEFAULT 0, 
	"Output" TEXT
);

CREATE TABLE "Leaderboard" (
	"ID" SERIAL PRIMARY KEY,
	"UserID" INTEGER,
	"MetadataID" INTEGER,
	"Points" INTEGER,
	"ValidatedPoints" INTEGER,
	"InvalidatedPoints" INTEGER,
	UNIQUE ("UserID", "MetadataID")
);

CREATE TABLE "HistoricalLeaderboard" (
	"ID" SERIAL PRIMARY KEY,
	"UserID" INTEGER,
	"MetadataID" INTEGER,
	"Points" INTEGER,
	"ValidatedPoints" INTEGER,
	"InvalidatedPoints" INTEGER,
	"SnapshotTime" TIMESTAMP DEFAULT NOW()
);

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO pseudobanic;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO pseudobanic;

/* DEFAULT USERS */
INSERT INTO "Users" ("Username", "APIKey", "AdminLevel", "DiscordID") 
VALUES ('Matt', 'd6f255d502040ab25fef389bb6ed9ffbdb16adc2db9e18e80edaafcfb1724fed', 5, 257642666244833281);

/* DEBUG APPS */
INSERT INTO "TasksMetadata" ("Name", "BinaryURL", "FileHash", "PassByFile") VALUES ('Debug App', 'Debug URL', 'Debug Hash', TRUE);

/* DEBUG TASKS */
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');
INSERT INTO "Tasks" ("MetadataID", "Status", "QuorumLeft", "InputData") VALUES (1, 1, 2, 'Test input data');