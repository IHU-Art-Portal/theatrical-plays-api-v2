# Documentation for Theatrical Plays Api

This document pertains to my thesis, which focuses on developing an API for managing theatrical play data.
Its primary objective is to support the implementation of a web-based, front end, an Android application, and an iOS application.
The API was built utilizing the .Net Framework and the C# programming language.

-------
## Api Response

Every server response is wrapped within the `TheatricalResponse` container object. 
This object comprises four possible fields: `data`, `success`, `message`, and the `errorCode`.

The `data` field contains the requested data.\
The `success` field indicates if the request was successful (boolean).\
The `message` field contains error or success messages of the request.\
The `errorCode` field is an enum that contains all possible errors.

Examples:\
<b>Successful Case:</b>
```json 
{
    "data": "some data",
    "success": true,
    "message": "Completed"
}
```
<b>Unsuccessful Case:</b>
```json
{
  "success": false,
  "message": "Performer does not exist",
  "errorCode": "NotFound" 
}
```
-------
## Collaborative projects
The solution is divided into four essential projects, for better management.
1) Theatrical.Api
2) Theatrical.Data
3) Theatrical.Dto
4) Theatrical.Services

Each project contains related job classes.
1) `Theatrical.Api`: The start-up class of the project lives here. It configures the services and the settings of the web API, also contains the controllers.
2) `Theatrical.Data`: The data project holds the models, the database schema and configuration, and migrations (if any).
3) `Theatrical.Dto`: The Dto project contains all the Data Transfer Objects, and the `TheatricalResponse` template container.
4) `Theatrical.Services`: The Service project includes all the services used, such as the repository which is used to interact with the database.

<I>A project may reference one or more projects in order to use its functions and data</I>.\
For example `Theatrical.Api` has references to all three other project, as it will be using the `Dtos`, `Data`, and `Services` a lot.

-------

## Api Requests
<b>Changes from version 1 (https://github.com/ar1st/theatrical-plays-api/blob/master/documentation.md) project:</b>
<ul><li><b>PeopleController</b>, is now called <b>PerformersController</b>.</li>
    <li>Added the ability to add new Performers.</li>
</ul>
<I>These changes are not yet implemented and are subject to change.</I><br>
<I>This text will be removed when everything is implemented and working.</I>

----
## Performers

---
This controller manages all the requests regarding a performer (former person).

**Create Performer**

| POST                 | /api/performers                            |
|----------------------|--------------------------------------------|
| **Parameters**       |                                            |
| *CreatePerformerDto* | {string: FullName, string: Image}          |
| **Responses**        |                                            |
| *PerformerDto*       | {int: Id, string: FullName, string: Image} |

----

**Get Performer**

This request is used to retrieve a performer and their image.
It returns a `PerformerDto` wrapped in `TheatricalResponse`.

| GET                               | /api/performers/{id}                                 |
|-----------------------------------|------------------------------------------------------|
| **Parameters**                    |                                                      |
| *id*                              | <u>Path variable</u>                                 |
|                                   | The integer identifier of the performer to retrieve. |
| **Responses**                     |                                                      |
| TheatricalResponse\<PerformerDto> | {id: Int, fullName: String, image: String}           |

---

**Get Performers**

This request is used to retrieve all performers.\
<I>If neither of page and size is specified, all results are returned.</I>

| GET                                              | /api/performers                            |
|--------------------------------------------------|--------------------------------------------|
| **Parameters**                                   |                                            |
| *page*                                           | <u>Query parameter</u>                     |
|                                                  | The index of the page to return. Optional  |
| *size*                                           | <u>Query parameter</u>                     |
|                                                  | The size of the page. Optional             |
| **Responses**                                    |                                            |
| TheatricalResponse<<List\<PerformerResponseDto>> | {id: Int, fullName: String, image: String} |

---

**Get Performers By Role**

This request returns performers filtered by the provided role.

| GET                                             | /api/performers/role                       |
|-------------------------------------------------|--------------------------------------------|
| **Parameters**                                  |                                            |
| *value*                                         | <u>Path parameter</u>                      |
|                                                 | The role provided to filter the results    |
| *page*                                          | <u>Query parameter</u>                     |
|                                                 | The index of the page to return. Optional  |
| *size*                                          | <u>Query parameter</u>                     |
|                                                 | The size of the page. Optional             |
| **Responses**                                   |                                            |
| TheatricalResponse\<List\<PerformerResponseDto> | {id: Int, fullName: String, image: String} |
