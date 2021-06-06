@echo off

..\..\SP.StudioCore\Tools\curl -k https://api.a8.to/Permission -F "file=@BW.Common\Properties\SystemPermission.xml" -H "action:XML" -t utf-8  > SystemPermission.tmp

for /f %%i in ("SystemPermission.tmp") do set size=%%~zi
if %size% equ 0 (
	echo 发生错误
	del SystemPermission.tmp	
	exit;
) else (
	echo 下载成功
	move /Y SystemPermission.tmp "BW.Common\Properties\SystemPermission.xml"	
)

