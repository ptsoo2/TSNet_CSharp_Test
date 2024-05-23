@echo off

:a
clear
netstat -ton | grep "172.30.1.62" | grep -iE "time_wait" | wc -l
netstat -ton | grep "172.30.1.62" | grep -iE "close_wait" | wc -l
sleep 1
goto a