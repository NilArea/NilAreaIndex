create schema NilArea;
create user 'nilorleans'@'%' identified by '9b6b00c9b2b312b2734bb9b781607b8b4730b6eda49026caadd880af4c6abf5d';
grant all privileges on NilArea.* to 'nilorleans'@'%' with grant option;
flush privileges;

use NilArea;
-- Orleans Shared Query table.
CREATE TABLE OrleansQuery
(
    QueryKey VARCHAR(64) NOT NULL,
    QueryText VARCHAR(8000) NOT NULL,

    CONSTRAINT OrleansQuery_Key PRIMARY KEY(QueryKey)
);

-- Orleans storage table.
CREATE TABLE OrleansStorage
(
    -- These are for the book keeping. Orleans calculates
    -- these hashes (see RelationalStorageProvide implementation),
    -- which are signed 32 bit integers mapped to the *Hash fields.
    -- The mapping is done in the code. The
    -- *String columns contain the corresponding clear name fields.
    --
    -- If there are duplicates, they are resolved by using GrainIdN0,
    -- GrainIdN1, GrainIdExtensionString and GrainTypeString fields.
    -- It is assumed these would be rarely needed.
    GrainIdHash                INT NOT NULL,
    GrainIdN0                BIGINT NOT NULL,
    GrainIdN1                BIGINT NOT NULL,
    GrainTypeHash            INT NOT NULL,
    GrainTypeString            NVARCHAR(512) NOT NULL,
    GrainIdExtensionString    NVARCHAR(512) NULL,
    ServiceId                NVARCHAR(150) NOT NULL,

    -- Payload
    PayloadBinary    BLOB NULL,

    -- Informational field, no other use.
    ModifiedOn DATETIME NOT NULL,

    -- The version of the stored payload.
    Version INT NULL

    -- The following would in principle be the primary key, but it would be too thick
    -- to be indexed, so the values are hashed and only collisions will be solved
    -- by using the fields. That is, after the indexed queries have pinpointed the right
    -- rows down to [0, n] relevant ones, n being the number of collided value pairs.
) ROW_FORMAT = COMPRESSED KEY_BLOCK_SIZE = 16;
ALTER TABLE OrleansStorage ADD INDEX IX_OrleansStorage (GrainIdHash, GrainTypeHash);

DELIMITER $$

CREATE PROCEDURE ClearStorage
(
    in _GrainIdHash INT,
    in _GrainIdN0 BIGINT,
    in _GrainIdN1 BIGINT,
    in _GrainTypeHash INT,
    in _GrainTypeString NVARCHAR(512),
    in _GrainIdExtensionString NVARCHAR(512),
    in _ServiceId NVARCHAR(150),
    in _GrainStateVersion INT
)
BEGIN
    DECLARE _newGrainStateVersion INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    DECLARE EXIT HANDLER FOR SQLWARNING BEGIN ROLLBACK; RESIGNAL; END;

    SET _newGrainStateVersion = _GrainStateVersion;

    -- Default level is REPEATABLE READ and may cause Gap Lock issues
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    START TRANSACTION;
    UPDATE OrleansStorage
    SET
        PayloadBinary = NULL,
        Version = Version + 1
    WHERE
        GrainIdHash = _GrainIdHash AND _GrainIdHash IS NOT NULL
      AND GrainTypeHash = _GrainTypeHash AND _GrainTypeHash IS NOT NULL
      AND GrainIdN0 = _GrainIdN0 AND _GrainIdN0 IS NOT NULL
      AND GrainIdN1 = _GrainIdN1 AND _GrainIdN1 IS NOT NULL
      AND GrainTypeString = _GrainTypeString AND _GrainTypeString IS NOT NULL
      AND ((_GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = _GrainIdExtensionString) OR _GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
      AND ServiceId = _ServiceId AND _ServiceId IS NOT NULL
      AND Version IS NOT NULL AND Version = _GrainStateVersion AND _GrainStateVersion IS NOT NULL
    LIMIT 1;

    IF ROW_COUNT() > 0
    THEN
        SET _newGrainStateVersion = _GrainStateVersion + 1;
    END IF;

    SELECT _newGrainStateVersion AS NewGrainStateVersion;
    COMMIT;
END$$

CREATE PROCEDURE DeleteStorage
(
    in _GrainIdHash INT,
    in _GrainIdN0 BIGINT,
    in _GrainIdN1 BIGINT,
    in _GrainTypeHash INT,
    in _GrainTypeString NVARCHAR(512),
    in _GrainIdExtensionString NVARCHAR(512),
    in _ServiceId NVARCHAR(150),
    in _GrainStateVersion INT
)
BEGIN
    DECLARE _newGrainStateVersion INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    DECLARE EXIT HANDLER FOR SQLWARNING BEGIN ROLLBACK; RESIGNAL; END;

    SET _newGrainStateVersion = _GrainStateVersion;

    -- Default level is REPEATABLE READ and may cause Gap Lock issues
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    START TRANSACTION;
    DELETE FROM OrleansStorage
    WHERE
        GrainIdHash = _GrainIdHash AND _GrainIdHash IS NOT NULL
      AND GrainTypeHash = _GrainTypeHash AND _GrainTypeHash IS NOT NULL
      AND GrainIdN0 = _GrainIdN0 AND _GrainIdN0 IS NOT NULL
      AND GrainIdN1 = _GrainIdN1 AND _GrainIdN1 IS NOT NULL
      AND GrainTypeString = _GrainTypeString AND _GrainTypeString IS NOT NULL
      AND ((_GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = _GrainIdExtensionString) OR _GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
      AND ServiceId = _ServiceId AND _ServiceId IS NOT NULL
      AND Version IS NOT NULL AND Version = _GrainStateVersion AND _GrainStateVersion IS NOT NULL
    LIMIT 1;

    IF ROW_COUNT() > 0
    THEN
        SET _newGrainStateVersion = _GrainStateVersion + 1;
    END IF;

    SELECT _newGrainStateVersion AS NewGrainStateVersion;
    COMMIT;
END$$

DELIMITER $$
CREATE PROCEDURE WriteToStorage
(
    in _GrainIdHash INT,
    in _GrainIdN0 BIGINT,
    in _GrainIdN1 BIGINT,
    in _GrainTypeHash INT,
    in _GrainTypeString NVARCHAR(512),
    in _GrainIdExtensionString NVARCHAR(512),
    in _ServiceId NVARCHAR(150),
    in _GrainStateVersion INT,
    in _PayloadBinary BLOB
)
BEGIN
    DECLARE _newGrainStateVersion INT;
    DECLARE _rowCount INT;
    DECLARE EXIT HANDLER FOR SQLEXCEPTION BEGIN ROLLBACK; RESIGNAL; END;
    DECLARE EXIT HANDLER FOR SQLWARNING BEGIN ROLLBACK; RESIGNAL; END;

    SET _newGrainStateVersion = _GrainStateVersion;

    -- Default level is REPEATABLE READ and may cause Gap Lock issues
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
    START TRANSACTION;

    -- Grain state is not null, so the state must have been read from the storage before.
    -- Let's try to update it.
    --
    -- When Orleans is running in normal, non-split state, there will
    -- be only one grain with the given ID and type combination only. This
    -- grain saves states mostly serially if Orleans guarantees are upheld. Even
    -- if not, the updates should work correctly due to version number.
    --
    -- In split brain situations there can be a situation where there are two or more
    -- grains with the given ID and type combination. When they try to INSERT
    -- concurrently, the table needs to be locked pessimistically before one of
    -- the grains gets @GrainStateVersion = 1 in return and the other grains will fail
    -- to update storage. The following arrangement is made to reduce locking in normal operation.
    --
    -- If the version number explicitly returned is still the same, Orleans interprets it so the update did not succeed
    -- and throws an InconsistentStateException.
    --
    -- See further information at https://learn.microsoft.com/dotnet/orleans/grains/grain-persistence.
    IF _GrainStateVersion IS NOT NULL
    THEN
        UPDATE OrleansStorage
        SET
            PayloadBinary = _PayloadBinary,
            ModifiedOn = UTC_TIMESTAMP(),
            Version = Version + 1
        WHERE
            GrainIdHash = _GrainIdHash AND _GrainIdHash IS NOT NULL
          AND GrainTypeHash = _GrainTypeHash AND _GrainTypeHash IS NOT NULL
          AND GrainIdN0 = _GrainIdN0 AND _GrainIdN0 IS NOT NULL
          AND GrainIdN1 = _GrainIdN1 AND _GrainIdN1 IS NOT NULL
          AND GrainTypeString = _GrainTypeString AND _GrainTypeString IS NOT NULL
          AND ((_GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = _GrainIdExtensionString) OR _GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
          AND ServiceId = _ServiceId AND _ServiceId IS NOT NULL
          AND Version IS NOT NULL AND Version = _GrainStateVersion AND _GrainStateVersion IS NOT NULL
        LIMIT 1;

        IF ROW_COUNT() > 0
        THEN
            SET _newGrainStateVersion = _GrainStateVersion + 1;
            SET _GrainStateVersion = _newGrainStateVersion;
        END IF;
    END IF;

    -- The grain state has not been read. The following locks rather pessimistically
    -- to ensure only on INSERT succeeds.
    IF _GrainStateVersion IS NULL
    THEN
        INSERT INTO OrleansStorage
        (
            GrainIdHash,
            GrainIdN0,
            GrainIdN1,
            GrainTypeHash,
            GrainTypeString,
            GrainIdExtensionString,
            ServiceId,
            PayloadBinary,
            ModifiedOn,
            Version
        )
        SELECT * FROM ( SELECT
                            _GrainIdHash,
                            _GrainIdN0,
                            _GrainIdN1,
                            _GrainTypeHash,
                            _GrainTypeString,
                            _GrainIdExtensionString,
                            _ServiceId,
                            _PayloadBinary,
                            UTC_TIMESTAMP(),
                            1) AS TMP
        WHERE NOT EXISTS
                  (
                      -- There should not be any version of this grain state.
                      SELECT 1
                      FROM OrleansStorage
                      WHERE
                          GrainIdHash = _GrainIdHash AND _GrainIdHash IS NOT NULL
                        AND GrainTypeHash = _GrainTypeHash AND _GrainTypeHash IS NOT NULL
                        AND GrainIdN0 = _GrainIdN0 AND _GrainIdN0 IS NOT NULL
                        AND GrainIdN1 = _GrainIdN1 AND _GrainIdN1 IS NOT NULL
                        AND GrainTypeString = _GrainTypeString AND _GrainTypeString IS NOT NULL
                        AND ((_GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = _GrainIdExtensionString) OR _GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
                        AND ServiceId = _ServiceId AND _ServiceId IS NOT NULL
                  ) LIMIT 1;

        IF ROW_COUNT() > 0
        THEN
            SET _newGrainStateVersion = 1;
        END IF;
    END IF;

    SELECT _newGrainStateVersion AS NewGrainStateVersion;
    COMMIT;
END$$

DELIMITER ;

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ReadFromStorageKey',
        'SELECT
            PayloadBinary,
            UTC_TIMESTAMP(),
            Version
        FROM
            OrleansStorage
        WHERE
            GrainIdHash = @GrainIdHash
            AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
            AND GrainIdN0 = @GrainIdN0 AND @GrainIdN0 IS NOT NULL
            AND GrainIdN1 = @GrainIdN1 AND @GrainIdN1 IS NOT NULL
            AND GrainTypeString = @GrainTypeString AND GrainTypeString IS NOT NULL
            AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
            AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
            LIMIT 1;'
    );

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'WriteToStorageKey','
    call WriteToStorage(@GrainIdHash, @GrainIdN0, @GrainIdN1, @GrainTypeHash, @GrainTypeString, @GrainIdExtensionString, @ServiceId, @GrainStateVersion, @PayloadBinary);'
    );

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ClearStorageKey','
    call ClearStorage(@GrainIdHash, @GrainIdN0, @GrainIdN1, @GrainTypeHash, @GrainTypeString, @GrainIdExtensionString, @ServiceId, @GrainStateVersion);'
    );

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'DeleteStorageKey','
    call DeleteStorage(@GrainIdHash, @GrainIdN0, @GrainIdN1, @GrainTypeHash, @GrainTypeString, @GrainIdExtensionString, @ServiceId, @GrainStateVersion);'
    );
-- Orleans Reminders table - https://learn.microsoft.com/dotnet/orleans/grains/timers-and-reminders
CREATE TABLE OrleansRemindersTable
(
    ServiceId NVARCHAR(150) NOT NULL,
    GrainId VARCHAR(150) NOT NULL,
    ReminderName NVARCHAR(150) NOT NULL,
    StartTime DATETIME NOT NULL,
    Period BIGINT NOT NULL,
    GrainHash INT NOT NULL,
    Version INT NOT NULL,

    CONSTRAINT PK_RemindersTable_ServiceId_GrainId_ReminderName PRIMARY KEY(ServiceId, GrainId, ReminderName)
);

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'UpsertReminderRowKey','
    INSERT INTO OrleansRemindersTable
    (
        ServiceId,
        GrainId,
        ReminderName,
        StartTime,
        Period,
        GrainHash,
        Version
    )
    VALUES
    (
        @ServiceId,
        @GrainId,
        @ReminderName,
        @StartTime,
        @Period,
        @GrainHash,
        last_insert_id(0)
    )
    ON DUPLICATE KEY
    UPDATE
        StartTime = @StartTime,
        Period = @Period,
        GrainHash = @GrainHash,
        Version = last_insert_id(Version+1);


    SELECT last_insert_id() AS Version;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ReadReminderRowsKey','
    SELECT
        GrainId,
        ReminderName,
        StartTime,
        Period,
        Version
    FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND GrainId = @GrainId AND @GrainId IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ReadReminderRowKey','
    SELECT
        GrainId,
        ReminderName,
        StartTime,
        Period,
        Version
    FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND GrainId = @GrainId AND @GrainId IS NOT NULL
        AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ReadRangeRows1Key','
    SELECT
        GrainId,
        ReminderName,
        StartTime,
        Period,
        Version
    FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND GrainHash > @BeginHash AND @BeginHash IS NOT NULL
        AND GrainHash <= @EndHash AND @EndHash IS NOT NULL;
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'ReadRangeRows2Key','
    SELECT
        GrainId,
        ReminderName,
        StartTime,
        Period,
        Version
    FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND ((GrainHash > @BeginHash AND @BeginHash IS NOT NULL)
        OR (GrainHash <= @EndHash AND @EndHash IS NOT NULL));
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'DeleteReminderRowKey','
    DELETE FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND GrainId = @GrainId AND @GrainId IS NOT NULL
        AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL
        AND Version = @Version AND @Version IS NOT NULL;
    SELECT ROW_COUNT();
');

INSERT INTO OrleansQuery(QueryKey, QueryText)
VALUES
    (
        'DeleteReminderRowsKey','
    DELETE FROM OrleansRemindersTable
    WHERE
        ServiceId = @ServiceId AND @ServiceId IS NOT NULL;
');
