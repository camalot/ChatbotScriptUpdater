

# CHATBOT SCRIPT UPDATER

This is an application that can be included with your streamlabs chatbot scripts and can go out and download the latest release (zipped) from github release.


### 

```json
{
  ...

	"OpenScriptUpdater": {
		"type": "button",
		"label": "Check For Updates",
		"tooltip": "Check for updates",
		"function": "OpenScriptUpdater",
		"wsevent": "EVENT_NONE"
	}

...
}

```


```python

# Add Global Variable to the top
Repo = "camalot/chatbot-shoutout"

#
# ...
#

# Add this function to the Script Module
def OpenScriptUpdater():
    currentDir = os.path.realpath(os.path.dirname(__file__))
    chatbotRoot = os.path.realpath(os.path.join(currentDir, "../../../"))
    libsDir = os.path.join(currentDir, "libs/updater")
    Parent.Log(ScriptName, libsDir)
    try:
        src_files = os.listdir(libsDir)
        tempdir = tempfile.mkdtemp()
        Parent.Log(ScriptName, tempdir)
        for file_name in src_files:
            full_file_name = os.path.join(libsDir, file_name)
            if os.path.isfile(full_file_name):
                Parent.Log(ScriptName, "Copy: " + full_file_name)
                shutil.copy(full_file_name, tempdir)
        updater = os.path.join(tempdir, "ChatbotScriptUpdater.exe")
        updaterConfigFile = os.path.join(tempdir, "chatbot.json")
        repoVals = Repo.split('/')
        updaterConfig = {
            "path": os.path.realpath(os.path.join(currentDir,"../")),
            "version": Version,
            "name": ScriptName,
            "chatbot": os.path.join(chatbotRoot, "Streamlabs Chatbot.exe"),
            "kill": [], # Array of process names to stop
            "execute": {
              "before": [{
								"command": "cmd",
								"arguments": [ "/c", "del /f /q /s *" ],
								"workingDirectory": "${PATH}\\${SCRIPT}\\Libs\\",
								"ignoreExitCode": true,
								"validExitCodes":  [0]
							}], # commands to run before extraction
              "after": [] # command to run after extraction
            },
            "script": os.path.basename(os.path.dirname(os.path.realpath(__file__))),
            "website": Website,
            "repository": {
                "owner": repoVals[0],
                "name": repoVals[1]
            }
        }
        Parent.Log(ScriptName, updater)
        configJson = json.dumps(updaterConfig)
        Parent.Log(ScriptName, configJson)
        with open(updaterConfigFile, "w+") as f:
            f.write(configJson)
        os.startfile(updater)
    except OSError as exc: # python >2.5
        raise

```
