# WindowManager (wmgr.dll)

This class library extends base WPF functionality on windows.

## Functions:
* Properly resizes windows with WindowStyle property of value None;
* Adjusts window size when TaskBar is in auto hide mode;
* SystemCommands added to window in AddResizeHook method;
* MinMaxClose animations restored;
* Added new manager (InstanceManager) that prevents multi instance of dependent app;

That`s all what i have for now

P.S. Don`t know why yet, but TaskBar functions doesn`t work in debug mode, only without debugging (VS 2022 Win 11)