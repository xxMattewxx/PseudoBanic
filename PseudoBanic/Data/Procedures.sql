/* POSTGRES DATABASE PROCEDURES */
/*
CREATE OR REPLACE FUNCTION RetrieveAtomic(userID INTEGER)
	RETURNS INTEGER AS $id$
DECLARE
	retrievedID INTEGER;
BEGIN
	PERFORM pg_advisory_xact_lock(69);
	SELECT "Tasks"."ID" INTO retrievedID 
		FROM "Tasks"
		JOIN "TasksMetadata" 
			ON "Tasks"."MetadataID" = "TasksMetadata"."ID"
		WHERE 
			"Status" = 1 AND 
			"QuorumLeft" > 0 AND
			NOT EXISTS(
				SELECT 1
					FROM "Assignments"
					WHERE "Assignments"."UserID" = userID AND
						"Assignments"."TaskID" = "Tasks"."ID"
					LIMIT 1
			)
		ORDER BY "Priority" DESC
		LIMIT 1;

	IF retrievedID > 0 THEN
		UPDATE "Tasks" SET "QuorumLeft" = "QuorumLeft" - 1 WHERE "ID" = retrievedID;
		INSERT INTO "Assignments" ("TaskID", "UserID", "Deadline") VALUES (retrievedID, userID, NOW() + INTERVAL '2 HOURS');
		RETURN retrievedID;
	END IF;
	RETURN -1;
END;
$id$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION SaveHistoricalLeaderboard(userID INTEGER, metaID INTEGER)
	RETURNS VOID AS $$
DECLARE
	points INTEGER;
	validatedPoints INTEGER;
	invalidatedPoints INTEGER;
BEGIN
	PERFORM pg_advisory_xact_lock(151);
	SELECT "Points", "ValidatedPoints", "InvalidatedPoints" INTO points, validatedPoints, invalidatedPoints
		FROM "Leaderboard"
		WHERE "UserID" = userID AND "MetadataID" = metaID
		LIMIT 1;
	INSERT INTO "HistoricalLeaderboard" ("UserID", "MetadataID", "Points", "ValidatedPoints", "InvalidatedPoints") VALUES (userID, metaID, points, validatedPoints, invalidatedPoints);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION IncreaseUserPoints(userID INTEGER, metaID INTEGER)
	RETURNS VOID AS $$
BEGIN
	PERFORM pg_advisory_xact_lock(151);
	INSERT INTO "Leaderboard" ("UserID", "MetadataID", "Points", "ValidatedPoints", "InvalidatedPoints") VALUES (userID, metaID, 0, 0, 0) ON CONFLICT DO NOTHING;
	UPDATE "Leaderboard" SET "Points" = "Points" + 1 WHERE "UserID" = userID AND "MetadataID" = metaID;
	PERFORM SaveHistoricalLeaderboard(userID, metaID);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION IncreaseUserValidatedPoints(userID INTEGER, metaID INTEGER)
	RETURNS VOID AS $$
BEGIN
	PERFORM pg_advisory_xact_lock(151);
	INSERT INTO "Leaderboard" ("UserID", "MetadataID", "Points", "ValidatedPoints", "InvalidatedPoints") VALUES (userID, metaID, 0, 0, 0) ON CONFLICT DO NOTHING;
	UPDATE "Leaderboard" SET "ValidatedPoints" = "ValidatedPoints" + 1 WHERE "UserID" = userID AND "MetadataID" = metaID;
	PERFORM SaveHistoricalLeaderboard(userID, metaID);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION IncreaseUserInvalidatedPoints(userID INTEGER, metaID INTEGER)
	RETURNS VOID AS $$
BEGIN
	PERFORM pg_advisory_xact_lock(151);
	INSERT INTO "Leaderboard" ("UserID", "MetadataID", "Points", "ValidatedPoints", "InvalidatedPoints") VALUES (userID, metaID, 0, 0, 0) ON CONFLICT DO NOTHING;
	UPDATE "Leaderboard" SET "InvalidatedPoints" = "InvalidatedPoints" + 1 WHERE "UserID" = userID AND "MetadataID" = metaID;
	PERFORM SaveHistoricalLeaderboard(userID, metaID);
END;
$$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION UpdateAtomic(assignmentID INTEGER, output TEXT)
	RETURNS INTEGER AS $result$
DECLARE
	functionRet INTEGER;
	taskID INTEGER;
	metaID INTEGER;
	userID INTEGER;
	matchingResultsCount INTEGER;
	quorumLeft INTEGER;
	pendingResultsCount INTEGER;
	tempRow RECORD;
BEGIN
	PERFORM pg_advisory_xact_lock(1337);
	SELECT
		"TaskID", "QuorumLeft", "MetadataID", "UserID" INTO taskID, quorumLeft, metaID, userID
	FROM "Assignments" 
	INNER JOIN "Tasks" 
		ON "Assignments"."TaskID" = "Tasks"."ID"
	WHERE 
		"Assignments"."ID" = assignmentID AND
		"Assignments"."State" = 0 AND
		"Tasks"."Status" = 1;

	IF taskID IS NULL THEN
		RETURN -1;
	END IF;

	UPDATE "Assignments" SET "State" = 1, "Output" = output WHERE "ID" = assignmentID;
	SELECT COUNT(*) INTO matchingResultsCount FROM "Assignments" WHERE "TaskID" = taskID AND "State" = 1 AND "Output" = output;
	PERFORM IncreaseUserPoints(userID, metaID);

	IF matchingResultsCount >= 2 THEN
		UPDATE "Assignments" SET "State" = 2, "Output" = NULL WHERE "TaskID" = taskID AND "Output" = output;
		UPDATE "Assignments" SET "State" = 3, "Output" = NULL WHERE "TaskID" = taskID AND "Output" != output;
		UPDATE "Tasks" SET "Consensus" = output, "QuorumLeft" = 0, "Status" = 2 WHERE "ID" = taskID;

		FOR tempRow IN 
			SELECT "UserID" AS tmpUserID FROM "Assignments" WHERE "TaskID" = taskID AND "State" = 2
		LOOP
			PERFORM IncreaseUserValidatedPoints(tempRow.tmpUserID, metaID);
		END LOOP;

		FOR tempRow IN 
			SELECT "UserID" AS tmpUserID FROM "Assignments" WHERE "TaskID" = taskID AND "State" = 3
		LOOP
			PERFORM IncreaseUserInvalidatedPoints(tempRow.tmpUserID, metaID);
		END LOOP;
		RETURN 2;
	ELSE
		IF quorumLeft > 0 THEN
			RETURN 0;
		END IF;

		SELECT COUNT(*) INTO pendingResultsCount FROM "Assignments" WHERE "TaskID" = taskID AND "State" = 0;
		IF pendingResultsCount = 0 THEN
			UPDATE "Tasks" SET "QuorumLeft" = "QuorumLeft" + 1 WHERE "ID" = taskID;
			RETURN 1;
		END IF;
	END IF;
	RETURN 0;
END;
$result$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION ReviveTheDead()
	RETURNS INTEGER AS $id$
DECLARE
	retrievedID INTEGER;
BEGIN
	PERFORM pg_advisory_xact_lock(1337);
	WITH t AS (
		SELECT "TaskID" as taskid
		FROM "Assignments"
		JOIN "Tasks"
		ON "Tasks"."ID" = "Assignments"."TaskID"
		WHERE
			NOW() > "Assignments"."Deadline" AND
			"State" = 0
	)
	UPDATE "Tasks" SET "QuorumLeft" = "QuorumLeft" + 1 FROM t WHERE "ID" = taskid;

	UPDATE "Assignments" SET "State" = 4 WHERE NOW() > "Deadline" AND "State" = 0;

	RETURN 1;
END;
$id$
LANGUAGE plpgsql;*/

