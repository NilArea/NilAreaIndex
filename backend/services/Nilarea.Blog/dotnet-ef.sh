#!/bin/bash

BYPASS_ENVIRONMENT_VALIDATION=true dotnet-ef migrations add InitilizationBlog
BYPASS_ENVIRONMENT_VALIDATION=true dotnet-ef database update