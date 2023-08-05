# RemoteShutdown
ASP.NET Core application to halt the local linux server remotely

Setup:
* Change in appsetting.json the HTTP Certificate and Key path
* Add to sudoers file permission for the app user to halt system:
```shutdownapp ALL=(ALL) NOPASSWD:/usr/sbin/halt```


How to call remotely:
```curl -H 'XAPIKEY:<APIKEYIN User Secrets>' https://<address>:<port>/shutdown```
