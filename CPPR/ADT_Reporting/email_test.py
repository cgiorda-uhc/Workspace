#! /usr/local/bin/python


SMTPserver = 'mailo2.uhc.com'
sender =     'chris_giordano@uhc.com'
destination = ['chris_giordano@uhc.com']

# typical values for text_subtype are plain, html, xml
text_subtype = 'plain'

content="""\
Test message
"""

subject="Sent from Python"

import sys
import time

#from smtplib import SMTP_SSL as SMTP       # this invokes the secure SMTP protocol (port 465, uses SSL)
from smtplib import SMTP                  # use this for standard SMTP protocol   (port 25, no encryption)

# old version
#from email.MIMEText import MIMEText
from email.mime.text import MIMEText

try:
    msg = MIMEText(content, text_subtype)
    msg['Subject']=       subject
    msg['From']   = sender # some SMTP servers will do this automatically, not all

    server= SMTP(SMTPserver, 25)
    # server.connect(SMTPserver,465)
    # identify ourselves to smtp client
    server.ehlo()
    # secure our email with tls encryption
    # server.starttls()
    # re-identify ourselves as an encrypted connection
    # server.ehlo()
    # server.set_debuglevel(False)
    # server.login(USERNAME, PASSWORD)
    try:
        for x in range(100):
            while True:
                try:
                    #code with possible error
                    server.sendmail(sender, destination, msg.as_string())
                    print("Sent = " + str(x))
                except:
                     print("Fail = " + str(x))
                     for i in range(3600):
                        time.sleep(1)
                        print('Waiting for email limit: [%d%%]\r'%i, end="")
                     continue
                else:
                     #the rest of the code
                     break

    finally:
        server.quit()

except:
    sys.exit( "mail failed; %s" % "CUSTOM_ERROR" ) # give an error message