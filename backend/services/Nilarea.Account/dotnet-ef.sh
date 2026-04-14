#!/bin/bash

export MYSQL_CONNECTION_STRING_FILE='/home/qianxi/source/repos/Web/nilarea/backend/services/secrets/mysql_connection_string'
export BYPASS_ENVIRONMENT_VALIDATION=true

dotnet-ef migration add
