#!/usr/bin/env bash

COMMAND=$1
REPO_DIR="$2"
LOCK_FILE="/tmp/cremaservice.exe.lock"

start() {
    osname="$(uname -a)"
    case "$osname" in
        *Linux*)
            /usr/bin/mono-service -l:$LOCK_FILE --debug cremaservice.exe $REPO_DIR 4004 4104
            ;;
        *Darwin*)
            trap stop SIGTERM
            trap stop SIGKILL
            /Library/Frameworks/Mono.framework/Versions/Current/Commands/mono-service -l:$LOCK_FILE --debug cremaservice.exe $REPO_DIR 4004 4104 &
            tail -f /dev/null &
            wait $!
            ;;
    esac
}

stop() {
    if [ -f "$LOCK_FILE" ]; then
        echo "- Remove lock file ($LOCK_FILE)."
		kill -15 $(cat $LOCK_FILE)
		rm $LOCK_FILE
	fi
}

case "$COMMAND" in
	start)
        start
		;;
	*)
		echo "Usage"
		echo "  $0 <command> <current directory> <repo dir>"
		;;

esac