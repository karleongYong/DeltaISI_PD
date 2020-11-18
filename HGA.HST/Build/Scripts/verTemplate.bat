@echo off
Rem : This script use for only VC# (AssemblyInfo.cs)
Rem : Generate template for SVN in AsseblyInfo.cs change the last version digit
Rem : to use $WCREV$ as a key to get SVN information
Rem : Argumenet passing
Rem : [1] Path of AssemblyInfo file (AssemblyInfo.template)
Rem : [2] Path of Replace script file(replace.vbs)
Rem :
Rem : 2011/08/15 Eakkaphob B. - Initial released
Rem : 2013/06/27 Sontaya S. - Add yyMM to third digit
Rem ------------------------------------------------------------

:verTemplate
@echo off

echo ---------------- Creating Template --------------------
Rem enable delayexpransion !XXX! to display in for loop
SETLOCAL ENABLEDELAYEDEXPANSION
rem set _file=C:\AAS_SDK\Projects\HGA\TICLogFileManagement\TICLogFileManagement\Build\Scripts\AssemblyInfo.template
set _file=%~1
rem VBScript to do find and replace string in a text file
rem set _scriptFile=C:\AAS_SDK\Projects\HGA\TICLogFileManagement\TICLogFileManagement\Build\Scripts\replace.vbs
set _scriptFile=%~2
set cAssemblyVersion=
set cAssemblyFileVersion=
set cAssemblyFileVersionAttribute=
set cAssemblyVersionReplaceString=
set cAssemblyFileVersionReplaceString=
set cAssemblyFileVersionAttributeReplaceString=

rem open file to read line by line to find the line which has "AssemblyVersion", "AssemblyFileVersion", and "AssemblyFileVersionAttribute" Keyword
rem split word by "[" ":" and "]" and use the second string phase which is should be AssemblyFileVersion("1.0.0.xx")
for /f "tokens=2 delims=[:]" %%G in ('"type %_file%  | findstr "AssemblyVersion AssemblyFileVersion AssemblyFileVersionAttribute""') do (
 		set _tmpStr=%%G
		rem echo !_tmpStr!
		rem replace the " to µ due to can't use double quote as delims
		call set _tmpStr=%%_tmpStr:"=µ%%
		rem echo !_tmpStr!
		rem split the word by us µ character finally go 1.0.0.xx
		rem get current version both AssmblyVersion and AssemblyFileVersion
		for /f "tokens=1,2 delims=µ" %%H in ("!_tmpStr!") do (
			if "%%H"==" AssemblyVersion(" (
				set cAssemblyVersion=%%I
				call :rTrim cAssemblyVersion
			)
			if "%%H"==" AssemblyFileVersion(" (
				set cAssemblyFileVersion=%%I
				call :rTrim cAssemblyFileVersion
			)
			if "%%H"==" AssemblyFileVersionAttribute(" (
				set cAssemblyFileVersionAttribute=%%I
				call :rTrim cAssemblyFileVersionAttribute
			)
		)	
)

rem replace last digit of AssembyVersion
if defined cAssemblyVersion (
				for /f "tokens=1-3 delims=." %%K in ("!cAssemblyVersion!") do (

					rem Change third digit to date time of building in format "YYMM"
					:: Check WMIC is available
					WMIC.EXE Alias /? >NUL 2>&1 || GOTO s_error

					:: Use WMIC to retrieve date and time
					FOR /F "skip=1 tokens=1-2" %%S IN ('WMIC Path Win32_LocalTime Get Month^,Year /Format:table') DO (
   						IF NOT "%%~T"=="" (
      							Set _yyyy=%%T
      							Set _mm=0%%S
						)
					)
					set _yyyy=!_yyyy:~-2!
					set _mm=!_mm:~-2!
					set _ThirdDigit=!_yyyy!!_mm!

					rem Concat string 1.0.0.$WCREV$
					rem Replace %%M by !_ThirdDigit! in order to change third digit
					set cAssemblyVersionReplaceString=%%K.%%L.!_ThirdDigit!.$WCREV$
					rem echo AF !cAssemblyVersionReplaceString! 
				)
) else goto no_asseblyversion

rem replace last digit of AssembyFileVersion or AssembyFileVersionAttribute
if defined cAssemblyFileVersion (
				for /f "tokens=1-3 delims=." %%K in ("!cAssemblyFileVersion!") do (

					rem Change third digit to date time of building in format "YYMM"
					:: Check WMIC is available
					WMIC.EXE Alias /? >NUL 2>&1 || GOTO s_error

					:: Use WMIC to retrieve date and time
					FOR /F "skip=1 tokens=1-2" %%S IN ('WMIC Path Win32_LocalTime Get Month^,Year /Format:table') DO (
   						IF NOT "%%~T"=="" (
      							Set _yyyy=%%T
      							Set _mm=0%%S
						)
					)
					set _yyyy=!_yyyy:~-2!
					set _mm=!_mm:~-2!
					set _ThirdDigit=!_yyyy!!_mm!

					rem Concat string 1.0.0.$WCREV$
					rem Replace %%M by !_ThirdDigit! in order to change third digit
					set cAssemblyFileVersionReplaceString=%%K.%%L.!_ThirdDigit!.$WCREV$
					rem echo AFS !cAssemblyFileVersionReplaceString! 
				)

)else (
	if defined cAssemblyFileVersionAttribute (
				for /f "tokens=1-3 delims=." %%K in ("!cAssemblyFileVersionAttribute!") do (

					rem Change third digit to date time of building in format "YYMM"
					:: Check WMIC is available
					WMIC.EXE Alias /? >NUL 2>&1 || GOTO s_error

					:: Use WMIC to retrieve date and time
					FOR /F "skip=1 tokens=1-2" %%S IN ('WMIC Path Win32_LocalTime Get Month^,Year /Format:table') DO (
   						IF NOT "%%~T"=="" (
      							Set _yyyy=%%T
      							Set _mm=0%%S
						)
					)
					set _yyyy=!_yyyy:~-2!
					set _mm=!_mm:~-2!
					set _ThirdDigit=!_yyyy!!_mm!

					rem Concat string 1.0.0.$WCREV$
					rem Replace %%M by !_ThirdDigit! in order to change third digit
					set cAssemblyFileVersionAttributeReplaceString=%%K.%%L.!_ThirdDigit!.$WCREV$
					rem echo AFS !cAssemblyFileVersionAttributeReplaceString! 
				)
	) else goto no_assemblyfileversion
)

echo Running replace text VBScript (replace.vbs)
Rem >nul used for silence message to output windows
cscript %_scriptFile% %_file% "%cAssemblyVersion%" "%cAssemblyVersionReplaceString%" > nul
echo AssemblyVersion change from "%cAssemblyVersion%" to "%cAssemblyVersionReplaceString%" --Sucesss

if defined cAssemblyFileVersion (
	Rem >nul used for silence message to output windows
	cscript %_scriptFile% %_file% "%cAssemblyFileVersion%" "%cAssemblyFileVersionReplaceString%" > nul
	echo AssemblyFileVersion change from "%cAssemblyFileVersion%" to "%cAssemblyFileVersionReplaceString%" --Sucesss
	goto end
)else (
	Rem >nul used for silence message to output windows
	cscript %_scriptFile% %_file% "%cAssemblyFileVersionAttribute%" "%cAssemblyFileVersionAttributeReplaceString%" > nul
	echo AssemblyFileVersionAttribute change from "%cAssemblyFileVersionAttribute%" to "!cAssemblyFileVersionAttributeReplaceString!" --Sucesss
	goto end
)

:no_asseblyversion
echo **** Creating template error due to can't find AssemblyVersion in %_file% ****
exit /B 10
goto end

:no_assemblyfileversion
echo **** Creating template error due to can't find either AssemblyFileVersion or AssemblyFileVersionAttribute in %_file% ****
exit /B 11
goto end

:s_error
Echo GetDate.cmd
Echo Displays date and time independent of OS Locale, Language or date format.
Echo Requires Windows XP Professional, Vista or Windows 7.
Echo WMIC not run on XP Home edition.
Echo.
Echo Returns 6 environment variables containing isodate,Year,Month,Day,hour and minute.
Based on the sorted date code by Rob van der Woude.
goto end

Rem -----------------------------
:rTrim 
REm string char max -- strips white spaces (or other characters) from the end of a string
Rem                 -- string [in,out] - string variable to be trimmed
Rem                 -- char   [in,opt] - character to be trimmed, default is space
Rem                 -- max    [in,opt] - maximum number of characters to be trimmed from the end, default is 32
Rem $created 20060101 :$changed 20080219 :$categories StringManipulation
Rem $source http://www.dostips.com 
SETLOCAL ENABLEDELAYEDEXPANSION
call set string=%%%~1%%
set char=%~2
set max=%~3
if "%char%"=="" set char= &rem one space
if "%max%"=="" set max=32
for /l %%a in (1,1,%max%) do if "!string:~-1!"=="%char%" set string=!string:~0,-1!
( ENDLOCAL & REM RETURN VALUES
    IF "%~1" NEQ "" SET %~1=%string%
)
goto :eof
Rem -----------------------------

:end
echo ---------------- Template is created ------------------



