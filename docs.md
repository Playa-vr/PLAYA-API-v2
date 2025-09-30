# General
- API base url: ``<protocol>://<host>/api/playa/v2``
    - Add website by entering ``<host>`` or ``<protocol>://<host>`` url.
    - Everytnig after host is discarded from the url.
    - App will use https if no protocol is specified.
- Use HTTP status code 200 Ok in most cases.
    - Use [Rsp](#rsp) object in response body for [API status codes](#list-of-all-api-status-codes-not-http) and error messages.
    - Do not wrap other HTTP protocol errors in Rsp object.
    - App will retry failed requests for these HTTP methods and status codes:
        - GET, HEAD, OPTIONS
        - 429 TooManyRequests, 503 ServiceUnavailable, 504 GatewayTimeout
- Response body for most requests is [Rsp](#rsp) object or its generic variant [Rsp&lt;Type&gt;](#rsptype).
- Content-Type: ``application/json`` or ``application/json; charset=utf-8``

# Initial Flow
1. User adding website in PlayaVR app.
1. He enters website url (where API is hosted).
    - Example: ``api.playavr.com``.
1. App executes [GetVersion](#getversion) request.
    - No connection to the server: "Network error" message.
    - Incorrect response:
        - API status code 503: stop API detection with "Under maintenance" message.
    - Correct response:
        - Server major version is above app's max supported version: "Please update" message.
        - Server major version is below app's min supported version: "Site not supported" message.
        - Ok version: continue
1. App executes [GetConfiguration](#getconfiguration) request.
1. If app have stored user session (and if ``auth`` enabled in [Configuration](#configuration-1)):
    - App execute [GetProfile](#getprofile) request to check session.
        - If response has API status code 401 and [RefreshToken](#refreshtoken) request failed:
            - App silently drop user session and continue as unauthorized (guest).
1. App execute requests to retrieve static data:
    - [GetCategories](#getcategories) (if ``categories`` enabled in [Configuration](#configuration-1))
    - [GetCategoriesGroups](#getcategoriesgroups) (if ``categories_groups`` enabled in [Configuration](#configuration-1))
1. App execute requests to retrieve current page content (default page is Videos):
    - [GetVideos](#getvideos)
    - [GetActors](#getactors) (if ``actors`` enabled in [Configuration](#configuration-1))
    - [GetStudios](#getstudios) (if ``studios`` enabled in [Configuration](#configuration-1))
1. When the user's authorization status changes, the app will execute requests to retrieve next data:
    - [GetVideos](#getvideos) (if current page is Videos)
    - [GetVideoStatuses](#getvideostatuses) (if version greater or equal 1.10)

# Requests

## Configuration
Authomatic API discovery, compatibility and behaviour adjusments.

### GetVersion
- Route: GET /version
- Response: [Rsp](#rsptype)&lt;[SemVersion](#semversion--string)&gt;

Lates API version implemented on the server.

### GetConfiguration
- Route: GET /config
- Response: [Rsp](#rsptype)&lt;[Configuration](#configuration-1)&gt;

Information about server and which modules is supported.

## Authentication
Authentification for website users, exchanging credentials for JWT access and refresh tokens, refreshing them and signing out.

- Authorization: ``Bearer <access-token>``
- Response with API status code 401 for RefreshToken request:
    - Refresh token is expired. User have to Sign In again.
    - App will drop user session. Next requests will be unauthorized (guest).
- Response with API status code 401 for any other request:
    - Access token is expired.
    - App will execute RefreshToken request. If succeded it will retry original request.
- Response with API status code 403:
    - User is authenticated, but does not have required permissions (eg: free user requesting premium content).
    - App will show error message.

### SignInPassword
- Route: POST /auth/sign-in-password
- Request: [SignInPasswordRequest](#signinpasswordrequest)
- Response: [Rsp](#rsptype)&lt;[Token](#token)&gt;

Use API status code 3 (AUTHENTICATION_FAILED) for invalid credentials or other issues other than 4 (ACCOUNT_IS_EXPIRED) or 5 (USER_IS_BLOCKED).
Put a helpful message into ``Rsp.status.message`` field.

### GetCode
- Route: GET /auth/code
- Response: [Rsp](#rsptype)&lt;[AuthenticationCode](#authenticationcode)&gt;

This request will be used for sent when Login window will open if ``auth_by_code`` is enabled in the [Configuration](#configuration) object.
The received [AuthenticationCode](#authenticationcode).Code will be shown to the user in the Login window. This code will be offered to the user for enter on your site using the {SiteBaseUrl}/playalogin route.
[AuthenticationCode](#authenticationcode).Token will be sent via [SignInCode](#SignInCode) request every 3 seconds to verify authorization.

### SignInCode
- Route: POST /auth/sign-in-code
- Request: [SignInCodeRequest](#signincoderequest)
- Response: [Rsp](#rsptype)&lt;[Token](#token)&gt;

Use API status code 3 (AUTHENTICATION_FAILED) to get invalid credentials or other issues other than 4 (ACCOUNT_IS_EXPIRED) or 5 (USER_IS_BLOCKED).
Put a helpful message into ``Rsp.status.message`` field.

### RefreshToken
- Route: POST /auth/refresh
- Request: [RefreshTokenRequest](#refreshtokenrequest)
- Response: [Rsp](#rsptype)&lt;[Token](#token)&gt;

### SignOut
- Route: POST /auth/sign-out
- Request: [SignOutRequest](#signoutrequest)
- Response: [Rsp](#rsp)

## User
Requests related to current user.

### GetProfile
- Authentication: Required
- Route: GET /user/profile
- Response: [Rsp](#rsptype)&lt;[UserProfile](#userprofile)&gt;

### GetScriptsInfo
- Authentication: Required
- Route: GET /user/scripts-info
- Response: [Rsp](#rsptype)&lt;[ScriptsInfo](#scriptsinfo)&gt;

## Analytics
Requests to track user activity.

### Event
- Authentication: Required
- Route: PUT /event
- Request: [EventRequest](#eventrequest)
- Response: [Rsp](#rsp)

App reports its activity.

## Data
Requets for retriveing videos and other content.

#### Pagination
- page-index: index (starting from 0) of requested page.
    - Valid values ``n >= 0``.
- page-size: number of items page should have.
    - Valid values ``n > 0``.
    - Last page (and next) may have less items than requested.
        - Example: if threre is 5 pages total, then 6-th page will be empty.
- Pagination must respect filters and sorting
- Alternative pagination schemes may be implemented later:
    - LIMIT OFFSET
    - Continuation Token

#### Sorting
- order: sort items by this criteria.
    - Valid values: specified in request defenitions.
- direction: ascending or descending.
    - Valid values: ``"asc"`` (default), ``"desc"``.

#### Filtering
- Request may inlcude several filters at once, but no more than 1 of each filter type
- Response should include content, that pass all filters

### GetVideos
- Route: GET /videos
- Query:
    - page-index &lt;long&gt;
    - page-size &lt;long&gt;
    - [order &lt;string&gt;]
        - ``"title"``, ``"release_date"``, ``"popularity"``
    - [direction &lt;string&gt;]
    - [title &lt;string&gt;]
        - Videos with partial match with this title.
        - Server may implement general text based search using descriptions, release dates, and other fields.
    - [studio &lt;string&gt;]
        - Videos with this studio id.
    - [actor &lt;string&gt;]
        - Videos with this actor id
    - [included-categories &lt;string&gt;]
        - Comma separated list of categories ids.
        - Videos that have ALL categories in included-categories list.
    - [excluded-categories &lt;string&gt;]
        - Comma separated list of categories ids.
        - Videos that does NOT have ANY categories from excluded-categories list.
    - [included-statuses &lt;string&gt;]
        - Comma separated list of statuses ids.
        - Videos that have ANY status in included-statuses list.
    - [excluded-statuses &lt;string&gt;]
        - Comma separated list of statuses ids.
        - Videos that does NOT have ANY status from excluded-statuses list.
- Response: [Rsp](#rsptype)&lt;[Page](#paget)&lt;[VideoListView](#videolistview)&gt;&gt;

### GetVideoDetails
- Route: GET /video/&lt;id&gt;
- Response: [Rsp](#rsptype)&lt;[VideoView](#videoview)&gt;

### GetActors
- Route: GET /actors
- Query:
    - page-index &lt;long&gt;
    - page-size &lt;long&gt;
    - [order &lt;string&gt;]
        - ``"title"``, ``"popularity"``
    - [direction &lt;string&gt;]
    - [title &lt;string&gt;]
        - Actors with partial match with this title.
        - Server may implement general text based search using other fields.
- Response: [Rsp](#rsptype)&lt;[Page](#paget)&lt;[ActorListView](#actorlistview)&gt;&gt;

### GetActorDetails
- Route: GET /actor/&lt;id&gt;
- Response: [Rsp](#rsptype)&lt;[ActorView](#actorview)&gt;

### GetStudios
- Route: GET /studios
- Query:
    - page-index &lt;long&gt;
    - page-size &lt;long&gt;
    - [order &lt;string&gt;]
        - ``"title"``, ``"popularity"``
    - [direction &lt;string&gt;]
- Response: [Rsp](#rsptype)&lt;[Page](#paget)&lt;[StudioListView](#studiolistview)&gt;&gt;

### GetStudioDetails
- Route: GET /studio/&lt;id&gt;
- Response: [Rsp](#rsptype)&lt;[StudioView](#studioview)&gt;

### GetCategories
- Route: GET /categories;
- Response: [Rsp](#rsptype)&lt;[CategoryListView](#categorylistview)[]&gt;

### GetCategoriesGroups
- Route: GET /categories-groups;
- Response: [Rsp](#rsptype)&lt;[CategoriesGroup](#categoriesgroup)[]&gt;

### GetVideoStatuses
- Route: GET /video-statuses;
- Response: [Rsp](#rsptype)&lt;[VideoStatus](#videostatus)[]&gt;

# Schema
- Fields with null values can be omitted from JSON
- If field is defined in Schema but missing in JSON - default value used
- All numbers are deserialized to 64 bit numeric types (long, double)
- Note: this document produced by C# developer. Use C# for reference to data types, principles and conventions.
- Table below is a brief explanation ot type system used.

| Type   | Description | Default value | Nullable | Example in JSON |
| ----   | ----------- | ------------- | -------- | --------------- |
| bool   | Logical bit | false | No | false<br>true |
| long   | Signed integer<br>64 bit (8 bytes)| 0 | No | 42<br>-1 |
| double | Signed floating point number<br>64 bit (8 bytes) | 0.0 | No | 777<br>69.420<br>1.7e+380 |
| Type?  | Type modifier.<br>Makes any type nullable | null | Yes | long is not nullable. Default values is 0<br>long? is nullable. Default value is null|
| string | Text | null | Yes | ""<br>"abc"|
| Object | | null | Yes | \{ \} |
| Type[] | Type modifier.<br>Makes any type an array | null | Yes | [ ]<br> [1, 2, 3]<br>["aaa", "bbb", "ccc"]|
| Bar : Foo| Aliasing or Inheritance<br><br>If Foo is primitive type, then Bar is same type as Foo with specific meaning and/or constraints.<br><br>Otherwise Type Bar inherits all fields from type Foo.|null|yes|SCHEMA Timestamp : long // UNIX timestamp<br>JSON 1675161596<br><br>SCHEMA Url : string // must contain absolute http or https url<br>JSON "https://example.org"<br><br>SCHEMA ShortVideo \{ "id": &lt;long&gt;, "title": &lt;string&gt; \}<br>SCHEMA FullVideo : ShortVideo \{ "url": &lt;Url&gt; \}<br>JSON of FullVideo<br>\{<br>  "id": 42,<br>  "title": "example",<br>  "url": "https://example.org"<br>\}|
| Foo<Bar>| Generic Type (Template)<br><br>Type Foo with parameter Bar.<br>Foo is Object and contains fields of type Bar direcly or through other generics with parameter Bar<br><br> Generic objects can't contain fields of itself. | null| Yes|SCHEMA Foo \{ "err": &lt;long&gt; \}<br>JSON of Foo \{ "err": 2 \}<br><br>SCHEMA Foo&lt;Type&gt; \{ "err": &lt;long&gt;, "data": &lt;Type&gt; \}<br>JSON of Foo&lt;string&gt; { "err": 2, "data": "abc" \}<br>JSON of Foo&lt;long[]&gt; \{ "err": 2, "data": [1,2,3] \}|

### Timestamp : long
[Epoch Unix Timestamp](https://www.unixtimestamp.com/) SECONDS SINCE JAN 01 1970 (UTC)

>Wed Feb 01 2023 10:59:35 GMT+0000
>
>     1675249175

### Url : string
Absolute links to images or videos.

>Mp4 sample stream
>
>      "https://www.sample-videos.com/video321/mp4/720/big_buck_bunny_720p_1mb.mp4"

### JWT : string
Json Web Token

### SemVersion : string
Version string according to [Semantic Versioning 2.0.0](https://semver.org/)

>Initial release version
>
>     "1.0.0"

>    Patch release
>
>     "1.0.1"

>Minor release
>
>     "1.1.0"

>Major release
>
>     "2.0.0"

### Status
|Name|Type|Description|
|-|-|-|
|code|long|Response statuts.<br>Default successful responses have API status 2.<br>Some requests may define other successful status codes.|
|message|string|Optional debug text with error description.<br>Not suited for displaying to users or machine parsing.|

> Success
>
>     { "code": 1, "message": "ok" }

> Error
>
>     { "code": 2, "message": "Usupported sort order: views." }

#### List of all API status codes (NOT HTTP)
|Code|Name|Description|
|-|-|-|
|0|GENERAL_FAILURE|Rarely used when something goes wrong in an unexpected place. Suitable for global exception handlers.|
|1|OK||
|2|ERROR|Suitable for manually caught exceptions, failed checks or data errors when no specific status code is defined.<br>Message should contain some debug info about the error so client developer could find out what happened on his own (not always possible, in such case message will be forwarded to server developers for clarification).|
|3|AUTHENTICATION_FAILED|Used when SignIn requests failed because of invalid credentials.|
|4|ACCOUNT_IS_EXPIRED|Used when SignIn requests failed because of user account subscription plan is expired.|
|5|USER_IS_BLOCKED|Used when SignIn requests failed because of user account is blocked at the site.|
|401|UNAUTHORIZED|For [RefreshToken](#refreshtoken) this means the session is expired and SignIn needed.<br>For other requests this means the access token is expired and [RefreshToken](#refreshtoken) needed.|
|403|FORBIDDEN|Request is authenticated, but does not have required permissions.<br>Example: user with free account requesting premium content.|
|404|NOT FOUND|Suitable for point read requests when requested content (videos, actors, studios) not found.<br>Do **not** use when whole route not found and server could not process request. Use HTTP 404 instead.|
|503|UNDER_MAINTENANCE|Suitable for controllable maintenance (when server is working fine, but access is restricted for maintenance purposes). Some requests should be not affected under controllable maintenance (related to API detection).<br>Do **not** use when server could not process requests at all (outage, crash, downtime). Use HTTP 503 instead.|

### Rsp
Response

|Name|Type|Description|
|-|-|-|
|status|[Status](#status)|Always included]

>Success
>
>     { "status": { "code": 1 } }

>Error
>
>      "status" { "code": 3, "message": "Invalid credentials." } }

### Rsp&lt;Type&gt;
|Name|Type|Description|
|-|-|-|
|status|[Status](#status)|Always included|
|data|Type|Included only when succeeded. May be missing in case of errors.<br>App won't process data for error responses. But json parsers do.<br>Do **not** pass wrong data in error responses because it could break the parser.<br>Example of **invalid** response: expected response type is Rsp<long>, but actual response is Rsp<string> (use Rsp without data for errors).|

>Successful Rsp<long>.
>
>     { "status": { "code": 1 }, "data": 42 }

>Successful Rsp<string>.
>
>     { "status": { "code": 1 }, "data": "example" }

>Successful Rsp<string> with null value. Null values can be omitted from JSON
>
>     { "status": { "code": 1 }, "data": null }
>     // same as
>     { "status": { "code": 1 } }

>Successful Rsp<Token>
>
>     { "status": {"code": 1 }, "data": { "access_token": "acc", "refresh_token": "ref" } }

>Error response Rsp<long>.<br>Data can be omitted for error responses.<br>Data is ignored for error responses.
>
>     { "status" { "code": 0, "message": "unknown error" }, "data": 999 }
>     // same as
>     { "status" { "code": 0, "message": "unknown error" } }

>[!CAUTION]
>Invalid error response Rsp&lt;long&gt;.<br>Data is ignored for error responses, but parser is unable to convert array to a number.
>
>     {
>         "status" { "code": 0, "message": "unknown error" }, 
>         "data": []
>     }

### Token
|Name|Type|Description|
|-|-|-|
|access_token|string||
|refresh_token|string||

>Example
>
>     {
>         "access_token": "eyJhbGciOiJIUzI1NiJ9.e30.ZRrHA1JJJW8opsbCGfG_HACGpVUMN_a9IV7pAx_Zmeo",
>         "refresh_token": "eyJhbGciOiJIUzI1NiJ9.e30.4E_Bsx-pJi3kOW9wVXN8CgbATwP09D9V5gxh9-9zSZ0"
>     }

### SignInPasswordRequest
|Name|Type|Description|
|-|-|-|
|login|string|login or email depending on site|
|password|string||

>Example
>
>     {
>         "login": "user@mail.domain",
>         "password": "p@$$w0rd"
>     }

### AuthenticationCode
|Name|Type|Description|
|-|-|-|
|code|string|The authorization code that will be shown to the user in the Login window.<br>We recommend that you do not exceed its length by more than 12 characters.|
|token|string|Authorization token|
|expires_at|[Timestamp](#timestamp--long)|Token expiration date|

>Example
>
>     {
>         "code": "2FS6",
>         "token": "eyJhbGciOiJIUzI1NiJ9.e30.4E_Bsx-pJi3kOW9wVXN8CgbATwP09D9V5gxh9-9zSZ0",
>         "expires_at": "1675264660",
>     }

### SignInCodeRequest
|Name|Type|Description|
|-|-|-|
|token|string|Token for sign in. For more information see [SignInCode](#signincode)|

>Example
>
>     {
>         "token": "jafjajhfastf8aft861523"
>     }

### RefreshTokenRequest
|Name|Type|Description|
|-|-|-|
|refresh_token|string||

>Example
>
>     {
>         "refresh_token": "eyJhbGciOiJIUzI1NiJ9.e30.4E_Bsx-pJi3kOW9wVXN8CgbATwP09D9V5gxh9-9zSZ0"
>     }

### SignOutRequest
|Name|Type|Description|
|-|-|-|
|refresh_token|string||

>Example
>
>     {
>         "refresh_token": "eyJhbGciOiJIUzI1NiJ9.e30.4E_Bsx-pJi3kOW9wVXN8CgbATwP09D9V5gxh9-9zSZ0"
>     }

### EventRequest
|Name|Type|Description|
|-|-|-|
|event_type|string|Event id.|

#### Video Stream Start
Event when user opens video

|Name|Type|Description|
|-|-|-|
|event_type|string|Value: "videoStreamStart"|
|video_id|string|Video id string from [VideoView](#videoview).id|
|video_quality|string|Quality name string from [VideoLink](#videolink).quality_name|

>Example of Video Stream Start event
>
>     {
>          "event_type" : "videoStreamStart",
>          "video_id" : "891998",
>          "video_quality" : "4K"
>     }

#### Video Stream End
Event when user closes video. Only sent for durations above 30 seconds.

|Name|Type|Description|
|-|-|-|
|event_type|string|Value: "videoStreamEnd"|
|video_id|string|Video id string from [VideoView](#videoview).id|
|video_quality|string|Quality name string from [VideoLink](#videolink).quality_name|
|duration|double|Watch duration in seconds|

>Example of Video Stream End event
>
>     {
>          "event_type" : "videoStreamEnd",
>          "video_id" : "891998",
>          "video_quality" : "4K",
>          "duration" : 90.5
>     }

#### Video Downloaded
Event when user downloads video.

|Name|Type|Description|
|-|-|-|
|event_type|string|Value: "videoDownloaded"|
|video_id|string|Video id string from [VideoView](#videoview).id|
|video_quality|string|Quality name string from [VideoLink](#videolink).quality_name|

>Example of Video Stream End event
>
>     {
>          "event_type" : "videoDownloaded",
>          "video_id" : "891998",
>          "video_quality" : "4K"
>     }

### UserProfile
|Name|Type|Description|
|-|-|-|
|display_name|string|Display name. Could be email, login or nickname.|
|role|string||

>Example
>
>     {
>         "display_name": "Bill Gates",
>         "role": "premium"
>     }

### ScriptsInfo
|Name|Type|Description|
|-|-|-|
|token|[JWT](#jwt--string)|Required to use scripts.|

>Example
>
>     {
>         "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c"
>     }

### Configuration
|Name|Type|Description|
|-|-|-|
|site_name|string||
|site_logo|[Url](#url--string)|Recommended: 256x256 pixels with transparent background|
|auth|bool?|Is [authentication](#authentication) and [user](#user) API supported. Default: true|
|auth_by_code|bool?|Is [authentication](#authentication) and [SignInCode](#signincode) API supported. This field will be ignored if ``auth`` field is false<br>Default: false.|
|actors|bool|Is [actors](#getactors) supported|
|categories|bool|Is [categories](#getcategories) supported|
|categories_groups|bool|Is [categories groups](#getcategoriesgroups) supported|
|studios|bool|Is [studios](#getstudios) supported|
|scripts|bool|Is [scripts](#getscriptsinfo) supported|
|masks|bool|Is [transparency](#transparencyinfo) supported|
|analytics|bool|Is [analytics](#analytics) supported|
|theme|long?|Visual theme used for the site.|
|ar|bool?|If this parameter is set to true, then the user's ability to change the TransparencyMode will be disabled. Default: false|
|nsfw|bool?|Show a warning about NSFW content when users first access a website. Default: true|

>Example
>
>     {
>         "site_name": "Sample Site",
>         "site_logo": "https://sample-videos.com/img/Sample-png-image-100kb.png",
>         "actors": true,
>         "categories": true,
>         "studios": true,
>         "scripts": true,
>         "nsfw": false,
>         "ar": false
>     }

### Page&lt;T&gt;
|Name|Type|Description|
|-|-|-|
|page_index|long|Index of page (starting from 0)|
|page_size|long|Page size<br>Eg 1: requested 20 items, but there are only 10. page_size should be 20.|
|page_total|long|Total count of pages (if item_total is 0 - page_total is 1)|
|item_total|long|Total count of items|
|content|T[]|Array of items. Each item is of type T. Page with 0 items should have an empty array.|

>Example
>
>     {
>         "page_index": 2,
>         "page_size": 20,
>         "page_total": 5,
>         "item_total": 90,
>         "content": [ {item 40}, {item 41},   , {item 60} ]
>     }

### VideoListView
|Name|Type|Description|
|-|-|-|
|id|string||
|title|string|Displayed to user.|
|subtitle|string|Displayed under title. Suggestions: fill with studio or actors or leave empty.|
|status|string|Optional. The video status that will be displayed on the video card to the right of the title. If the field is not empty and has a value, then the status bar will be displayed. Otherwise it won't be displayed.|
|preview_image|[Url](#url--string)|Preview image.|
|has_scripts|bool|Shows whether the video has scripts. The value must be equal to the expression OR from each detail to the video.<br>Default value is false|
|release_date|[Timestamp](#timestamp--long)||
|details|[VideoListView.Details](#videolistviewdetails)[]|Trailer and Full video details. Both are optional.|

>Example
>
>     {
>         "id": "1",
>         "title": "Video Title",
>         "subtitle": "Video Subtitle",
>         "preview_image": "https://www.sample-videos.com/img/Sample-jpg-image-100kb.jpg",
>         "release_date": "1675264660",
>         "has_scripts": false,
>         "details": [ ... ]
>     }

### VideoListView.Details
|Name|Type|Description|
|-|-|-|
|type|string|"trailer"<br>"full"|
|duration_seconds|long|Duration in seconds|
|transparency_mode|long|The type of [TransparencyInfo](#transparencyinfo) in video|
|has_scripts|bool|Has script for video|

>Example
>
>     { "type": "trailer", "duration_seconds": 5, "has_scripts": false }

### VideoView
|Name|Type|Description|
|-|-|-|
|id|string||
|title|string|Displayed to user.|
|subtitle|string|Displayed under title. Suggestions: fill with studio or actors or leave empty.|
|status|string|Optional. The video status that will be displayed on the video card to the right of the title. If the field is not empty and has a value, then the status bar will be displayed. Otherwise it won't be displayed.|
|description|string||
|preview_image|[Url](#url--string)|Preview image.|
|release_date|[Timestamp](#timestamp--long)||
|studio|[VideoView.Studio](#videoviewstudio)[]||
|categories|[VideoView.Category](#videoviewcategory)[]||
|actors|[VideoView.Actor](#videoviewactor)[]||
|views|long||
|transparency|[TransparencyInfo](#transparencyinfo)||
|details|[VideoView.Details](#videoviewdetails)[]|Trailer and Full video details. Both are optional.|

>Example
>
>     {
>         "id": "1",
>         "title": "Video Title",
>         "subtitle": "Video Subtitle",
>         "description": "Video Description"
>         "preview_image": "https://www.sample-videos.com/img/Sample-jpg-image-100kb.jpg",
>         "release_date": "1675264660",
>         "studio": { ... },
>         "actors": [ ... ],
>         "categories": [ ... ],
>         "details": [ ... ]
>     }

### VideoView.Studio
|Name|Type|Description|
|-|-|-|
|id|string||
|title|string|Displayed to user.|

>Example
>
>     { "id": "studio_id", "title": "Studio Title" }

### VideoView.Category
|Name|Type|Description|
|-|-|-|
|id|string||
|title|string|Displayed to user.|

>Example
>
>     { "id": "category_id", "title": "Category Title" }

### VideoView.Actor
|Name|Type|Description|
|-|-|-|
|id|string||
|title|string|Displayed to user.|

>Example
>
>     { "id": "actor_id", "title": "Actor Title" }

### VideoView.Details
|Name|Type|Description|
|-|-|-|
|type|string|"trailer"<br>"full"|
|duration_seconds|long|Duration in seconds|
|timeline_atlas|[TimelineAtlas](#timelineatlas)|Optional|
|timeline_markers|[Timeline](#timeline-timelinemarker)|Optional|
|links|[VideoLink](#videolink)[]|Include all link objects even if they are unavailable to the user because he is not authorized or not premium. For such cases set [VideoLink](#videolink).url to null.|
|alpha_mask|[Url](#url--string)|Optional<br>Url to external alpha mask video. External mask should be enabled in [TransparencyInfo](#transparencyinfo).|
|script_info|[VideoScriptInfo](#videoscriptinfo)|Optional.|

>Example
>
>     {
>        "type": "trailer", 
>        "duration_seconds": 5,
>        "links": [ ... ]
>     }

### TimelineAtlas
|Name|Type|Description|
|-|-|-|
|version|long|Supported versions:<br>**1**: Single image with grid of frames.<br>Every frame has the same size and time interval.<br>Row and column count may vary.<br>Max allowed image size is 4096x4096.<br>Example of 8 frames packed in 4 by 2 grid.<br>0 1 2 3<br>4 5 6 7|
|url|[Url](#url--string)|Url to atlas image|
|frame_width|long?|[Optional] Pixel width of each frame.<br>If missing - calculated from columns and image width.|
|frame_height|long?|[Optional] Pixel height of each frame.<br>If missing - calculated from rows and image height.|
|columns|long?|[Optional] Columns count of grid.<br>If missing - calculated from frame_width and image width.|
|rows|long?|[Optional] Rows count of grid.<br>If missing - calculated from frame_height and image height.|
|frames|long?|[Optional] Frame count. Used then last frames in the grid are unused.<br>If missing - every frame in the grid is used.|
|interval_ms|long?|[Optional] Interval in milliseconds between frames.<br>If missing - every frame is equally distributed along video duration.|
|aspect_ratio|long?|[Optional] Aspect ratio used to display each frame (width / height).<br>If missing - calculated from frame_width and frame_height.|

>Example
>
>     {
>         "version": 1,
>         "url": "https://raw.githubusercontent.com/transitive-bullshit/ffmpeg-generate-video-preview/master/media/big-buck-bunny-6x5.jpg",
>         "frame_width": 160,
>         "frame_height": 90,
>         "columns": 6,
>         "rows": 5
>     }

Provide at least one pair (provide both if possible):
- "frame_width" and "frame_height"
- "columns" and "rows"

Use "frames" when last frames in the grid are unused and do not belong to video duration.

When frames distributed by fixed time intervals use "interval_ms". If frames distributed equally along video duration (for example by fixed percentage) leave "interval_ms" as null.

Use "aspect_ratio" when atlas contains distorted frames.

### Timeline : [TimelineMarker](#timelinemarker)[]
Timeline used for displaying chapters (segments) on seekbar and for automatic camera adjustments.

Each marker must have time. Other fields are optional.
App merges markers with equal time into one marker, then sorts them by time.

If some field is missing (or null) - app use value from previous marker or default value if none of previous markers have non null value.
Exception - title field. Each non null title creates a point in seekbar.

If there is no marker at time 0 - app creates it automatically with empty title and null values for other fields. 

> Example
>
>     [
>       { "time": 0, "title": "Start" },
>       { "time": 5000, "zoom": 1 },
>       { "time": 10000, "zoom": 0, "height": -1 },
>       { "time": 15000, "tilt": 45 },
>       { "time": 20000, "height": 1 },
>       { "time": 25000, "tilt": 0 },
>       { "time": 30000, "zoom": -1, "tilt": -30, "title": "Middle" },
>       { "time": 35000, "title": "End" },
>       { "time": 40000, "zoom": 0, "height": 0, "tilt": 0 }
>     ]


### TimelineMarker
|Name|Type|Description|
|-|-|-|
|time|long|Time in milliseconds|
|title|string|Chapter (segment). Empty string is valid title (chapter without name). Each marker with a non null title displayed on timeline (seekbar). You can seek chapters using next/previous chapter hotkeys.|
|tilt|double?|Camera rotation (up, down) in degrees from -90 to 90.<br>Positive values rotate camera up.|
|zoom|double?|Camera zoom is normalized in range from -1 to 1.<br>Positive values bring video closer to camera.|
|height|double?|Camera height in normalized range from -1 to 1.<br>Positive values move camera up.|

>Chapter marker
>
>     { "time": 5000, "title": "Chapter 2" }

>Tilt and Zoom marker
>
>     { "time": 15000, "tilt": -30.5, "zoom": 0.25 }

### VideoLink
When possible provide the same link for stream and download.

|Name|Type|Description|
|-|-|-|
|is_stream|bool|Note: Hosting (CDN) must support HTTP range requests (partial requests).|
|is_download|bool|Note: Hosting (CDN) must respond with a nonzero Content-Length header.|
|url|[Url](#url--string)|If user is not allowed to watch or download - use null.|
|unavailable_reason|string|Displayed to users. English only. Truncated to 15 symbols.<br>If url is present - use null.<br>If url is missing - use reason.<br>Example values:<br>"login": user need to be logged in (for unauthorized users)<br>"premium": user does not have premium subscribtion (for unauthorized or free users)|
|projection|string|"FLT" - shown on flat display<br>"180" - front half sphere<br>"360" - full sphere<br>"FSH" - fisheye|
|stereo|string|"MN" - mono.<br>"LR" - stereo. Side by side. Left eye on left half.<br>"RL" - stereo. Side by side. Left eye on right half.<br>"TB" - stereo. Over under. Left eye on top half.<br>"BT" - stereo. Over under. Left eye on bottom half.|
|quality_name|string|Displayed to users. English only. Truncated to 15 symbols.<br>Example values:<br>"2K"<br>"1080p"<br>"FullHD"|
|quality_order|long|Greater quality has bigger order value.<br><br>Users can select preferred quality (in app settings).<br>Some platforms have quality limitations.<br><br>Map your qualities in [range](#quality-range).<br>Selection order example:<br>User selected 4K as preferred quality.<br>App will select first available link in such order:<br>45, 44, 43, ..., 2, 1, 0, 46, 47, 48, ..., 98, 99, 100.<br>(from middle down to 0 and then from middle+1 up to 100)<br><br>Quality limitation example:<br>PlayaVR for Android and iOS smartphones does does not support videos above 4K.<br>Every link with value greater than 49 will be unavailable.|

>Available link
>
>     { 
>         "is_stream": true,
>         "url": "https://www.sample-videos.com/video321/mp4/720/big_buck_bunny_720p_1mb.mp4",
>         "projection": "FLT",
>         "stereo": "MN",
>         "quality_name": "720p",
>         "quality_order": 15
>     }

> Unavailable link
>
>     { 
>         "is_stream": true,
>         "is_download": true,
>         "UnavailableReason": "Premium",
>         "projection": "FLT",
>         "stereo": "MN",
>         "quality_name": "1080p",
>         "quality_order": 25
>     }

#### Quality Range
|Quality|Middle|Range|
|-|-|-|
|2K-|25|0..29|
|3K|35|30..39|
|4K|45|40..49|
|5K|55|50..59|
|6K|65|60..69|
|7K|75|70..79|
|8K+|85|80..100|

### TransparencyInfo
It is recommended to use exact data from preset files generatedh by Playa application.

|Name|Type|Description|
|-|-|-|
|m|long|0 - None<br>1 - Embedded Alpha Mask<br>2 - Chroma Key<br>3 - External Alpha Mask|
|a|[AlphaChannel](#alphachannel)[]|Array of up to 3 alpha channels<br>Order of channels: 1) Red 2) Green 3) Blue<br><b>Red channel is used when no channel is set as available.</b>|
|i|bool|Is chroma key inverted:<br>false - Hide pixels<br>true - Show pixels|
|c|[ChormaKeyLayer](#chromakeylayer)[]|Array of up to 4 chroma key layers|

>Embeded AlphaMask example
>
>     { "m": 1 }

>External AlphaMask with 2 channels example
>
>     {"m":3,"a":[{"a":true,"e":true,"n":"Bunny"},{"a":true,"e":false,"n":"Grass"}]}

>ChromaKey example
>
>     {"m":2,"i":false,"c":[{"e":true,"r":0.2,"f":0.5,"c":{"r":18,"g":218,"b":0},"h":0.1,"s":-0.25,"v":-0.8}]}

### AlphaChannel
|Name|Type|Description|
|-|-|-|
|a|bool|Is channel available (can be enabled)|
|e|bool|Is channel enabled by default|
|n|string|Channel name (what is visible when this channel enabled)|

### ChromaKeyLayer
|Name|Type|Description|
|-|-|-|
|e|bool|Is layer enabled|
|r|double|Range [0..1]|
|f|double|Smooth [0..1]|
|c|[Color24](#color24)|
|h|double|Hue importance [-1..1]|
|s|double|Saturation importance [-1..1]|
|v|double|Brightness importance [-1..1]|

>Example
>
>     {"e": true,"r":0.2,"f":0.5,"c":{"r": 18,"g":218,"b":0},"h":0.1,"s":-0.25,"v":-0.8}

### Color24
|Name|Type|Description|
|-|-|-|
|r|long|Red [0..255]|
|g|long|Green [0..255]|
|b|long|Blue [0..255]|

>Example
>
>     { "r": 18, "g": 218, "b": 0 }


### VideoScriptInfo
|Name|Type|Description|
|-|-|-|
|id|string|Unique script id.|
|generation_source|int|Script generation source, where:<br>0 - Manual,<br>1 - AI.|

>Example
>
>     { "id": "1239572", "generation_source": 1 }

### ActorListView
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|preview|[Url](#url--string)|[Optional] Preview image.|

>Example
>
>     {"id": "actor_id", "title": "Actor Title", "preview": "https://www.sample-videos.com/img/Sample-jpg-image-50kb.jpg" }

### ActorView
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|preview|[Url](#url--string)|[Optional] Preview image.|
|studios|[ActorView.Studio](#actorviewstudio)[]|[Optional]|
|properties|[ActorView.Property](#actorviewproperty)[]|[Optional]|
|aliases|string[]|[Optional]
|views|long||
|banner|[Url](#url--string)|[Optional] Big horizontal image.|

>Example
>
>     {
>         "id": "actor_id",
>         "title": "Actor Id",
>         "preview": "https://www.sample-videos.com/img/Sample-jpg-image-50kb.jpg"
>         "studios": [ ... ],
>         "properties": [ ... ],
>         "aliases": [ "Another Actor Name" ],
>         "views": 500
>     }


### ActorView.Studio
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|

>Example
>
>     { "id": "studio_id", "title": "Studio Id" }

### ActorView.Property
|Name|Type|Description|
|-|-|-|
|id|string|
|value|string||

>Example
>
>     { "name": "Birthdate", "value": "16 Jan" }

### StudioListView
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|preview|[Url](#url--string)|[Optional] Preview image.|

>Example
>
>     { "id": "studio_id", "title": "Studio Title" }

### StudioView
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|preview|[Url](#url--string)|[Optional] Preview image.|
|description|string||
|views|long||

>Example
>
>     {
>         "id": "studio_id",
>         "title": "Studio Id",
>         "description": "All about ^_^ since 1337",
>         "views": 500
>     }

### CategoryListView
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|preview|[Url](#url--string)|[Optional] Preview image.|

>Example
>
>     { "id": "category_id", "title": "Category Id" }

### CategoriesGroup
|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|
|items|[CategoryListView](#categorylistview)[]||

>Example
>
>     { "id": "top", "title": "Top", "items": [ ... ] }

### VideoStatus
It is used to filter the list of videos. The PLAY'A app will display a list of the received from the [GetVideoStatuses](#getvideostatuses) request for filtering if it contains MORE THAN ONE status.
Most often, these statuses are displayed in the video and filtered only for authorized users with the roles "admin" and "moderator".

|Name|Type|Description|
|-|-|-|
|id|string|
|title|string|Displayed to user.|

>Example
>
>     { "id": "published-", "title": "Published" }
