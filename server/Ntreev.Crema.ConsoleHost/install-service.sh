#!/usr/bin/env bash

CREMA_EXECUTION_FILENAME="cremaserver.exe"
OPTION_REPO_DIR="$1"

validate() {
    local HAS_ERROR=false

    if ! [ `command -v svn` ]; then
        echo "ERROR: 'svn' is not installed."
        HAS_ERROR=true
    fi

    if ! [ `command -v mono` ]; then
        echo "ERROR: 'mono' is not installed."
        HAS_ERROR=true
    fi

    if ! [ -f "$CREMA_EXECUTION_FILENAME" ]; then
        echo "ERROR: '$CREMA_EXECUTION_FILENAME' file is not exists."
        HAS_ERROR=true
    fi
    
    if $HAS_ERROR; then
        exit 1
    fi
}

install() {
    content="$(cat install-service.linux.template)"
    content="${content//\{pwd\}/`pwd`}"
    content="${content//\{repo-dir\}/$OPTION_REPO_DIR}"
    echo "$content" > /etc/systemd/system/crema.service
    if [ $? != 0 ]; then
        echo "ERROR: Could not write file. (etc/systemd/system/crema.service)"
        exit 1
    fi

    systemctl enable crema.service
    systemctl start crema.service
}

install_on_mac() {
    content="$(cat install-service.macos.template)"
    content="${content//\{pwd\}/`pwd`}"
    content="${content//\{repo-dir\}/$OPTION_REPO_DIR}"
    echo "$content" > /Library/LaunchDaemons/crema.plist
    if [ $? != 0 ]; then
        echo "ERROR: Could not write file. (crema.plist)"
        exit 1
    fi

    launchctl load -w /Library/LaunchDaemons/crema.plist
    launchctl start crema
}

help() {
    echo "Usage"
    echo " $0 <crema-repo-path>"
}

if [ -z "$OPTION_REPO_DIR" ]; then
    help
    exit
fi

osname="$(uname -a)"
case "$osname" in
    *Linux*)
        install
        ;;
    *Darwin*)
        install_on_mac
        ;;
    *)
        echo "ERROR: Not supports os. ('$osname')"
        exit 1
    ;;
esac