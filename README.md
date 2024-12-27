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

Ready to deploy the bot? Follow these steps:

1. **Clone this repo:**
   ```bash
   git clone https://github.com/Civermau/Country-Mentioned-Bot.git
   ```
2. Open the project in Visual Studio or your preferred C# IDE.

3. Ensure you have the necessary images in the `Images` directory.

4. Update `config.json` with your Discord bot token.

5. Build and run the solution.

6. Invite the bot to your Discord server and start mentioning countries!
