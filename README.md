# Documentation for Theatrical Plays Api

This document pertains to my thesis, which focuses on developing an API for managing theatrical play data.
Its primary objective is to support the implementation of a web-based, front end, an Android application, and an iOS application.
The API was built utilizing the .Net Framework and the C# programming language.

-------
## Api Response

Every server response is wrapped within the `ApiResponse` container object. 
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
  "message": "Person does not exist",
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
3) `Theatrical.Dto`: The Dto project contains all the Data Transfer Objects, and the `ApiResponse` template container.
4) `Theatrical.Services`: The Service project includes all the services used, such as the repository which is used to interact with the database.

<I>A project may reference one or more projects in order to use its functions and data</I>.\
For example `Theatrical.Api` has references to all three other project, as it will be using the `Dtos`, `Data`, and `Services` a lot.

-------

## Api Requests
<b>Changes from version 1 (https://github.com/ar1st/theatrical-plays-api/blob/master/documentation.md) project:</b>
<ul><li><b>/api/people/letter</b> endpoint has changed to <b>/api/people/initials/letters</b></li>
    <li><b>/api/people/initials/letters</b> functionality changed to return results based on the provided initials,
instead of one provided letter.</li>
</ul>
<I>New changes will be listed here.</I><br>
<I>This text will be removed when everything is implemented and working.</I>

## User

---
This controller manages all the requests regarding user registration, login, verify, balance.

---

This endpoint is used by the users to register.
It returns a `UserDtoRole` wrapped in `ApiResponse`.

**Register**

| POST                             | /api/user/register                                    |
|----------------------------------|-------------------------------------------------------|
| **Parameters**                   |                                                       |
| *RegisterUserDto*                | {string: Email, string: Password, int Role}           |
| **Responses**                    |                                                       |
| *ApiResponse&lt;UserDtoRole&gt;* | {int: Id, string: Email, bool: Enabled, string: Note} |

----

This endpoint is used to verify their email.
It returns a message wrapped in `ApiResponse`.

**Verify**

| GET            | /api/user/verify                                       |
|----------------|--------------------------------------------------------|
| **Parameters** |                                                        |
| *string*       | <u>Query parameter</u>                                 |
|                | *token* from user's email.                             |
| **Responses**  |                                                        |
| *ApiResponse*  | {Message field overrides with the appropriate message} |

---

**Login**

This endpoint is used to login.
It returns a `JwtDto` wrapped in `ApiResponse`.

| POST                  | /api/user/login                                             |
|-----------------------|-------------------------------------------------------------|
| **Parameters**        |                                                             |
| *LoginUserDto*        | <u>JSON object</u>                                          |
|                       | {string: Email, string Password}                            |
| **Responses**         |                                                             |
| *ApiResponse<JwtDto>* | {string: access_token, string: token_type, int: expires_in} |

---

**Balance**

This endpoint is used to find someone's balance (credits).
It returns a message wrapped in `ApiResponse`. The message overrides the data field.

| GET                   | /api/user/{id}/balance                    |
|-----------------------|-------------------------------------------|
| **Parameters**        |                                           |
| int                   | Id of the user                            |
| **Responses**         |                                           |
| *ApiResponse<string>* | {data field is overridden with a message} |


----

## Person

---
This controller manages all the requests regarding a person.

**Create Person**

| POST              | /api/person                                |
|-------------------|--------------------------------------------|
| **Parameters**    |                                            |
| *CreatePersonDto* | {string: FullName, string: Image}          |
| **Responses**     |                                            |
| *PersonDto*       | {int: Id, string: FullName, string: Image} |

----

**Get Person**

This request is used to retrieve a performer <s>and their image</s>.
It returns a `PersonDto` wrapped in `ApiResponse`.

| GET                     | /api/person/{id}                                     |
|-------------------------|------------------------------------------------------|
| **Parameters**          |                                                      |
| *id*                    | <u>Path variable</u>                                 |
|                         | The integer identifier of the performer to retrieve. |
| **Responses**           |                                                      |
| ApiResponse\<PersonDto> | {id: Int, fullName: String, image: String}           |

---

**Get Persons**

This request is used to retrieve all people.\
<I>If neither of page and size is specified, all results are returned.</I>

| GET                                    | /api/people                                |
|----------------------------------------|--------------------------------------------|
| **Parameters**                         |                                            |
| *page*                                 | <u>Query parameter</u>                     |
|                                        | The index of the page to return. Optional  |
| *size*                                 | <u>Query parameter</u>                     |
|                                        | The size of the page. Optional             |
| **Responses**                          |                                            |
| ApiResponse<<List\<PersonResponseDto>> | {id: Int, fullName: String, image: String} |

---

**Get Persons By Role**

This request returns performers filtered by the provided role.

| GET                                   | /api/people/role                           |
|---------------------------------------|--------------------------------------------|
| **Parameters**                        |                                            |
| *value*                               | <u>Path parameter</u>                      |
|                                       | The role provided to filter the results    |
| *page*                                | <u>Query parameter</u>                     |
|                                       | The index of the page to return. Optional  |
| *size*                                | <u>Query parameter</u>                     |
|                                       | The size of the page. Optional             |
| **Responses**                         |                                            |
| ApiResponse\<List\<PersonResponseDto> | {id: Int, fullName: String, image: String} |
