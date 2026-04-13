#!/bin/bash

BYPASS_ENVIRONMENT_VALIDATION=true dotnet-ef migrations add InitilizationAccount
BYPASS_ENVIRONMENT_VALIDATION=true dotnet-ef database update
