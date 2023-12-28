# M3U-Editor

Console Application to modify groups for IPTV m3u files.  
Can be set to run on start up and periodically update the m3u file.  

I recomend publishing the m3u file to something like google drive and using a direct link to get to show get the file. Ex. https://sites.google.com/site/gdocs2direct/  

If you are using xtreme codes you should be able to generate an m3u link using something like this:

Get the m3u file:  
<host>/get.php?username=<user>&password=<pass>&output=ts&type=m3u_plus

You can then host the new m3u file, and access it in your apps.


Some apps will need the EPG file which should be able to be entered directly from your service provider.  
Get the EPG/XMLTV File:  
<host>/xmltv.php?username=<user>&password=<pass>&output=ts&type=m3u_plus