@ECHO OFF
SETLOCAL

SET CREMA_EXECUTION_FILENAME=cremaserver.exe
SET CREMA_DAEMON_COMMAND=%1
SET CREMA_REPO_PATH=%~2%

:VALIDATE
IF NOT EXIST "%CREMA_EXECUTION_FILENAME%" (
    CALL :WRITE-ERROR "ERROR: '%CREMA_EXECUTION_FILENAME%' file is not exists."
    GOTO :END
)

IF "%CREMA_DAEMON_COMMAND%" == "" (
    CALL :WRITE-ERROR "ERROR: The command argument is missing."
    CALL :HELP
    GOTO :END
)

IF "%CREMA_REPO_PATH%" == "" (
    CALL :WRITE-ERROR "ERROR: The path argument is missing."
    CALL :HELP
    GOTO :END
)
GOTO :RUN

:RUN
IF "%CREMA_DAEMON_COMMAND%" == "init" (
	cremaserver.exe init "%CREMA_REPO_PATH%" --force
	GOTO :END
)
IF "%CREMA_DAEMON_COMMAND%" == "start" (
	cremaserver.exe run "%CREMA_REPO_PATH%"
	GOTO :END
)
IF "%CREMA_DAEMON_COMMAND%" == "init-start" (
	cremaserver.exe init "%CREMA_REPO_PATH%" --force
	cremaserver.exe run "%CREMA_REPO_PATH%"
	GOTO :END
)
CALL :WRITE-ERROR "ERROR: Unknown command or path"
GOTO :END

:HELP
ECHO Usage
ECHO "    .\%~nx0 init|start|init-start ^<crema-repo-path^>"
EXIT /B

:WRITE-TEXT
ECHO %~1%
EXIT /B

:WRITE-ERROR
ECHO %~1% 1>&2
EXIT /B 1

:END
IF NOT "%ERRORLEVEL%" == "0" (
    ECHO.
    ECHO One or more errors have occurred.
)
@ECHO ON