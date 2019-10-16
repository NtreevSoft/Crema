#!/usr/bin/env bash

uninstall() {
    if ! [ -e "/etc/systemd/system/crema.service" ]; then
        echo "WARN: Crema service not exists."
        exit 0;
    fi

    systemctl stop crema.service
    systemctl disable crema.service
    rm /etc/systemd/system/crema.service

    if [ $? != 0 ]; then
        echo "ERROR: Could not delete file. (etc/systemd/system/crema.service)"
        exit 1
    fi
}

uninstall_on_mac() {
    launchctl stop crema
    launchctl unload -w crema.plist
    rm crema.plist
}

help() {
    echo "Usage"
    echo " $0"
}

osname="$(uname -a)"
case "$osname" in
    *Linux*)
        uninstall
        ;;
    *Darwin*)
        uninstall_on_mac
        ;;
    *)
        echo "ERROR: Not supports os. ('$osname')"
        exit 1
    ;;
esac