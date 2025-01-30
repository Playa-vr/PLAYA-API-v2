## 1.6.0 External AlphaMask
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
