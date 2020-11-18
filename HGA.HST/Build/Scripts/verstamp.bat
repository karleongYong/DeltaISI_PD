@echo off
Rem : This script uses SubWcRev to obtain svn revision information and 
Rem : use as the final didgits of assembly file version 
Rem : This script file is meant to be used as a pre-build event in VS Studio 
Rem :
Rem : Eakkaphob B.
Rem : 2011/08/13
Rem --------------------------------------------------------

echo ############################################################################################################################################
echo #Start verstamp.bat file
setlocal

if not exist "C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" goto no_svnwcrev

Rem get working directory from argument 1, once this batch file is called
SET workingDir=%0%\..\
SET assemblyFile="%1%Properties\AssemblyInfo.cs"
SET templateFile="%workingDir%AssemblyInfo.template" 
SET scriptFile="%workingDir%replace.vbs"

Rem copy an AssemblyInfo.cs for creating template 
if not exist "%assemblyFile%" goto no_assembly
copy %assemblyFile% %templateFile%
if not exist "%templateFile%" goto no_template
echo.

Rem change curret directory
cd /d "%workingDir%"

Rem call script to create template for updating version
call verTemplate.bat %templateFile% %scriptFile%

Rem : Run SubWVRev tool 
echo.
echo -------------- TortoiseSVN Information ----------------
"C:\Program Files\TortoiseSVN\bin\SubWCRev.exe" %1 %templateFile% %assemblyFile%
echo -------------------------------------------------------
goto end

Rem error handling section
:no_svnwcrev
echo #*****Batch file error -- SubWCRev (C:\Program Files\TortoiseSVN\bin\SubWCRev.exe) is not present.*****
exit /B 1
goto end

:no_assembly
echo #*****Batch file error -- Assembly file (%assemblyFile%) is not found.*****
exit /B 2
goto end

:no_template
echo #*****Batch file error -- Template file (%templateFile%) is not found.*****
exit /B 3
goto end

:end
echo #End verstamp.bat file
echo ############################################################################################################################################
echo Software Revision Standard Rev.04 [17Jun13]
echo ############################################################################################################################################
endlocal