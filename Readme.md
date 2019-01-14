## How to run the project

### In development mode
1. Set the build Configuration to either Debug or Release.
2. Start the PortalTeme.Auth project using the **PortalTeme.Auth Profile** and not the **IIS Express** one. You could start the project without attaching the debuger by pressing <kbd>Ctrl</kbd> + <kbd>F5</kbd> in Visual Studio.
3. Start the PortalTeme project with or without the debugger (<kbd>F5</kbd> or <kbd>Ctrl</kbd> + <kbd>F5</kbd>).

### In production mode
1. Go to both project's Properties screen. In the Debug screen set the **ASPNETCORE_ENVIRONMENT** variable to **Production**.
2. Build the angular app by using the **npm build --prod** command inside the **ClientApp** folder.
3. Follow the steps from the development mode.
