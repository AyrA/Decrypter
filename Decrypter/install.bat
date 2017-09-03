@ECHO OFF
SETLOCAL
PUSHD %~dp0
SET DECRYPTER=%CD%\Decrypter.exe
IF NOT EXIST "%DECRYPTER%" GOTO ERRLOC
GOTO ADMINTEST

:ADMINTEST
REM random file
SET RND=%SYSTEMROOT%\System32\__TMP_ADMINTEST.tmp
REM Try to delete existing file (if any).
IF EXIST "%RND%" DEL "%RND%"
REM If file still exists we are probably not admin
IF EXIST "%RND%" GOTO NOADMIN
REM Try to create temp file
ECHO You can safely delete this file if you want to>"%RND%"
REM If file does not exists we are probably not admin.
IF NOT EXIST "%RND%" GOTO NOADMIN
REM Delete file
DEL "%RND%"
GOTO SETUP

:NOADMIN
ECHO You need to run this file as administrator.
ECHO Right click the file and chose "Run as Administrator".
GOTO END

:SETUP
REM Associate Encrypted containers with a common file type
ASSOC .rsdf=EncryptedLinkContainer
ASSOC .ccf=EncryptedLinkContainer
ASSOC .dlc=EncryptedLinkContainer
REM Associate file type with the decrypter executable
FTYPE EncryptedLinkContainer="%DECRYPTER%" "%%1"
REM Success
ECHO Installation complete
GOTO END

:ERRLOC
REM Decrypter.exe was not found
ECHO Unable to locate %DECRYPTER%
ECHO.
ECHO Ensure you run this file from the location that contains Decrypter.exe
GOTO END

:END
POPD
ENDLOCAL
PAUSE
