# 🚀 Vortex

Project built in a "vibe-coding" style — exploratory, creative and iterative, focused on experimenting with real-world hardware + software integration.

# What it is

Vortex is a real-time animation and playback system built in .NET, designed to run on a Raspberry Pi connected to a 16x16 LED matrix and speaker.

It integrates with LibreSpot (Spotify client) by reading its logs to detect playback state and dynamically switch between animation modes.

When music is playing → reactive animations (inspired by Windows Media Player)

When idle → pixel-art animations (emojis, clocks, etc.)

The goal is to create a DIY Divoom-like device, combining software, embedded systems and hardware experimentation.

# Key ideas

Event-driven behavior based on external system logs (LibreSpot)

Real-time rendering pipeline for LED matrix

Separation between playback state and visual system

Focus on low-resource environments (Raspberry Pi)

# Architecture

Animations/ → visual logic and effects

Playback/ → state detection and control

Rendering/ → LED matrix rendering pipeline

# Why this exists

This project is less about the final product and more about exploring:

hardware/software integration

real-time systems

creative coding

# How to run

Open Vortex.sln in Rider or Visual Studio

Run the Vortex project

# 🤝 Contribution

Feel free to fork, experiment and extend the idea.
