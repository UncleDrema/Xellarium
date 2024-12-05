#!/bin/bash
set -e

# create xellarium database
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOS
  CREATE DATABASE xellarium;
EOS
 
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "xellarium" <<-EOSQL
  ALTER SYSTEM RESET shared_preload_libraries;
  CREATE EXTENSION pglogical;

  SELECT pglogical.create_node(
      node_name := '$TARGET',
      dsn := 'host=$TARGET port=5432 dbname=xellarium user=$POSTGRES_USER password=$POSTGRES_PASSWORD');
    
EOSQL