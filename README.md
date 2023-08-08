# Documentation for Theatrical Plays Api

📜 This document pertains to my thesis, which focuses on developing an API for managing theatrical play data. 🎭📊
Its primary objective is to support the implementation of a web-based, front end, an Android application, and an iOS application. 🌐📱
The API was built utilizing the .Net Framework and the C# programming language. ⚙️🔧


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

# 🙋‍♂️ User

---
The `UserController` is responsible for managing requests like registration, login, verify, and balance.

## 📚 Methods

| Method   | Endpoint                 |
|----------|--------------------------|
| **POST** | `/api/user`              |
| **GET**  | `/api/user/verify`       |
| **POST** | `/api/user/login`        |
| **GET**  | `/api/user/{id}/balance` |

### 🌟 Register

| Method   | Endpoint    |
|----------|-------------|
| **POST** | `/api/user` |

Use this method to register for the service.

| Parameter         | Type         | Description                         |
|-------------------|--------------|-------------------------------------|
| `RegisterUserDto` | Request body | Data for creating a user.           |

**Responses:**

- If successful, returns an `ApiResponse` with a success message and related data.
- If validation fails, returns an `ApiResponse` with an appropriate error message and related data.

----


### 🔒 Verify

| Method  | Endpoint           |
|---------|--------------------|
| **GET** | `/api/user/verify` |

Use this method to verify an account. ✅

| Parameter  | Type            | Description                      |
|------------|-----------------|----------------------------------|
| `token`    | Query Parameter | Verification code sent by email. |

**Responses:**

- If successful, returns an `ApiResponse` with a success message and related data. 👍
- If validation fails, returns an `ApiResponse` with an appropriate error message and related data. ❌

---

### 🚀 Login

| Method   | Endpoint          |
|----------|-------------------|
| **POST** | `/api/user/login` |

This endpoint is used to login. 📝
It returns a `JwtDto` wrapped in `ApiResponse`.

| Parameter      | Type         | Description                       |
|----------------|--------------|-----------------------------------|
| `LoginUserDto` | Request body | {string: Email, string: Password} |

**Responses:**

- If successful, returns a `JwtDto` wrapped in `ApiResponse`. ✅
- If validation fails, returns an `ApiResponse` with an appropriate error message and related data. ❌


---

### ⚖️ Balance

| Method  | Endpoint                 |
|---------|--------------------------|
| **GET** | `/api/user/{id}/balance` |

This endpoint is used to find someone's balance (credits). 💰
It returns a message wrapped in `ApiResponse`. The message overrides the data field.

| Parameter | Type           | Description     |
|-----------|----------------|-----------------|
| `Id`      | Path parameter | ID of the user  |

**Responses:**

- If successful, returns an `ApiResponse` with a success message and related data. 👍
- If validation fails, returns an `ApiResponse` with an appropriate error message and related data. ❌

----

# 👤 Person

The `PeopleController` manages all the requests regarding a person. 👤

## 📚 Methods

| Method     | Endpoint                         |
|------------|----------------------------------|
| **GET**    | `/api/people`                    |
| **GET**    | `/api/people/{id}`               |
| **DELETE** | `/api/people/{id}`               |
| **GET**    | `/api/people/{id}/photos`        |
| **GET**    | `/api/people/{id}/productions`   |
| **POST**   | `/api/people`                    |
| **GET**    | `/api/people/initials/{letters}` |
| **GET**    | `/api/people/role/{role}`        |

---

### 🔍 Get People

| Method   | Endpoint      |
|----------|---------------|
| **GET**  | `/api/people` |

Retrieves a list of all people. 👥
Pagination is available for this request. 📄

| Parameter | Type            | Description                             |
|-----------|-----------------|-----------------------------------------|
| `page`    | Query parameter | (Optional) Page number for pagination.  |
| `size`    | Query parameter | (Optional) Number of items per page.    |

**Response:**

- If successful, returns an `ApiResponse`📦 containing a list of `PersonDto`🧍 objects wrapped in a `PaginationResult`📜.
- If validation fails, returns an `ApiResponse` with an appropriate error message and data.

---

### 👤 Get Person

| Method   | Endpoint          |
|----------|-------------------|
| **GET**  | `/api/people{id}` |

This request is used to retrieve a person. 👤

| Parameter | Type            | Description     |
|-----------|-----------------|-----------------|
| `id`      | Path parameter  | ID of a person. |

**Response:**

- If successful, returns a `PersonDto`🧍 wrapped in `ApiResponse`📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message and data.


---

### 🗑️ Delete Person

| Method     | Endpoint          |
|------------|-------------------|
| **DELETE** | `/api/people{id}` |

🔥 This request deletes a Person by their ID. 👤</br>
⚠️ Use with caution ⚠️

| Parameter  |  Type          | Description                 |
|------------|----------------|-----------------------------|
| `id`       | Path parameter | ID of the person to delete. |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message. 🎉
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


----


### 📸 Get Person Photos

| Method    | Endpoint                  |
|-----------|---------------------------|
| **GET**   | `/api/people/{id}/photos` |

This request returns a `Person`'s `Images`. 📸 <br>
Pagination is available for this request. 📄 <br>

| Parameter | Type             | Description                            |
|-----------|------------------|----------------------------------------|
| `id`      | Path parameter   | ID of the person                       |
| `page`    | Query parameter  | (Optional) Page number for pagination. |
| `size`    | Query parameter  | (Optional) Number of items per page.   |

**Response:**

- If successful, returns a `PersonDto` 🧍 wrapped in `PaginationResult` 📜 which is wrapped in `ApiResponse` 📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


---


### 🎬 Get Person Productions

| Method    | Endpoint                       |
|-----------|--------------------------------|
| **GET**   | `/api/people/{id}/productions` |

This request returns all the `Productions` that one `Person` partakes in. 🎬 <br>
Pagination is available for this request. 📄 <br>

| Parameter | Type             | Description                            |
|-----------|------------------|----------------------------------------|
| `id`      | Path parameter   | ID of the person                       |
| `page`    | Query parameter  | (Optional) Page number for pagination. |
| `size`    | Query parameter  | (Optional) Number of items per page.   |

**Response:**

- If successful, returns a `PersonProductionsRoleInfo` 🧍 for every item, wrapped in `PaginationResult` 📜 which is wrapped in `ApiResponse` 📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


---

### 🎉 Create Person

| Method   | Endpoint        |
|----------|-----------------|
| **POST** | `/api/people/`  |

🌟 This request creates a new person.

| Parameter         | Type         | Description                                                                |
|-------------------|--------------|----------------------------------------------------------------------------|
| `CreatePersonDto` | Request body | {string: FullName, List&lt;string&gt;: url of image (if any), int: System} |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns a `PersonDto` 🧍 for the newly created entry, wrapped in `ApiResponse` 📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


----


### 📝 Get People By Initial Letter

| Method  | Endpoint                         |
|---------|----------------------------------|
| **GET** | `/api/people/initials/{letters}` |

🔎 This request returns people filtered by the provided initial letters. 👥
Pagination available for this request. 📄

| Parameter | Type             | Description                            |
|-----------|------------------|----------------------------------------|
| `letters` | Path parameter   | Initials of a person's name.           |
| `page`    | Query parameter  | (Optional) Page number for pagination. |
| `size`    | Query parameter  | (Optional) Number of items per page.   |

**Response:**

- If successful, returns a `PersonDto` 🧍 wrapped in `PaginationResult` 📜 which is wrapped in `ApiResponse` 📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


---

### 🔎 Get People By Role

| Method  | Endpoint                  |
|---------|---------------------------|
| **GET** | `/api/people/role/{role}` |

🔍 This request returns people filtered by the provided role. 👥
Pagination available for this request. 📄

| Parameter | Type               | Description                            |
|-----------|--------------------|----------------------------------------|
| `role`    | Path parameter     | Search people by role.                 |
| `page`    | Query parameter    | (Optional) Page number for pagination. |
| `size`    | Query parameter    | (Optional) Number of items per page.   |


**Response:**

- If successful, returns a `PersonDto` 🧍 for each item (if pagination was initiated), wrapped in `PaginationResult` 📜 which is wrapped in `ApiResponse` 📦.
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌


---

# 📜 Logs

The `LogsController` is responsible for managing requests related to logs.

## 📚 Methods

| Method  | Endpoint                               |
|---------|----------------------------------------|
| **GET** | `/api/logs`                            |

### 🔍 Get Logs

Retrieves a list of all logs with optional pagination.

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

An `ApiResponse` containing a paginated list of `LogDto` objects wrapped in a `PaginationResult`.


----

# 🎉 Event

The `EventsController` is responsible for managing requests related to events.

## 📚 Methods

| Method   | Endpoint           |
|----------|--------------------|
| **GET**  | `/api/events`      |
| **POST** | `/api/events`      |

---

### 📅 Get Events

| Method   | Endpoint           |
|----------|--------------------|
| **GET**  | `/api/events`      |

Retrieve a list of all events with optional pagination.

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `EventDto` objects wrapped in a `PaginationResult`.
- If validation fails, returns an `ApiResponse` with an appropriate error message.

### 🎉 Create Event

| Method   | Endpoint           |
|----------|--------------------|
| **POST** | `/api/events`      |

Create a new event.

| Parameter        | Type           | Description                  |
|------------------|----------------|------------------------------|
| `createEventDto` | Request body   | Data for creating the event. |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message.
- If validation fails, returns an `ApiResponse` with an appropriate error message.

---

# 🏛️ Venues

The `VenuesController` is responsible for managing requests related to venues.

## Methods

| Method    | Endpoint            |
|-----------|---------------------|
| **GET**   | `/api/venues`       |
| **GET**   | `/api/venues/{id}`  |
| **POST**  | `/api/venues/`      |

### 📜 Get Venues

| Method   | Endpoint             |
|----------|----------------------|
| **GET**  | `/api/venues`        |

Retrieve a list of all venues with optional pagination.

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `VenueDto` objects wrapped in a `PaginationResult`.
- If validation fails, returns an `ApiResponse` with an appropriate error message.


### 🏛️ Get Venue

| Method   | Endpoint             |
|----------|----------------------|
| **GET**  | `/api/venues/{id}`   |

Retrieve a specific venue by its ID.

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `id`      | Path parameter  | ID of the venue to retrieve.       |

**Response:**

- If successful, returns an `ApiResponse` containing a `VenueDto` object.
- If validation fails or the venue is not found, returns an `ApiResponse` with an appropriate error message.

### 🏟️ Create Venue

| Method   | Endpoint             |
|----------|----------------------|
| **POST** | `/api/venues`        |

Create a new venue.

| Parameter        | Type           | Description                    |
|------------------|----------------|--------------------------------|
| `venueCreateDto` | Request body   | Data for creating the venue.   |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message.
- If an error occurs, returns an `ApiResponse` with an appropriate error message.

---

# 💰 Transactions

🕊️ The `TransactionsController` is responsible for managing requests related to transactions. 📊💸

## Methods

| Method   | Endpoint                      |
|----------|-------------------------------|
| **POST** | `/api/transactions`           |
| **GET**  | `/api/transactions/{id}`      |
| **GET**  | `/api/transactions/user/{id}` |

### 📤 Post Transaction

| Method   | Endpoint                  |
|----------|---------------------------|
| **POST** | `/api/transactions`       |

🌟 Make a new transaction. 💰📄

| Parameter        | Type           | Description                    |
|------------------|----------------|--------------------------------|
| `transactionDto` | Request body   | Data for the transaction.      |

**Response:**

- If successful, returns an `ApiResponse` containing a `Transaction` object wrapped in a success message. ✅📦
- If an error occurs, returns an `ApiResponse` with an appropriate error message. ❌🚫

---

### 📥 Get Transaction

| Method   | Endpoint                  |
|----------|---------------------------|
| **GET**  | `/api/transactions/{id}`  |

🔍 Get details of a specific transaction. 💼💳

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `id`      | Path parameter  | ID of the transaction to retrieve. |

**Response:**

- If successful, returns an `ApiResponse` containing a `TransactionDtoFetch` object. ✅📋
- If the transaction is not found, returns an `ApiResponse` with an appropriate error message. ❌🔍

---

### 🛒 Get User Transactions

| Method   | Endpoint                         |
|----------|----------------------------------|
| **GET**  | `/api/transactions/user/{id}`    |

📚 Get all transactions of a specific user. 👤📊

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `id`      | Path parameter  | ID of the user.                    |

**Response:**

- If successful, returns an `ApiResponse` containing a list of `TransactionDtoFetch` objects. ✅📋📊
- If the user's transactions are not found, returns an `ApiResponse` with an appropriate error message. ❌👤📊

---
# 👑 Roles

🎭 The `RolesController` is responsible for managing requests related to roles. 👥🔧


## Methods

| Method   | Endpoint               |
|----------|------------------------|
| **POST** | `/api/roles/{role}`    |
| **GET**  | `/api/roles`           |


### ➕ Create Role

| Method   | Endpoint                  |
|----------|---------------------------|
| **POST** | `/api/roles/{role}`       |

🌟 Create a new role. 🆕🎭

| Parameter | Type            | Description                           |
|-----------|-----------------|---------------------------------------|
| `role`    | Path parameter  | The name of the role to be created.   |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message. ✅🎉
- If the role already exists, returns an `ApiResponse` with an appropriate error message. ❌🎭🔑


---

### 🎭 Get Roles

| Method   | Endpoint             |
|----------|----------------------|
| **GET**  | `/api/roles`         |

📜 Retrieve a list of all roles with optional pagination (oldest to newest). 📃🔄

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `RoleDto` objects wrapped in a `PaginationResult`. ✅📜📄
- If no roles are found, returns an `ApiResponse` with an appropriate error message. ❌🔍🎭


---

# 🎬 Productions

The `ProductionsController` is responsible for managing requests related to productions. 🎥

## 📚 Methods

| Method   | Endpoint               |
|----------|------------------------|
| **POST** | `/api/productions`     |
| **GET**  | `/api/productions`     |

### 🌟 Create Production

| Method   | Endpoint                  |
|----------|---------------------------|
| **POST** | `/api/productions`        |

🌟 Creates a new production. 🆕🎬


| Parameter              | Type             | Description                         |
|------------------------|------------------|-------------------------------------|
| `createProductionDto`  | Request body     | Data for creating the production.   |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` containing the created `ProductionDto` object wrapped in a success message. ✅🎬📦
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌🚫📋

---

### 🎬 **Get Productions** 🎥

| Method   | Endpoint             |
|----------|----------------------|
| **GET**  | `/api/productions`   |

🔍 Retrieve a list of all productions with optional pagination. 📜


| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `ProductionDto` objects wrapped in a `PaginationResult`. ✅🎬📜📋
- If no productions are found, returns an `ApiResponse` with an appropriate error message. ❌🎬🔍


---

# 📋 Organizers 📅

The `OrganizersController` is responsible for managing requests related to organizers. 📊

## 📌 Methods 🛠️

| Method   | Endpoint              |
|----------|-----------------------|
| **POST** | `/api/organizers`     |
| **GET**  | `/api/organizers`     |

### 🔹 Create Organizer ➕

| Method   | Endpoint                  |
|----------|---------------------------|
| **POST** | `/api/organizers`         |

🌟 Create a new organizer. 🆕👤


| Parameter             | Type             | Description                       |
|-----------------------|------------------|-----------------------------------|
| `organizerCreateDto`  | Request body     | Data for creating the organizer.  |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message. ✅🎉
- If an error occurs, returns an `ApiResponse` with an appropriate error message. ❌🚫


### 📋 Get Organizers 📅

| Method   | Endpoint             |
|----------|----------------------|
| **GET**  | `/api/organizers`    |

📜 Retrieve a list of all organizers with optional pagination. 📃👤🔄


| Parameter | Type            | Description                            |
|-----------|-----------------|----------------------------------------|
| `page`    | Query parameter | (Optional) Page number for pagination. |
| `size`    | Query parameter | (Optional) Number of items per page.   |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `OrganizerDto` objects wrapped in a `PaginationResult`. ✅👤📜📋
- If no organizers are found, returns an `ApiResponse` with an appropriate error message. ❌👤🔍

---

# 📜 Contributions 🤝

The `ContributionsController` is responsible for managing requests related to contributions.💡

## 📌 Methods 🛠️

| Method   | Endpoint                 |
|----------|--------------------------|
| **GET**  | `/api/contributions`     |
| **POST** | `/api/contributions`     |


### 📜 Get Contributions

| Method   | Endpoint                 |
|----------|--------------------------|
| **GET**  | `/api/contributions`     |

🔍 Retrieves a list of all contributions with optional pagination. 📄

| Parameter | Type            | Description                        |
|-----------|-----------------|------------------------------------|
| `page`    | Query parameter | Page number for pagination.        |
| `size`    | Query parameter | Number of items per page.          |

**Response:**

- If successful, returns an `ApiResponse` containing a paginated list of `ContributionDto` objects wrapped in a `PaginationResult`. ✅📜📋
- If no contributions are found, returns an `ApiResponse` with an appropriate error message. ❌🔍


---

### 📝 Create Contribution 🌟

| Method   | Endpoint                 |
|----------|--------------------------|
| **POST** | `/api/contributions`     |

Creates a new contribution. 🚀

| Parameter          | Type           | Description                         |
|--------------------|----------------|-------------------------------------|
| `contributionDto`  | Request body   | Data for creating the contribution. |

🔐 **Authorization:**
Requires admin authorization. 👑

**Response:**

- If successful, returns an `ApiResponse` with a success message. ✅🎉
- If validation fails, returns an `ApiResponse` with an appropriate error message. ❌🚫📋

---
