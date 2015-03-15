# What is it? #

Boxing Stopwatch is a little application that allows to display a digital timer. Basically you can define the length of a round and the pause between each round to let it run.

I develop this tool to for my dojo. We have a PC in the training room to listen to music during training sessions =) Feel free to use it as well.

# Screenshots #

![http://grrbrr.de/projects/gui.png](http://grrbrr.de/projects/gui.png)

# Features #

  * Start & Stop for the digital timer
  * Configure the length for round and pause phases (For example 2 minute round, 30 second pause to simulate a ring-fight)
  * You can define multiple round templates for later use
  * Fullscreen mode
  * Customizable colors
  * Internal web service allows you to remote control the application from any device via web interface (http://localhost:8080)
  * Plays ring sounds at the at of round as well as 10 second warning


# Todo #
  * Enhance remote control (Implement clock display)
  * Implement background images into configuration window
  * Add new phase "Get Ready" with configurable time. "Get Ready" is a countdown before a new round begins - So it will be used before the very first round and after each pause phase before a new round begins
  * Add feature to show a information text between clock and round display to indicate the current phase ... such like "Get Ready ... ", "Pause ... "
  * Make it possible to add custom sounds via configuration window


# System Requirements #
  * .NET 4.0
  * Requires administrator permissions
