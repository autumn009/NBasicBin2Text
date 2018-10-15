@echo off

set opt=
set diff=tstasc
set dst=result.txt
call runtestsub.cmd

set opt=-p
set diff=tstascp
set dst=resultp.txt
call runtestsub.cmd

set opt=-e
set diff=tstasce
set dst=resulte.txt
call runtestsub.cmd

set opt=-p -e
set diff=tstascpe
set dst=resultpe.txt
call runtestsub.cmd

set opt=-g
set diff=tstascg
set dst=resultg.txt
call runtestsub.cmd

set opt=-l
set diff=tstascl
set dst=resultl.txt
call runtestsub.cmd

pause
