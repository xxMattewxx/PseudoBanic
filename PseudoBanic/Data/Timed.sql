/* POSTGRES DATABASE TIMED ACTIONS */
/*
	To use timed actions, pg_cron has to be set up and enabled for user pseudobanic:
	GRANT USAGE ON SCHEMA cron TO pseudobanic;
*/

/* REGEN ASSIGNMENTS THAT WERE DEADLINED */
SELECT cron.schedule('* * * * *', 'SELECT ReviveTheDead()');