@ECHO OFF
SETLOCAL

SET CREMA_EXECUTION_FILENAME=cremaserver.exe
SET CREMA_REPO_PATH=%~1%

:VALIDATE
IF NOT EXIST "%CREMA_EXECUTION_FILENAME%" (
    CALL :WRITE-ERROR "ERROR: '%CREMA_EXECUTION_FILENAME%' file is not exists."
    GOTO :END
)

IF "%CREMA_REPO_PATH%" == "" (
    CALL :WRITE-ERROR "ERROR: The path argument is missing."
    CALL :HELP
    GOTO :END
)
GOTO :RUN

:RUN
.\cremaserver.exe service install "%CREMA_REPO_PATH%"
GOTO :END

:HELP
ECHO Usage
ECHO     .\%~nx0 ^<crema-repo-path^>
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