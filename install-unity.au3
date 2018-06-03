#cs ----------------------------------------------------------------------------

 AutoIt Version: 3.3.14.5
 Author:         myName

 Script Function:
	Template AutoIt script.

#ce ----------------------------------------------------------------------------

#include <AutoItConstants.au3>
#include <ScreenCapture.au3>

; Script Start - Add your code below here
Run("C:\Program Files\Unity\Editor\Unity.exe")

; Wait for Unity to Start
WinWaitActive("[Chrome_WidgetWin_0]", "", 60)

; Give Unity some time to load
Sleep(15 * 1000)

; Get the position of the Unity Window
$unityPos = WinGetPos("[Active]")

; User name is selected by default
; **Send User Name**
; Advance to password
Send("{TAB}")
; Password is now selected
; **Send Password**
; Press Enter to advance to the next page
Send("{ENTER}")

; Wait for the next page to load
Sleep(5 * 1000)

; Click on the Free License
MouseMove($unityPos[0] + 700, $unityPos[1] + 300)
MouseClick($MOUSE_CLICK_LEFT)

; Click Next
MouseMove($unityPos[0] + 700, $unityPos[1] + 450)
MouseClick($MOUSE_CLICK_LEFT)

; Click on the bottom bullet point
MouseMove($unityPos[0] + 500, $unityPos[1] + 375)
MouseClick($MOUSE_CLICK_LEFT)

; Click Next
MouseMove($unityPos[0] + 700, $unityPos[1] + 425)
MouseClick($MOUSE_CLICK_LEFT)

; Wait for next page to load
Sleep(10 * 1000)

; Close the Window
WinClose("[Active]")
