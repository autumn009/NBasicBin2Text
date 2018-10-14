@echo off

set opt=
set diff=tstasc
set dst=result.txt
call runtestsub.cmd

set opt=-p
set diff=tstascp
set dst=resultp.txt
call runtestsub.cmd

pause
