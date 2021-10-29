SELECT * FROM "Tasks" 
WHERE "QuorumLeft" > 0 AND "Status" = 1 AND NOT EXISTS (SELECT * FROM "Assignments" WHERE "UserID" = 2 AND "TaskID" = "Tasks"."ID") ORDER BY "Priority" LIMIT 1;

SELECT * FROM pg_indexes WHERE tablename = 'Tasks';
SELECT * FROM pg_indexes WHERE tablename = 'Assignments';
SELECT * FROM pg_indexes WHERE tablename = 'TaskContributors';

Workunits


Assignments

DROP TABLE "TaskContributors";
CREATE TABLE "TaskContributors" (
    "ID" SERIAL PRIMARY KEY,
    "TaskID" INTEGER,
    "UserID" INTEGER,
    "Priority" INTEGER,
    FOREIGN KEY ("UserID") REFERENCES "Users"("ID"),
    FOREIGN KEY ("TaskID") REFERENCES "Tasks"("ID")
);
SELECT PopulateInitialContributors();

CREATE INDEX contributors_taskid_idx ON "TaskContributors"("TaskID");
CREATE INDEX contributors_userid_idx ON "TaskContributors"("UserID");
CREATE INDEX contributors_priority_idx ON "TaskContributors"("Priority" DESC);

CREATE INDEX contributors_userid_priority_idx ON "TaskContributors"("UserID", "Priority" DESC);
CREATE INDEX contributors_priority_userid_idx ON "TaskContributors"("Priority" DESC, "UserID");
CREATE INDEX contributors_userid_idx ON "TaskContributors"("UserID" DESC);
CREATE INDEX contributors_taskid_idx ON "TaskContributors"("TaskID");
CREATE INDEX contributors_userid_taskid_idx ON "TaskContributors"("UserID","TaskID");
CREATE INDEX contributors_priority_idx ON "TaskContributors"("Priority" DESC);
CREATE INDEX contributors_taskid_userid_idx ON "TaskContributors"("TaskID", "UserID");
CREATE INDEX contributors_taskid_priority_idx ON "TaskContributors"("TaskID", "Priority" DESC);
CREATE INDEX contributors_userid_priority_idx ON "TaskContributors"("TaskID","UserID","Priority" DESC);
CREATE INDEX contributors_userid_taskid_priority_idx ON "TaskContributors"("TaskID","UserID","Priority" DESC);

SELECT "Tasks"."ID" FROM "TaskContributors" AS org
    JOIN "Tasks" ON org."TaskID" = "Tasks"."ID"
    WHERE "UserID" IS NULL AND NOT EXISTS (
        SELECT 1 FROM "TaskContributors" WHERE "TaskID" = org."TaskID" AND "UserID" = 1 LIMIT 1
    )
    ORDER BY org."Priority" DESC
    LIMIT 1;

CREATE OR REPLACE FUNCTION PopulateInitialContributors() 
  RETURNS VOID 
AS
$$
DECLARE 
   tempRow RECORD;
BEGIN
    FOR tempRow IN
        SELECT "ID" AS tmpTaskID,"Priority" AS tmpPriority FROM "Tasks" 
    LOOP 
        INSERT INTO "TaskContributors" ("TaskID", "UserID", "Priority") VALUES (tempRow.tmpTaskID, NULL, tempRow.tmpPriority);
        INSERT INTO "TaskContributors" ("TaskID", "UserID", "Priority") VALUES (tempRow.tmpTaskID, NULL, tempRow.tmpPriority);
    END LOOP;
END;
$$ 
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION UpdateOrInsert(taskID INTEGER, userID INTEGER) 
  RETURNS VOID 
AS
$$
DECLARE 
    id INTEGER;
    currID INTEGER;
BEGIN
    SELECT "ID" into currID FROM "TaskContributors" WHERE "TaskID" = taskID AND "UserID" = userID LIMIT 1;
    IF currID > 0 THEN
        RETURN;
    END IF;
    
    SELECT "ID" into id FROM "TaskContributors" WHERE "TaskID" = taskID AND "UserID" IS NULL LIMIT 1;
    IF id > 0 THEN
        UPDATE "TaskContributors" SET "UserID" = userID WHERE "ID" = id;    
    ELSE
        INSERT INTO "TaskContributors" ("TaskID", "UserID", "Priority") VALUES (taskID, userID, 1000);
    END IF;
END;
$$ 
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION PopulateFromAssignments() 
  RETURNS VOID 
AS
$$
DECLARE 
    tempRow RECORD;
BEGIN
    FOR tempRow IN
        SELECT "TaskID" AS taskID,"UserID" AS userID FROM "Assignments" 
    LOOP 
        PERFORM UpdateOrInsert(tempRow.taskID, tempRow.userID);
    END LOOP;
END;
$$ 
LANGUAGE plpgsql;
    
CREATE OR REPLACE FUNCTION RetrieveAtomic(userID INTEGER)
	RETURNS INTEGER AS $id$
DECLARE
	retrievedID INTEGER;
BEGIN
	PERFORM pg_advisory_xact_lock(69);
    /* Select from the contributors table a task that our current user didn't do yet */
	SELECT "Tasks"."ID" INTO retrievedID FROM "TaskContributors" AS org
    JOIN "Tasks" ON org."TaskID" = "Tasks"."ID"
    WHERE "Tasks"."Status" = 1 AND "Tasks"."QuorumLeft" > 0 AND
        "UserID" IS NULL AND NOT EXISTS (
        SELECT 1 FROM "TaskContributors" WHERE "TaskID" = org."TaskID" AND "UserID" = userID LIMIT 1
    )
    ORDER BY org."Priority" DESC
    LIMIT 1;

	IF retrievedID > 0 THEN
		UPDATE "Tasks" SET "QuorumLeft" = "QuorumLeft" - 1 WHERE "ID" = retrievedID;
		INSERT INTO "Assignments" ("TaskID", "UserID", "Deadline") VALUES (retrievedID, userID, NOW() + INTERVAL '2 HOURS');
		RETURN retrievedID;
	END IF;
	RETURN -1;
END;