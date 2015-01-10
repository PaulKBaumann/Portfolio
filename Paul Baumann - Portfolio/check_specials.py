from bs4 import BeautifulSoup
import urllib.request
import smtplib
import datetime
from time import sleep

###########################
# Written by Paul Baumann #
################################################################################
# This Python script scraps the daily specials page for a local pub. It checks
# for today's special and waits until the next day to try again. It checks every
# 15 minutes until the next day's specials have been posted.
#
# The BeautifulSoup library is used to grab and parse the HTML.
# smtplib is used in conjunction with Gmail to send a text message to a phone.
#
# It is important to note that Gmail security settings were altered to allow
# third party applications to send messages from the account.
#
#############
# Constants #
################################################################################
url = "http://www.dogandbullhouse.com/chalkboard_specials.php"
gmail = 'paulkbaumann@gmail.com'
password = '********' #replace with password
phone = '5555555555' #replace with mobile phone number

update_interval = 900 #15 minutes
day_map = {0 : "Monday", 1 : "Tuesday", 2 : "Wednesday", 3 : "Thursday",
           4 : "Friday", 5 : "Saturday", 6 : "Sunday"}
################################################################################

old_day = -1 #initialize to -1 to guarantee first run

while True:

    #check website once per day
    current_day = datetime.datetime.today().weekday()
    if current_day == old_day:
        sleep(update_interval)
        continue
    current_day_name = day_map[current_day]

    #Grab the current menu    
    response = urllib.request.urlopen(url)
    html = response.read()
    soup = BeautifulSoup(html)
    text = soup.find(id="dynamic_content").get_text()

    if current_day_name not in text:
        #Menu hasn't been updated; wait and try again
        sleep(update_interval)
        continue
    else:

        old_day = current_day

        #reformat for SMS; following line gets rid of extra whitespace
        text = text.replace('\n\n\n', '\n\n')
        text = text.encode('utf-8')

        #connect to Gmail
        server = smtplib.SMTP( "smtp.gmail.com", 587 )
        server.starttls()
        server.login( gmail, password )

        #Send to SMS portal.
        #@tmomail.net is for T-Mobile. Each carrier has their own
        server.sendmail('Paul', phone + '@tmomail.net', text)
        print(text)

        print('sent')
        sleep(update_interval)
        #break
