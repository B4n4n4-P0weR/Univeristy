-- 0. Drop database if exists
DROP DATABASE studexam;

-- 1. Create new database studexam
CREATE DATABASE studexam;

-- 2. Connect to newly created database
\connect studexam

-- 3. Add extension to support comparisons
CREATE EXTENSION btree_gist;

-- 4. Create unique identifiers for objects in database (with reserve for fixed-id objects)
CREATE SEQUENCE id_seq START WITH 10000000;

-- 5. Create tables
CREATE TABLE student (
	id		int		PRIMARY KEY DEFAULT nextval('id_seq'),
	name		text
);

CREATE TABLE subject (
	id		int		PRIMARY KEY DEFAULT nextval('id_seq'),
	name		text		NOT NULL
);

CREATE TABLE teacher (
	id		int		PRIMARY KEY DEFAULT nextval('id_seq'),
	name		text		NOT NULL
);

CREATE TABLE teaching (
	id		int		PRIMARY KEY DEFAULT nextval('id_seq'),
	subject		int		NOT NULL REFERENCES subject(id),
	teacher		int		NOT NULL REFERENCES teacher(id),
	period		tstzrange	NOT NULL,

	EXCLUDE USING gist (
		subject WITH =,
		teacher WITH =,
		period	WITH &&
	)
);

CREATE TABLE exam (
	id		int		PRIMARY KEY DEFAULT nextval('id_seq'),
	subject		int		NOT NULL REFERENCES subject(id),
	teacher		int		NOT NULL REFERENCES teacher(id),
	student		int		NOT NULL REFERENCES student(id),
	score		int		NOT NULL
);

-- 6. Function to generate random string
-- First we generate a series to create a dummy table with required number of rows
-- Next we select from this table, but instead of taking the column value we
--   1. Generate random number from 1 to 26
--   2. Use it to index into the alphabet and take a 1-length substring (indexing starts at 1)
--   3. We run STRING_AGG on it with empty separator to join it into a word
CREATE OR REPLACE FUNCTION fn_gen_random_string (
	arg_string_length	int,
	arg_alphabet 		text	DEFAULT	'abcdefghijklmnopqrstuvwxyz'
)
RETURNS text
LANGUAGE SQL
AS $$
	SELECT STRING_AGG(SUBSTRING(arg_alphabet, RANDOM(1, LENGTH(arg_alphabet)), 1), '') from generate_series(1, arg_string_length) LIMIT 1;
$$;

-- 7. Special subfunction to generate name
CREATE OR REPLACE FUNCTION fn_gen_random_name ()
RETURNS text
LANGUAGE SQL
AS $$
	SELECT INITCAP(fn_gen_random_string(random(2, 12)) || ' ' || fn_gen_random_string(random(2, 12)));
$$;

-- 8. Use this functions to fill tables with strings
INSERT INTO student (name) SELECT fn_gen_random_name() FROM generate_series(1, 1000);
INSERT INTO teacher (name) SELECT fn_gen_random_name() FROM generate_series(1, 1000);
INSERT INTO subject (name) SELECT INITCAP(fn_gen_random_string(random(4, 10))) FROM generate_series(1, 1000);
