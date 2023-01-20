#!/bin/bash

source config.sh
echo $info_sep
echo -e "INFO: included config.sh\n"

docker stop $container
echo $info_sep
echo -e "INFO: stopped postgres\n"

docker rm $container
echo $info_sep
echo -e "INFO: removed postgres\n"
