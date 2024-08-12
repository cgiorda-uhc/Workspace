# -*- coding: utf-8 -*-
"""
Created on Tue Jun 13 11:17:33 2023

@author: squack
"""
import paramiko
import time
import decrypt_password as d
import sys

# python -m pip install  --trusted-host pypi.org --trusted-host pypi.python.org --trusted-host files.pythonhosted.org email.mime
# \\nasv0009\onc_uhg_emp_win_sas\Program\UEP\ICUE_ADT\EI_ER_report\SAS_file\SDR
# MS\vc_auto password is: .517N6PCm<|o1=Uv
# python "C:\Windows\Python\ADT_automation_Snowflake_20231121.py"
#CHECK FOR NEW DATA
#   NOTHING =  EXIT
#   SOMETHING = BELOW

p = paramiko.SSHClient()
p.set_missing_host_key_policy(paramiko.AutoAddPolicy()) 
usr_dict = {'squack1':'squack'}
usr = 'squack1'
pwd = d.decrypt_password(usr_dict[usr])
p.connect("sasfusionm6.uhc.com", port=22, username=usr_dict[usr], password=pwd)
sh = p.invoke_shell()
sh.send("cd '/hpsasfin/int/winfiles7/Program/UEP/ICUE_ADT/EI_ER_report/SAS_file/SDR/'\r; sas ADT_Daily_Report_Snowflake_20231121.sas\r")
time.sleep(10)
sh.close()
p.close()


# CHECK FILE AND OTHER ISSUES
# SEND EMAILS ACCORDINGLY