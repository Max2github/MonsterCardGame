#!/bin/bash

source config.sh
echo $info_sep
echo -e "INFO: included config.sh\n"

# docker pull postgres image
docker pull postgres
echo $info_sep
echo -e "INFO: pulled / downloaded postgres\n"

# start docker
docker run -d --name $container -e POSTGRES_USER=$user -e POSTGRES_PASSWORD=$pass -p 5432:5432 postgres
echo $info_sep
echo -e "INFO: started postgres\n"

timeout=10
echo -e "INFO: wait for $timeout seconds"
echo -e "$info_sep\n"
sleep $timeout

# create DB
docker exec -i $container createdb -U $user $db_name
echo $info_sep
echo -e "INFO: created database \"$db_name\" with user \"$user\"\n"
