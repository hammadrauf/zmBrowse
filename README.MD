# zmBrowse
Zone-Minder (Security Camera Station Software) Events Browser

## Images
![Image 1](https://github.com/hammadrauf/zmBrowse/blob/main/images/screenshot-generating.png?raw=true)

![Image 2](https://github.com/hammadrauf/zmBrowse/blob/main/images/screenshot-ready.png?raw=true)

![Image 3](https://github.com/hammadrauf/zmBrowse/blob/main/images/screenshot-mouse-clicks.png?raw=true)

## Description
zmBroswe is a helper application to be used in tandem with a Zone-Minder installation on a local area network.
zmBroswe is primarily written for Windows 10/11 computers, but it can be executed on Linux GUI Desktops using Wine,
and on MacOS can be most probably used by using Wine and Rosetta.

## How To Configure ZoneMinder for use with zmBrowse
1. Install ZoneMinder on your server.
	- 1. ZoneMinder Installation - [Official Installation Guide](https://zoneminder.readthedocs.io/en/latest/installationguide/index.html)	
1. On ZoneMinder Options page go to Config tab.
	a. Set the following options:
		- EVENT_IMAGE_DIGITS: 5
1. On ZoneMinder Options page go to Storage tab.
	a. Note down the following option values:
		- Storage: Default - Path (Should be something like /var/cache/zoneminder/events)
1. You can share this path using Samba or NFS. Sharing with Samba has been tested with zmBrowse to work ok.
```
user01@someserver:~$ sudo ls -lart /var/cache/zoneminder/events
total 20
drwxr-xr-x  5 www-data www-data 12288 Jan 29 20:07 ..
lrwxrwxrwx  1 www-data www-data     1 Jan 29 21:47 IPCAM -> 1
drwxr-xr-x  3 www-data www-data  4096 Jan 29 21:47 .
drwxr-xr-x 73 www-data www-data  4096 May  5 01:10 1

user01@someserver:~$ sudo cat /etc/samba/smb.conf | grep -A6 -i '\[ZM_EVENTS\]'
[ZM_Events]
comment = Zoneminder Events
path = /var/cache/zoneminder/events
valid users = @users
create mask = 0660
directory mask = 0771
read only = yes
user01@someserver:~$

user01@someserver:~$ sudo tree /var/cache/zoneminder/events -L 2
/var/cache/zoneminder/events
├── 1
│   ├── 2023-10-25
│   ├── 2023-11-09
│   ├── 2023-11-10
│   ├── 2023-11-11
│   ├── 2025-02-28
│   ├── 2025-03-01
│   ├── 2025-03-02
│   ├── 2025-03-03
│   ├── 2025-03-04
│   ├── 2025-03-05
│   ├── 2025-03-06
│   ├── 2025-03-07
│   ├── 2025-03-08
│   ├── 2025-03-09
│   ├── 2025-03-10
│   ├── 2025-03-11
│   ├── 2025-03-12
│   ├── 2025-03-13
│   ├── 2025-03-14
│   ├── 2025-03-15
│   ├── 2025-03-16
│   ├── 2025-03-17
│   ├── 2025-03-18
│   ├── 2025-03-19
│   ├── 2025-03-20
│   ├── 2025-03-21
│   ├── 2025-03-22
│   ├── 2025-03-23
│   ├── 2025-03-24
│   ├── 2025-03-25
│   ├── 2025-03-26
│   ├── 2025-03-27
│   ├── 2025-03-28
│   ├── 2025-03-29
│   ├── 2025-03-30
│   ├── 2025-03-31
│   ├── 2025-04-01
│   ├── 2025-04-02
│   ├── 2025-04-03
│   ├── 2025-04-04
│   ├── 2025-04-05
│   ├── 2025-04-06
│   ├── 2025-04-07
│   ├── 2025-04-08
│   ├── 2025-04-09
│   ├── 2025-04-10
│   ├── 2025-04-11
│   ├── 2025-04-12
│   ├── 2025-04-13
│   ├── 2025-04-14
│   ├── 2025-04-15
│   ├── 2025-04-16
│   ├── 2025-04-17
│   ├── 2025-04-18
│   ├── 2025-04-19
│   ├── 2025-04-20
│   ├── 2025-04-21
│   ├── 2025-04-22
│   ├── 2025-04-23
│   ├── 2025-04-24
│   ├── 2025-04-25
│   ├── 2025-04-26
│   ├── 2025-04-27
│   ├── 2025-04-28
│   ├── 2025-04-29
│   ├── 2025-04-30
│   ├── 2025-05-01
│   ├── 2025-05-02
│   ├── 2025-05-03
│   ├── 2025-05-04
│   └── 2025-05-05
└── IPCAM -> 1

74 directories, 0 files
user01@someserver:~$

user01@someserver:~$ sudo tree /var/cache/zoneminder/events/1/2025-05-05 -L 1
/var/cache/zoneminder/events/1/2025-05-05
├── 78782
├── 78783
├── 78784
├── 78785
├── 78786
├── 78787
├── 78788
├── 78789
├── 78790
├── 78791
├── 78792
├── 78793
├── 78794
├── 78795
├── 78796
├── 78797
├── 78798
├── 78799
├── 78800
├── 78801
├── 78802
├── 78803
├── 78804
├── 78805
├── 78806
├── 78807
├── 78808
├── 78809
├── 78810
├── 78811
├── 78812
├── 78813
├── 78814
├── 78815
├── 78816
├── 78817
├── 78818
├── 78819
├── 78820
├── 78821
├── 78822
├── 78823
├── 78824
├── 78825
├── 78826
├── 78827
├── 78828
├── 78829
├── 78830
├── 78831
├── 78832
├── 78833
├── 78834
└── 78835

55 directories, 0 files
user01@someserver:~$

```

## How To Use zmBrowse
1. Install zmBrowse on your Windows 10/11 PC or Linux GUI/Desktop (with Wine [https://www.winehq.org/](https://www.winehq.org/) )
	- Download the latest installer named "zmBrowse-Installer-win-*.exe" from [Release Page Link](https://github.com/hammadrauf/zmBrowse/releases)
1. Run the installer and follow the on screen instructions.
1. Once installed, run the zmBrowse application.
1. Click on the "Settings" button and enter the following details:
	- ZM_SERVER EVENTS_PATH: Path to the events folder on your ZoneMinder server. This should be the path you noted down in step 3 above.
	- If ZoneMinder server is on local area network, then you will need to save its connection user name and password in Windows Credential Manager.
	- On Linux if using Wine, if your Zone-Minder is on Local Area Network, then you can map the network drive first.
1. Save Settings and Exit zmBrowse.
1. Restart zmBroswe application, your settings should now take effect.

## Author
- Hammad Rauf

## License
- MIT License (MIT), Copyright (c) 2025 Hammad Rauf <rauf.hammad@gmail.com>
