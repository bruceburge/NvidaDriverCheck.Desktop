# NvidaDriverCheck.Desktop

From http://www.geforce.com/whats-new/tag/drivers it Gets all the dates, with the CSS class date-display-single

Will setup a folder in your user local app data folder, and create a text file, that text only contains the date string from the last time it checked. It will then compare the local latest, to the one posted to determine if there has been a new driver published.

Uses Nlog, and overly logs everything.

Will create a small system tray icon, checks for new driver on start up, and then once every 24 hours, will show a desktop notification when 
a new post is made. 
