## 1.9.1 Added video status for list view
This is not breaking change.

- Added new status field at [VideoListView](docs.md#videolistview)

## 1.9.0 Added authorization via code feature
This is not breaking change.

- Added new requests for Log-in with code: [GetCode](docs.md#getcode) and [SignInCode](docs.md#signincode)
- Added new API status codes
- Added [AuthenticationCode](docs.md#authenticationcode) object

This feature will be added in Play'a 3.0.5

## 1.8.0 New information about the VideoScript
This is breaking change.

This update includes changing information about VideoScript [VideoScript](docs.md#videoscript) at [VideoView.Details](docs.md#videoviewdetails).

- Removed ScriptId field from [VideoView.Details](docs.md#videoviewdetails) object
- Added [ScriptInfo](docs.md#scriptinfo) field to [VideoView.Details](docs.md#videoviewdetails) object

This feature will be added in Play'a 3.0.5

## 1.7.3 New Additional Information for list view
This is not breaking change.

This update includes adding additional information about site [Configuration](docs.md#configuration) and [VideoListView](docs.md#videolistview)
Including script support in video and the type of [TransparencyMode](docs.md#transparencymode).

- Extended [Configuration](docs.md#configuration) object
- Extended [VideoListView.Details](docs.md#videolistviewdetails) object

## 1.7.2 Alpha mask mode
This is breaking change. But considering lack of Play'a app releases: there is nothnig to break.

Support for 1.6.0+ features is expected to be added in Play'a 2.3.13

- Altered [TransparencyInfo](docs.md#transparencyinfo) object. External alpha mask will use newely added mode 3 instead of mode 1 reserved for embedded alpha mask.

## 1.7.0 Scripts
- Added [GetScriptsInfo](docs.md#getscriptsinfo) request
- Added [ScriptsInfo](docs.md#scriptsinfo) object
- Extended [VideoView.Details](docs.md#videoviewdetails) object
- Fixed some links in docs

## 1.6.0 External AlphaMask
> [!IMPORTANT]
> TransparencyInfo defenition altered in 1.7.2 update. Play'a 2.3.12 does not received this update. All changes postponed to the next relase.

This update adds support for playing passthrough videos with alpha mask from external video file.
Playa app will stream both (color and mask) videos at the same time.


On top of that, alpha mask now can have up to 3 toggleable layers.
Those layers stored in red, green and blue (in that order) color channels and can overlap.
User can enable/disable individual layers to hide/show different sets of objects.
For convinience each layer can have user-friendly name.


To use this new features you need to configure them in TrancparencyInfo object.
Passthrough settings window in Play'a was extended for this.
You need to enable EditMode to see and modify these options.


- Added [AlphaChannel](docs.md#alphachannel) object
- Extended [TransparencyInfo](docs.md#transparencyinfo) object
- Extended [VideoView.Details](docs.md#videoviewdetails) object


Support for new features is expected to be added in Play'a 2.3.12

## 1.5.0 Public Release
