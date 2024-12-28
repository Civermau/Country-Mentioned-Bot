# 🌍 Country Mentioned Bot

A Discord bot that responds with images of countries when they are mentioned in chat messages.

## 📜 Project Overview

This bot listens for country names in messages and sends an image of the mentioned country if available.

<div align="center">
  <img src="https://github.com/user-attachments/assets/344b3eed-5dd3-4806-9164-b2f0c6c194e9" alt="Country Mentioned Bot">
</div>

## 💻 How It Works

The project is structured into several key components:

### 1. **Program.cs**
   - Initializes the Discord client and configures event handlers.
   - Loads configuration and country data from JSON files.
   - Handles message events to detect country mentions and respond with images.

### 2. **countries.json**
   - Contains a list of country names that the bot can recognize and respond to.

### 3. **config.json**
   - Stores configuration settings, including the bot token.

### 4. **Images Directory**
   - Contains images of countries named in `countries.json`.
   - Images are named after the country they represent (e.g., `MEXICO.jpg`).

## 🛠️ Setting Up
Since discord bots should always be active, the code must be kept running constantly, to do this, the recommended option is to have the bot code running on a server
Personally, I have the bot running on an [Orange Pi 5](http://www.orangepi.org/html/hardWare/computerAndMicrocontrollers/details/Orange-Pi-5.html) of 16GB RAM, but the bot requires almost no resources
You can probably run it on a Raspberry Pi Zero, or a raspberry 2 or 3, they are cheap and should work.

You must have dotnet installed, locally if you want to edit, on the server to run it on there

Ready to deploy the bot? Follow these steps:

### If you want to edit this bot
1. **Clone this repo:**
   ```bash
   git clone https://github.com/Civermau/Country-Mentioned-Bot.git
   ```
2. Open the project in Visual Studio or your preferred C# IDE.

3. Edit the code to your liking

4. Ensure you have the necessary images in the `Images` directory.

5. Update `config.json` with your Discord bot token.

6. Build and run the solution. use `dotnet build` & `dotnet run`

### If you want to upload it to a server
 1. **Clone this repo on the server, or upload your version to a server**
   ```bash
   git clone https://github.com/Civermau/Country-Mentioned-Bot.git
   ```
 2. run `dotnet publish -O path/of/bot` to generate the DLL, replace path to the desired location
 3. move the file `config.json` (that should have now the token of YOUR bot) to the publish folder
 4. move the file `countries.json` to the publish folder
 5. move the dir `Images` to the publish folder
 6. run `dotnet "country mentioned bot.dll"` or the name of the file you selected
