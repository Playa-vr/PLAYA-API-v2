# Introduction

This repository contains:
- [Documentation](docs.md) of Playa V2 protocol used in PlayaVR app to access and play videos hosted on third party websites
- Minimal server implementation (in Asp.Net Core) to understand where to start.

# Getting Started

1. Clone repository.
1. Open PlayaApiV2.sln in VisualStudio or other IDE
1. Launch PlayaApiV2 project.
	- Popup to install development SSL sertificate may appear.
1. Open PlayaVR application
1. Go to ``Web`` page
1. Click ``Add Website`` button
1. Enter ``localhost:4430`` and click ``Connect``
	- If you does not install SSL previously: use ``http://localhost:8080
1. PlayaVR should successfuly connect to website and show one sample video.

# Note

The server is 100% bare bones.
While you can use it as starting platform to build your own server - it is recommended to use some well established project template or use different programming language.