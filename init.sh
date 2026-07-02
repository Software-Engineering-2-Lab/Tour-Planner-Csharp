#!/bin/bash
set -e

# Execute SQL commands using the default Postgres SuperUser
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    -- Create the restricted application user
    CREATE USER $DB_USER WITH PASSWORD '$DB_PASSWORD';

    -- Grant basic database privileges
    GRANT ALL PRIVILEGES ON DATABASE $POSTGRES_DB TO $DB_USER;

    -- Connect to the target database
    \c $POSTGRES_DB;

    -- Grant schema privileges (Required for PostgreSQL 15+)
    GRANT ALL ON SCHEMA public TO $DB_USER;
EOSQL