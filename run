#!/bin/bash
set -e

source ./scripts/utils/shared

declare -a commands
for i in ./scripts/*; do
    if [ -d "$i" ]; then
        continue
    fi

    file_name=$(basename "$i")
    commands+=("$file_name")
done

command="$1"

is_command=$(is_in_array "$command" "${commands[@]}")

if [ "$command" == "" ] || [ "$command" == "help" ] || [ "$is_command" == "0" ]; then
    echo "Available commands:"
    for i in "${commands[@]}"; do
        echo -e "\t$i"
    done
    log_usage "<command>" "<args>"
    log_usage "<command>" "help"
    exit 1
fi

bash ./scripts/$command "${@:2}"

exit 0